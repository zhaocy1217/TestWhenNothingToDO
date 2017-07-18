namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.DataCenter;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    internal class CUILoadingSystem : Singleton<CUILoadingSystem>
    {
        private static int _pvpLoadingIndex = 1;
        private static CUIFormScript _singlePlayerLoading;
        private const uint MAX_PLAYER_NUM = 5;
        public static string PVE_PATH_LOADING = "UGUI/Form/System/PvE/Adv/Form_Adv_Loading.prefab";
        public static string PVP_PATH_LOADING = "UGUI/Form/System/PvP/Loading/Form_Loading.prefab";
        private static int SoloRandomNum = 1;

        private static int GenerateMultiRandomNum()
        {
            int dwConfValue = (int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x5e).dwConfValue;
            int num2 = _pvpLoadingIndex;
            _pvpLoadingIndex++;
            if (_pvpLoadingIndex > dwConfValue)
            {
                _pvpLoadingIndex = 1;
            }
            return num2;
        }

        private static string GenerateRandomPveLoadingTips(int randNum)
        {
            string text = Singleton<CTextManager>.GetInstance().GetText(string.Format("Loading_PVE_Tips_{0}", randNum));
            if (string.IsNullOrEmpty(text))
            {
                text = Singleton<CTextManager>.GetInstance().GetText(string.Format("Loading_PVE_Tips_{0}", 0));
            }
            return text;
        }

        private static string GenerateRandomPvpLoadingTips(int randNum)
        {
            string text = Singleton<CTextManager>.GetInstance().GetText(string.Format("Loading_PVP_Tips_{0}", randNum));
            if (string.IsNullOrEmpty(text))
            {
                text = Singleton<CTextManager>.GetInstance().GetText(string.Format("Loading_PVP_Tips_{0}", 0));
            }
            return text;
        }

        private static int GenerateSoloRandomNum()
        {
            int dwConfValue = (int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x5d).dwConfValue;
            if ((SoloRandomNum > dwConfValue) || (SoloRandomNum < 4))
            {
                SoloRandomNum = 4;
            }
            return SoloRandomNum++;
        }

        private static GameObject GetMemberItem(uint ObjId)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PVP_PATH_LOADING);
            if (form != null)
            {
                List<Player>.Enumerator enumerator = Singleton<GamePlayerCenter>.GetInstance().GetAllPlayers().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.PlayerId == ObjId)
                    {
                        int num = enumerator.Current.CampPos + 1;
                        return ((enumerator.Current.PlayerCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_1) ? form.get_gameObject().get_transform().FindChild("DownPanel").FindChild(string.Format("Down_Player{0}", num)).get_gameObject() : form.get_gameObject().get_transform().FindChild("UpPanel").FindChild(string.Format("Up_Player{0}", num)).get_gameObject());
                    }
                }
            }
            return null;
        }

        private static Player GetPlayer(COM_PLAYERCAMP Camp, int Pos)
        {
            List<Player>.Enumerator enumerator = Singleton<GamePlayerCenter>.GetInstance().GetAllCampPlayers(Camp).GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (Pos == (enumerator.Current.CampPos + 1))
                {
                    return enumerator.Current;
                }
            }
            return null;
        }

        private static uint GetPlayerId(COM_PLAYERCAMP Camp, int Pos)
        {
            List<Player>.Enumerator enumerator = Singleton<GamePlayerCenter>.GetInstance().GetAllPlayers().GetEnumerator();
            while (enumerator.MoveNext())
            {
                if ((enumerator.Current.PlayerCamp == Camp) && (Pos == (enumerator.Current.CampPos + 1)))
                {
                    return enumerator.Current.PlayerId;
                }
            }
            return 0;
        }

        public void HideLoading()
        {
            Singleton<CUIManager>.GetInstance().ClearEventGraphicsData();
            if (Singleton<LobbyLogic>.instance.inMultiGame)
            {
                HideMultiLoading();
            }
            else
            {
                SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                if (curLvelContext != null)
                {
                    if (curLvelContext.IsMobaModeWithOutGuide())
                    {
                        HideMultiLoading();
                    }
                    else
                    {
                        this.HidePveLoading();
                    }
                }
            }
        }

        private static void HideMultiLoading()
        {
            Singleton<CUIManager>.GetInstance().CloseForm(PVP_PATH_LOADING);
        }

        private void HidePveLoading()
        {
            _singlePlayerLoading = null;
            Singleton<CUIManager>.GetInstance().CloseForm(PVE_PATH_LOADING);
        }

        public override void Init()
        {
            base.Init();
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightPrepare, new RefAction<DefaultGameEventParam>(this.onGamePrepareFight));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightStart, new RefAction<DefaultGameEventParam>(this.onGameStartFight));
            Singleton<GameEventSys>.instance.AddEventHandler(GameEventDef.Event_MultiRecoverFin, new Action(this, (IntPtr) this.onGameRecoverFin));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.ADVANCE_STOP_LOADING, new Action(this, (IntPtr) this.HideLoading));
            Singleton<GameEventSys>.instance.AddEventHandler<PreDialogStartedEventParam>(GameEventDef.Event_PreDialogStarted, new RefAction<PreDialogStartedEventParam>(this.OnPreDialogStarted));
        }

        private static void On_IntimacyRela_LoadingClick(CUIEvent uiEvent)
        {
            GameObject obj2 = uiEvent.m_srcWidget.get_transform().get_parent().get_parent().FindChild("Txt_Qinmidu").get_gameObject();
            obj2.CustomSetActive(!obj2.get_activeSelf());
        }

        private void onGamePrepareFight(ref DefaultGameEventParam prm)
        {
            if (MonoSingleton<Reconnection>.instance.isProcessingRelayRecover)
            {
            }
        }

        private void onGameRecoverFin()
        {
            this.HideLoading();
        }

        private void onGameStartFight(ref DefaultGameEventParam prm)
        {
            if (!MonoSingleton<Reconnection>.instance.isProcessingRelayRecover)
            {
                this.HideLoading();
            }
        }

        [MessageHandler(0x43c)]
        public static void OnMultiGameLoadProcess(CSPkg msg)
        {
            GameObject memberItem = GetMemberItem(msg.stPkgData.stMultGameLoadProcessRsp.dwObjId);
            if (memberItem != null)
            {
                GameObject obj3 = memberItem.get_transform().Find("Txt_LoadingPct").get_gameObject();
                if (obj3 != null)
                {
                    obj3.GetComponent<Text>().set_text(string.Format("{0}%", msg.stPkgData.stMultGameLoadProcessRsp.wProcess));
                }
            }
        }

        private void OnPreDialogStarted(ref PreDialogStartedEventParam eventParam)
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if (((curLvelContext != null) && (curLvelContext.m_preDialogId > 0)) && (curLvelContext.m_preDialogId == eventParam.PreDialogId))
            {
                this.HideLoading();
            }
        }

        public static void OnSelfLoadProcess(float progress)
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if (curLvelContext != null)
            {
                if (curLvelContext.IsMobaModeWithOutGuide())
                {
                    uint objId = 0;
                    if (Singleton<WatchController>.GetInstance().IsWatching)
                    {
                        Player playerByUid = Singleton<GamePlayerCenter>.GetInstance().GetPlayerByUid(Singleton<WatchController>.GetInstance().TargetUID);
                        objId = (playerByUid == null) ? 0 : playerByUid.PlayerId;
                    }
                    if (objId == 0)
                    {
                        Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                        objId = (hostPlayer == null) ? 0 : hostPlayer.PlayerId;
                    }
                    GameObject memberItem = GetMemberItem(objId);
                    if (memberItem != null)
                    {
                        Transform transform = memberItem.get_transform().Find("Txt_LoadingPct");
                        if (transform != null)
                        {
                            transform.GetComponent<Text>().set_text(string.Format("{0}%", Convert.ToUInt16((float) (progress * 100f))));
                        }
                    }
                    if (curLvelContext.m_isWarmBattle)
                    {
                        CFakePvPHelper.FakeLoadProcess(progress);
                    }
                }
                else if (_singlePlayerLoading != null)
                {
                    _singlePlayerLoading.m_formWidgets[2].GetComponent<Text>().set_text(string.Format("{0}%", (int) (Mathf.Clamp(progress, 0f, 1f) * 100f)));
                    _singlePlayerLoading.m_formWidgets[3].GetComponent<Image>().CustomFillAmount(Mathf.Clamp(progress, 0f, 1f));
                }
            }
        }

        public void ShowLoading()
        {
            Singleton<BurnExpeditionController>.instance.Clear();
            MonoSingleton<ShareSys>.instance.m_bShowTimeline = false;
            Singleton<CUIManager>.GetInstance().CloseAllForm(null, true, true);
            Singleton<CUIManager>.GetInstance().ClearEventGraphicsData();
            if (Singleton<LobbyLogic>.instance.inMultiGame)
            {
                ShowMultiLoading();
            }
            else
            {
                SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                if (curLvelContext != null)
                {
                    if (curLvelContext.IsMobaModeWithOutGuide())
                    {
                        ShowMultiLoading();
                    }
                    else
                    {
                        this.ShowPveLoading();
                    }
                }
            }
        }

        private static void ShowMultiLoading()
        {
            CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm(PVP_PATH_LOADING, false, false);
            if (formScript != null)
            {
                if (!Singleton<CUIEventManager>.GetInstance().HasUIEventListener(enUIEventID.IntimacyRela_LoadingClick))
                {
                    Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IntimacyRela_LoadingClick, new CUIEventManager.OnUIEventHandler(CUILoadingSystem.On_IntimacyRela_LoadingClick));
                }
                SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
                IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.StaticBattleDataProvider);
                IGameActorDataProvider provider2 = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.ServerDataProvider);
                ActorStaticData actorData = new ActorStaticData();
                ActorMeta actorMeta = new ActorMeta();
                ActorMeta meta2 = new ActorMeta();
                ActorServerData data2 = new ActorServerData();
                actorMeta.ActorType = ActorTypeDef.Actor_Type_Hero;
                string str = null;
                for (int i = 1; i <= 2; i++)
                {
                    List<Player> allCampPlayers = Singleton<GamePlayerCenter>.GetInstance().GetAllCampPlayers((COM_PLAYERCAMP) i);
                    if (allCampPlayers == null)
                    {
                        DebugHelper.Assert(false, "Loading Players is Null");
                    }
                    else
                    {
                        Transform transform = (i != 1) ? formScript.get_transform().FindChild("DownPanel") : formScript.get_transform().FindChild("UpPanel");
                        for (int j = 1; j <= 5L; j++)
                        {
                            str = (i != 1) ? string.Format("Down_Player{0}", j) : string.Format("Up_Player{0}", j);
                            transform.FindChild(str).get_gameObject().CustomSetActive(false);
                        }
                        List<Player>.Enumerator enumerator = allCampPlayers.GetEnumerator();
                        COM_PLAYERCAMP playerCamp = Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp;
                        while (enumerator.MoveNext())
                        {
                            Player current = enumerator.Current;
                            if (current != null)
                            {
                                str = (i != 1) ? string.Format("Down_Player{0}", current.CampPos + 1) : string.Format("Up_Player{0}", current.CampPos + 1);
                                GameObject obj3 = transform.FindChild(str).get_gameObject();
                                obj3.CustomSetActive(true);
                                GameObject obj4 = obj3.get_transform().Find("RankFrame").get_gameObject();
                                bool flag = (current.PlayerCamp == playerCamp) && (!Singleton<WatchController>.GetInstance().IsWatching || !Singleton<WatchController>.GetInstance().IsReplaying);
                                if (obj4 != null)
                                {
                                    if (flag)
                                    {
                                        string rankFrameIconPath = CLadderView.GetRankFrameIconPath((byte) current.GradeOfRank, current.ClassOfRank);
                                        if (string.IsNullOrEmpty(rankFrameIconPath))
                                        {
                                            obj4.CustomSetActive(false);
                                        }
                                        else
                                        {
                                            Image image = obj4.GetComponent<Image>();
                                            if (image != null)
                                            {
                                                image.SetSprite(rankFrameIconPath, formScript, true, false, false, false);
                                            }
                                            obj4.CustomSetActive(true);
                                        }
                                    }
                                    else
                                    {
                                        obj4.CustomSetActive(false);
                                    }
                                }
                                Transform transform2 = obj3.get_transform().Find("RankClassText");
                                if (transform2 != null)
                                {
                                    GameObject obj5 = transform2.get_gameObject();
                                    if (flag && CLadderView.IsSuperKing((byte) current.GradeOfRank, current.ClassOfRank))
                                    {
                                        obj5.CustomSetActive(true);
                                        obj5.GetComponent<Text>().set_text(current.ClassOfRank.ToString());
                                    }
                                    else
                                    {
                                        obj5.CustomSetActive(false);
                                    }
                                }
                                Text component = obj3.get_transform().Find("Txt_PlayerName/Name").get_gameObject().GetComponent<Text>();
                                component.set_text(current.Name);
                                Image image2 = obj3.get_transform().Find("Txt_PlayerName/NobeIcon").get_gameObject().GetComponent<Image>();
                                if (image2 != null)
                                {
                                    MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(image2, (int) current.VipLv, true);
                                }
                                Text text2 = obj3.get_transform().Find("Txt_HeroName").get_gameObject().GetComponent<Text>();
                                actorMeta.ConfigId = (int) current.CaptainId;
                                text2.set_text(!actorDataProvider.GetActorStaticData(ref actorMeta, ref actorData) ? null : actorData.TheResInfo.Name);
                                GameObject obj6 = obj3.get_transform().Find("Txt_Qinmidu").get_gameObject();
                                if ((obj6 != null) && (current.IntimacyData != null))
                                {
                                    Text text3 = obj6.get_transform().Find("Text").get_gameObject().GetComponent<Text>();
                                    if (text3 != null)
                                    {
                                        text3.set_text(current.IntimacyData.title);
                                    }
                                    GameObject obj7 = obj6.get_transform().Find("BlueBg").get_gameObject();
                                    GameObject obj8 = obj6.get_transform().Find("RedBg").get_gameObject();
                                    if (current.IntimacyData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY)
                                    {
                                        obj7.CustomSetActive(true);
                                        obj8.CustomSetActive(false);
                                    }
                                    else if (current.IntimacyData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER)
                                    {
                                        obj7.CustomSetActive(false);
                                        obj8.CustomSetActive(true);
                                    }
                                }
                                GameObject obj9 = obj3.get_transform().Find("btns").get_gameObject();
                                if (obj9 != null)
                                {
                                    if ((current.IntimacyData != null) && (current.IntimacyData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY))
                                    {
                                        obj9.get_transform().Find("btnFriend").get_gameObject().CustomSetActive(true);
                                        obj9.get_transform().Find("btnLover").get_gameObject().CustomSetActive(false);
                                    }
                                    else if ((current.IntimacyData != null) && (current.IntimacyData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER))
                                    {
                                        obj9.get_transform().Find("btnFriend").get_gameObject().CustomSetActive(false);
                                        obj9.get_transform().Find("btnLover").get_gameObject().CustomSetActive(true);
                                    }
                                    else
                                    {
                                        obj9.CustomSetActive(false);
                                    }
                                }
                                component.set_color(Color.get_white());
                                GameObject obj10 = obj3.get_transform().Find("Txt_PlayerName_Mine").get_gameObject();
                                bool flag2 = (Singleton<WatchController>.GetInstance().IsWatching && (current.PlayerUId == Singleton<WatchController>.GetInstance().TargetUID)) || (!Singleton<WatchController>.GetInstance().IsWatching && (current.PlayerId == Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerId));
                                if (flag2)
                                {
                                    obj10.CustomSetActive(true);
                                }
                                else
                                {
                                    obj10.CustomSetActive(false);
                                }
                                GameObject obj11 = obj3.get_transform().Find("Txt_LoadingPct").get_gameObject();
                                if (obj11 != null)
                                {
                                    Text text4 = obj11.GetComponent<Text>();
                                    if (current.Computer)
                                    {
                                        text4.set_text(!curLvelContext.m_isWarmBattle ? "100%" : "1%");
                                    }
                                    else
                                    {
                                        text4.set_text((!MonoSingleton<Reconnection>.instance.isProcessingRelayRecover && !Singleton<WatchController>.GetInstance().IsWatching) ? "1%" : "100%");
                                    }
                                }
                                ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(current.CaptainId);
                                if (dataByKey != null)
                                {
                                    meta2.PlayerId = current.PlayerId;
                                    meta2.ActorCamp = (COM_PLAYERCAMP) i;
                                    meta2.ConfigId = (int) dataByKey.dwCfgID;
                                    meta2.ActorType = ActorTypeDef.Actor_Type_Hero;
                                    Image image3 = obj3.get_transform().Find("Hero").get_gameObject().GetComponent<Image>();
                                    if (provider2.GetActorServerData(ref meta2, ref data2))
                                    {
                                        ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin((uint) data2.TheActorMeta.ConfigId, data2.SkinId);
                                        if (heroSkin != null)
                                        {
                                            image3.SetSprite(CUIUtility.s_Sprite_Dynamic_BustHero_Dir + StringHelper.UTF8BytesToString(ref heroSkin.szSkinPicID), formScript, true, false, false, true);
                                            if (heroSkin.dwSkinID > 0)
                                            {
                                                text2.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("LoadingSkinNameTxt"), heroSkin.szSkinName, heroSkin.szHeroName));
                                            }
                                            if (flag2)
                                            {
                                                text2.set_color(CUIUtility.s_Text_Color_MyHeroName);
                                                Outline outline = text2.get_gameObject().GetComponent<Outline>();
                                                if (outline != null)
                                                {
                                                    outline.set_effectColor(CUIUtility.s_Text_OutLineColor_MyHeroName);
                                                }
                                            }
                                        }
                                        bool bActive = ((current.PlayerCamp == playerCamp) && (!Singleton<WatchController>.GetInstance().IsWatching || !Singleton<WatchController>.GetInstance().IsReplaying)) && (curLvelContext.m_isWarmBattle || !current.Computer);
                                        Transform transform3 = obj3.get_transform().Find("heroProficiencyBgImg");
                                        if (transform3 != null)
                                        {
                                            transform3.get_gameObject().CustomSetActive(bActive);
                                            if (bActive)
                                            {
                                                CUICommonSystem.SetHeroProficiencyBgImage(formScript, transform3.get_gameObject(), (int) data2.TheProficiencyInfo.Level, true);
                                            }
                                        }
                                        Transform transform4 = obj3.get_transform().Find("heroProficiencyImg");
                                        if (transform4 != null)
                                        {
                                            transform4.get_gameObject().CustomSetActive(bActive);
                                            if (bActive)
                                            {
                                                CUICommonSystem.SetHeroProficiencyIconImage(formScript, transform4.get_gameObject(), (int) data2.TheProficiencyInfo.Level);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        image3.SetSprite(CUIUtility.s_Sprite_Dynamic_BustHero_Dir + StringHelper.UTF8BytesToString(ref dataByKey.szImagePath), formScript, true, false, false, true);
                                    }
                                    uint skillID = 0;
                                    if (provider2.GetActorServerCommonSkillData(ref meta2, out skillID) && (skillID != 0))
                                    {
                                        ResSkillCfgInfo info2 = GameDataMgr.skillDatabin.GetDataByKey(skillID);
                                        if (info2 != null)
                                        {
                                            obj3.get_transform().Find("SelSkill").get_gameObject().CustomSetActive(true);
                                            string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Skill_Dir, Utility.UTF8Convert(info2.szIconPath));
                                            obj3.get_transform().Find("SelSkill/Icon").GetComponent<Image>().SetSprite(prefabPath, formScript.GetComponent<CUIFormScript>(), true, false, false, false);
                                        }
                                        else
                                        {
                                            obj3.get_transform().Find("SelSkill").get_gameObject().CustomSetActive(false);
                                        }
                                    }
                                    else
                                    {
                                        obj3.get_transform().Find("SelSkill").get_gameObject().CustomSetActive(false);
                                    }
                                    Transform transform5 = obj3.get_transform().Find("skinLabelImage");
                                    if (transform5 != null)
                                    {
                                        CUICommonSystem.SetHeroSkinLabelPic(formScript, transform5.get_gameObject(), dataByKey.dwCfgID, data2.SkinId);
                                    }
                                }
                            }
                            else
                            {
                                DebugHelper.Assert(false, "Loading Player is Null");
                            }
                        }
                    }
                }
                GameObject widget = formScript.GetWidget(0);
                GameObject obj13 = formScript.GetWidget(1);
                GameObject obj14 = formScript.GetWidget(2);
                if (curLvelContext.IsGameTypeGuildMatch())
                {
                    widget.CustomSetActive(false);
                    obj13.CustomSetActive(false);
                    obj14.CustomSetActive(true);
                    CSDT_CAMP_EXT_GUILDMATCH[] campExtGuildMatchInfo = curLvelContext.GetCampExtGuildMatchInfo();
                    if ((campExtGuildMatchInfo != null) && (campExtGuildMatchInfo.Length == 2))
                    {
                        Image image5 = formScript.GetWidget(3).GetComponent<Image>();
                        Text text5 = formScript.GetWidget(4).GetComponent<Text>();
                        Image image6 = formScript.GetWidget(5).GetComponent<Image>();
                        Text text6 = formScript.GetWidget(6).GetComponent<Text>();
                        image5.SetSprite(CUIUtility.s_Sprite_Dynamic_GuildHead_Dir + campExtGuildMatchInfo[0].dwGuildHeadID, formScript, true, false, false, false);
                        text5.set_text(StringHelper.UTF8BytesToString(ref campExtGuildMatchInfo[0].szGuildName));
                        image6.SetSprite(CUIUtility.s_Sprite_Dynamic_GuildHead_Dir + campExtGuildMatchInfo[1].dwGuildHeadID, formScript, true, false, false, false);
                        text6.set_text(StringHelper.UTF8BytesToString(ref campExtGuildMatchInfo[1].szGuildName));
                    }
                }
                else
                {
                    widget.CustomSetActive(true);
                    obj13.CustomSetActive(true);
                    obj14.CustomSetActive(false);
                    formScript.GetWidget(7).GetComponent<Text>().set_text(GenerateRandomPvpLoadingTips(GenerateMultiRandomNum()));
                    obj13.CustomSetActive(MonoSingleton<Reconnection>.instance.isProcessingRelayRecover);
                }
            }
        }

        private void ShowPveLoading()
        {
            _singlePlayerLoading = Singleton<CUIManager>.GetInstance().OpenForm(PVE_PATH_LOADING, false, true);
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            int randNum = 0;
            if (curLvelContext != null)
            {
                if (curLvelContext.IsGameTypeGuide())
                {
                    if (CBattleGuideManager.Is1v1GuideLevel(curLvelContext.m_mapID))
                    {
                        randNum = 1;
                    }
                    else if (CBattleGuideManager.Is5v5GuideLevel(curLvelContext.m_mapID))
                    {
                        randNum = 2;
                    }
                    else if (curLvelContext.m_mapID == 8)
                    {
                        randNum = 3;
                    }
                    else
                    {
                        randNum = GenerateSoloRandomNum();
                    }
                }
                else
                {
                    randNum = GenerateSoloRandomNum();
                }
            }
            _singlePlayerLoading.m_formWidgets[1].GetComponent<Text>().set_text(GenerateRandomPveLoadingTips(randNum));
            _singlePlayerLoading.m_formWidgets[2].GetComponent<Text>().set_text(string.Format("{0}%", 0));
            _singlePlayerLoading.m_formWidgets[3].GetComponent<Image>().CustomFillAmount(0f);
            Image component = _singlePlayerLoading.m_formWidgets[4].GetComponent<Image>();
            if (randNum >= 4)
            {
                MonoSingleton<BannerImageSys>.GetInstance().TrySetLoadingImage((uint) randNum, component);
            }
            else
            {
                component.SetSprite(string.Format("{0}{1}{2}", "UGUI/Sprite/Dynamic/", "Loading/LoadingNotice", randNum), _singlePlayerLoading, true, false, false, false);
            }
        }

        public enum enLoadingFormWidget
        {
            TipsLeft,
            TipsRight,
            GuildMatchPanel,
            LeftGuildHead,
            LeftGuildName,
            RightGuildHead,
            RightGuildName,
            TipsLeftText
        }

        public enum SinglePlayerLoadingFormWidget
        {
            LoadingNotice = 4,
            LoadingProgress = 2,
            LoadingProgressBar = 3,
            None = -1,
            Reserve = 0,
            Tips = 1
        }
    }
}

