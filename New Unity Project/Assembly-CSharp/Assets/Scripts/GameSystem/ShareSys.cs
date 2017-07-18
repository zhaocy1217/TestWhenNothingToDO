namespace Assets.Scripts.GameSystem
{
    using Apollo;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class ShareSys : MonoSingleton<ShareSys>
    {
        [CompilerGenerated]
        private static IDIPSys.LoadRCallBack <>f__am$cache2A;
        [CompilerGenerated]
        private static IDIPSys.LoadRCallBack <>f__am$cache2B;
        private float g_fDownloadOutTime = 10f;
        private const string HeroShowImgDir = "UGUI/Sprite/Dynamic/HeroShow/";
        private bool isRegisterd;
        private bool m_bAdreo306;
        private bool m_bClickShareFriendBtn;
        private bool m_bClickTimeLineBtn;
        public bool m_bHide;
        private bool m_bIsQQGameTeamOwner;
        private bool m_bShareHero;
        private bool m_bSharePvpForm;
        public bool m_bShowTimeline;
        public bool m_bWinPVPResult;
        private List<CLoadReq> m_DownLoadSkinList = new List<CLoadReq>();
        private Image m_FriendBtnImage;
        private string m_MarkStr = string.Empty;
        private string m_QQGameTeamStr = string.Empty;
        public COMDT_RECRUITMENT_SOURCE m_RecruitSource;
        private string m_RoomInfoStr = string.Empty;
        private string m_RoomModeId = string.Empty;
        public ShareActivityParam m_ShareActivityParam = new ShareActivityParam(false);
        public SHARE_INFO m_ShareInfo;
        private string m_sharePic = CFileManager.GetCachePath("share.jpg");
        public string m_SharePicCDNCachePath = string.Empty;
        private ListView<CSDT_SHARE_TLOG_INFO> m_ShareReportInfoList = new ListView<CSDT_SHARE_TLOG_INFO>();
        private CSPKG_JOINMULTIGAMEREQ m_ShareRoom;
        private Transform m_ShareSkinPicImage;
        private string m_ShareSkinPicLoading = string.Empty;
        private string m_ShareSkinPicNotFound = string.Empty;
        private string m_ShareSkinPicOutofTime = string.Empty;
        private string m_ShareStr = string.Empty;
        private COMDT_INVITE_JOIN_INFO m_ShareTeam;
        private static string[] m_Support3rdAppList = new string[] { "QQGameTeam", "PenguinEsport", "GameHelper" };
        private Transform m_TimelineBtn;
        private Image m_TimeLineBtnImage;
        private string m_WakeupOpenId = string.Empty;
        public static string s_formShareMysteryDiscountPath = "UGUI/Form/System/ShareUI/Form_ShareMystery_Discount.prefab";
        public static string s_formShareNewAchievementPath = "UGUI/Form/System/Achieve/Form_Achievement_Share.prefab";
        public static string s_formShareNewHeroPath = "UGUI/Form/System/ShareUI/Form_ShareNewHero.prefab";
        public static string s_formSharePVPPath = "UGUI/Form/System/ShareUI/Form_SharePVPResult.prefab";
        public static string s_imageSharePVPAchievement = (CUIUtility.s_Sprite_Dynamic_PvpAchievementShare_Dir + "Img_PVP_ShareAchievement_");
        public static readonly string SNS_SHARE_COMMON = "SNS_SHARE_SEND_COMMON";
        public static readonly string SNS_SHARE_RECALL_FRIEND = "SNS_SHARE_RECALL_FRIEND";
        public static readonly string SNS_SHARE_SEND_HEART = "SNS_SHARE_SEND_HEART";

        public void AddshareReportInfo(uint dwType, uint dwSubType)
        {
            bool flag = false;
            for (int i = 0; i < this.m_ShareReportInfoList.Count; i++)
            {
                CSDT_SHARE_TLOG_INFO csdt_share_tlog_info = this.m_ShareReportInfoList[i];
                if ((csdt_share_tlog_info.dwType == dwType) && (csdt_share_tlog_info.dwSubType == dwSubType))
                {
                    csdt_share_tlog_info.dwCnt++;
                    flag = true;
                }
            }
            if (!flag)
            {
                CSDT_SHARE_TLOG_INFO item = new CSDT_SHARE_TLOG_INFO();
                item.dwCnt = 1;
                item.dwType = dwType;
                item.dwSubType = dwSubType;
                this.m_ShareReportInfoList.Add(item);
            }
        }

        private void BtnGray(GameObject imageBtnObj, bool bShow)
        {
            if (imageBtnObj != null)
            {
                Image component = imageBtnObj.GetComponent<Image>();
                if (component != null)
                {
                    if (bShow)
                    {
                        component.set_color(new Color(1f, 1f, 1f, 1f));
                        component.GetComponent<CUIEventScript>().set_enabled(true);
                    }
                    else
                    {
                        component.set_color(new Color(0f, 1f, 1f, 1f));
                        component.GetComponent<CUIEventScript>().set_enabled(false);
                    }
                }
            }
        }

        private void BtnGray(Image imgeBtn, bool bShow)
        {
            if (imgeBtn != null)
            {
                if (bShow)
                {
                    imgeBtn.set_color(new Color(1f, 1f, 1f, 1f));
                    imgeBtn.GetComponent<CUIEventScript>().set_enabled(true);
                }
                else
                {
                    imgeBtn.set_color(new Color(0f, 1f, 1f, 1f));
                    imgeBtn.GetComponent<CUIEventScript>().set_enabled(false);
                }
            }
        }

        [DebuggerHidden]
        private IEnumerator Capture(Rect screenShotRect, Action<string> callback)
        {
            <Capture>c__Iterator2D iteratord = new <Capture>c__Iterator2D();
            iteratord.screenShotRect = screenShotRect;
            iteratord.callback = callback;
            iteratord.<$>screenShotRect = screenShotRect;
            iteratord.<$>callback = callback;
            iteratord.<>f__this = this;
            return iteratord;
        }

        private bool CheckEnterShareTeamLimit(ref string paramDevicePlatStr, ref string paramPlatformStr, bool CheckPlat = true, bool CheckDevicePlat = false)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xbf).dwConfValue;
            if ((masterRoleInfo != null) && (masterRoleInfo.PvpLevel < dwConfValue))
            {
                object[] replaceArr = new object[] { dwConfValue };
                Singleton<CUIManager>.GetInstance().OpenTips("Enter_Room_Level_Limit", true, 1f, null, replaceArr);
                return false;
            }
            if (CheckDevicePlat)
            {
                int result = 0;
                if (!int.TryParse(paramDevicePlatStr, out result) || (Application.get_platform() != result))
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("Enter_Room_Different_Device", true, 1.5f, null, new object[0]);
                    return false;
                }
            }
            if (CheckPlat)
            {
                int num3 = -1;
                if (!int.TryParse(paramPlatformStr, out num3) || (num3 != ApolloConfig.platform))
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("Enter_Room_Different_Platform", true, 1.5f, null, new object[0]);
                    return false;
                }
            }
            if (!Singleton<GameStateCtrl>.GetInstance().isBattleState && !Singleton<GameStateCtrl>.GetInstance().isLoadingState)
            {
                return true;
            }
            Singleton<CUIManager>.GetInstance().OpenTips("Enter_Room_InBattle", true, 1.5f, null, new object[0]);
            return false;
        }

        private bool CheckQQGameTeamInfo(COM_ROOM_TYPE roomType, byte mapType, uint mapID)
        {
            char[] separator = new char[] { '_' };
            string[] strArray = this.m_QQGameTeamStr.Split(separator);
            if (strArray == null)
            {
                return false;
            }
            int length = strArray.Length;
            if ((strArray == null) || (((length != 4) && (length != 10)) && (strArray.Length != 0x10)))
            {
                return false;
            }
            if (!this.IsSupport3rdAPP(strArray[0]))
            {
                return false;
            }
            string str = strArray[1];
            char[] chArray2 = new char[] { '-' };
            string[] strArray2 = str.Split(chArray2);
            if ((strArray2 == null) || (strArray2.Length != 3))
            {
                return false;
            }
            return (((((COM_ROOM_TYPE) Convert.ToUInt32(strArray2[0])) == roomType) && (Convert.ToByte(strArray2[1]) == mapType)) && (Convert.ToUInt32(strArray2[2]) == mapID));
        }

        private void ClearQQGameCreateInfo()
        {
            this.m_ShareStr = string.Empty;
            this.m_RoomModeId = string.Empty;
            this.m_ShareRoom = null;
            this.m_ShareTeam = null;
        }

        private void ClearRoomData()
        {
            this.m_ShareRoom = null;
            this.m_ShareStr = string.Empty;
        }

        public void ClearShareDataMsg()
        {
            this.ClearTeamDataMsg();
            this.ClearRoomData();
        }

        private void ClearTeamDataMsg()
        {
            this.m_ShareTeam = null;
            this.m_ShareStr = string.Empty;
        }

        public void CloseNewHeroForm(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CUICommonSystem.s_newHeroOrSkinPath);
            if (form != null)
            {
                DynamicShadow.EnableDynamicShow(form.get_gameObject(), false);
            }
            this.RemoveDownLoading(this.m_ShareInfo.shareSkinUrl);
            this.m_ShareInfo.clear();
            this.m_bShowTimeline = false;
            Singleton<CUIManager>.GetInstance().CloseForm(CUICommonSystem.s_newHeroOrSkinPath);
            Singleton<EventRouter>.instance.BroadCastEvent(EventID.Mall_Get_Product_OK);
        }

        public void CloseShareHeroForm(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().CloseForm(s_formShareNewHeroPath);
            this.m_bShareHero = false;
            this.m_bClickShareFriendBtn = false;
            this.m_ShareSkinPicImage = null;
            this.m_FriendBtnImage = null;
            this.m_TimeLineBtnImage = null;
        }

        [DebuggerHidden]
        public IEnumerator DownloadImageByTag2(string preUrl, CLoadReq req, LoadRCallBack3 callBack, string tagPath)
        {
            <DownloadImageByTag2>c__Iterator2C iteratorc = new <DownloadImageByTag2>c__Iterator2C();
            iteratorc.preUrl = preUrl;
            iteratorc.tagPath = tagPath;
            iteratorc.req = req;
            iteratorc.callBack = callBack;
            iteratorc.<$>preUrl = preUrl;
            iteratorc.<$>tagPath = tagPath;
            iteratorc.<$>req = req;
            iteratorc.<$>callBack = callBack;
            iteratorc.<>f__this = this;
            return iteratorc;
        }

        private string GetCDNSharePicUrl(string url, int type)
        {
            string str = string.Empty;
            if (type == 1)
            {
                return string.Format("{0}HeroSharePic/{1}.jpg", BannerImageSys.GlobalLoadPath, url);
            }
            if (type == 2)
            {
                str = string.Format("{0}SkinSharePic/{1}.jpg", BannerImageSys.GlobalLoadPath, url);
            }
            return str;
        }

        private GameObject GetCloseBtn(CUIFormScript form)
        {
            if (form != null)
            {
                if (form.m_formPath == s_formShareNewHeroPath)
                {
                    return form.GetWidget(2);
                }
                if (form.m_formPath == s_formSharePVPPath)
                {
                    return form.GetWidget(1);
                }
                if (form.m_formPath == s_formShareNewAchievementPath)
                {
                    return form.GetWidget(3);
                }
                if (form.m_formPath == s_formShareMysteryDiscountPath)
                {
                    return form.GetWidget(1);
                }
                if (form.m_formPath == PvpAchievementForm.s_formSharePVPDefeatPath)
                {
                    return form.GetWidget(0);
                }
                if (form.m_formPath == SettlementSystem.SettlementFormName)
                {
                    return form.get_gameObject().get_transform().FindChild("Panel/Btn_Share_PVP_DATA_CLOSE").get_gameObject();
                }
                if (form.m_formPath == "UGUI/Form/System/ShareUI/Form_SharePVPLadder.prefab")
                {
                    return form.get_gameObject().get_transform().FindChild("Button_Close").get_gameObject();
                }
                object[] inParameters = new object[] { form.m_formPath };
                DebugHelper.Assert(false, "ShareSys.GetCloseBtn(): error form path = {0}", inParameters);
            }
            return null;
        }

        private GameObject GetDisplayPanel(CUIFormScript form)
        {
            if (form != null)
            {
                if (form.m_formPath == s_formShareNewHeroPath)
                {
                    return form.GetWidget(0);
                }
                if (form.m_formPath == s_formSharePVPPath)
                {
                    return form.GetWidget(0);
                }
                if (form.m_formPath == s_formShareNewAchievementPath)
                {
                    return form.GetWidget(4);
                }
                if (form.m_formPath == PvpAchievementForm.s_formSharePVPDefeatPath)
                {
                    return form.GetWidget(2);
                }
                if (form.m_formPath == SettlementSystem.SettlementFormName)
                {
                    return form.get_gameObject().get_transform().FindChild("Panel").get_gameObject();
                }
                if (form.m_formPath == "UGUI/Form/System/ShareUI/Form_SharePVPLadder.prefab")
                {
                    return form.get_gameObject().get_transform().FindChild("ShareFrame").get_gameObject();
                }
                if (form.m_formPath == s_formShareMysteryDiscountPath)
                {
                    return form.get_gameObject().get_transform().FindChild("DiscountShow").get_gameObject();
                }
                object[] inParameters = new object[] { form.m_formPath };
                DebugHelper.Assert(false, "ShareSys.GetDisplayPanel(): error form path = {0}", inParameters);
            }
            return null;
        }

        private ELoadError GetLoadReq(CLoadReq url)
        {
            for (int i = 0; i < this.m_DownLoadSkinList.Count; i++)
            {
                CLoadReq req = this.m_DownLoadSkinList[i];
                if (url.m_Url == req.m_Url)
                {
                    CLoadReq req2 = this.m_DownLoadSkinList[i];
                    return req2.m_LoadError;
                }
            }
            return ELoadError.None;
        }

        private GameObject GetNotShowBtn(CUIFormScript form)
        {
            GameObject obj2 = null;
            if (form == null)
            {
                return null;
            }
            if (form.m_formPath == SettlementSystem.SettlementFormName)
            {
                obj2 = form.get_gameObject().get_transform().FindChild("Panel/Logo").get_gameObject();
            }
            return obj2;
        }

        private Rect GetScreenShotRect(GameObject displayeRect)
        {
            RectTransform transform = (displayeRect == null) ? new RectTransform() : displayeRect.GetComponent<RectTransform>();
            float num = transform.get_rect().get_width() * 0.5f;
            float num2 = transform.get_rect().get_height() * 0.5f;
            Vector3 vector = displayeRect.get_transform().TransformPoint(new Vector3(-num, -num2, 0f));
            vector = Singleton<CUIManager>.instance.FormCamera.WorldToScreenPoint(vector);
            Vector3 vector2 = displayeRect.get_transform().TransformPoint(new Vector3(num, num2, 0f));
            vector2 = Singleton<CUIManager>.instance.FormCamera.WorldToScreenPoint(vector2);
            return new Rect(vector.x, vector.y, Math.Abs((float) (vector.x - vector2.x)), Math.Abs((float) (vector.y - vector2.y)));
        }

        public void GShare(string buttonType, string sharePathPic)
        {
            this.m_bClickTimeLineBtn = true;
            this.Share(buttonType, sharePathPic);
        }

        protected override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_CloseNewHeroorSkin, new CUIEventManager.OnUIEventHandler(this.CloseNewHeroForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_NewHero, new CUIEventManager.OnUIEventHandler(this.OpenShareNewHeroForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_CloseNewHeroShareForm, new CUIEventManager.OnUIEventHandler(this.CloseShareHeroForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_ShareFriend, new CUIEventManager.OnUIEventHandler(this.ShareFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Player_Info_Pvp_Share, new CUIEventManager.OnUIEventHandler(this.ShareMyPlayInfoToFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_ShareTimeLine, new CUIEventManager.OnUIEventHandler(this.ShareTimeLine));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_ShareSavePic, new CUIEventManager.OnUIEventHandler(this.SavePic));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_SharePVPScore, new CUIEventManager.OnUIEventHandler(this.SettlementShareBtnHandle));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_SharePVPSCcoreClose, new CUIEventManager.OnUIEventHandler(this.OnCloseShowPVPSCore));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_MysteryDiscount, new CUIEventManager.OnUIEventHandler(this.ShareMysteryDiscount));
            uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x88).dwConfValue;
            Singleton<CTimerManager>.GetInstance().AddTimer((int) (dwConfValue * 0x3e8), -1, new CTimer.OnTimeUpHandler(this.OnReportShareInfo));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.SHARE_PVP_SETTLEDATA_CLOSE, new Action(this, (IntPtr) this.On_SHARE_PVP_SETTLEDATA_CLOSE));
            this.m_SharePicCDNCachePath = CFileManager.GetCachePath();
            string cachePath = CFileManager.GetCachePath("SkinSharePic");
            try
            {
                if (!Directory.Exists(cachePath))
                {
                    Directory.CreateDirectory(cachePath);
                }
                this.m_SharePicCDNCachePath = cachePath;
            }
            catch (Exception exception)
            {
                Debug.Log("sharesys cannot create dictionary " + exception);
                this.m_SharePicCDNCachePath = CFileManager.GetCachePath();
            }
            this.m_ShareSkinPicNotFound = Singleton<CTextManager>.GetInstance().GetText("Share_Skin_Pic_Error_NotFound");
            this.m_ShareSkinPicOutofTime = Singleton<CTextManager>.GetInstance().GetText("Share_Skin_Pic_Error_OutofTime");
            this.m_ShareSkinPicLoading = Singleton<CTextManager>.GetInstance().GetText("Share_Skin_Pic_Error_Loading");
            this.m_bAdreo306 = this.IsAdreo306();
            this.PreLoadPVPImage();
        }

        private bool IsAdreo306()
        {
            string str = SystemInfo.get_graphicsDeviceName().ToLower();
            char[] separator = new char[] { ' ', '\t', '\r', '\n', '+', '-', ':', '\0' };
            string[] strArray = str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (((strArray != null) && (strArray.Length != 0)) && (strArray[0] == "adreno"))
            {
                int result = 0;
                for (int i = 1; i < strArray.Length; i++)
                {
                    bool flag = int.TryParse(strArray[i], out result);
                    if (result == 0x132)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool isDownLoading(CLoadReq url)
        {
            for (int i = 0; i < this.m_DownLoadSkinList.Count; i++)
            {
                CLoadReq req = this.m_DownLoadSkinList[i];
                if (url.m_Url == req.m_Url)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsInstallPlatform()
        {
            if (Singleton<ApolloHelper>.GetInstance().IsPlatformInstalled(Singleton<ApolloHelper>.GetInstance().CurPlatform))
            {
                return true;
            }
            if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.Wechat)
            {
                Singleton<CUIManager>.GetInstance().OpenMessageBox("未安装微信，无法使用该功能", false);
            }
            else if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.QQ)
            {
                Singleton<CUIManager>.GetInstance().OpenMessageBox("未安装手机QQ，无法使用该功能", false);
            }
            return false;
        }

        public bool IsQQGameTeamCreate()
        {
            return !string.IsNullOrEmpty(this.m_QQGameTeamStr);
        }

        private bool IsSupport3rdAPP(string tag)
        {
            for (int i = 0; i < m_Support3rdAppList.Length; i++)
            {
                if (tag.Equals(m_Support3rdAppList[i]))
                {
                    return true;
                }
            }
            return false;
        }

        private void LoadShareSkinImage(CLoadReq loadReq, Image imageObj)
        {
            string url = loadReq.m_Url;
            string cachePath = loadReq.m_CachePath;
            string cDNSharePicUrl = this.GetCDNSharePicUrl(url, loadReq.m_Type);
            string str4 = MonoSingleton<IDIPSys>.GetInstance().ToMD5(cDNSharePicUrl);
            string path = CFileManager.CombinePath(cachePath, str4);
            if (File.Exists(path))
            {
                byte[] buffer = File.ReadAllBytes(path);
                Texture2D textured = new Texture2D(4, 4, 5, false);
                if ((textured.LoadImage(buffer) && this.m_bShareHero) && (imageObj != null))
                {
                    imageObj.get_gameObject().CustomSetActive(true);
                    if (this.m_bShareHero && (imageObj != null))
                    {
                        imageObj.SetSprite(Sprite.Create(textured, new Rect(0f, 0f, (float) textured.get_width(), (float) textured.get_height()), new Vector2(0.5f, 0.5f)), ImageAlphaTexLayout.None);
                    }
                    if (this.m_FriendBtnImage != null)
                    {
                        this.BtnGray(this.m_FriendBtnImage, true);
                    }
                    if ((this.m_TimeLineBtnImage != null) && !this.m_bShowTimeline)
                    {
                        this.BtnGray(this.m_TimeLineBtnImage, true);
                    }
                }
            }
            else
            {
                switch (this.GetLoadReq(loadReq))
                {
                    case ELoadError.NotFound:
                        Singleton<CUIManager>.GetInstance().OpenTips(this.m_ShareSkinPicNotFound, false, 1.5f, null, new object[0]);
                        return;

                    case ELoadError.OutOfTime:
                        Singleton<CUIManager>.GetInstance().OpenTips(this.m_ShareSkinPicOutofTime, false, 1.5f, null, new object[0]);
                        return;
                }
                Singleton<CUIManager>.GetInstance().OpenTips(this.m_ShareSkinPicLoading, false, 0.5f, null, new object[0]);
            }
        }

        private void On_SHARE_PVP_SETTLEDATA_CLOSE()
        {
            this.m_bSharePvpForm = false;
        }

        public void OnCloseShowPVPSCore(CUIEvent ievent)
        {
            Singleton<CUIManager>.GetInstance().CloseForm(s_formSharePVPPath);
            this.m_bSharePvpForm = false;
            MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.PvPShareFin, new uint[0]);
            Singleton<CChatController>.instance.ShowPanel(true, false);
        }

        private static void OnLoadNewHeroOrSkin3DModel(GameObject rawImage, uint heroId, uint skinId, bool bInitAnima)
        {
            CUI3DImageScript script = (rawImage == null) ? null : rawImage.GetComponent<CUI3DImageScript>();
            string objectName = CUICommonSystem.GetHeroPrefabPath(heroId, (int) skinId, true).ObjectName;
            GameObject model = (script == null) ? null : script.AddGameObject(objectName, false, false);
            CHeroAnimaSystem instance = Singleton<CHeroAnimaSystem>.GetInstance();
            instance.Set3DModel(model);
            if (model == null)
            {
                objectName = null;
            }
            else if (bInitAnima)
            {
                instance.InitAnimatList();
                instance.InitAnimatSoundList(heroId, skinId);
                instance.OnModePlayAnima("Come");
            }
        }

        private void OnReportShareInfo(int timerSequence)
        {
            if (Singleton<CBattleSystem>.instance.FormScript == null)
            {
                this.ReportShareInfo();
            }
        }

        public void OnShareCallBack()
        {
            IApolloSnsService service = IApollo.Instance.GetService(1) as IApolloSnsService;
            if ((service != null) && !this.isRegisterd)
            {
                service.onShareEvent += delegate (ApolloShareResult shareResponseInfo) {
                    object[] args = new object[] { shareResponseInfo.result, shareResponseInfo.platform, shareResponseInfo.drescription, shareResponseInfo.extInfo };
                    Debug.Log("sns += " + string.Format("share result:{0} \n share platform:{1} \n share description:{2}\n share extInfo:{3}", args));
                    if (shareResponseInfo.result == ApolloResult.Success)
                    {
                        if ((shareResponseInfo.extInfo != SNS_SHARE_SEND_HEART) && (shareResponseInfo.extInfo != SNS_SHARE_RECALL_FRIEND))
                        {
                            if (this.m_bClickTimeLineBtn)
                            {
                                this.m_bShowTimeline = true;
                                Singleton<EventRouter>.instance.BroadCastEvent<Transform>(EventID.SHARE_TIMELINE_SUCC, this.m_TimelineBtn);
                                this.UpdateTimelineBtn();
                            }
                            uint dwType = 0;
                            if (this.m_bShareHero)
                            {
                                dwType = 0;
                            }
                            else if (this.m_bSharePvpForm)
                            {
                                dwType = 1;
                            }
                            if (this.m_bClickShareFriendBtn)
                            {
                                if (ApolloConfig.platform == ApolloPlatform.Wechat)
                                {
                                    this.AddshareReportInfo(dwType, 3);
                                }
                                else if (ApolloConfig.platform == ApolloPlatform.QQ)
                                {
                                    this.AddshareReportInfo(dwType, 2);
                                }
                            }
                            else if (this.m_bClickTimeLineBtn)
                            {
                                if (ApolloConfig.platform == ApolloPlatform.Wechat)
                                {
                                    this.AddshareReportInfo(dwType, 5);
                                }
                                else if (ApolloConfig.platform == ApolloPlatform.QQ)
                                {
                                    this.AddshareReportInfo(dwType, 4);
                                }
                            }
                            CTaskSys.Send_Share_Task();
                            if (this.m_bClickTimeLineBtn)
                            {
                                this.SendShareActivityDoneMsg();
                            }
                            this.m_bClickTimeLineBtn = false;
                            this.m_bClickShareFriendBtn = false;
                        }
                    }
                    else
                    {
                        this.m_bClickTimeLineBtn = false;
                        this.m_bClickShareFriendBtn = false;
                    }
                };
                this.isRegisterd = true;
            }
        }

        [MessageHandler(0x10d4)]
        public static void OnShareReport(CSPkg msg)
        {
            Debug.Log("share report " + msg.stPkgData.stShareTLogRsp.iErrCode);
        }

        public void OpenShareNewHeroForm(CUIEvent uiEvent)
        {
            this.m_ShareActivityParam.clear();
            this.AddshareReportInfo(0, 0);
            this.m_bShareHero = true;
            this.m_bClickShareFriendBtn = false;
            if (this.m_ShareInfo.rewardType == COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO)
            {
                uint[] kShareParam = new uint[] { this.m_ShareInfo.heroId };
                this.m_ShareActivityParam.set(3, 1, kShareParam);
                this.ShowNewHeroShare(this.m_ShareInfo.heroId, this.m_ShareInfo.skinId, false, this.m_ShareInfo.rewardType, false);
            }
            else if (this.m_ShareInfo.rewardType == COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN)
            {
                uint skinId = this.m_ShareInfo.skinId;
                if ((this.m_ShareInfo.heroId != 0) && (this.m_ShareInfo.skinId != 0))
                {
                    skinId = CSkinInfo.GetSkinCfgId(this.m_ShareInfo.heroId, this.m_ShareInfo.skinId);
                }
                uint[] numArray2 = new uint[] { skinId };
                this.m_ShareActivityParam.set(3, 1, numArray2);
                this.ShowNewSkinShare(this.m_ShareInfo.heroId, this.m_ShareInfo.skinId, false, this.m_ShareInfo.rewardType, false);
            }
        }

        public void OpenShowSharePVPFrom(RES_SHOW_ACHIEVEMENT_TYPE type)
        {
            <OpenShowSharePVPFrom>c__AnonStorey6F storeyf = new <OpenShowSharePVPFrom>c__AnonStorey6F();
            storeyf.<>f__this = this;
            this.m_ShareActivityParam.clear();
            this.m_bSharePvpForm = true;
            CUIFormScript form = Singleton<CUIManager>.GetInstance().OpenForm(s_formSharePVPPath, false, true);
            this.UpdateSharePVPForm(form, form.GetWidget(0));
            storeyf.shareImg = form.GetWidget(2);
            storeyf.btnClose = form.GetWidget(3);
            storeyf.btnFriend = form.GetWidget(4);
            storeyf.btnZone = form.GetWidget(5);
            this.BtnGray(storeyf.btnClose, false);
            this.BtnGray(storeyf.btnFriend, false);
            this.BtnGray(storeyf.btnZone, false);
            string preUrl = string.Format("{0}PVPShare/Img_PVP_ShareAchievement_{1}.jpg", BannerImageSys.GlobalLoadPath, (int) type);
            base.StartCoroutine(MonoSingleton<IDIPSys>.GetInstance().DownloadImage(preUrl, new IDIPSys.LoadRCallBack(storeyf.<>m__7F), 10));
        }

        public string PackQQGameTeamData(int iRoomEntity, uint dwRoomID, uint dwRoomSeq, ulong ullFeature)
        {
            if (string.IsNullOrEmpty(this.m_QQGameTeamStr))
            {
                return string.Empty;
            }
            object[] args = new object[] { iRoomEntity, dwRoomID, dwRoomSeq, ullFeature, (int) Application.get_platform(), (int) ApolloConfig.platform };
            string str = string.Format("{0}_{1}_{2}_{3}_{4}_{5}", args);
            Debug.Log(str);
            return str;
        }

        public string PackQQGameTeamData(ulong uuid, uint dwTeamId, uint dwTeamSeq, int iTeamEntity, ulong ullTeamFeature, byte bInviterGradeOfRank, byte bGameMode, byte bPkAI, byte bAILevel, int maxTeamerNum)
        {
            if (string.IsNullOrEmpty(this.m_QQGameTeamStr))
            {
                return string.Empty;
            }
            object[] args = new object[] { uuid, dwTeamId, dwTeamSeq, iTeamEntity, ullTeamFeature, bInviterGradeOfRank, bGameMode, bPkAI, bAILevel, maxTeamerNum, (int) Application.get_platform(), (int) ApolloConfig.platform };
            string str = string.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}_{8}_{9}_{10}_{11}", args);
            Debug.Log(str);
            return str;
        }

        public string PackRecruitFriendInfo()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo == null)
            {
                return string.Empty;
            }
            ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
            if (accountInfo == null)
            {
                return string.Empty;
            }
            object[] args = new object[] { masterRoleInfo.playerUllUID, MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID, accountInfo.OpenId, CUICommonSystem.GetPlatformArea() };
            return string.Format("mqq_{0}_{1}_{2}_{3}", args);
        }

        public string PackRoomData(int iRoomEntity, uint dwRoomID, uint dwRoomSeq, byte bMapType, uint dwMapId, ulong ullFeature)
        {
            object[] args = new object[] { iRoomEntity, dwRoomID, dwRoomSeq, bMapType, dwMapId, ullFeature, (int) Application.get_platform(), (int) ApolloConfig.platform };
            string str = string.Format("ShareRoom_{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}", args);
            Debug.Log(str);
            return str;
        }

        public string PackTeamData(ulong uuid, uint dwTeamId, uint dwTeamSeq, int iTeamEntity, ulong ullTeamFeature, byte bInviterGradeOfRank, byte bGameMode, byte bPkAI, byte bMapType, uint dwMapId, byte bAILevel, byte bMaxTeamNum, byte bTeamGradeOfRank)
        {
            object[] args = new object[] { uuid, dwTeamId, dwTeamSeq, iTeamEntity, ullTeamFeature, bInviterGradeOfRank, bGameMode, bPkAI, bMapType, dwMapId, bAILevel, (int) Application.get_platform(), (int) ApolloConfig.platform, bMaxTeamNum, bTeamGradeOfRank };
            string str = string.Format("ShareTeam_{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}_{8}_{9}_{10}_{11}_{12}_{13}_{14}", args);
            Debug.Log(str);
            return str;
        }

        private void PreLoadPVPImage()
        {
            string preUrl = string.Format("{0}PVPShare/Img_PVP_ShareDefeat_0.jpg", BannerImageSys.GlobalLoadPath);
            if (<>f__am$cache2A == null)
            {
                <>f__am$cache2A = delegate (Texture2D text2) {
                };
            }
            base.StartCoroutine(MonoSingleton<IDIPSys>.GetInstance().DownloadImage(preUrl, <>f__am$cache2A, 10));
            int[] numArray = new int[] { 0, 3, 6, 7, 8 };
            preUrl = string.Empty;
            for (int i = 0; i < numArray.Length; i++)
            {
                preUrl = string.Format("{0}PVPShare/Img_PVP_ShareAchievement_{1}.jpg", BannerImageSys.GlobalLoadPath, numArray[i]);
                if (<>f__am$cache2B == null)
                {
                    <>f__am$cache2B = delegate (Texture2D text2D) {
                    };
                }
                base.StartCoroutine(MonoSingleton<IDIPSys>.GetInstance().DownloadImage(preUrl, <>f__am$cache2B, 10));
            }
        }

        public void PreLoadShareSkinImage(CLoadReq loadReq)
        {
            <PreLoadShareSkinImage>c__AnonStorey6A storeya = new <PreLoadShareSkinImage>c__AnonStorey6A();
            storeya.loadReq = loadReq;
            storeya.<>f__this = this;
            if (!this.SharePicIsExist(storeya.loadReq.m_Url, this.m_SharePicCDNCachePath, storeya.loadReq.m_Type) && !this.isDownLoading(storeya.loadReq))
            {
                this.m_DownLoadSkinList.Add(storeya.loadReq);
                string str = string.Empty;
                if (storeya.loadReq.m_Type == 1)
                {
                    str = string.Format("{0}HeroSharePic/{1}.jpg", BannerImageSys.GlobalLoadPath, storeya.loadReq.m_Url);
                }
                else if (storeya.loadReq.m_Type == 2)
                {
                    str = string.Format("{0}SkinSharePic/{1}.jpg", BannerImageSys.GlobalLoadPath, storeya.loadReq.m_Url);
                }
                if (!string.IsNullOrEmpty(str))
                {
                    base.StartCoroutine(this.DownloadImageByTag2(str, storeya.loadReq, new LoadRCallBack3(storeya.<>m__77), this.m_SharePicCDNCachePath));
                }
            }
        }

        [DebuggerHidden]
        private IEnumerator QQGameTeamStateChgGet(string url)
        {
            <QQGameTeamStateChgGet>c__Iterator2E iteratore = new <QQGameTeamStateChgGet>c__Iterator2E();
            iteratore.url = url;
            iteratore.<$>url = url;
            return iteratore;
        }

        private void RefeshPhoto(string filename)
        {
            AndroidJavaClass class2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            if (class2 != null)
            {
                AndroidJavaObject @static = class2.GetStatic<AndroidJavaObject>("currentActivity");
                if (@static != null)
                {
                    Debug.Log("RefeshPhoto in unity");
                    object[] objArray1 = new object[] { filename };
                    @static.Call("RefeshPhoto", objArray1);
                }
            }
        }

        private void RemoveDownLoading(string url)
        {
            for (int i = this.m_DownLoadSkinList.Count - 1; i >= 0; i--)
            {
                CLoadReq req = this.m_DownLoadSkinList[i];
                if (req.m_Url == url)
                {
                    this.m_DownLoadSkinList.Remove(this.m_DownLoadSkinList[i]);
                }
            }
        }

        private void ReportShareInfo()
        {
            CSDT_TRANK_TLOG_INFO[] uiTlog = Singleton<RankingSystem>.instance.GetUiTlog();
            if ((uiTlog.Length != 0) || (this.m_ShareReportInfoList.Count != 0))
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x10d3);
                int count = this.m_ShareReportInfoList.Count;
                msg.stPkgData.stShareTLogReq.bNum = (byte) count;
                for (int i = 0; i < count; i++)
                {
                    msg.stPkgData.stShareTLogReq.astShareDetail[i].dwType = this.m_ShareReportInfoList[i].dwType;
                    msg.stPkgData.stShareTLogReq.astShareDetail[i].dwSubType = this.m_ShareReportInfoList[i].dwSubType;
                    msg.stPkgData.stShareTLogReq.astShareDetail[i].dwCnt = this.m_ShareReportInfoList[i].dwCnt;
                }
                count = uiTlog.Length;
                msg.stPkgData.stShareTLogReq.dwTrankNum = (uint) count;
                for (int j = 0; j < count; j++)
                {
                    msg.stPkgData.stShareTLogReq.astTrankDetail[j].dwType = uiTlog[j].dwType;
                    msg.stPkgData.stShareTLogReq.astTrankDetail[j].dwCnt = uiTlog[j].dwCnt;
                }
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
                this.m_ShareReportInfoList.Clear();
                Singleton<RankingSystem>.instance.ClearUiTlog();
            }
        }

        public void RequestShareHeroSkinForm(uint heroID, uint skinID, COM_REWARDS_TYPE kType)
        {
            this.m_ShareInfo.heroId = heroID;
            this.m_ShareInfo.skinId = skinID;
            this.m_ShareInfo.rewardType = kType;
            int num = 1;
            if (kType == COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN)
            {
                num = 2;
                ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroID, skinID);
                if (heroSkin != null)
                {
                    this.m_ShareInfo.shareSkinUrl = heroSkin.szShareSkinUrl;
                }
            }
            else
            {
                this.m_ShareInfo.shareSkinUrl = this.m_ShareInfo.heroId.ToString();
            }
            CLoadReq loadReq = new CLoadReq();
            loadReq.m_Url = this.m_ShareInfo.shareSkinUrl;
            loadReq.m_CachePath = this.m_SharePicCDNCachePath;
            loadReq.m_LoadError = ELoadError.None;
            loadReq.m_Type = num;
            MonoSingleton<ShareSys>.GetInstance().PreLoadShareSkinImage(loadReq);
            this.OpenShareNewHeroForm(null);
        }

        public void SavePic(CUIEvent uiEvent)
        {
            <SavePic>c__AnonStorey6E storeye = new <SavePic>c__AnonStorey6E();
            storeye.<>f__this = this;
            storeye.btnClose = this.GetCloseBtn(uiEvent.m_srcFormScript);
            if (storeye.btnClose != null)
            {
                if (storeye.btnClose != null)
                {
                    storeye.btnClose.CustomSetActive(false);
                }
                Singleton<CUIManager>.instance.CloseTips();
                storeye.bSettltment = false;
                if (uiEvent.m_srcFormScript.m_formPath == SettlementSystem.SettlementFormName)
                {
                    storeye.bSettltment = true;
                    Singleton<SettlementSystem>.GetInstance().SnapScreenShotShowBtn(false);
                }
                GameObject displayPanel = this.GetDisplayPanel(uiEvent.m_srcFormScript);
                if (displayPanel != null)
                {
                    Rect screenShotRect = this.GetScreenShotRect(displayPanel);
                    storeye.notShowObj = this.GetNotShowBtn(uiEvent.m_srcFormScript);
                    if (storeye.notShowObj != null)
                    {
                        storeye.notShowObj.CustomSetActive(true);
                    }
                    base.StartCoroutine(this.Capture(screenShotRect, new Action<string>(storeye.<>m__7B)));
                    uint dwType = 0;
                    if (this.m_bShareHero)
                    {
                        dwType = 0;
                    }
                    else if (this.m_bSharePvpForm)
                    {
                        dwType = 1;
                    }
                    this.AddshareReportInfo(dwType, 1);
                }
            }
        }

        private void SendQQGameTeamCreateMsg(string roomInfoStr)
        {
            Debug.Log("QQGameTeamCreate");
            if (!string.IsNullOrEmpty(roomInfoStr))
            {
                char[] separator = new char[] { '-' };
                string[] strArray = roomInfoStr.Split(separator);
                if ((strArray != null) && (strArray.Length == 3))
                {
                    COM_ROOM_TYPE com_room_type = (COM_ROOM_TYPE) Convert.ToInt32(strArray[0]);
                    COM_BATTLE_MAP_TYPE mapType = (COM_BATTLE_MAP_TYPE) Convert.ToInt32(strArray[1]);
                    uint mapId = Convert.ToUInt32(strArray[2]);
                    if (com_room_type == COM_ROOM_TYPE.COM_ROOM_TYPE_MATCH)
                    {
                        if (Singleton<LobbyLogic>.instance.isLogin)
                        {
                            ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo((byte) mapType, mapId);
                            CMatchingSystem.ReqCreateTeam(mapId, false, mapType, pvpMapCommonInfo.bMaxAcntNum / 2, COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE, false);
                        }
                    }
                    else if ((com_room_type == COM_ROOM_TYPE.COM_ROOM_TYPE_NORMAL) && Singleton<LobbyLogic>.instance.isLogin)
                    {
                        CRoomSystem.ReqCreateRoom(mapId, (byte) mapType, false);
                    }
                }
            }
        }

        public void SendQQGameTeamStateChgMsg(QQGameTeamEventType EventType, COM_ROOM_TYPE roomType = 0, byte mapType = 0, uint mapID = 0, string roomStr = "")
        {
            Debug.Log("QQGameTeamStateChg" + EventType);
            if (!string.IsNullOrEmpty(this.m_QQGameTeamStr))
            {
                if (EventType == QQGameTeamEventType.join)
                {
                    if (!this.CheckQQGameTeamInfo(roomType, mapType, mapID))
                    {
                        this.m_QQGameTeamStr = string.Empty;
                        this.m_WakeupOpenId = string.Empty;
                        return;
                    }
                    this.m_RoomInfoStr = roomStr;
                    if (this.m_bIsQQGameTeamOwner)
                    {
                        Singleton<CUIManager>.GetInstance().OpenTips("QQGameTeam_Tips1", true, 1.5f, null, new object[0]);
                        this.m_bIsQQGameTeamOwner = false;
                    }
                }
                if (this.m_RoomInfoStr != null)
                {
                    ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
                    if (accountInfo != null)
                    {
                        string url = string.Empty;
                        if (this.m_MarkStr == "PenguinEsport")
                        {
                            object[] args = new object[] { EventType.ToString(), accountInfo.OpenId, accountInfo.GetToken(ApolloTokenType.Access).Value, this.m_QQGameTeamStr + "_" + this.m_RoomInfoStr, ApolloConfig.GetAppID() };
                            url = string.Format("http://game.egame.qq.com/cgi-bin/game_notify?event={0}&openid={1}&openkey={2}&gamedata={3}&appid={4}", args);
                        }
                        else if (this.m_MarkStr == "GameHelper")
                        {
                            object[] objArray2 = new object[] { EventType.ToString(), accountInfo.OpenId, accountInfo.GetToken(ApolloTokenType.Access).Value, this.m_QQGameTeamStr + "_" + this.m_RoomInfoStr };
                            url = string.Format("http://api.helper.qq.com/play/smobanotify?event={0}&openid={1}&openkey={2}&gamedata={3}", objArray2);
                        }
                        else
                        {
                            object[] objArray3 = new object[] { EventType.ToString(), accountInfo.OpenId, accountInfo.GetToken(ApolloTokenType.Access).Value, this.m_QQGameTeamStr + "_" + this.m_RoomInfoStr };
                            url = string.Format("http://openmobile.qq.com/gameteam/game_notify?event={0}&openid={1}&openkey={2}&gamedata={3}", objArray3);
                        }
                        Debug.Log("QQGameTeamStateChg:" + url);
                        base.StartCoroutine(this.QQGameTeamStateChgGet(url));
                        if (EventType == QQGameTeamEventType.end)
                        {
                            this.m_QQGameTeamStr = string.Empty;
                            this.m_WakeupOpenId = string.Empty;
                            this.m_RoomInfoStr = string.Empty;
                        }
                        this.ClearQQGameCreateInfo();
                    }
                }
            }
        }

        private void SendRoomDataMsg(bool clearData = true)
        {
            if (this.m_ShareRoom != null)
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x1458);
                msg.stPkgData.stJoinMultiGameReq = this.m_ShareRoom;
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
                Singleton<WatchController>.GetInstance().Stop();
                Debug.Log("share roomdata msg");
            }
            if (clearData)
            {
                this.ClearRoomData();
            }
        }

        public void SendShareActivityDoneMsg()
        {
            if (Singleton<ActivitySys>.GetInstance().IsShareTask && this.m_ShareActivityParam.bUsed)
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x1472);
                msg.stPkgData.stWealContentShareDone.bShareType = this.m_ShareActivityParam.bShareType;
                msg.stPkgData.stWealContentShareDone.bParamCnt = this.m_ShareActivityParam.bParamCnt;
                for (int i = 0; i < this.m_ShareActivityParam.bParamCnt; i++)
                {
                    msg.stPkgData.stWealContentShareDone.ShareParam[i] = this.m_ShareActivityParam.ShareParam[i];
                }
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
                Debug.Log(string.Format("SendShareActivityDoneMsg{0}/{1}/{2} ", this.m_ShareActivityParam.bShareType, this.m_ShareActivityParam.bParamCnt, this.m_ShareActivityParam.ShareParam));
            }
            this.m_ShareActivityParam.clear();
        }

        public void SendShareDataMsg()
        {
            if (!string.IsNullOrEmpty(this.m_ShareStr))
            {
                this.UnPackSNSDataStr(this.m_ShareStr);
                this.m_ShareStr = string.Empty;
            }
            else
            {
                if (this.m_ShareRoom != null)
                {
                    this.SendRoomDataMsg(string.IsNullOrEmpty(this.m_RoomModeId));
                }
                if (this.m_ShareTeam != null)
                {
                    this.SendTeamDataMsg(string.IsNullOrEmpty(this.m_RoomModeId));
                }
                if (!string.IsNullOrEmpty(this.m_RoomModeId))
                {
                    this.SendQQGameTeamCreateMsg(this.m_RoomModeId);
                }
                this.m_ShareStr = string.Empty;
            }
        }

        private void SendTeamDataMsg(bool clearData = true)
        {
            if (this.m_ShareTeam != null)
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x145a);
                msg.stPkgData.stJoinTeamReq.stInviteJoinInfo = this.m_ShareTeam;
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
                Debug.Log("share teamdata msg");
            }
            if (clearData)
            {
                this.ClearTeamDataMsg();
            }
        }

        public void SetShareDefeatImage(Transform imageTransform, CUIFormScript form)
        {
            <SetShareDefeatImage>c__AnonStorey70 storey = new <SetShareDefeatImage>c__AnonStorey70();
            storey.imageTransform = imageTransform;
            storey.<>f__this = this;
            if ((storey.imageTransform != null) && (form != null))
            {
                storey.btnBarrige = form.GetWidget(3);
                storey.btnFriend = form.GetWidget(4);
                storey.btnZone = form.GetWidget(5);
                this.BtnGray(storey.btnBarrige, false);
                this.BtnGray(storey.btnFriend, false);
                this.BtnGray(storey.btnZone, false);
                string preUrl = string.Format("{0}PVPShare/Img_PVP_ShareDefeat_0.jpg", BannerImageSys.GlobalLoadPath);
                base.StartCoroutine(MonoSingleton<IDIPSys>.GetInstance().DownloadImage(preUrl, new IDIPSys.LoadRCallBack(storey.<>m__80), 10));
            }
        }

        public static void SetSharePlatfText(Text platText)
        {
            if (null != platText)
            {
                if (ApolloConfig.platform == ApolloPlatform.QQ)
                {
                    platText.set_text("分享空间");
                }
                else
                {
                    platText.set_text("分享朋友圈");
                }
            }
        }

        private void SettlementShareBtnHandle(CUIEvent ievent)
        {
            if (!MonoSingleton<NewbieGuideManager>.GetInstance().isNewbieGuiding)
            {
                Singleton<CChatController>.instance.ShowPanel(false, false);
                this.AddshareReportInfo(1, 0);
                this.OpenShowSharePVPFrom(RES_SHOW_ACHIEVEMENT_TYPE.RES_SHOW_ACHIEVEMENT_COUNT);
            }
        }

        private void Share(string buttonType, string sharePathPic)
        {
            IApolloSnsService service = IApollo.Instance.GetService(1) as IApolloSnsService;
            if (service != null)
            {
                FileStream stream = new FileStream(sharePathPic, FileMode.Open, FileAccess.Read);
                byte[] array = new byte[stream.Length];
                int count = Convert.ToInt32(stream.Length);
                stream.Read(array, 0, count);
                stream.Close();
                this.OnShareCallBack();
                if (ApolloConfig.platform == ApolloPlatform.Wechat)
                {
                    if (buttonType == "TimeLine/Qzone")
                    {
                        service.SendToWeixinWithPhoto(ApolloShareScene.TimeLine, "MSG_INVITE", array, count, string.Empty, "WECHAT_SNS_JUMP_APP");
                    }
                    else if (buttonType == "Session")
                    {
                        service.SendToWeixinWithPhoto(ApolloShareScene.Session, "apollo test", array, count);
                    }
                }
                else if (ApolloConfig.platform == ApolloPlatform.QQ)
                {
                    if (buttonType == "TimeLine/Qzone")
                    {
                        service.SendToQQWithPhoto(ApolloShareScene.TimeLine, sharePathPic);
                    }
                    else if (buttonType == "Session")
                    {
                        service.SendToQQWithPhoto(ApolloShareScene.QSession, sharePathPic);
                    }
                }
            }
        }

        public void ShareFriend(CUIEvent uiEvent)
        {
            <ShareFriend>c__AnonStorey6B storeyb = new <ShareFriend>c__AnonStorey6B();
            storeyb.<>f__this = this;
            Singleton<ApolloHelper>.GetInstance().m_bShareQQBox = false;
            this.m_bClickTimeLineBtn = false;
            if (this.IsInstallPlatform())
            {
                storeyb.btnClose = this.GetCloseBtn(uiEvent.m_srcFormScript);
                if (storeyb.btnClose != null)
                {
                    Singleton<CUIManager>.instance.CloseTips();
                    storeyb.btnClose.CustomSetActive(false);
                    storeyb.bSettltment = false;
                    if (uiEvent.m_srcFormScript.m_formPath == SettlementSystem.SettlementFormName)
                    {
                        storeyb.bSettltment = true;
                        Singleton<SettlementSystem>.GetInstance().SnapScreenShotShowBtn(false);
                    }
                    GameObject displayPanel = this.GetDisplayPanel(uiEvent.m_srcFormScript);
                    if (displayPanel != null)
                    {
                        Rect screenShotRect = this.GetScreenShotRect(displayPanel);
                        this.m_bClickShareFriendBtn = true;
                        storeyb.notShowObj = this.GetNotShowBtn(uiEvent.m_srcFormScript);
                        if (storeyb.notShowObj != null)
                        {
                            storeyb.notShowObj.CustomSetActive(true);
                        }
                        base.StartCoroutine(this.Capture(screenShotRect, new Action<string>(storeyb.<>m__78)));
                    }
                }
            }
        }

        private void ShareMyPlayInfoToFriend(CUIEvent uiEvent)
        {
            <ShareMyPlayInfoToFriend>c__AnonStorey6C storeyc = new <ShareMyPlayInfoToFriend>c__AnonStorey6C();
            storeyc.<>f__this = this;
            if (this.IsInstallPlatform())
            {
                CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CPlayerInfoSystem.sPlayerInfoFormPath);
                if (form != null)
                {
                    GameObject widget = form.GetWidget(2);
                    GameObject displayeRect = form.GetWidget(15);
                    storeyc.btnGraph = Utility.FindChild(widget, "btnGraph");
                    storeyc.btnDetail = Utility.FindChild(widget, "btnDetail");
                    if ((storeyc.btnGraph != null) && (storeyc.btnDetail != null))
                    {
                        storeyc.btnGraphActive = storeyc.btnGraph.get_activeSelf();
                        storeyc.btnDetailActive = storeyc.btnDetail.get_activeSelf();
                        storeyc.btnGraph.CustomSetActive(false);
                        storeyc.btnDetail.CustomSetActive(false);
                        Rect screenShotRect = this.GetScreenShotRect(displayeRect);
                        base.StartCoroutine(this.Capture(screenShotRect, new Action<string>(storeyc.<>m__79)));
                    }
                }
            }
        }

        private void ShareMysteryDiscount(CUIEvent uiEvent)
        {
            MySteryShop instance = Singleton<MySteryShop>.GetInstance();
            if (instance.IsGetDisCount())
            {
                CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm(s_formShareMysteryDiscountPath, false, true);
                DebugHelper.Assert(formScript != null, "神秘商店分享form打开失败");
                if (formScript != null)
                {
                    GameObject widget = formScript.GetWidget(0);
                    if (widget != null)
                    {
                        Image component = widget.GetComponent<Image>();
                        if (component != null)
                        {
                            component.SetSprite(instance.GetDiscountNumIconPath((uint) instance.GetDisCount()), formScript, true, false, false, false);
                        }
                    }
                    GameObject obj3 = formScript.GetWidget(2);
                    if (obj3 != null)
                    {
                        SetSharePlatfText(obj3.GetComponent<Text>());
                    }
                }
            }
        }

        private bool SharePicIsExist(string url, string tagPath, int type)
        {
            string cDNSharePicUrl = this.GetCDNSharePicUrl(url, type);
            string str2 = MonoSingleton<IDIPSys>.GetInstance().ToMD5(cDNSharePicUrl);
            return File.Exists(CFileManager.CombinePath(tagPath, str2));
        }

        public void ShareRecruitFriend(string title, string desc)
        {
            Singleton<ApolloHelper>.GetInstance().ShareRecruitFriend(title, desc, this.PackRecruitFriendInfo());
        }

        public void ShareTimeLine(CUIEvent uiEvent)
        {
            <ShareTimeLine>c__AnonStorey6D storeyd = new <ShareTimeLine>c__AnonStorey6D();
            storeyd.<>f__this = this;
            Singleton<ApolloHelper>.GetInstance().m_bShareQQBox = false;
            if (this.IsInstallPlatform())
            {
                storeyd.btnClose = this.GetCloseBtn(uiEvent.m_srcFormScript);
                if (storeyd.btnClose != null)
                {
                    Debug.Log(" m_bClickTimeLineBtn " + this.m_bClickTimeLineBtn);
                    this.m_TimelineBtn = uiEvent.m_srcWidget.get_transform();
                    this.m_bClickTimeLineBtn = true;
                    this.m_bClickShareFriendBtn = false;
                    storeyd.btnClose.CustomSetActive(false);
                    Singleton<CUIManager>.instance.CloseTips();
                    storeyd.bSettltment = false;
                    if (uiEvent.m_srcFormScript.m_formPath == SettlementSystem.SettlementFormName)
                    {
                        storeyd.bSettltment = true;
                        Singleton<SettlementSystem>.GetInstance().SnapScreenShotShowBtn(false);
                    }
                    GameObject displayPanel = this.GetDisplayPanel(uiEvent.m_srcFormScript);
                    if (displayPanel != null)
                    {
                        Rect screenShotRect = this.GetScreenShotRect(displayPanel);
                        storeyd.notShowObj = this.GetNotShowBtn(uiEvent.m_srcFormScript);
                        if (storeyd.notShowObj != null)
                        {
                            storeyd.notShowObj.CustomSetActive(true);
                        }
                        base.StartCoroutine(this.Capture(screenShotRect, new Action<string>(storeyd.<>m__7A)));
                    }
                }
            }
        }

        public void ShowNewHeroShare(uint heroId, uint skinId, bool bInitAnima = true, COM_REWARDS_TYPE rewardType = 5, bool interactableTransition = false)
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(s_formShareNewHeroPath, false, true);
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CUICommonSystem.s_newHeroOrSkinPath);
            enFormPriority priority = enFormPriority.Priority1;
            if (form != null)
            {
                priority = form.m_priority;
            }
            script.SetPriority(priority);
            script.GetWidget(2).CustomSetActive(true);
            this.m_FriendBtnImage = script.GetWidget(4).GetComponent<Image>();
            this.m_TimeLineBtnImage = script.GetWidget(5).GetComponent<Image>();
            if (this.m_FriendBtnImage != null)
            {
                this.BtnGray(this.m_FriendBtnImage, false);
            }
            if (this.m_TimeLineBtnImage != null)
            {
                this.BtnGray(this.m_TimeLineBtnImage, false);
            }
            Image component = script.GetWidget(0).GetComponent<Image>();
            component.get_gameObject().CustomSetActive(false);
            this.m_ShareSkinPicImage = component.get_transform();
            if (!string.IsNullOrEmpty(this.m_ShareInfo.shareSkinUrl))
            {
                CLoadReq loadReq = new CLoadReq();
                loadReq.m_Url = this.m_ShareInfo.shareSkinUrl;
                loadReq.m_CachePath = MonoSingleton<ShareSys>.instance.m_SharePicCDNCachePath;
                loadReq.m_LoadError = ELoadError.None;
                loadReq.m_Type = 1;
                this.LoadShareSkinImage(loadReq, component);
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips("图片还没有上传", false, 1.5f, null, new object[0]);
            }
            SetSharePlatfText(script.GetWidget(3).GetComponent<Text>());
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                Text text = script.GetWidget(1).GetComponent<Text>();
                text.set_text(masterRoleInfo.GetHaveHeroCount(false).ToString());
                text.get_gameObject().CustomSetActive(true);
            }
            script.GetWidget(6).GetComponent<Text>().get_gameObject().CustomSetActive(false);
        }

        public void ShowNewSkinShare(uint heroId, uint skinId, bool bInitAnima = true, COM_REWARDS_TYPE rewardType = 5, bool interactableTransition = false)
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(s_formShareNewHeroPath, false, true);
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CUICommonSystem.s_newHeroOrSkinPath);
            enFormPriority priority = enFormPriority.Priority1;
            if (form != null)
            {
                priority = form.m_priority;
            }
            script.SetPriority(priority);
            script.GetWidget(2).CustomSetActive(true);
            Image component = script.GetWidget(0).GetComponent<Image>();
            component.get_gameObject().CustomSetActive(false);
            this.m_ShareSkinPicImage = component.get_transform();
            Text text = script.GetWidget(1).GetComponent<Text>();
            if (text != null)
            {
                text.get_gameObject().CustomSetActive(false);
            }
            this.m_FriendBtnImage = script.GetWidget(4).GetComponent<Image>();
            this.m_TimeLineBtnImage = script.GetWidget(5).GetComponent<Image>();
            if (this.m_FriendBtnImage != null)
            {
                this.BtnGray(this.m_FriendBtnImage, false);
            }
            if (this.m_TimeLineBtnImage != null)
            {
                this.BtnGray(this.m_TimeLineBtnImage, false);
            }
            if (!string.IsNullOrEmpty(this.m_ShareInfo.shareSkinUrl))
            {
                CLoadReq loadReq = new CLoadReq();
                loadReq.m_Url = this.m_ShareInfo.shareSkinUrl;
                loadReq.m_CachePath = MonoSingleton<ShareSys>.instance.m_SharePicCDNCachePath;
                loadReq.m_LoadError = ELoadError.None;
                loadReq.m_Type = 2;
                this.LoadShareSkinImage(loadReq, component);
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips("图片还没有上传", false, 1.5f, null, new object[0]);
            }
            SetSharePlatfText(script.GetWidget(3).GetComponent<Text>());
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                Text text2 = script.GetWidget(6).GetComponent<Text>();
                text2.get_gameObject().CustomSetActive(true);
                text2.set_text(masterRoleInfo.GetHeroSkinCount(false).ToString());
            }
        }

        public bool UnpackInviteSNSData(string data, string wakeupOpenId)
        {
            Debug.Log("rcv " + data);
            if (string.IsNullOrEmpty(data))
            {
                return false;
            }
            char[] separator = new char[] { '_' };
            string[] strArray = data.Split(separator);
            if ((strArray != null) && (strArray[0] == "mqq"))
            {
                this.UnpackRecruitFriendInfo(data);
                this.m_ShareStr = string.Empty;
                return true;
            }
            if (MonoSingleton<NewbieGuideManager>.GetInstance().isNewbieGuiding)
            {
                Debug.Log("正在新手引导中");
                return false;
            }
            this.m_WakeupOpenId = wakeupOpenId;
            if (Singleton<LobbyLogic>.instance.isLogin)
            {
                return this.UnPackSNSDataStr(data);
            }
            this.m_ShareStr = data;
            return true;
        }

        public bool UnPackQQGameTeamData(string data)
        {
            Debug.Log("UnpackQQGameTeamData");
            ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
            if ((accountInfo != null) && (accountInfo.OpenId != this.m_WakeupOpenId))
            {
                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Common_Login_Different_Account_Tip"), enUIEventID.Login_Change_Account_Yes, enUIEventID.Login_Change_Account_No, false);
                Singleton<ApolloHelper>.GetInstance().IsWXGameCenter = false;
                Singleton<ApolloHelper>.GetInstance().IsQQGameCenter = false;
                return true;
            }
            this.m_QQGameTeamStr = data;
            this.m_bIsQQGameTeamOwner = false;
            char[] separator = new char[] { '_' };
            string[] strArray = data.Split(separator);
            if (strArray != null)
            {
                int length = strArray.Length;
                if (length > 0)
                {
                    this.m_MarkStr = strArray[0];
                }
                if ((strArray == null) || (((length != 4) && (length != 10)) && (strArray.Length != 0x10)))
                {
                    return false;
                }
                if (!this.IsSupport3rdAPP(strArray[0]))
                {
                    return false;
                }
                if (!this.CheckEnterShareTeamLimit(ref strArray[length - 2], ref strArray[length - 1], length != 4, false))
                {
                    return false;
                }
                this.m_RoomModeId = strArray[1];
                char[] chArray2 = new char[] { '-' };
                string[] strArray2 = this.m_RoomModeId.Split(chArray2);
                if ((strArray2 == null) || (strArray2.Length != 3))
                {
                    return false;
                }
                COM_ROOM_TYPE com_room_type = (COM_ROOM_TYPE) Convert.ToInt32(strArray2[0]);
                COM_BATTLE_MAP_TYPE com_battle_map_type = (COM_BATTLE_MAP_TYPE) Convert.ToInt32(strArray2[1]);
                uint num2 = Convert.ToUInt32(strArray2[2]);
                if (length == 4)
                {
                    this.SendQQGameTeamCreateMsg(this.m_RoomModeId);
                    this.m_bIsQQGameTeamOwner = true;
                    return true;
                }
                switch (com_room_type)
                {
                    case COM_ROOM_TYPE.COM_ROOM_TYPE_MATCH:
                        this.m_ShareTeam = new COMDT_INVITE_JOIN_INFO();
                        this.m_ShareTeam.iInviteType = 2;
                        this.m_ShareTeam.stInviteDetail.stInviteJoinTeam = new COMDT_INVITE_TEAM_DETAIL();
                        if ((((ulong.TryParse(strArray[4], out this.m_ShareTeam.stInviterInfo.ullUid) && uint.TryParse(strArray[5], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.dwTeamId)) && (uint.TryParse(strArray[6], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.dwTeamSeq) && int.TryParse(strArray[7], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.iTeamEntity))) && ((ulong.TryParse(strArray[8], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.ullTeamFeature) && byte.TryParse(strArray[9], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.bGradeOfInviter)) && (byte.TryParse(strArray[10], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.bGameMode) && byte.TryParse(strArray[11], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.bPkAI)))) && (byte.TryParse(strArray[12], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.bAILevel) && byte.TryParse(strArray[13], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.bMaxTeamNum)))
                        {
                            this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.bMapType = (byte) com_battle_map_type;
                            this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.dwMapId = num2;
                            if (Singleton<LobbyLogic>.instance.isLogin)
                            {
                                this.SendTeamDataMsg(false);
                            }
                            return true;
                        }
                        break;

                    case COM_ROOM_TYPE.COM_ROOM_TYPE_NORMAL:
                        this.m_ShareRoom = new CSPKG_JOINMULTIGAMEREQ();
                        if ((int.TryParse(strArray[4], out this.m_ShareRoom.iRoomEntity) && uint.TryParse(strArray[5], out this.m_ShareRoom.dwRoomID)) && (uint.TryParse(strArray[6], out this.m_ShareRoom.dwRoomSeq) && ulong.TryParse(strArray[7], out this.m_ShareRoom.ullFeature)))
                        {
                            this.m_ShareRoom.bMapType = (byte) com_battle_map_type;
                            this.m_ShareRoom.dwMapId = num2;
                            if (Singleton<LobbyLogic>.instance.isLogin)
                            {
                                this.SendRoomDataMsg(false);
                            }
                            return true;
                        }
                        break;
                }
            }
            return false;
        }

        public bool UnpackRecruitFriendInfo(string data)
        {
            char[] separator = new char[] { '_' };
            string[] strArray = data.Split(separator);
            if (((strArray == null) || (strArray.Length != 5)) || (strArray[0] != "mqq"))
            {
                return false;
            }
            Debug.Log("UnpackRecruitFriendInfo " + data);
            this.m_RecruitSource = new COMDT_RECRUITMENT_SOURCE();
            uint result = 0;
            uint.TryParse(strArray[2], out result);
            this.m_RecruitSource.stUin.dwLogicWorldId = result;
            uint num2 = 0;
            uint.TryParse(strArray[1], out num2);
            this.m_RecruitSource.stUin.ullUid = num2;
            int num3 = 0;
            int.TryParse(strArray[4], out num3);
            this.m_RecruitSource.iPlatId = num3;
            Utility.StringToByteArray(strArray[3], ref this.m_RecruitSource.szOpenID);
            return true;
        }

        public bool UnpackRoomData(string data)
        {
            Debug.Log("UnpackRoomData");
            char[] separator = new char[] { '_' };
            string[] strArray = data.Split(separator);
            if (((strArray == null) || (strArray.Length != 9)) || (strArray[0] != "ShareRoom"))
            {
                return false;
            }
            if (!this.CheckEnterShareTeamLimit(ref strArray[7], ref strArray[8], true, false))
            {
                return false;
            }
            this.m_ShareRoom = new CSPKG_JOINMULTIGAMEREQ();
            if (((!int.TryParse(strArray[1], out this.m_ShareRoom.iRoomEntity) || !uint.TryParse(strArray[2], out this.m_ShareRoom.dwRoomID)) || (!uint.TryParse(strArray[3], out this.m_ShareRoom.dwRoomSeq) || !byte.TryParse(strArray[4], out this.m_ShareRoom.bMapType))) || (!uint.TryParse(strArray[5], out this.m_ShareRoom.dwMapId) || !ulong.TryParse(strArray[6], out this.m_ShareRoom.ullFeature)))
            {
                return false;
            }
            if (Singleton<LobbyLogic>.instance.isLogin)
            {
                this.SendRoomDataMsg(true);
            }
            return true;
        }

        private bool UnPackSNSDataStr(string data)
        {
            char[] separator = new char[] { '_' };
            string[] strArray = data.Split(separator);
            if ((strArray != null) && (strArray[0] == "ShareRoom"))
            {
                return this.UnpackRoomData(data);
            }
            if ((strArray != null) && (strArray[0] == "ShareTeam"))
            {
                return this.UnpackTeamData(data);
            }
            if ((strArray == null) || !this.IsSupport3rdAPP(strArray[0]))
            {
                return false;
            }
            if (!this.UnPackQQGameTeamData(data))
            {
                this.m_QQGameTeamStr = string.Empty;
                this.m_WakeupOpenId = string.Empty;
                return false;
            }
            return true;
        }

        public bool UnpackTeamData(string data)
        {
            Debug.Log("UnpackTeamData");
            char[] separator = new char[] { '_' };
            string[] strArray = data.Split(separator);
            if ((((strArray != null) && (strArray.Length == 0x10)) && (strArray[0] == "ShareTeam")) && this.CheckEnterShareTeamLimit(ref strArray[12], ref strArray[13], true, false))
            {
                this.m_ShareTeam = new COMDT_INVITE_JOIN_INFO();
                this.m_ShareTeam.iInviteType = 2;
                this.m_ShareTeam.stInviteDetail.stInviteJoinTeam = new COMDT_INVITE_TEAM_DETAIL();
                try
                {
                    if ((((ulong.TryParse(strArray[1], out this.m_ShareTeam.stInviterInfo.ullUid) && uint.TryParse(strArray[2], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.dwTeamId)) && (uint.TryParse(strArray[3], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.dwTeamSeq) && int.TryParse(strArray[4], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.iTeamEntity))) && ((ulong.TryParse(strArray[5], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.ullTeamFeature) && byte.TryParse(strArray[6], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.bGradeOfInviter)) && (byte.TryParse(strArray[7], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.bGameMode) && byte.TryParse(strArray[8], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.bPkAI)))) && (((byte.TryParse(strArray[9], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.bMapType) && uint.TryParse(strArray[10], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.dwMapId)) && (byte.TryParse(strArray[11], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.bAILevel) && byte.TryParse(strArray[14], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.bMaxTeamNum))) && (byte.TryParse(strArray[15], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.bGradeOfRank) && Singleton<LobbyLogic>.instance.isLogin)))
                    {
                        this.SendTeamDataMsg(true);
                    }
                }
                catch (Exception exception)
                {
                    Debug.Log(exception.ToString());
                }
            }
            return false;
        }

        public void UpdateShareGradeForm(CUIFormScript form)
        {
            if (form != null)
            {
                SetSharePlatfText(Utility.GetComponetInChild<Text>(form.get_gameObject(), "ShareGroup/Button_TimeLine/ClickText"));
                if (this.m_bShowTimeline)
                {
                    Transform transform = null;
                    foreach (Text text in form.get_transform().GetComponentsInChildren<Text>())
                    {
                        if (((text != null) && (text.get_text() == "分享空间")) || (text.get_text() == "分享朋友圈"))
                        {
                            Transform transform2 = text.get_transform().get_parent();
                            if (transform2.GetComponent<Button>() != null)
                            {
                                transform = transform2;
                                break;
                            }
                        }
                    }
                    if (transform != null)
                    {
                        GameObject obj2 = transform.get_gameObject();
                        if ((obj2 != null) || this.m_bShowTimeline)
                        {
                            obj2.GetComponent<CUIEventScript>().set_enabled(false);
                            obj2.GetComponent<Button>().set_interactable(false);
                            obj2.GetComponent<Image>().set_color(new Color(obj2.GetComponent<Image>().get_color().r, obj2.GetComponent<Image>().get_color().g, obj2.GetComponent<Image>().get_color().b, 0.37f));
                            Text componentInChildren = obj2.GetComponentInChildren<Text>();
                            componentInChildren.set_color(new Color(componentInChildren.get_color().r, componentInChildren.get_color().g, componentInChildren.get_color().b, 0.37f));
                        }
                    }
                }
            }
        }

        public void UpdateSharePVPForm(CUIFormScript form, GameObject shareRootGO)
        {
            if (form != null)
            {
                SetSharePlatfText(Utility.GetComponetInChild<Text>(form.get_gameObject(), "ShareGroup/Button_TimeLine/ClickText"));
                if (this.m_bShowTimeline)
                {
                    Transform transform = null;
                    foreach (Text text in form.get_transform().GetComponentsInChildren<Text>())
                    {
                        if (((text != null) && (text.get_text() == "分享空间")) || (text.get_text() == "分享朋友圈"))
                        {
                            Transform transform2 = text.get_transform().get_parent();
                            if (transform2.GetComponent<Button>() != null)
                            {
                                transform = transform2;
                                break;
                            }
                        }
                    }
                    if (transform != null)
                    {
                        GameObject obj2 = transform.get_gameObject();
                        if ((obj2 != null) || this.m_bShowTimeline)
                        {
                            obj2.GetComponent<CUIEventScript>().set_enabled(false);
                            obj2.GetComponent<Button>().set_interactable(false);
                            obj2.GetComponent<Image>().set_color(new Color(obj2.GetComponent<Image>().get_color().r, obj2.GetComponent<Image>().get_color().g, obj2.GetComponent<Image>().get_color().b, 0.37f));
                            Text componentInChildren = obj2.GetComponentInChildren<Text>();
                            componentInChildren.set_color(new Color(componentInChildren.get_color().r, componentInChildren.get_color().g, componentInChildren.get_color().b, 0.37f));
                        }
                    }
                }
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    CUIHttpImageScript componetInChild = Utility.GetComponetInChild<CUIHttpImageScript>(shareRootGO, "PlayerHead");
                    componetInChild.SetImageUrl(masterRoleInfo.HeadUrl);
                    Utility.GetComponetInChild<Text>(shareRootGO, "PlayerName").set_text(masterRoleInfo.Name);
                    DictionaryView<uint, PlayerKDA>.Enumerator enumerator = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat.GetEnumerator();
                    PlayerKDA rkda = null;
                    int[] numArray = new int[3];
                    while (enumerator.MoveNext())
                    {
                        KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                        PlayerKDA rkda2 = current.Value;
                        if (rkda2.IsHost)
                        {
                            rkda = rkda2;
                        }
                        numArray[(int) rkda2.PlayerCamp] += rkda2.numKill;
                    }
                    Utility.FindChild(componetInChild.get_gameObject(), "Mvp").CustomSetActive(Singleton<BattleStatistic>.instance.GetMvpPlayer(rkda.PlayerCamp, this.m_bWinPVPResult) == rkda.PlayerId);
                    if (rkda != null)
                    {
                        Utility.GetComponetInChild<Text>(shareRootGO, "HostKillNum").set_text(rkda.numKill.ToString());
                        Utility.GetComponetInChild<Text>(shareRootGO, "HostDeadNum").set_text(rkda.numDead.ToString());
                        Utility.GetComponetInChild<Text>(shareRootGO, "HostAssistNum").set_text(rkda.numAssist.ToString());
                        Utility.GetComponetInChild<Text>(shareRootGO, "HostKillTotalNum").set_text(numArray[(int) rkda.PlayerCamp].ToString());
                        Utility.GetComponetInChild<Text>(shareRootGO, "OppoKillTotalNum").set_text(numArray[(int) BattleLogic.GetOppositeCmp(rkda.PlayerCamp)].ToString());
                        ListView<HeroKDA>.Enumerator enumerator2 = rkda.GetEnumerator();
                        if (enumerator2.MoveNext())
                        {
                            HeroKDA okda = enumerator2.Current;
                            ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey((uint) okda.HeroId);
                            Utility.GetComponetInChild<Image>(shareRootGO, "HeroHead").SetSprite(CUIUtility.s_Sprite_Dynamic_Icon_Dir + StringHelper.UTF8BytesToString(ref dataByKey.szImagePath), form, true, false, false, false);
                            int num2 = 1;
                            for (int i = 1; i < 13; i++)
                            {
                                switch (((PvpAchievement) i))
                                {
                                    case PvpAchievement.Legendary:
                                        if (okda.LegendaryNum > 0)
                                        {
                                            CSettlementView.SetAchievementIcon(form, shareRootGO, PvpAchievement.Legendary, num2++);
                                        }
                                        break;

                                    case PvpAchievement.PentaKill:
                                        if (okda.PentaKillNum > 0)
                                        {
                                            CSettlementView.SetAchievementIcon(form, shareRootGO, PvpAchievement.PentaKill, num2++);
                                        }
                                        break;

                                    case PvpAchievement.QuataryKill:
                                        if (okda.QuataryKillNum > 0)
                                        {
                                            CSettlementView.SetAchievementIcon(form, shareRootGO, PvpAchievement.QuataryKill, num2++);
                                        }
                                        break;

                                    case PvpAchievement.TripleKill:
                                        if (okda.TripleKillNum > 0)
                                        {
                                            CSettlementView.SetAchievementIcon(form, shareRootGO, PvpAchievement.TripleKill, num2++);
                                        }
                                        break;

                                    case PvpAchievement.DoubleKill:
                                        if (okda.DoubleKillNum <= 0)
                                        {
                                        }
                                        break;

                                    case PvpAchievement.KillMost:
                                        if (okda.bKillMost)
                                        {
                                            CSettlementView.SetAchievementIcon(form, shareRootGO, PvpAchievement.KillMost, num2++);
                                        }
                                        break;

                                    case PvpAchievement.HurtMost:
                                        if (okda.bHurtMost)
                                        {
                                            CSettlementView.SetAchievementIcon(form, shareRootGO, PvpAchievement.HurtMost, num2++);
                                        }
                                        break;

                                    case PvpAchievement.HurtTakenMost:
                                        if (okda.bHurtTakenMost)
                                        {
                                            CSettlementView.SetAchievementIcon(form, shareRootGO, PvpAchievement.HurtTakenMost, num2++);
                                        }
                                        break;

                                    case PvpAchievement.AsssistMost:
                                        if (okda.bAsssistMost)
                                        {
                                            CSettlementView.SetAchievementIcon(form, shareRootGO, PvpAchievement.AsssistMost, num2++);
                                        }
                                        break;
                                }
                            }
                            for (int j = num2; j <= 6; j++)
                            {
                                CSettlementView.SetAchievementIcon(form, shareRootGO, PvpAchievement.NULL, j);
                            }
                        }
                    }
                }
            }
        }

        private void UpdateTimelineBtn()
        {
            if (this.m_TimelineBtn != null)
            {
                GameObject obj2 = this.m_TimelineBtn.get_gameObject();
                if (this.m_bShowTimeline && (obj2 != null))
                {
                    obj2.GetComponent<CUIEventScript>().set_enabled(false);
                    obj2.GetComponent<Button>().set_interactable(false);
                    obj2.GetComponent<Image>().set_color(new Color(obj2.GetComponent<Image>().get_color().r, obj2.GetComponent<Image>().get_color().g, obj2.GetComponent<Image>().get_color().b, 0.37f));
                    Text componentInChildren = obj2.GetComponentInChildren<Text>();
                    componentInChildren.set_color(new Color(componentInChildren.get_color().r, componentInChildren.get_color().g, componentInChildren.get_color().b, 0.37f));
                }
                this.m_TimelineBtn = null;
            }
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Share_ClosePVPAchievement);
        }

        [CompilerGenerated]
        private sealed class <Capture>c__Iterator2D : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action<string> <$>callback;
            internal Rect <$>screenShotRect;
            internal ShareSys <>f__this;
            internal Color <c>__6;
            internal byte[] <data>__7;
            internal Exception <e>__8;
            internal string <filename>__0;
            internal int <i>__5;
            internal Color[] <noAlphaColors>__4;
            internal Texture2D <result>__1;
            internal Texture2D <src>__2;
            internal Color[] <srcColors>__3;
            internal Action<string> callback;
            internal Rect screenShotRect;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = null;
                        this.$PC = 1;
                        goto Label_02F2;

                    case 1:
                        this.$current = new WaitForEndOfFrame();
                        this.$PC = 2;
                        goto Label_02F2;

                    case 2:
                        try
                        {
                            this.<filename>__0 = this.<>f__this.m_sharePic;
                            this.<result>__1 = null;
                            if (Application.get_platform() == 11)
                            {
                                if (this.<>f__this.m_bAdreo306)
                                {
                                    this.<src>__2 = new Texture2D((int) this.screenShotRect.get_width(), (int) this.screenShotRect.get_height(), 5, false);
                                    this.<src>__2.ReadPixels(this.screenShotRect, 0, 0);
                                    this.<src>__2.Apply();
                                    this.<srcColors>__3 = this.<src>__2.GetPixels();
                                    this.<noAlphaColors>__4 = new Color[this.<srcColors>__3.Length];
                                    this.<i>__5 = 0;
                                    while (this.<i>__5 < this.<srcColors>__3.Length)
                                    {
                                        this.<c>__6 = this.<srcColors>__3[this.<i>__5];
                                        this.<noAlphaColors>__4[this.<i>__5] = new Color(this.<c>__6.r, this.<c>__6.g, this.<c>__6.b);
                                        this.<i>__5++;
                                    }
                                    this.<result>__1 = new Texture2D((int) this.screenShotRect.get_width(), (int) this.screenShotRect.get_height(), 3, false);
                                    this.<result>__1.SetPixels(this.<noAlphaColors>__4);
                                    this.<result>__1.Apply();
                                    Object.Destroy(this.<src>__2);
                                }
                                else
                                {
                                    this.<result>__1 = new Texture2D((int) this.screenShotRect.get_width(), (int) this.screenShotRect.get_height(), 3, false);
                                    this.<result>__1.ReadPixels(this.screenShotRect, 0, 0);
                                    this.<result>__1.Apply();
                                }
                            }
                            else
                            {
                                this.<result>__1 = new Texture2D((int) this.screenShotRect.get_width(), (int) this.screenShotRect.get_height(), 3, false);
                                this.<result>__1.ReadPixels(this.screenShotRect, 0, 0);
                                this.<result>__1.Apply();
                            }
                            this.<data>__7 = null;
                            if (this.<result>__1 != null)
                            {
                                this.<data>__7 = this.<result>__1.EncodeToJPG();
                                Object.Destroy(this.<result>__1);
                            }
                            if (this.<data>__7 != null)
                            {
                                CFileManager.WriteFile(this.<filename>__0, this.<data>__7);
                            }
                            if (this.callback != null)
                            {
                                this.callback(this.<filename>__0);
                            }
                        }
                        catch (Exception exception)
                        {
                            this.<e>__8 = exception;
                            object[] inParameters = new object[] { this.<e>__8.Message };
                            DebugHelper.Assert(false, "Exception in ShareSys.Capture, {0}", inParameters);
                        }
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_02F2:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <DownloadImageByTag2>c__Iterator2C : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal ShareSys.LoadRCallBack3 <$>callBack;
            internal string <$>preUrl;
            internal ShareSys.CLoadReq <$>req;
            internal string <$>tagPath;
            internal ShareSys <>f__this;
            internal float <beginTime>__3;
            internal string <key>__0;
            internal string <localCachePath>__1;
            internal bool <outOfTime>__4;
            internal Texture2D <tex>__5;
            internal WWW <www>__2;
            internal ShareSys.LoadRCallBack3 callBack;
            internal string preUrl;
            internal ShareSys.CLoadReq req;
            internal string tagPath;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<key>__0 = MonoSingleton<IDIPSys>.GetInstance().ToMD5(this.preUrl);
                        this.<localCachePath>__1 = CFileManager.CombinePath(this.tagPath, this.<key>__0);
                        this.<www>__2 = null;
                        this.<www>__2 = new WWW(this.preUrl);
                        this.<beginTime>__3 = Time.get_time();
                        this.<outOfTime>__4 = false;
                        break;

                    case 1:
                        if ((Time.get_time() - this.<beginTime>__3) <= this.<>f__this.g_fDownloadOutTime)
                        {
                            break;
                        }
                        this.<outOfTime>__4 = true;
                        goto Label_00DD;

                    default:
                        goto Label_01D2;
                }
                if (!this.<www>__2.get_isDone() && string.IsNullOrEmpty(this.<www>__2.get_error()))
                {
                    this.$current = null;
                    this.$PC = 1;
                    return true;
                }
            Label_00DD:
                if (this.<outOfTime>__4)
                {
                    this.req.m_LoadError = ShareSys.ELoadError.OutOfTime;
                    this.<www>__2.Dispose();
                    this.callBack(null, this.req);
                }
                else if (!string.IsNullOrEmpty(this.<www>__2.get_error()))
                {
                    this.req.m_LoadError = ShareSys.ELoadError.NotFound;
                    this.<www>__2.Dispose();
                    this.callBack(null, this.req);
                }
                else
                {
                    this.req.m_LoadError = ShareSys.ELoadError.SUCC;
                    this.<tex>__5 = this.<www>__2.get_texture();
                    if ((this.<tex>__5 != null) && (this.<localCachePath>__1 != null))
                    {
                        CFileManager.WriteFile(this.<localCachePath>__1, this.<www>__2.get_bytes());
                    }
                    if (this.callBack != null)
                    {
                        this.callBack(this.<tex>__5, this.req);
                    }
                }
                this.$PC = -1;
            Label_01D2:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <OpenShowSharePVPFrom>c__AnonStorey6F
        {
            internal ShareSys <>f__this;
            internal GameObject btnClose;
            internal GameObject btnFriend;
            internal GameObject btnZone;
            internal GameObject shareImg;

            internal void <>m__7F(Texture2D text2D)
            {
                if (((this.shareImg != null) && (this.shareImg.GetComponent<Image>() != null)) && (text2D != null))
                {
                    this.<>f__this.BtnGray(this.btnClose, true);
                    this.<>f__this.BtnGray(this.btnFriend, true);
                    this.<>f__this.BtnGray(this.btnZone, true);
                    this.shareImg.GetComponent<Image>().SetSprite(Sprite.Create(text2D, new Rect(0f, 0f, (float) text2D.get_width(), (float) text2D.get_height()), new Vector2(0.5f, 0.5f)), ImageAlphaTexLayout.None);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <PreLoadShareSkinImage>c__AnonStorey6A
        {
            internal ShareSys <>f__this;
            internal ShareSys.CLoadReq loadReq;

            internal void <>m__77(Texture2D text2, ShareSys.CLoadReq tempLoadReq)
            {
                if (this.<>f__this.m_DownLoadSkinList.Count > 0)
                {
                    for (int i = this.<>f__this.m_DownLoadSkinList.Count - 1; i >= 0; i--)
                    {
                        ShareSys.CLoadReq item = this.<>f__this.m_DownLoadSkinList[i];
                        if (item.m_Url == tempLoadReq.m_Url)
                        {
                            this.<>f__this.m_DownLoadSkinList.Remove(item);
                            if (tempLoadReq.m_LoadError != ShareSys.ELoadError.SUCC)
                            {
                                this.<>f__this.m_DownLoadSkinList.Add(tempLoadReq);
                            }
                        }
                    }
                }
                Debug.Log("skic share pic tempLoadReq " + tempLoadReq.m_LoadError);
                if ((this.<>f__this.m_bShareHero && (this.<>f__this.m_ShareSkinPicImage != null)) && (this.loadReq.m_Url == this.<>f__this.m_ShareInfo.shareSkinUrl))
                {
                    if (tempLoadReq.m_LoadError == ShareSys.ELoadError.SUCC)
                    {
                        this.<>f__this.m_ShareSkinPicImage.get_gameObject().CustomSetActive(true);
                        Image component = this.<>f__this.m_ShareSkinPicImage.GetComponent<Image>();
                        if (component != null)
                        {
                            component.SetSprite(Sprite.Create(text2, new Rect(0f, 0f, (float) text2.get_width(), (float) text2.get_height()), new Vector2(0.5f, 0.5f)), ImageAlphaTexLayout.None);
                        }
                        if (this.<>f__this.m_FriendBtnImage != null)
                        {
                            this.<>f__this.BtnGray(this.<>f__this.m_FriendBtnImage, true);
                        }
                        if ((this.<>f__this.m_TimeLineBtnImage != null) && !this.<>f__this.m_bShowTimeline)
                        {
                            this.<>f__this.BtnGray(this.<>f__this.m_TimeLineBtnImage, true);
                        }
                    }
                    else if (tempLoadReq.m_LoadError == ShareSys.ELoadError.OutOfTime)
                    {
                        Singleton<CUIManager>.GetInstance().OpenTips(this.<>f__this.m_ShareSkinPicOutofTime, false, 1.5f, null, new object[0]);
                    }
                    else if (tempLoadReq.m_LoadError == ShareSys.ELoadError.NotFound)
                    {
                        Singleton<CUIManager>.GetInstance().OpenTips(this.<>f__this.m_ShareSkinPicNotFound, false, 1.5f, null, new object[0]);
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <QQGameTeamStateChgGet>c__Iterator2E : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal string <$>url;
            internal WWW <www>__0;
            internal string url;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<www>__0 = new WWW(this.url);
                        this.$current = this.<www>__0;
                        this.$PC = 1;
                        return true;

                    case 1:
                        if (this.<www>__0.get_error() != null)
                        {
                            Debug.Log("QQGameTeamStateChgRequestError......" + this.<www>__0.get_error());
                        }
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <SavePic>c__AnonStorey6E
        {
            internal ShareSys <>f__this;
            internal bool bSettltment;
            internal GameObject btnClose;
            internal GameObject notShowObj;

            internal void <>m__7B(string filename)
            {
                if (this.btnClose != null)
                {
                    this.btnClose.CustomSetActive(true);
                }
                if (this.notShowObj != null)
                {
                    this.notShowObj.CustomSetActive(false);
                }
                if (this.bSettltment)
                {
                    Singleton<SettlementSystem>.GetInstance().SnapScreenShotShowBtn(true);
                }
                if (Application.get_platform() == 11)
                {
                    try
                    {
                        string path = "/mnt/sdcard/DCIM/Sgame";
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        path = string.Format("{0}/share_{1}.png", path, DateTime.Now.ToFileTimeUtc());
                        Debug.Log("sns += SavePic " + path);
                        FileStream stream = new FileStream(this.<>f__this.m_sharePic, FileMode.Open, FileAccess.Read);
                        byte[] array = new byte[stream.Length];
                        int count = Convert.ToInt32(stream.Length);
                        stream.Read(array, 0, count);
                        stream.Close();
                        File.WriteAllBytes(path, array);
                        this.<>f__this.RefeshPhoto(path);
                        Singleton<CUIManager>.instance.OpenTips("成功保存到相册", false, 1.5f, null, new object[0]);
                    }
                    catch (Exception exception)
                    {
                        object[] inParameters = new object[] { exception.Message };
                        DebugHelper.Assert(false, "Error In Save Pic, {0}", inParameters);
                        Singleton<CUIManager>.instance.OpenTips("保存到相册出错", false, 1.5f, null, new object[0]);
                    }
                }
                else if (Application.get_platform() == 8)
                {
                    this.<>f__this.RefeshPhoto(this.<>f__this.m_sharePic);
                    Singleton<CUIManager>.instance.OpenTips("成功保存到相册", false, 1.5f, null, new object[0]);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <SetShareDefeatImage>c__AnonStorey70
        {
            internal ShareSys <>f__this;
            internal GameObject btnBarrige;
            internal GameObject btnFriend;
            internal GameObject btnZone;
            internal Transform imageTransform;

            internal void <>m__80(Texture2D text2D)
            {
                if (((this.imageTransform != null) && (this.imageTransform.GetComponent<Image>() != null)) && (text2D != null))
                {
                    this.<>f__this.BtnGray(this.btnBarrige, true);
                    this.<>f__this.BtnGray(this.btnFriend, true);
                    this.<>f__this.BtnGray(this.btnZone, true);
                    this.imageTransform.GetComponent<Image>().SetSprite(Sprite.Create(text2D, new Rect(0f, 0f, (float) text2D.get_width(), (float) text2D.get_height()), new Vector2(0.5f, 0.5f)), ImageAlphaTexLayout.None);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <ShareFriend>c__AnonStorey6B
        {
            internal ShareSys <>f__this;
            internal bool bSettltment;
            internal GameObject btnClose;
            internal GameObject notShowObj;

            internal void <>m__78(string filename)
            {
                Debug.Log("sns += capture showfriend filename" + filename);
                this.<>f__this.Share("Session", this.<>f__this.m_sharePic);
                this.btnClose.CustomSetActive(true);
                if (this.notShowObj != null)
                {
                    this.notShowObj.CustomSetActive(false);
                }
                if (this.bSettltment)
                {
                    Singleton<SettlementSystem>.GetInstance().SnapScreenShotShowBtn(true);
                    this.bSettltment = false;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <ShareMyPlayInfoToFriend>c__AnonStorey6C
        {
            internal ShareSys <>f__this;
            internal GameObject btnDetail;
            internal bool btnDetailActive;
            internal GameObject btnGraph;
            internal bool btnGraphActive;

            internal void <>m__79(string filename)
            {
                this.<>f__this.Share("Session", this.<>f__this.m_sharePic);
                this.btnGraph.CustomSetActive(this.btnGraphActive);
                this.btnDetail.CustomSetActive(this.btnDetailActive);
            }
        }

        [CompilerGenerated]
        private sealed class <ShareTimeLine>c__AnonStorey6D
        {
            internal ShareSys <>f__this;
            internal bool bSettltment;
            internal GameObject btnClose;
            internal GameObject notShowObj;

            internal void <>m__7A(string filename)
            {
                Debug.Log("sns += capture showfriend filename" + filename);
                this.<>f__this.Share("TimeLine/Qzone", this.<>f__this.m_sharePic);
                this.btnClose.CustomSetActive(true);
                if (this.notShowObj != null)
                {
                    this.notShowObj.CustomSetActive(false);
                }
                if (this.bSettltment)
                {
                    Singleton<SettlementSystem>.GetInstance().SnapScreenShotShowBtn(true);
                    this.bSettltment = false;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CLoadReq
        {
            public string m_Url;
            public ShareSys.ELoadError m_LoadError;
            public string m_CachePath;
            public int m_Type;
        }

        public enum ELoadError
        {
            None,
            SUCC,
            NotFound,
            OutOfTime
        }

        private enum HeroShareFormWidgets
        {
            DisplayRect,
            HeroAmount,
            ButtonClose,
            ShareClickText,
            ShareFriendBtn,
            TimeLineBtn,
            SkinAmount
        }

        public delegate void LoadRCallBack3(Texture2D image, ShareSys.CLoadReq req);

        private enum MysteryDiscountFOrmWigets
        {
            DiscountNum,
            ButtonClose,
            ShareClickText
        }

        public enum PVPShareFormWidgets
        {
            DisplayRect,
            ButtonClose,
            ShareImg,
            BtnSave,
            BtnFriend,
            BtnZone
        }

        public enum QQGameTeamEventType
        {
            join,
            start,
            end,
            leave
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SHARE_INFO
        {
            public uint heroId;
            public uint skinId;
            public COM_REWARDS_TYPE rewardType;
            public float beginDownloadTime;
            public string shareSkinUrl;
            public void clear()
            {
                this.shareSkinUrl = string.Empty;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ShareActivityParam
        {
            public byte bShareType;
            public byte bParamCnt;
            public uint[] ShareParam;
            public bool bUsed;
            public ShareActivityParam(bool buse)
            {
                this.bUsed = buse;
                this.bShareType = 0;
                this.bParamCnt = 0;
                this.ShareParam = null;
            }

            public void clear()
            {
                this.bUsed = false;
                this.ShareParam = null;
                this.bParamCnt = 0;
                this.bShareType = 0;
            }

            public void set(byte kShareType, byte kParamCnt, uint[] kShareParam)
            {
                this.clear();
                this.bUsed = true;
                this.bShareType = kShareType;
                this.bParamCnt = kParamCnt;
                this.ShareParam = new uint[kParamCnt];
                for (int i = 0; i < kParamCnt; i++)
                {
                    this.ShareParam[i] = kShareParam[i];
                }
            }
        }
    }
}

