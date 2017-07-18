namespace Assets.Scripts.GameSystem
{
    using Apollo;
    using Assets.Scripts.UI;
    using com.tencent.pandora;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class PandroaSys : MonoSingleton<PandroaSys>
    {
        private bool m_bInit;
        public bool m_bOpenWeixinZone;
        public bool m_bShowBoxBtn;
        private bool m_bShowPopNew;
        public bool m_bShowRedPoint;
        public bool m_bShowWeixinZone;
        private bool m_bstartOPenRedBox;

        protected override void Init()
        {
            base.Init();
        }

        private void InitEvent()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pandroa_ShowActBox, new CUIEventManager.OnUIEventHandler(this.OnShowActBox));
        }

        private void InitPara()
        {
            ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
            string str = "Smoba";
            string openId = accountInfo.OpenId;
            string str3 = "qq";
            string str4 = string.Empty;
            string str5 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID.ToString();
            string str6 = string.Empty;
            string str7 = string.Empty;
            foreach (ApolloToken token in accountInfo.TokenList)
            {
                if (ApolloConfig.platform == ApolloPlatform.Wechat)
                {
                    if (token.Type == ApolloTokenType.Access)
                    {
                        str6 = token.Value;
                    }
                }
                else if (ApolloConfig.platform == ApolloPlatform.QQ)
                {
                    if (token.Type == ApolloTokenType.Pay)
                    {
                        str7 = token.Value;
                    }
                    if (token.Type == ApolloTokenType.Access)
                    {
                        str6 = token.Value;
                    }
                }
            }
            if (ApolloConfig.platform == ApolloPlatform.QQ)
            {
                str3 = "qq";
                if (Application.get_platform() == 11)
                {
                    str4 = "1";
                }
                else if (Application.get_platform() == 8)
                {
                    str4 = "2";
                }
            }
            else if (ApolloConfig.platform == ApolloPlatform.Wechat)
            {
                str3 = "wx";
                if (Application.get_platform() == 11)
                {
                    str4 = "3";
                }
                else if (Application.get_platform() == 8)
                {
                    str4 = "4";
                }
            }
            string str8 = MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString();
            string appID = ApolloConfig.appID;
            if (ApolloConfig.platform == ApolloPlatform.Wechat)
            {
                appID = ApolloConfig.WXAppID;
            }
            string appVersion = CVersion.GetAppVersion();
            string str11 = "1";
            GameObject thePandoraParent = base.get_gameObject();
            Pandora.Instance.SetPandoraParent(thePandoraParent);
            Pandora.Instance.SetPanelBaseDepth(0x3e8);
            Pandora.Instance.SetCallback(new Action<Dictionary<string, string>>(this.OnPandoraEvent));
            Pandora.Instance.SetGetDjImageCallback(new Func<GameObject, int, int, int>(this, (IntPtr) this.OnGetDjImageCallback));
            Dictionary<string, string> dictPara = new Dictionary<string, string>();
            dictPara.Add("sOpenId", openId);
            dictPara.Add("sServiceType", str);
            dictPara.Add("sAcountType", str3);
            dictPara.Add("sArea", str4);
            dictPara.Add("sPartition", str8);
            dictPara.Add("sAppId", appID);
            dictPara.Add("sRoleId", str5);
            dictPara.Add("sAccessToken", str6);
            dictPara.Add("sPayToken", str7);
            dictPara.Add("sGameVer", appVersion);
            dictPara.Add("sPlatID", str11);
            Pandora.Instance.SetUserData(dictPara);
        }

        public void InitSys()
        {
            if (!this.m_bInit)
            {
                this.InitEvent();
                Debug.Log("Pandora InitSys");
                this.m_bInit = true;
                this.m_bShowPopNew = false;
                this.m_bShowBoxBtn = false;
                this.m_bShowRedPoint = false;
                Pandora.Instance.Init();
                this.InitPara();
            }
        }

        public void InitWechatLink()
        {
            if (ApolloConfig.platform == ApolloPlatform.Wechat)
            {
                ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
                string str = "Smoba";
                string openId = accountInfo.OpenId;
                string str3 = "qq";
                string str4 = string.Empty;
                string str5 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID.ToString();
                string str6 = string.Empty;
                string str7 = string.Empty;
                foreach (ApolloToken token in accountInfo.TokenList)
                {
                    if (ApolloConfig.platform == ApolloPlatform.Wechat)
                    {
                        if (token.Type == ApolloTokenType.Access)
                        {
                            str6 = token.Value;
                        }
                    }
                    else if (ApolloConfig.platform == ApolloPlatform.QQ)
                    {
                        if (token.Type == ApolloTokenType.Pay)
                        {
                            str7 = token.Value;
                        }
                        if (token.Type == ApolloTokenType.Access)
                        {
                            str6 = token.Value;
                        }
                    }
                }
                if (ApolloConfig.platform == ApolloPlatform.QQ)
                {
                    str3 = "qq";
                    if (Application.get_platform() == 11)
                    {
                        str4 = "1";
                    }
                    else if (Application.get_platform() == 8)
                    {
                        str4 = "2";
                    }
                }
                else if (ApolloConfig.platform == ApolloPlatform.Wechat)
                {
                    str3 = "wx";
                    if (Application.get_platform() == 11)
                    {
                        str4 = "3";
                    }
                    else if (Application.get_platform() == 8)
                    {
                        str4 = "4";
                    }
                }
                string str8 = MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString();
                string appID = ApolloConfig.appID;
                if (ApolloConfig.platform == ApolloPlatform.Wechat)
                {
                    appID = ApolloConfig.WXAppID;
                }
                string appVersion = CVersion.GetAppVersion();
                string str11 = "1";
                Dictionary<string, string> userDataDict = new Dictionary<string, string>();
                userDataDict.Add("sOpenId", openId);
                userDataDict.Add("sServiceType", str);
                userDataDict.Add("sAcountType", str3);
                userDataDict.Add("sArea", str4);
                userDataDict.Add("sPartition", str8);
                userDataDict.Add("sAppId", appID);
                userDataDict.Add("sRoleId", str5);
                userDataDict.Add("sAccessToken", str6);
                userDataDict.Add("sPayToken", str7);
                userDataDict.Add("sGameVer", appVersion);
                userDataDict.Add("sPlatID", str11);
                this.m_bShowWeixinZone = false;
                if ((ApolloConfig.platform == ApolloPlatform.Wechat) && this.m_bOpenWeixinZone)
                {
                    if (PlayerPrefs.GetInt("SHOW_WEIXINZONE") >= 1)
                    {
                        this.m_bShowWeixinZone = true;
                    }
                    WeChatLink.Instance.BeginGetGameZoneUrl(userDataDict, new Action<Dictionary<string, string>>(this.OnGetGameZoneUrl));
                }
            }
        }

        public int OnGetDjImageCallback(GameObject imgObj, int itemType, int itemID)
        {
            if (((itemType < 0) || (itemID < 0)) || (imgObj == null))
            {
                return -1;
            }
            CUseable useable = CUseableManager.CreateUseable((COM_ITEM_TYPE) itemType, (uint) itemID, 0);
            if (useable == null)
            {
                return -2;
            }
            string iconPath = useable.GetIconPath();
            Image component = imgObj.GetComponent<Image>();
            int num = 0;
            if (component == null)
            {
                imgObj.AddComponent<Image2>();
            }
            else
            {
                num = 1;
            }
            Image image = imgObj.GetComponent<Image>();
            if (image != null)
            {
                CUIUtility.SetImageSprite(image, iconPath, null, true, false, false, false);
            }
            return num;
        }

        private void OnGetGameZoneUrl(Dictionary<string, string> mDic)
        {
            if ((mDic != null) && mDic.ContainsKey("showGameZone"))
            {
                if (mDic["showGameZone"] == "1")
                {
                    Debug.Log("revce OnGetGameZoneUrl ");
                    this.m_bShowWeixinZone = true;
                    PlayerPrefs.SetInt("SHOW_WEIXINZONE", 2);
                    PlayerPrefs.Save();
                }
                else
                {
                    this.m_bShowWeixinZone = false;
                    PlayerPrefs.SetInt("SHOW_WEIXINZONE", 0);
                    PlayerPrefs.Save();
                }
            }
        }

        public void OnPandoraEvent(Dictionary<string, string> dictData)
        {
            Logger.DEBUG("OnPandoraEvent enter");
            if (this.m_bInit && ((dictData != null) && dictData.ContainsKey("type")))
            {
                string str = dictData["type"];
                string s = string.Empty;
                if (dictData.ContainsKey("content"))
                {
                    s = dictData["content"];
                }
                if (str == "redpoint")
                {
                    int result = 0;
                    int.TryParse(s, out result);
                    if (result <= 0)
                    {
                        this.m_bShowRedPoint = false;
                    }
                    else
                    {
                        this.m_bShowRedPoint = true;
                    }
                    Singleton<ActivitySys>.GetInstance().PandroaUpdateBtn();
                    Singleton<EventRouter>.instance.BroadCastEvent("IDIPNOTICE_UNREAD_NUM_UPDATE");
                }
                else if (str.Equals("showIcon"))
                {
                    if (s.Equals("1"))
                    {
                        this.m_bShowBoxBtn = true;
                    }
                    else
                    {
                        this.m_bShowBoxBtn = false;
                    }
                    Singleton<ActivitySys>.GetInstance().PandroaUpdateBtn();
                }
                else if (str.Equals("closePopNews"))
                {
                    this.m_bShowPopNew = true;
                }
                else if (str.Equals("ShowGameWindow"))
                {
                    int num2 = 0;
                    int.TryParse(s, out num2);
                    Logger.DEBUG("ShowGameWindow:" + num2);
                    if (num2 > 0)
                    {
                        CUICommonSystem.JumpForm((RES_GAME_ENTRANCE_TYPE) num2, 0, 0);
                    }
                }
                else if (str.Equals("refreshDiamond"))
                {
                    Debug.Log("pandorasys ShowGameWindow");
                    Singleton<CPaySystem>.GetInstance().OnPandroiaPaySuccss();
                }
                else if (str.Equals("share"))
                {
                    string str3 = string.Empty;
                    string url = string.Empty;
                    string title = string.Empty;
                    string desc = string.Empty;
                    string str7 = string.Empty;
                    int num3 = 0;
                    if (dictData.ContainsKey("sendType"))
                    {
                        str3 = dictData["sendType"];
                        num3++;
                    }
                    if (dictData.ContainsKey("url"))
                    {
                        url = dictData["url"];
                        num3++;
                    }
                    if (dictData.ContainsKey("title"))
                    {
                        title = dictData["title"];
                        num3++;
                    }
                    if (dictData.ContainsKey("desc"))
                    {
                        desc = dictData["desc"];
                        num3++;
                    }
                    if (dictData.ContainsKey("static_kind"))
                    {
                        str7 = dictData["static_kind"];
                        num3++;
                    }
                    if (num3 == 5)
                    {
                        int num4 = 0;
                        if (int.TryParse(str3, out num4))
                        {
                            Singleton<ApolloHelper>.GetInstance().ShareRedBox(num4, url, title, desc);
                        }
                    }
                    else
                    {
                        Debug.Log("pandroa sys parm error");
                    }
                }
            }
        }

        private void OnShowActBox(CUIEvent uiEvent)
        {
            if (this.m_bInit)
            {
                this.ShowActBox();
            }
        }

        public void PausePandoraSys(bool bPause)
        {
            if (this.m_bInit)
            {
                Dictionary<string, string> cmdDict = new Dictionary<string, string>();
                if (bPause)
                {
                    cmdDict["type"] = "inMainSence";
                    cmdDict["content"] = "0";
                    Pandora.Instance.Do(cmdDict);
                }
                else
                {
                    cmdDict["type"] = "inMainSence";
                    cmdDict["content"] = "1";
                    Pandora.Instance.Do(cmdDict);
                }
            }
        }

        public void ShowActBox()
        {
            Debug.Log("Pandora ShowActBox1");
            if (this.m_bInit)
            {
                Debug.Log("Pandora ShowActBox2");
                Dictionary<string, string> cmdDict = new Dictionary<string, string>();
                cmdDict["type"] = "open";
                cmdDict["content"] = "Lucky";
                Pandora.Instance.Do(cmdDict);
            }
        }

        public void ShowActiveActBoxBtn(CUIFormScript uiForm)
        {
            if (uiForm != null)
            {
                if (this.m_bInit)
                {
                    string str = Singleton<CTextManager>.GetInstance().GetText("pandroa_Btn_Text");
                    Transform transform = uiForm.get_gameObject().get_transform().Find("Panel/PandroaBtn");
                    if (transform != null)
                    {
                        if (this.m_bShowBoxBtn)
                        {
                            transform.get_gameObject().CustomSetActive(true);
                        }
                        else
                        {
                            transform.get_gameObject().CustomSetActive(false);
                        }
                        Transform transform2 = transform.Find("Hotspot");
                        if (transform2 != null)
                        {
                            if (this.m_bShowRedPoint)
                            {
                                transform2.get_gameObject().CustomSetActive(true);
                            }
                            else
                            {
                                transform2.get_gameObject().CustomSetActive(false);
                            }
                        }
                        Transform transform3 = transform.Find("Text");
                        if (transform3 != null)
                        {
                            Text component = transform3.GetComponent<Text>();
                            if (component != null)
                            {
                                component.set_text(str);
                            }
                        }
                    }
                }
                else
                {
                    Transform transform4 = uiForm.get_gameObject().get_transform().Find("Panel/PandroaBtn");
                    if (transform4 != null)
                    {
                        transform4.get_gameObject().CustomSetActive(false);
                        Transform transform5 = transform4.Find("Hotspot");
                        if (transform5 != null)
                        {
                            transform5.get_gameObject().CustomSetActive(false);
                        }
                    }
                }
            }
        }

        public void ShowPopNews()
        {
            if (this.m_bInit && !this.m_bShowPopNew)
            {
                this.m_bShowPopNew = true;
                Dictionary<string, string> cmdDict = new Dictionary<string, string>();
                cmdDict["type"] = "open";
                cmdDict["content"] = "LuckyPop";
                Pandora.Instance.Do(cmdDict);
            }
        }

        public void StartOpenRedBox(int bWin, int bMvp, int bLegaendary, int bPENTAKILL, int bQUATARYKIL, int bTRIPLEKILL)
        {
            Debug.Log("Pandora StartOpenRedBox1");
            if (this.m_bInit)
            {
                this.m_bstartOPenRedBox = true;
                Debug.Log("Pandora StartOpenRedBox2");
                Dictionary<string, string> cmdDict = new Dictionary<string, string>();
                cmdDict["type"] = "open";
                cmdDict["content"] = "RedPacket";
                cmdDict["is_legendary"] = bLegaendary.ToString();
                cmdDict["is_mvp"] = bMvp.ToString();
                cmdDict["is_penta_kill"] = bPENTAKILL.ToString();
                cmdDict["is_quadra_kill"] = bQUATARYKIL.ToString();
                cmdDict["is_triple_kill"] = bTRIPLEKILL.ToString();
                cmdDict["is_victory"] = bWin.ToString();
                Pandora.Instance.Do(cmdDict);
            }
        }

        public void StopRedBox()
        {
            Debug.Log("Pandora StopRedBox1");
            if (this.m_bInit && this.m_bstartOPenRedBox)
            {
                this.m_bstartOPenRedBox = false;
                Debug.Log("Pandora StopRedBox2");
                Dictionary<string, string> cmdDict = new Dictionary<string, string>();
                cmdDict["type"] = "inSettlement";
                cmdDict["content"] = "0";
                Pandora.Instance.Do(cmdDict);
            }
        }

        public void UninitSys()
        {
            Debug.Log("Pandora UnInitSys");
            this.m_bInit = false;
            this.m_bShowPopNew = false;
            this.m_bShowBoxBtn = false;
            this.m_bShowRedPoint = false;
            this.m_bOpenWeixinZone = false;
            this.m_bShowWeixinZone = false;
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Pandroa_ShowActBox, new CUIEventManager.OnUIEventHandler(this.OnShowActBox));
            Pandora.Instance.Logout();
        }

        public bool ShowRedPoint
        {
            get
            {
                return this.m_bShowRedPoint;
            }
        }
    }
}

