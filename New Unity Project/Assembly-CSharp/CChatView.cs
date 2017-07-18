using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CChatView
{
    private EChatChannel _tab;
    public Animator Anim;
    private bool bInited;
    public bool bRefreshNew = true;
    public bool bShow;
    public GameObject bubbleNode;
    public static int ChatFaceCount = 0x4d;
    public CUIListScript ChatFaceListScript;
    public CUIFormScript chatForm;
    public static int ChatMaxLength = 20;
    public CChatParser ChatParser = new CChatParser();
    private int checkTimer = -1;
    private List<uint> curChannels;
    private GameObject deleteGameObject;
    private static float double_line_height = 76f;
    public static Vector2 entrySizeLobby = new Vector2(355f, 42f);
    public static Vector2 entrySizeRoom = new Vector2(350f, 42f);
    public static Vector2 entrySizeTeam = new Vector2(350f, 42f);
    private ListView<COMDT_FRIEND_INFO> friendTablist;
    public CUIListScript FriendTabListScript;
    public CanvasGroup FriendTabListScript_cg;
    public GameObject image_template;
    public GameObject info_node_obj;
    public Text info_text;
    private InputField inputField;
    private bool lastB;
    private CUIListScript listScript;
    public CUIListScript LobbyScript;
    public CanvasGroup LobbyScript_cg;
    private GameObject loudSpeakerNode;
    private bool m_inputTextChanged;
    private CChatViewEntryNode m_viewEntryNode = new CChatViewEntryNode();
    public static int max_bubble_num = 0x63;
    private GameObject nodeGameObject;
    private GameObject screenBtn;
    private GameObject sendBtn;
    private GameObject sendLoudSpeaker;
    private GameObject sendSpeaker;
    private static float single_line_height = 38f;
    public GameObject text_template;
    private GameObject toolBarNode;
    private static float trible_line_height = 100f;

    private ListView<CChatEntity> _getList()
    {
        ListView<CChatEntity> view = null;
        if (this.CurTab == EChatChannel.Lobby)
        {
            CChatChannel channel = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.Lobby);
            if (channel == null)
            {
                return null;
            }
            return channel.list;
        }
        if (this.CurTab == EChatChannel.Room)
        {
            CChatChannel channel2 = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.Room);
            if (channel2 == null)
            {
                return null;
            }
            return channel2.list;
        }
        if (this.CurTab == EChatChannel.Guild)
        {
            CChatChannel channel3 = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.Guild);
            if (channel3 == null)
            {
                return null;
            }
            return channel3.list;
        }
        if (this.CurTab == EChatChannel.GuildMatchTeam)
        {
            CChatChannel channel4 = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.GuildMatchTeam);
            if (channel4 == null)
            {
                return null;
            }
            return channel4.list;
        }
        if (this.CurTab == EChatChannel.Friend_Chat)
        {
            CChatSysData sysData = Singleton<CChatController>.instance.model.sysData;
            if (sysData == null)
            {
                return null;
            }
            CChatChannel friendChannel = Singleton<CChatController>.GetInstance().model.channelMgr.GetFriendChannel(sysData.ullUid, sysData.dwLogicWorldId);
            if (friendChannel == null)
            {
                return null;
            }
            return friendChannel.list;
        }
        if (this.CurTab == EChatChannel.Team)
        {
            CChatChannel channel6 = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.Team);
            if (channel6 == null)
            {
                return null;
            }
            return channel6.list;
        }
        if (this.CurTab != EChatChannel.Settle)
        {
            return view;
        }
        CChatChannel channel7 = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.Settle);
        if (channel7 == null)
        {
            return null;
        }
        return channel7.list;
    }

    private void _refresh_friends_list(CUIListScript listScript, ListView<COMDT_FRIEND_INFO> data_list)
    {
        if (listScript != null)
        {
            int count = data_list.Count;
            listScript.SetElementAmount(count);
            for (int i = 0; i < count; i++)
            {
                CUIListElementScript elemenet = listScript.GetElemenet(i);
                if ((elemenet != null) && listScript.IsElementInScrollArea(i))
                {
                    this.Show_FriendTabItem(elemenet.get_gameObject(), data_list[i]);
                }
            }
        }
    }

    private void _refresh_list(CUIListScript listScript, ListView<CChatEntity> data_list, bool bShow_Last, List<Vector2> sizeVec, CChatChannel channel)
    {
        if ((listScript != null) && (channel != null))
        {
            this.calc_size(data_list, sizeVec);
            int count = data_list.Count;
            listScript.SetElementAmount(count, sizeVec);
            if (this.bRefreshNew)
            {
                listScript.MoveElementInScrollArea(count - 1, true);
            }
            int num2 = 0;
            for (int i = 0; i < count; i++)
            {
                CUIListElementScript elemenet = listScript.GetElemenet(i);
                if ((elemenet != null) && listScript.IsElementInScrollArea(i))
                {
                    this.Show_ChatItem(elemenet.get_gameObject(), data_list[i]);
                    num2 = i;
                }
            }
            if (!this.bRefreshNew)
            {
                int unreadMeanbleChatEntCount = channel.GetUnreadMeanbleChatEntCount(num2);
                if (unreadMeanbleChatEntCount > 0)
                {
                    if (this.bubbleNode != null)
                    {
                        this.bubbleNode.CustomSetActive(true);
                    }
                    Text component = this.bubbleNode.get_transform().Find("Text").GetComponent<Text>();
                    if (component != null)
                    {
                        if (unreadMeanbleChatEntCount > max_bubble_num)
                        {
                            component.set_text(string.Format("{0}+", max_bubble_num));
                        }
                        else
                        {
                            component.set_text(unreadMeanbleChatEntCount.ToString());
                        }
                    }
                }
                else if (this.bubbleNode != null)
                {
                    this.bubbleNode.CustomSetActive(false);
                }
            }
            else if (this.bubbleNode != null)
            {
                this.bubbleNode.CustomSetActive(false);
            }
            this.bRefreshNew = true;
        }
    }

    private void _setRedPoint(Text redText, int count)
    {
        if (redText != null)
        {
            redText.get_gameObject().CustomSetActive(false);
            redText.get_transform().get_parent().get_gameObject().CustomSetActive(false);
            if (count > 0)
            {
                redText.get_transform().get_parent().get_gameObject().CustomSetActive(true);
                redText.set_text(count.ToString());
                if ((count <= 9) && (count >= 1))
                {
                    redText.get_gameObject().CustomSetActive(true);
                }
            }
        }
    }

    public void BuildTabList(List<uint> list, int index = 0)
    {
        this.listScript.SetElementAmount(list.Count);
        CUIListElementScript elemenet = null;
        for (int i = 0; i < this.listScript.m_elementAmount; i++)
        {
            elemenet = this.listScript.GetElemenet(i);
            elemenet.GetComponent<CUIEventScript>().m_onClickEventParams.tag = i;
            elemenet.get_gameObject().get_transform().FindChild("Text").GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText(CChatUT.GetEChatChannel_Text(list[i])));
        }
        this.listScript.SelectElement(index, true);
    }

    private void calc_size(ListView<CChatEntity> data_list, List<Vector2> sizeVec)
    {
        if ((data_list != null) && (sizeVec != null))
        {
            sizeVec.Clear();
            for (int i = 0; i < data_list.Count; i++)
            {
                CChatEntity entity = data_list[i];
                if (entity.type == EChaterType.LeaveRoom)
                {
                    sizeVec.Add(new Vector2(CChatParser.element_width, CChatParser.element_half_height));
                }
                else
                {
                    float num2 = (entity.numLine <= 1) ? CChatParser.element_height : (CChatParser.element_height + CChatParser.lineHeight);
                    sizeVec.Add(new Vector2(CChatParser.element_width, num2));
                }
            }
        }
    }

    public void Clear_EntryForm_Node()
    {
        if (this.m_viewEntryNode != null)
        {
            this.m_viewEntryNode.Clear_EntryForm_Node();
        }
    }

    public void ClearChatForm()
    {
        this._tab = EChatChannel.None;
        this.info_node_obj = null;
        this.info_text = null;
        this.bShow = false;
        this.bRefreshNew = true;
        this.bInited = false;
        if (this.friendTablist != null)
        {
            this.friendTablist.Clear();
        }
        this.friendTablist = null;
        this.bubbleNode = null;
        this.lastB = false;
        if (this.checkTimer != -1)
        {
            Singleton<CTimerManager>.GetInstance().RemoveTimer(this.checkTimer);
        }
        this.checkTimer = -1;
        this.listScript = null;
        this.LobbyScript = (CUIListScript) (this.FriendTabListScript = null);
        this.LobbyScript_cg = (CanvasGroup) (this.FriendTabListScript_cg = null);
        this.ChatFaceListScript = null;
        this.inputField = null;
        this.toolBarNode = null;
        this.screenBtn = null;
        this.nodeGameObject = null;
        this.deleteGameObject = null;
        this.m_inputTextChanged = false;
        this.sendSpeaker = null;
        this.sendLoudSpeaker = null;
        this.loudSpeakerNode = null;
        this.friendTablist = null;
        this.chatForm = null;
    }

    public void ClearInputText()
    {
        if (this.inputField != null)
        {
            this.inputField.set_text(string.Empty);
        }
    }

    public void CloseSpeakerEntryNode()
    {
    }

    private void Create_Content(GameObject pNode, string content, bool bText, float x, float y, float width, bool bSelf, bool bVip = false, bool bUse_Entry = false)
    {
        if (bText)
        {
            if ((this.text_template != null) && (pNode != null))
            {
                Text text = this.GetText("text", this.text_template, pNode);
                text.set_text(CUIUtility.RemoveEmoji(content));
                if (!bVip)
                {
                    if (bSelf)
                    {
                        text.set_color(new Color(0f, 0f, 0f));
                    }
                    else
                    {
                        text.set_color(CUIUtility.s_Color_White);
                    }
                }
                else if (bSelf)
                {
                    text.set_color(CUIUtility.s_Text_Color_Vip_Chat_Self);
                }
                else
                {
                    text.set_color(CUIUtility.s_Text_Color_Vip_Chat_Other);
                }
                if (bUse_Entry)
                {
                    text.set_color(CUIUtility.s_Color_White);
                }
                if (bUse_Entry)
                {
                    text.get_rectTransform().set_anchoredPosition(new Vector2(x, (float) -CChatParser.chat_entry_lineHeight));
                }
                else
                {
                    text.get_rectTransform().set_anchoredPosition(new Vector2(x, y));
                }
                text.get_rectTransform().set_sizeDelta(new Vector2(width, text.get_rectTransform().get_sizeDelta().y));
            }
        }
        else
        {
            int num;
            int.TryParse(content, out num);
            this.GetImage(num, this.image_template, pNode).get_rectTransform().set_anchoredPosition(new Vector2(x, y));
        }
    }

    public void CreateDetailChatForm()
    {
        if (this.chatForm == null)
        {
            this.chatForm = Singleton<CUIManager>.GetInstance().OpenForm(CChatController.ChatFormPath, false, true);
            this.Init();
        }
    }

    public void Flag_Readed(EChatChannel e)
    {
        if (e == EChatChannel.Friend_Chat)
        {
            CChatSysData sysData = Singleton<CChatController>.instance.model.sysData;
            Singleton<CChatController>.GetInstance().model.channelMgr.GetFriendChannel(sysData.ullUid, sysData.dwLogicWorldId).ReadAll();
        }
        else
        {
            Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(e).ReadAll();
        }
    }

    private Image GetImage(int index, GameObject template, GameObject pNode)
    {
        GameObject obj2 = Object.Instantiate(template);
        obj2.set_name("image");
        obj2.CustomSetActive(true);
        obj2.get_transform().SetParent(pNode.get_transform());
        Image component = obj2.GetComponent<Image>();
        component.get_rectTransform().set_localPosition(new Vector3(0f, 0f, 0f));
        component.get_rectTransform().set_pivot(new Vector2(0f, 0f));
        component.get_rectTransform().set_localScale(new Vector3(1f, 1f, 1f));
        if (index > 0x4d)
        {
            index = 0x4d;
        }
        if (index < 1)
        {
            index = 1;
        }
        component.SetSprite(string.Format("UGUI/Sprite/Dynamic/ChatFace/{0}", index), null, true, false, false, false);
        component.SetNativeSize();
        return component;
    }

    public string GetInputText()
    {
        if (this.inputField != null)
        {
            return this.inputField.get_text();
        }
        return null;
    }

    private Text GetText(string name, GameObject template, GameObject pNode)
    {
        GameObject obj2 = Object.Instantiate(template);
        obj2.set_name("text");
        obj2.CustomSetActive(true);
        obj2.get_transform().SetParent(pNode.get_transform());
        Text component = obj2.GetComponent<Text>();
        component.get_rectTransform().set_localPosition(new Vector3(0f, 0f, 0f));
        component.get_rectTransform().set_pivot(new Vector2(0f, 0f));
        component.get_rectTransform().set_localScale(new Vector3(1f, 1f, 1f));
        return component;
    }

    private int GetUnReadCount(EChatChannel channelType)
    {
        if (channelType == EChatChannel.Friend)
        {
            return Singleton<CChatController>.instance.model.channelMgr.GetFriendTotal_UnreadCount();
        }
        CChatChannel channel = Singleton<CChatController>.instance.model.channelMgr.GetChannel(channelType);
        if (channel != null)
        {
            return channel.GetUnreadCount();
        }
        return 0;
    }

    public void HideDetailChatForm()
    {
        this.bShow = false;
        if (this.chatForm != null)
        {
            this.nodeGameObject.CustomSetActive(false);
        }
        this.SetCheckTimerEnable(false);
    }

    private void Init()
    {
        Singleton<CChatController>.instance.model.channelMgr.GetChannel(EChatChannel.Lobby).Init_Timer();
        this.nodeGameObject = Utility.FindChild(this.chatForm.get_gameObject(), "node");
        this.nodeGameObject.CustomSetActive(false);
        this.Anim = this.nodeGameObject.GetComponent<Animator>();
        this.info_node_obj = this.chatForm.get_transform().FindChild("node/info_node").get_gameObject();
        this.info_text = this.chatForm.get_transform().FindChild("node/info_node/Text").get_gameObject().GetComponent<Text>();
        this.inputField = this.chatForm.get_transform().FindChild("node/ToolBar/InputField").get_gameObject().GetComponent<InputField>();
        this.inputField.get_onValueChange().AddListener(new UnityAction<string>(this, (IntPtr) this.On_InputFiled_ValueChange));
        this.toolBarNode = this.chatForm.get_transform().FindChild("node/ToolBar").get_gameObject();
        this.sendBtn = this.chatForm.get_transform().FindChild("node/ToolBar/SendBtn").get_gameObject();
        this.bubbleNode = this.chatForm.get_transform().FindChild("node/bubble").get_gameObject();
        this.listScript = this.chatForm.get_transform().FindChild("node/Tab/List").get_gameObject().GetComponent<CUIListScript>();
        this.curChannels = Singleton<CChatController>.instance.model.channelMgr.CurActiveChannels;
        this.BuildTabList(this.curChannels, 0);
        this.sendSpeaker = Utility.FindChild(this.chatForm.get_gameObject(), "node/SendSpeaker");
        this.sendLoudSpeaker = Utility.FindChild(this.chatForm.get_gameObject(), "node/SendLoudSpeaker");
        this.loudSpeakerNode = Utility.FindChild(this.chatForm.get_gameObject(), "node/ListView/LobbyChatList/LoudSpeakerNode");
        this.LobbyScript = this.chatForm.get_transform().FindChild("node/ListView/LobbyChatList").get_gameObject().GetComponent<CUIListScript>();
        this.FriendTabListScript = this.chatForm.get_transform().FindChild("node/ListView/FriendItemList").get_gameObject().GetComponent<CUIListScript>();
        this.FriendTabListScript.m_alwaysDispatchSelectedChangeEvent = true;
        this.ChatFaceListScript = this.chatForm.get_transform().FindChild("node/ListView/ChatFaceList").get_gameObject().GetComponent<CUIListScript>();
        this.screenBtn = Utility.FindChild(this.chatForm.get_gameObject(), "node/ListView/Button");
        this.LobbyScript_cg = this.LobbyScript.GetComponent<CanvasGroup>();
        this.FriendTabListScript_cg = this.FriendTabListScript.GetComponent<CanvasGroup>();
        if (this.FriendTabListScript != null)
        {
            this.FriendTabListScript.get_gameObject().CustomSetActive(true);
        }
        if (this.LobbyScript != null)
        {
            this.LobbyScript.get_gameObject().CustomSetActive(true);
        }
        this.deleteGameObject = Utility.FindChild(this.chatForm.get_gameObject(), "node/ToolBar/delete");
        this.deleteGameObject.CustomSetActive(false);
        this.SetInputFiledEnable(false);
        this.InitCheckTimer();
        this.text_template = Utility.FindChild(this.chatForm.get_gameObject(), "Text_template");
        this.image_template = Utility.FindChild(this.chatForm.get_gameObject(), "Image_template");
        this.Refresh_All_RedPoint();
        this._tab = EChatChannel.None;
        this.bInited = true;
        CUIEventScript componetInChild = Utility.GetComponetInChild<CUIEventScript>(this.chatForm.get_gameObject(), "node/SendSpeaker");
        CUIEventScript script2 = Utility.GetComponetInChild<CUIEventScript>(this.chatForm.get_gameObject(), "node/SendLoudSpeaker");
        componetInChild.m_onClickEventParams.commonUInt32Param1 = 0x2739;
        script2.m_onClickEventParams.commonUInt32Param1 = 0x273a;
        this.Refresh_EntryForm();
    }

    public void InitCheckTimer()
    {
        if (this.checkTimer == -1)
        {
            this.checkTimer = Singleton<CTimerManager>.GetInstance().AddTimer(40, 0, new CTimer.OnTimeUpHandler(this.On_CheckInputField_Focus));
            UT.ResetTimer(this.checkTimer, true);
        }
    }

    public bool IsCheckHistory()
    {
        ListView<CChatEntity> view = this._getList();
        if ((view == null) || (this.LobbyScript == null))
        {
            return false;
        }
        int count = view.Count;
        if (count == 0)
        {
            return false;
        }
        return !this.LobbyScript.IsElementInScrollArea(count - 1);
    }

    public void Jump2FriendChat(COMDT_FRIEND_INFO info)
    {
        if (info != null)
        {
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Chat_EntryPanel_Click);
            CChatModel model = Singleton<CChatController>.GetInstance().model;
            model.sysData.ullUid = info.stUin.ullUid;
            model.sysData.dwLogicWorldId = info.stUin.dwLogicWorldId;
            if (model.channelMgr._getChannel(EChatChannel.Friend, info.stUin.ullUid, info.stUin.dwLogicWorldId) == null)
            {
                CChatChannel channel = model.channelMgr.CreateChannel(EChatChannel.Friend, info.stUin.ullUid, info.stUin.dwLogicWorldId);
            }
            this.curChannels = Singleton<CChatController>.instance.model.channelMgr.CurActiveChannels;
            int index = this.curChannels.IndexOf(4);
            if (index == -1)
            {
                this.curChannels.Add(4);
                index = this.curChannels.Count - 1;
            }
            this.BuildTabList(this.curChannels, index);
            this.CurTab = EChatChannel.Friend_Chat;
            this.listScript.GetSelectedElement().get_gameObject().get_transform().FindChild("Text").GetComponent<Text>().set_text(Utility.UTF8Convert(info.szUserName));
        }
    }

    public void On_Chat_FaceList_Selected(CUIEvent uiEvent)
    {
        if (this.bInited)
        {
            int num = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex() + 1;
            this.inputField.set_text(string.Format("{0}%{1}", this.inputField.get_text(), num));
        }
    }

    public void On_Chat_ScreenButton_Click()
    {
        this.SetChatFaceShow(false);
    }

    private void On_CheckInputField_Focus(int timer)
    {
        if (this.lastB != this.inputField.get_isFocused())
        {
            this.lastB = this.inputField.get_isFocused();
            this.SetInputFiledEnable(this.lastB);
        }
    }

    public void On_Friend_TabList_Selected(CUIEvent uiEvent)
    {
        if (this.bInited)
        {
            DebugHelper.Assert(uiEvent.m_srcWidgetIndexInBelongedList <= (this.friendTablist.Count - 1), "---Chat, On_Friend_TabList_Selected");
            if (uiEvent.m_srcWidgetIndexInBelongedList <= (this.friendTablist.Count - 1))
            {
                int selectedIndex = (uiEvent.m_srcWidgetScript as CUIListScript).GetSelectedIndex();
                COMDT_FRIEND_INFO comdt_friend_info = this.friendTablist[selectedIndex];
                Singleton<CChatController>.GetInstance().model.sysData.ullUid = comdt_friend_info.stUin.ullUid;
                Singleton<CChatController>.GetInstance().model.sysData.dwLogicWorldId = comdt_friend_info.stUin.dwLogicWorldId;
                COfflineChatIndex cOfflineChatIndex = Singleton<CChatController>.instance.model.GetCOfflineChatIndex(comdt_friend_info.stUin.ullUid, comdt_friend_info.stUin.dwLogicWorldId);
                if (cOfflineChatIndex != null)
                {
                    CChatNetUT.Send_Clear_Offline(cOfflineChatIndex.indexList);
                    Singleton<CChatController>.instance.model.ClearCOfflineChatIndex(cOfflineChatIndex);
                }
                GameObject obj2 = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetElemenet(selectedIndex).get_gameObject();
                this._setRedPoint(obj2.get_transform().FindChild("head/redPoint/Text").GetComponent<Text>(), 0);
                CUICommonSystem.DelRedDot(this.listScript.GetElemenet(0).get_gameObject());
                this.CurTab = EChatChannel.Friend_Chat;
                this.listScript.GetSelectedElement().get_gameObject().get_transform().FindChild("Text").GetComponent<Text>().set_text(Utility.UTF8Convert(comdt_friend_info.szUserName));
            }
        }
    }

    public void On_FriendsList_ElementEnable(CUIEvent uievent)
    {
        int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
        COMDT_FRIEND_INFO info = null;
        ListView<COMDT_FRIEND_INFO> friendTablist = this.friendTablist;
        if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < friendTablist.Count))
        {
            info = friendTablist[srcWidgetIndexInBelongedList];
        }
        if ((info != null) && (uievent.m_srcWidget != null))
        {
            this.Show_FriendTabItem(uievent.m_srcWidget, info);
        }
    }

    private void On_InputFiled_ValueChange(string arg0)
    {
        this.m_inputTextChanged = true;
    }

    public void On_List_ElementEnable(CUIEvent uievent)
    {
        if (uievent != null)
        {
            int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
            CChatEntity ent = null;
            ListView<CChatEntity> view = this._getList();
            if (view != null)
            {
                if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < view.Count))
                {
                    ent = view[srcWidgetIndexInBelongedList];
                }
                if ((ent != null) && (uievent.m_srcWidget != null))
                {
                    this.Show_ChatItem(uievent.m_srcWidget, ent);
                }
                if ((srcWidgetIndexInBelongedList == (view.Count - 1)) && (this.bubbleNode != null))
                {
                    this.bubbleNode.CustomSetActive(false);
                }
            }
        }
    }

    public void On_Tab_Change(int index)
    {
        if (this.bInited && ((index >= 0) && (index < this.curChannels.Count)))
        {
            this.CurTab = this.curChannels[index];
        }
    }

    public void OpenSpeakerEntryNode(string constent)
    {
    }

    public void Process_Friend_Tip()
    {
        this.info_node_obj.CustomSetActive(true);
        bool flag = Singleton<CFriendContoller>.instance.model.IsAnyFriendExist(true);
        if (!Singleton<CFriendContoller>.instance.model.IsAnyFriendExist(false))
        {
            this.info_text.set_text(UT.GetText("Chat_NoFriend_Tip"));
        }
        else if (!flag)
        {
            this.info_text.set_text(UT.GetText("Chat_NoOnlineFriend_Tip"));
        }
        else
        {
            this.info_text.set_text(UT.GetText("Chat_HasFriend_Tip"));
        }
    }

    public void Rebuild_ChatFace_List()
    {
        this.ChatFaceListScript.SetElementAmount(ChatFaceCount);
        for (int i = 0; i < ChatFaceCount; i++)
        {
            Image component = this.ChatFaceListScript.GetElemenet(i).GetComponent<Image>();
            UT.SetChatFace(this.ChatFaceListScript.m_belongedFormScript, component, i + 1);
        }
    }

    public void ReBuildTabText()
    {
        if (this.listScript != null)
        {
            CUIListElementScript elemenet = null;
            for (int i = 0; i < this.curChannels.Count; i++)
            {
                elemenet = this.listScript.GetElemenet(i);
                if (elemenet == null)
                {
                    return;
                }
                elemenet.get_gameObject().get_transform().FindChild("Text").GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText(CChatUT.GetEChatChannel_Text(this.curChannels[i])));
            }
        }
    }

    public void Refresh_All_RedPoint()
    {
        CChatModel model = Singleton<CChatController>.instance.model;
        if (((this.curChannels != null) && (model != null)) && (this.listScript != null))
        {
            for (int i = 0; i < this.curChannels.Count; i++)
            {
                int alertNum = 0;
                uint num3 = this.curChannels[i];
                if (num3 == 4)
                {
                    alertNum = model.channelMgr.GetFriendTotal_UnreadCount();
                }
                else
                {
                    CChatChannel channel = model.channelMgr.GetChannel((EChatChannel) num3);
                    if (channel != null)
                    {
                        alertNum = channel.GetUnreadCount();
                    }
                    else
                    {
                        alertNum = 0;
                    }
                }
                CUIListElementScript elemenet = this.listScript.GetElemenet(i);
                if (elemenet != null)
                {
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

    public void Refresh_ChatEntity_List(bool bForce = true, EChatChannel tab = 12)
    {
        if (((this.nodeGameObject != null) && (bForce || (this.CurTab == tab))) && (this.nodeGameObject.get_activeSelf() && this.nodeGameObject.get_activeInHierarchy()))
        {
            ListView<CChatEntity> list = null;
            CChatChannel friendChannel = null;
            if (this.CurTab == EChatChannel.Lobby)
            {
                friendChannel = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.Lobby);
                list = friendChannel.list;
                friendChannel.ReadAll();
            }
            else if (this.CurTab == EChatChannel.Room)
            {
                friendChannel = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.Room);
                list = friendChannel.list;
                friendChannel.ReadAll();
            }
            else if (this.CurTab == EChatChannel.Guild)
            {
                friendChannel = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.Guild);
                list = friendChannel.list;
                friendChannel.ReadAll();
            }
            else if (this.CurTab == EChatChannel.GuildMatchTeam)
            {
                friendChannel = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.GuildMatchTeam);
                list = friendChannel.list;
                friendChannel.ReadAll();
            }
            else if (this.CurTab == EChatChannel.Friend_Chat)
            {
                CChatSysData sysData = Singleton<CChatController>.instance.model.sysData;
                friendChannel = Singleton<CChatController>.GetInstance().model.channelMgr.GetFriendChannel(sysData.ullUid, sysData.dwLogicWorldId);
                list = friendChannel.list;
                this.Flag_Readed(EChatChannel.Friend_Chat);
            }
            else
            {
                if (this.CurTab == EChatChannel.Friend)
                {
                    this.friendTablist = Singleton<CFriendContoller>.GetInstance().model.GetValidChatFriendList();
                    this.friendTablist.Sort(new Comparison<COMDT_FRIEND_INFO>(CFriendModel.FriendDataSortForChatFriendList));
                    this._refresh_friends_list(this.FriendTabListScript, this.friendTablist);
                    return;
                }
                if (this.CurTab == EChatChannel.Team)
                {
                    friendChannel = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.Team);
                    list = friendChannel.list;
                    friendChannel.ReadAll();
                }
                else if (this.CurTab == EChatChannel.Settle)
                {
                    friendChannel = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.Settle);
                    list = friendChannel.list;
                    friendChannel.ReadAll();
                }
            }
            if (list != null)
            {
                this._refresh_list(this.LobbyScript, list, true, friendChannel.sizeVec, friendChannel);
            }
            this.Refresh_ChatInputView();
        }
    }

    public void Refresh_ChatInputView()
    {
        if ((this.inputField != null) && (this.sendBtn != null))
        {
            if (this.CurTab == EChatChannel.Lobby)
            {
                CChatChannel channel = Singleton<CChatController>.instance.model.channelMgr.GetChannel(EChatChannel.Lobby);
                if (Singleton<CChatController>.instance.CheckSend(EChatChannel.Lobby, string.Empty, false) == CChatController.enCheckChatResult.CdLimit)
                {
                    this.inputField.get_placeholder().GetComponent<Text>().set_text(string.Format(Singleton<CTextManager>.instance.GetText("Chat_Common_Tips_2"), channel.Get_Left_CDTime()));
                }
                else if (!CSysDynamicBlock.bChatPayBlock)
                {
                    string[] args = new string[] { Singleton<CChatController>.GetInstance().model.sysData.restChatFreeCnt.ToString() };
                    this.inputField.get_placeholder().GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText("Chat_Common_Tips_1", args));
                    bool bActive = Singleton<CChatController>.GetInstance().model.sysData.restChatFreeCnt > 0;
                    this.sendBtn.get_transform().GetChild(0).get_gameObject().CustomSetActive(!bActive);
                    this.sendBtn.get_transform().GetChild(1).get_gameObject().CustomSetActive(bActive);
                    if (!bActive)
                    {
                        GameObject obj2 = this.sendBtn.get_transform().Find("CostObj").get_gameObject();
                        enPayType payType = CMallSystem.ResBuyTypeToPayType(Singleton<CChatController>.GetInstance().model.sysData.chatCostType);
                        if (this.chatForm != null)
                        {
                            CHeroSkinBuyManager.SetPayCostIcon(this.chatForm, obj2.get_transform(), payType);
                        }
                        obj2.get_transform().Find("priceTxt").GetComponent<Text>().set_text(string.Format("x{0}", Singleton<CChatController>.GetInstance().model.sysData.chatCostNum));
                    }
                }
                else
                {
                    this.inputField.get_placeholder().GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText("Chat_Common_Tips_5"));
                    this.sendBtn.get_transform().GetChild(0).get_gameObject().CustomSetActive(false);
                    this.sendBtn.get_transform().GetChild(1).get_gameObject().CustomSetActive(true);
                }
            }
            else if (this.CurTab == EChatChannel.Team)
            {
                this.inputField.get_placeholder().GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText("Chat_Common_Tips_11"));
                this.sendBtn.get_transform().GetChild(0).get_gameObject().CustomSetActive(false);
                this.sendBtn.get_transform().GetChild(1).get_gameObject().CustomSetActive(true);
            }
            else
            {
                this.inputField.get_placeholder().GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText("Chat_Common_Tips_5"));
                this.sendBtn.get_transform().GetChild(0).get_gameObject().CustomSetActive(false);
                this.sendBtn.get_transform().GetChild(1).get_gameObject().CustomSetActive(true);
            }
        }
    }

    public void Refresh_EntryForm()
    {
        if (this.m_viewEntryNode != null)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLobbySystem.LOBBY_FORM_PATH);
            if ((form != null) && !form.IsHided())
            {
                GameObject entryGameObject = form.get_transform().Find("entry_node").get_gameObject();
                this.m_viewEntryNode.Refresh_EntryForm(entryGameObject);
            }
            CUIFormScript script2 = Singleton<CUIManager>.instance.GetForm(CRoomSystem.PATH_ROOM);
            if ((script2 != null) && !script2.IsHided())
            {
                GameObject obj3 = script2.get_transform().Find("entry_node").get_gameObject();
                this.m_viewEntryNode.Refresh_EntryForm(obj3);
            }
            CUIFormScript script3 = Singleton<CUIManager>.instance.GetForm(CMatchingSystem.PATH_MATCHING_MULTI);
            if ((script3 != null) && !script3.IsHided())
            {
                GameObject obj4 = script3.get_transform().Find("entry_node").get_gameObject();
                this.m_viewEntryNode.Refresh_EntryForm(obj4);
            }
            CUIFormScript script4 = Singleton<CUIManager>.instance.GetForm(SettlementSystem.SettlementFormName);
            if ((script4 != null) && !script4.IsHided())
            {
                GameObject obj5 = script4.get_transform().Find("Panel/entry_node").get_gameObject();
                this.m_viewEntryNode.Refresh_EntryForm(obj5);
            }
            CUIFormScript script5 = Singleton<CUIManager>.instance.GetForm(CGuildMatchSystem.GuildMatchFormPath);
            if ((script5 != null) && !script5.IsHided())
            {
                GameObject obj6 = script5.get_transform().Find("entry_node").get_gameObject();
                this.m_viewEntryNode.Refresh_EntryForm(obj6);
            }
        }
    }

    public void RefreshGuildRecruitInfoNode()
    {
        this.info_node_obj.CustomSetActive(true);
        this.info_text.set_text(Singleton<CTextManager>.GetInstance().GetText("Guild_Recruit_Chat_Tip"));
    }

    public void RefreshGuildRecruitList()
    {
        CChatModel model = Singleton<CChatController>.GetInstance().model;
        if ((model != null) && (model.sysData != null))
        {
            this.chatForm.GetWidget(0).GetComponent<CUIListScript>().SetElementAmount(model.sysData.m_guildRecruitInfos.Count);
        }
    }

    public void SetChatFaceShow(bool bShow)
    {
        if (this.bInited)
        {
            this.ChatFaceListScript.get_gameObject().CustomSetActive(bShow);
            this.screenBtn.CustomSetActive(bShow);
            if (bShow && (this.ChatFaceListScript.GetElementAmount() < ChatFaceCount))
            {
                this.Rebuild_ChatFace_List();
            }
        }
    }

    public void SetCheckTimerEnable(bool b)
    {
        if (this.checkTimer != -1)
        {
            UT.ResetTimer(this.checkTimer, !b);
        }
    }

    public void SetEntryChannelImage(EChatChannel v)
    {
        if (this.m_viewEntryNode != null)
        {
            this.m_viewEntryNode.SetEntryChannelImage(v);
        }
    }

    public void SetEntryVisible(bool bShow)
    {
        if (this.m_viewEntryNode != null)
        {
            this.m_viewEntryNode.SetVisivble(bShow);
        }
    }

    public void SetGuildRecruitListElement(CUIEvent uiEvent)
    {
        CChatModel model = Singleton<CChatController>.GetInstance().model;
        if (((model != null) && (model.sysData != null)) && ((uiEvent.m_srcWidgetIndexInBelongedList >= 0) && (uiEvent.m_srcWidgetIndexInBelongedList < model.sysData.m_guildRecruitInfos.Count)))
        {
            GuildRecruitInfo info = model.sysData.m_guildRecruitInfos[uiEvent.m_srcWidgetIndexInBelongedList];
            Transform transform = uiEvent.m_srcWidget.get_transform();
            if (transform != null)
            {
                CUIHttpImageScript component = transform.Find("head/pnlSnsHead/HttpImage").GetComponent<CUIHttpImageScript>();
                Text text = transform.Find("head/LevelBg/Level").GetComponent<Text>();
                Text text2 = transform.Find("name").GetComponent<Text>();
                Text text3 = transform.Find("content").GetComponent<Text>();
                CUIEventScript script2 = transform.Find("btnApplyJoin").GetComponent<CUIEventScript>();
                component.SetImageUrl(Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(info.senderHeadUrl));
                text.set_text(info.senderLevel.ToString());
                text2.set_text(info.senderName);
                string[] args = new string[] { info.guildName, info.limitLevel.ToString(), CGuildHelper.GetLadderGradeLimitText(info.limitGrade) };
                text3.set_text(Singleton<CTextManager>.GetInstance().GetText("Guild_Recruit_Info_Format", args));
                script2.m_onClickEventParams.commonUInt64Param1 = info.guildId;
            }
        }
    }

    public void SetInputFiledEnable(bool bEnable)
    {
        if (bEnable)
        {
            this.deleteGameObject.CustomSetActive(false);
            this.inputField.MoveTextEnd(false);
        }
        else
        {
            this.deleteGameObject.CustomSetActive(false);
        }
    }

    public void SetShow(CanvasGroup cg, bool bShow)
    {
        if (cg != null)
        {
            if (bShow)
            {
                cg.set_alpha(1f);
                cg.set_blocksRaycasts(true);
            }
            else
            {
                cg.set_alpha(0f);
                cg.set_blocksRaycasts(false);
            }
        }
    }

    public void Show_ChatItem(GameObject node, CChatEntity ent)
    {
        if ((node != null) && (ent != null))
        {
            GameObject obj2 = null;
            GameObject obj3 = node.get_transform().Find("self").get_gameObject();
            GameObject obj4 = node.get_transform().Find("other").get_gameObject();
            GameObject obj5 = node.get_transform().Find("system").get_gameObject();
            GameObject obj6 = node.get_transform().Find("offline").get_gameObject();
            GameObject obj7 = node.get_transform().Find("time").get_gameObject();
            GameObject obj8 = node.get_transform().Find("speaker_1").get_gameObject();
            GameObject obj9 = node.get_transform().Find("speaker_2").get_gameObject();
            obj3.CustomSetActive(false);
            obj4.CustomSetActive(false);
            obj5.CustomSetActive(false);
            obj6.CustomSetActive(false);
            obj7.CustomSetActive(false);
            obj8.CustomSetActive(false);
            obj9.CustomSetActive(false);
            if ((ent.type == EChaterType.System) || (ent.type == EChaterType.LeaveRoom))
            {
                obj2 = obj5;
            }
            else if (ent.type == EChaterType.OfflineInfo)
            {
                obj2 = obj6;
            }
            else if (ent.type == EChaterType.Self)
            {
                obj2 = obj3;
            }
            else if (ent.type == EChaterType.Time)
            {
                obj2 = obj7;
            }
            else if (ent.type == EChaterType.Speaker)
            {
                obj2 = obj8;
            }
            else if (ent.type == EChaterType.LoudSpeaker)
            {
                obj2 = obj9;
            }
            else
            {
                obj2 = obj4;
            }
            obj2.CustomSetActive(true);
            CUIEventScript componetInChild = Utility.GetComponetInChild<CUIEventScript>(obj2, "pnlSnsHead");
            if (componetInChild != null)
            {
                componetInChild.m_onClickEventParams.commonUInt64Param1 = ent.ullUid;
                componetInChild.m_onClickEventParams.tag2 = (int) ent.iLogicWorldID;
            }
            if ((ent.type == EChaterType.System) || (ent.type == EChaterType.LeaveRoom))
            {
                this.ShowRawText(obj2, ent);
            }
            else if (ent.type == EChaterType.OfflineInfo)
            {
                this.ShowRawText(obj2, ent);
            }
            else if (ent.type == EChaterType.Time)
            {
                this.ShowRawText(obj2, ent);
            }
            else if (ent.type == EChaterType.Speaker)
            {
                this.ShowRich(obj2, ent);
            }
            else if (ent.type == EChaterType.LoudSpeaker)
            {
                this.ShowRich(obj2, ent);
            }
            else
            {
                this.ShowRich(obj2, ent);
            }
            ent.bHasReaded = true;
        }
    }

    public void Show_FriendTabItem(GameObject node, COMDT_FRIEND_INFO info)
    {
        node.get_transform().FindChild("name").GetComponent<Text>().set_text(UT.Bytes2String(info.szUserName));
        node.get_transform().FindChild("head/LevelBg/Level").GetComponent<Text>().set_text(info.dwPvpLvl.ToString());
        Text component = node.get_transform().FindChild("head/redPoint/Text").GetComponent<Text>();
        GameObject obj2 = node.get_transform().FindChild("head/pnlSnsHead/HttpImage").get_gameObject();
        string url = UT.Bytes2String(info.szHeadUrl);
        if (!CSysDynamicBlock.bSocialBlocked)
        {
            obj2.GetComponent<CUIHttpImageScript>().SetImageUrl(Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(url));
        }
        Image img = obj2.GetComponent<Image>();
        if (img != null)
        {
            bool flag = Singleton<CFriendContoller>.instance.model.IsFriendOfflineOnline(info.stUin.ullUid, info.stUin.dwLogicWorldId);
            UT.SetImage(img, !flag);
        }
        if (info.stGameVip != null)
        {
            GameObject obj3 = node.get_transform().Find("head/pnlSnsHead/NobeIcon").get_gameObject();
            MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(obj3.GetComponent<Image>(), (int) info.stGameVip.dwCurLevel, false);
            GameObject obj4 = node.get_transform().Find("head/pnlSnsHead/NobeImag").get_gameObject();
            MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(obj4.GetComponent<Image>(), (int) info.stGameVip.dwHeadIconId);
        }
        CChatChannel friendChannel = Singleton<CChatController>.GetInstance().model.channelMgr.GetFriendChannel(info.stUin.ullUid, info.stUin.dwLogicWorldId);
        this._setRedPoint(component, friendChannel.GetUnreadCount());
        CChatEntity last = friendChannel.GetLast();
        Text text2 = node.get_transform().FindChild("Text").GetComponent<Text>();
        if (last != null)
        {
            text2.set_text(last.text);
        }
        else
        {
            text2.set_text(Singleton<CTextManager>.instance.GetText("Chat_Common_Tips_3"));
        }
        if ((info.stGameVip != null) && (info.stGameVip.dwCurLevel > 0))
        {
            text2.set_color(CUIUtility.s_Text_Color_Vip_Chat_Other);
        }
    }

    public void ShowDetailChatForm()
    {
        CChatChannelMgr.EChatTab chatTab = Singleton<CChatController>.GetInstance().model.channelMgr.ChatTab;
        switch (chatTab)
        {
            case CChatChannelMgr.EChatTab.Normal:
                this.chatForm.SetPriority(enFormPriority.Priority2);
                this.sendSpeaker.CustomSetActive(true);
                this.sendLoudSpeaker.CustomSetActive(true);
                break;

            case CChatChannelMgr.EChatTab.Room:
            case CChatChannelMgr.EChatTab.Team:
                this.sendSpeaker.CustomSetActive(false);
                this.sendLoudSpeaker.CustomSetActive(false);
                break;

            case CChatChannelMgr.EChatTab.Settle:
                this.sendSpeaker.CustomSetActive(false);
                this.sendLoudSpeaker.CustomSetActive(false);
                break;
        }
        if (this.bInited && (this.chatForm != null))
        {
            this._tab = EChatChannel.None;
            this.bShow = true;
            this.nodeGameObject.CustomSetActive(true);
            this.curChannels = Singleton<CChatController>.GetInstance().model.channelMgr.CurActiveChannels;
            this.curChannels.Sort();
            if (chatTab == CChatChannelMgr.EChatTab.Normal)
            {
                this.SortChannels();
            }
            this.BuildTabList(this.curChannels, 0);
            this.CurTab = Singleton<CChatController>.instance.model.sysData.LastChannel;
            int index = 0;
            for (int i = 0; i < this.curChannels.Count; i++)
            {
                if (((EChatChannel) this.curChannels[i]) == this.CurTab)
                {
                    index = i;
                }
            }
            UT.SetListIndex(this.listScript, index);
            this.SetChatFaceShow(false);
            this.SetCheckTimerEnable(true);
            if (CSysDynamicBlock.bLobbyEntryBlocked)
            {
                Transform transform = this.chatForm.get_transform().FindChild("node/SendSpeaker");
                Transform transform2 = this.chatForm.get_transform().FindChild("node/SendLoudSpeaker");
                if (transform != null)
                {
                    transform.get_gameObject().CustomSetActive(false);
                }
                if (transform2 != null)
                {
                    transform2.get_gameObject().CustomSetActive(false);
                }
            }
        }
    }

    public void ShowEntryForm()
    {
        if (this.chatForm != null)
        {
            switch (Singleton<CChatController>.GetInstance().model.channelMgr.ChatTab)
            {
                case CChatChannelMgr.EChatTab.Normal:
                    this.chatForm.SetPriority(enFormPriority.Priority0);
                    break;

                case CChatChannelMgr.EChatTab.Room:
                case CChatChannelMgr.EChatTab.Team:
                case CChatChannelMgr.EChatTab.Settle:
                    this.chatForm.SetPriority(enFormPriority.Priority5);
                    break;
            }
        }
        this.Refresh_EntryForm();
    }

    public void ShowLoudSpeaker(bool bShow, COMDT_CHAT_MSG_HORN data = new COMDT_CHAT_MSG_HORN())
    {
        if (this.loudSpeakerNode != null)
        {
            if (data != null)
            {
                string str;
                string str2;
                string str3;
                COMDT_GAME_VIP_CLIENT comdt_game_vip_client;
                CUIHttpImageScript componetInChild = Utility.GetComponetInChild<CUIHttpImageScript>(this.loudSpeakerNode, "pnlSnsHead/HttpImage");
                Text text = Utility.GetComponetInChild<Text>(this.loudSpeakerNode, "name");
                Text text2 = Utility.GetComponetInChild<Text>(this.loudSpeakerNode, "content");
                Image image = Utility.GetComponetInChild<Image>(this.loudSpeakerNode, "pnlSnsHead/HttpImage/NobeIcon");
                Image image2 = Utility.GetComponetInChild<Image>(this.loudSpeakerNode, "pnlSnsHead/HttpImage/NobeImag");
                Text text3 = Utility.GetComponetInChild<Text>(this.loudSpeakerNode, "pnlSnsHead/HttpImage/bg/level");
                CUIEventScript script2 = Utility.GetComponetInChild<CUIEventScript>(this.loudSpeakerNode, "pnlSnsHead");
                script2.m_onClickEventID = enUIEventID.Chat_Form_Open_Mini_Player_Info_Form;
                script2.m_onClickEventParams.commonUInt64Param1 = data.stFrom.ullUid;
                script2.m_onClickEventParams.tag2 = data.stFrom.iLogicWorldID;
                CChatUT.GetUser(EChaterType.LoudSpeaker, data.stFrom.ullUid, (uint) data.stFrom.iLogicWorldID, out str, out str2, out str3, out comdt_game_vip_client);
                text.set_text(str);
                text2.set_text(Utility.UTF8Convert(data.szContent));
                text3.set_text(str2);
                UT.SetHttpImage(componetInChild, str3);
                if (comdt_game_vip_client != null)
                {
                    MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(image, (int) comdt_game_vip_client.dwCurLevel, false);
                    MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(image2, (int) comdt_game_vip_client.dwHeadIconId);
                }
                if (text2.get_preferredHeight() > 60f)
                {
                    this.loudSpeakerNode.GetComponent<RectTransform>().set_sizeDelta(new Vector2(544f, 106f));
                }
                else
                {
                    this.loudSpeakerNode.GetComponent<RectTransform>().set_sizeDelta(new Vector2(544f, 86f));
                }
            }
            this.loudSpeakerNode.CustomSetActive(bShow && (data != null));
        }
    }

    private void ShowRawText(GameObject playerNode, CChatEntity ent)
    {
        Text component = playerNode.get_transform().Find("Text").GetComponent<Text>();
        if (component != null)
        {
            component.set_text(ent.text);
            if (ent.type == EChaterType.LeaveRoom)
            {
                component.set_alignment(3);
            }
            else
            {
                component.set_alignment(0);
            }
            RectTransform transform = component.get_transform() as RectTransform;
            if (transform != null)
            {
                transform.set_sizeDelta(new Vector2(transform.get_sizeDelta().x, double_line_height));
            }
        }
    }

    private void ShowRich(GameObject playerNode, CChatEntity ent)
    {
        if ((ent.TextObjList != null) && (ent.TextObjList.Count != 0))
        {
            playerNode.get_transform().Find("name").get_gameObject().GetComponent<Text>().set_text(ent.name);
            GameObject obj2 = playerNode.get_transform().Find("pnlSnsHead/HttpImage").get_gameObject();
            UT.SetHttpImage(obj2.GetComponent<CUIHttpImageScript>(), ent.head_url);
            Image component = obj2.GetComponent<Image>();
            if (((component != null) && (ent.type == EChaterType.Friend)) && (ent.type == EChaterType.Friend))
            {
                bool flag = Singleton<CFriendContoller>.instance.model.IsFriendOfflineOnline(ent.ullUid, ent.iLogicWorldID);
                UT.SetImage(component, !flag);
            }
            if (ent.stGameVip != null)
            {
                GameObject obj3 = playerNode.get_transform().Find("pnlSnsHead/HttpImage/NobeIcon").get_gameObject();
                if (obj3 != null)
                {
                    MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(obj3.GetComponent<Image>(), (int) ent.stGameVip.dwCurLevel, false);
                }
                GameObject obj4 = playerNode.get_transform().Find("pnlSnsHead/HttpImage/NobeImag").get_gameObject();
                if (obj4 != null)
                {
                    MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(obj4.GetComponent<Image>(), (int) ent.stGameVip.dwHeadIconId);
                }
            }
            playerNode.get_transform().Find("pnlSnsHead/HttpImage/bg/level").get_gameObject().GetComponent<Text>().set_text(ent.level);
            GameObject pNode = playerNode.get_transform().Find("textImgNode").get_gameObject();
            if (pNode != null)
            {
                for (int j = 0; j < pNode.get_transform().get_childCount(); j++)
                {
                    Object.Destroy(pNode.get_transform().GetChild(j).get_gameObject());
                }
            }
            bool flag2 = false;
            for (int i = 0; i < ent.TextObjList.Count; i++)
            {
                CTextImageNode node = ent.TextObjList[i];
                if (node.bText)
                {
                    if (!flag2)
                    {
                        flag2 = node.posY <= -52f;
                    }
                    if (ent.stGameVip != null)
                    {
                        this.Create_Content(pNode, node.content, true, node.posX, node.posY, node.width, ent.type == EChaterType.Self, ent.stGameVip.dwCurLevel > 0, false);
                    }
                }
                else if (ent.stGameVip != null)
                {
                    this.Create_Content(pNode, node.content, false, node.posX, node.posY, node.width, ent.type == EChaterType.Self, ent.stGameVip.dwCurLevel > 0, false);
                }
            }
            RectTransform transform = playerNode.get_transform().Find("textImgNode").get_transform() as RectTransform;
            if (ent.numLine == 1)
            {
                transform.set_sizeDelta(new Vector2(ent.final_width, single_line_height));
            }
            else if (ent.numLine == 2)
            {
                transform.set_sizeDelta(new Vector2(ent.final_width, double_line_height));
            }
            else
            {
                transform.set_sizeDelta(new Vector2(ent.final_width, trible_line_height));
            }
        }
    }

    private void SortChannels()
    {
        EChatChannel none = EChatChannel.None;
        for (int i = 0; i < this.curChannels.Count; i++)
        {
            none = this.curChannels[i];
            if ((none == EChatChannel.Friend) && (this.GetUnReadCount(none) > 0))
            {
                this.curChannels.RemoveAt(i);
                this.curChannels.Insert(0, (uint) none);
                return;
            }
            if ((none == EChatChannel.Guild) && (this.GetUnReadCount(none) > 0))
            {
                this.curChannels.RemoveAt(i);
                this.curChannels.Insert(0, (uint) none);
                return;
            }
        }
    }

    public void Update()
    {
        if (this.m_inputTextChanged && (this.inputField != null))
        {
            this.m_inputTextChanged = false;
            string str = this.inputField.get_text();
            if ((str != null) && (str.Length > ChatMaxLength))
            {
                this.inputField.DeactivateInputField();
                this.inputField.set_text(str.Substring(0, ChatMaxLength));
                Singleton<CUIManager>.GetInstance().OpenMessageBox(string.Format(Singleton<CTextManager>.instance.GetText("chat_input_max"), ChatMaxLength), false);
            }
        }
    }

    public void UpView(bool bup)
    {
        if (this.chatForm != null)
        {
            if (bup)
            {
                this.chatForm.SetPriority(enFormPriority.Priority5);
            }
            else
            {
                this.chatForm.RestorePriority();
            }
        }
    }

    public EChatChannel CurTab
    {
        get
        {
            return this._tab;
        }
        set
        {
            if (this._tab != value)
            {
                this._tab = value;
                this.SetChatFaceShow(false);
                this.SetShow(this.LobbyScript_cg, false);
                this.SetShow(this.FriendTabListScript_cg, false);
                if (this.bubbleNode != null)
                {
                    this.bubbleNode.CustomSetActive(false);
                }
                this.toolBarNode.CustomSetActive(true);
                this.info_node_obj.CustomSetActive(false);
                if (Singleton<CChatController>.instance.view != null)
                {
                    Singleton<CChatController>.instance.view.ShowLoudSpeaker(false, null);
                }
                this.Flag_Readed(this.CurTab);
                GameObject widget = this.chatForm.GetWidget(0);
                widget.CustomSetActive(false);
                switch (this._tab)
                {
                    case EChatChannel.Team:
                        this.SetShow(this.LobbyScript_cg, true);
                        this.Refresh_ChatEntity_List(true, EChatChannel.None);
                        break;

                    case EChatChannel.Room:
                        this.SetShow(this.LobbyScript_cg, true);
                        this.Refresh_ChatEntity_List(true, EChatChannel.None);
                        break;

                    case EChatChannel.Lobby:
                        if ((Singleton<CChatController>.instance.view != null) && (Singleton<CLoudSpeakerSys>.instance.CurLoudSpeaker != null))
                        {
                            Singleton<CChatController>.instance.view.ShowLoudSpeaker(true, Singleton<CLoudSpeakerSys>.instance.CurLoudSpeaker);
                        }
                        this.SetShow(this.LobbyScript_cg, true);
                        this.Refresh_ChatEntity_List(true, EChatChannel.None);
                        break;

                    case EChatChannel.GuildMatchTeam:
                        this.SetShow(this.LobbyScript_cg, true);
                        this.Refresh_ChatEntity_List(true, EChatChannel.None);
                        break;

                    case EChatChannel.Friend:
                        this.SetShow(this.FriendTabListScript_cg, true);
                        this.Refresh_ChatEntity_List(true, EChatChannel.None);
                        this.toolBarNode.CustomSetActive(false);
                        this.Process_Friend_Tip();
                        break;

                    case EChatChannel.Guild:
                        this.SetShow(this.LobbyScript_cg, true);
                        this.Refresh_ChatEntity_List(true, EChatChannel.None);
                        break;

                    case EChatChannel.GuildRecruit:
                        widget.CustomSetActive(true);
                        this.RefreshGuildRecruitList();
                        this.RefreshGuildRecruitInfoNode();
                        break;

                    case EChatChannel.Friend_Chat:
                        this.SetShow(this.LobbyScript_cg, true);
                        this.Refresh_ChatEntity_List(true, EChatChannel.None);
                        break;

                    case EChatChannel.Settle:
                        this.SetShow(this.LobbyScript_cg, true);
                        this.Refresh_ChatEntity_List(true, EChatChannel.None);
                        break;
                }
                this.Refresh_All_RedPoint();
            }
        }
    }

    public class CChatViewEntryNode
    {
        private GameObject channel_default;
        private Image channel_friend;
        private Image channel_guild;
        private Image channel_guildMatchTeam;
        private Image channel_lobby;
        private Image channel_room;
        private Image channel_speaker;
        private Image channel_team;
        private GameObject entry_bubble;
        private Text entry_bubble_CountText;
        private GameObject entry_node;
        private GameObject entry_node_lobby_bg;
        private GameObject entry_node_speaker;
        public GameObject image_template;
        public GameObject text_template;
        private Text txt_down;

        public void Clear()
        {
            this.txt_down = null;
            this.channel_friend = null;
            this.channel_guild = null;
            this.channel_guildMatchTeam = null;
            this.channel_lobby = null;
            this.channel_room = null;
            this.channel_team = null;
            this.channel_speaker = null;
            this.channel_default = null;
            this.entry_node = null;
            this.entry_node_lobby_bg = null;
            this.entry_node_speaker = null;
            this.text_template = null;
            this.image_template = null;
            this.entry_bubble = null;
            this.entry_bubble_CountText = null;
        }

        public void Clear_EntryForm_Node()
        {
            if ((this.txt_down != null) && (this.txt_down.get_gameObject() != null))
            {
                GameObject obj2 = this.txt_down.get_gameObject();
                for (int i = 0; i < obj2.get_transform().get_childCount(); i++)
                {
                    Object.Destroy(obj2.get_transform().GetChild(i).get_gameObject());
                }
            }
        }

        private void Create_Content(GameObject pNode, string content, bool bText, float x, float y, float width, bool bSelf, bool bVip = false, bool bUse_Entry = false)
        {
            if (bText)
            {
                if ((this.text_template != null) && (pNode != null))
                {
                    Text text = this.GetText("text", this.text_template, pNode);
                    text.set_text(CUIUtility.RemoveEmoji(content));
                    if (!bVip)
                    {
                        if (bSelf)
                        {
                            text.set_color(new Color(0f, 0f, 0f));
                        }
                        else
                        {
                            text.set_color(CUIUtility.s_Color_White);
                        }
                    }
                    else if (bSelf)
                    {
                        text.set_color(CUIUtility.s_Text_Color_Vip_Chat_Self);
                    }
                    else
                    {
                        text.set_color(CUIUtility.s_Text_Color_Vip_Chat_Other);
                    }
                    if (bUse_Entry)
                    {
                        text.set_color(CUIUtility.s_Color_White);
                    }
                    if (bUse_Entry)
                    {
                        text.get_rectTransform().set_anchoredPosition(new Vector2(x, (float) -CChatParser.chat_entry_lineHeight));
                    }
                    else
                    {
                        text.get_rectTransform().set_anchoredPosition(new Vector2(x, y));
                    }
                    text.get_rectTransform().set_sizeDelta(new Vector2(width, text.get_rectTransform().get_sizeDelta().y));
                }
            }
            else
            {
                int num;
                int.TryParse(content, out num);
                this.GetImage(num, this.image_template, pNode).get_rectTransform().set_anchoredPosition(new Vector2(x, y));
            }
        }

        private Image GetImage(int index, GameObject template, GameObject pNode)
        {
            GameObject obj2 = Object.Instantiate(template);
            obj2.set_name("image");
            obj2.CustomSetActive(true);
            obj2.get_transform().SetParent(pNode.get_transform());
            Image component = obj2.GetComponent<Image>();
            component.get_rectTransform().set_localPosition(new Vector3(0f, 0f, 0f));
            component.get_rectTransform().set_pivot(new Vector2(0f, 0f));
            component.get_rectTransform().set_localScale(new Vector3(1f, 1f, 1f));
            if (index > 0x4d)
            {
                index = 0x4d;
            }
            if (index < 1)
            {
                index = 1;
            }
            component.SetSprite(string.Format("UGUI/Sprite/Dynamic/ChatFace/{0}", index), null, true, false, false, false);
            component.SetNativeSize();
            return component;
        }

        private Text GetText(string name, GameObject template, GameObject pNode)
        {
            GameObject obj2 = Object.Instantiate(template);
            obj2.set_name("text");
            obj2.CustomSetActive(true);
            obj2.get_transform().SetParent(pNode.get_transform());
            Text component = obj2.GetComponent<Text>();
            component.get_rectTransform().set_localPosition(new Vector3(0f, 0f, 0f));
            component.get_rectTransform().set_pivot(new Vector2(0f, 0f));
            component.get_rectTransform().set_localScale(new Vector3(1f, 1f, 1f));
            return component;
        }

        private void InitInsideNode(GameObject entryNode)
        {
            this.entry_node = entryNode;
            this.entry_node.CustomSetActive(true);
            this.entry_node_speaker = Utility.FindChild(this.entry_node, "entry_node_speaker");
            this.entry_node_speaker.CustomSetActive(false);
            this.entry_node_lobby_bg = Utility.FindChild(this.entry_node, "normal_node/LobbyBg");
            this.txt_down = Utility.GetComponetInChild<Text>(this.entry_node, "normal_node/txt_down");
            this.txt_down.set_text(string.Empty);
            this.channel_friend = this.entry_node.get_transform().FindChild("normal_node/channel_img/friend").GetComponent<Image>();
            this.channel_guild = this.entry_node.get_transform().FindChild("normal_node/channel_img/gulid").GetComponent<Image>();
            this.channel_lobby = this.entry_node.get_transform().FindChild("normal_node/channel_img/lobby").GetComponent<Image>();
            this.channel_room = this.entry_node.get_transform().FindChild("normal_node/channel_img/room").GetComponent<Image>();
            this.channel_team = this.entry_node.get_transform().FindChild("normal_node/channel_img/team").GetComponent<Image>();
            this.channel_speaker = this.entry_node.get_transform().FindChild("normal_node/channel_img/speaker").GetComponent<Image>();
            this.channel_default = Utility.FindChild(this.entry_node, "normal_node/channel_img/default");
            Transform transform = this.entry_node.get_transform().FindChild("normal_node/channel_img/guildMatchTeam");
            if (transform != null)
            {
                this.channel_guildMatchTeam = transform.GetComponent<Image>();
            }
            this.entry_bubble = Utility.FindChild(this.entry_node, "bubble");
            if (this.entry_bubble != null)
            {
                this.entry_bubble_CountText = this.entry_bubble.get_transform().FindChild("Text").GetComponent<Text>();
            }
        }

        public void Refresh_EntryForm(GameObject entryGameObject)
        {
            this.Clear();
            if (entryGameObject != null)
            {
                this.text_template = entryGameObject.get_transform().Find("template/Text_template").get_gameObject();
                this.image_template = entryGameObject.get_transform().Find("template/Image_template").get_gameObject();
                this.InitInsideNode(entryGameObject);
                CChatEntity entryEntity = Singleton<CChatController>.GetInstance().model.sysData.entryEntity;
                if ((this.txt_down != null) && (this.txt_down.get_gameObject() != null))
                {
                    GameObject pNode = this.txt_down.get_gameObject();
                    this.SetEntryChannelImage(Singleton<CChatController>.GetInstance().model.sysData.CurChannel);
                    this.Clear_EntryForm_Node();
                    bool bActive = Singleton<CChatController>.GetInstance().model.sysData.CurChannel == EChatChannel.Speaker;
                    this.entry_node_lobby_bg.CustomSetActive(!bActive);
                    this.entry_node_speaker.CustomSetActive(bActive);
                    if (bActive)
                    {
                        CUIAutoScroller component = this.entry_node_speaker.GetComponent<CUIAutoScroller>();
                        component.SetText(CUIUtility.RemoveEmoji(entryEntity.text));
                        component.StopAutoScroll();
                        component.StartAutoScroll(true);
                    }
                    else if (entryEntity.TextObjList.Count > 0)
                    {
                        for (int i = 0; i < entryEntity.TextObjList.Count; i++)
                        {
                            CTextImageNode node = entryEntity.TextObjList[i];
                            if (node.bText)
                            {
                                this.Create_Content(pNode, node.content, true, node.posX, node.posY, node.width, entryEntity.type == EChaterType.Self, false, true);
                            }
                            else
                            {
                                this.Create_Content(pNode, node.content, false, node.posX, node.posY, node.width, entryEntity.type == EChaterType.Self, false, true);
                            }
                        }
                    }
                    else
                    {
                        this.Create_Content(pNode, Singleton<CTextManager>.instance.GetText("ChatEntry_Default_Text"), true, 0f, 0f, 200f, true, false, true);
                        this.SetEntryChannelImage(EChatChannel.Default);
                    }
                    this.SetBubbleCount(Singleton<CChatController>.instance.model.channelMgr.GetAllFriendUnreadCount());
                }
            }
        }

        public void SetBubbleCount(int count)
        {
            if (count > 0x63)
            {
                count = 0x63;
            }
            this.entry_bubble.CustomSetActive(count > 0);
            if (this.entry_bubble_CountText != null)
            {
                this.entry_bubble_CountText.set_text(count.ToString());
            }
        }

        public void SetEntryChannelImage(EChatChannel v)
        {
            if (this.channel_friend != null)
            {
                this.channel_friend.get_gameObject().CustomSetActive(false);
            }
            if (this.channel_guild != null)
            {
                this.channel_guild.get_gameObject().CustomSetActive(false);
            }
            if (this.channel_guildMatchTeam != null)
            {
                this.channel_guildMatchTeam.get_gameObject().CustomSetActive(false);
            }
            if (this.channel_lobby != null)
            {
                this.channel_lobby.get_gameObject().CustomSetActive(false);
            }
            if (this.channel_room != null)
            {
                this.channel_room.get_gameObject().CustomSetActive(false);
            }
            if (this.channel_team != null)
            {
                this.channel_team.get_gameObject().CustomSetActive(false);
            }
            if (this.channel_speaker != null)
            {
                this.channel_speaker.get_gameObject().CustomSetActive(false);
            }
            if (this.channel_default != null)
            {
                this.channel_default.get_gameObject().CustomSetActive(false);
            }
            switch (v)
            {
                case EChatChannel.Team:
                    if (this.channel_team != null)
                    {
                        this.channel_team.get_gameObject().CustomSetActive(true);
                    }
                    return;

                case EChatChannel.Room:
                case EChatChannel.Settle:
                    if (this.channel_room != null)
                    {
                        this.channel_room.get_gameObject().CustomSetActive(true);
                    }
                    return;

                case EChatChannel.Lobby:
                    if (this.channel_lobby != null)
                    {
                        this.channel_lobby.get_gameObject().CustomSetActive(true);
                    }
                    return;

                case EChatChannel.GuildMatchTeam:
                    if (this.channel_guildMatchTeam != null)
                    {
                        this.channel_guildMatchTeam.get_gameObject().CustomSetActive(true);
                    }
                    return;

                case EChatChannel.Friend:
                    if (this.channel_friend != null)
                    {
                        this.channel_friend.get_gameObject().CustomSetActive(true);
                    }
                    return;

                case EChatChannel.Guild:
                    if (this.channel_guild != null)
                    {
                        this.channel_guild.get_gameObject().CustomSetActive(true);
                    }
                    return;

                case EChatChannel.Default:
                    if (this.channel_default != null)
                    {
                        this.channel_default.CustomSetActive(true);
                    }
                    return;
            }
        }

        public void SetVisivble(bool bShow)
        {
            this.entry_node.CustomSetActive(bShow);
        }
    }

    private enum enChatFormWidgets
    {
        GuildRecruitList
    }
}

