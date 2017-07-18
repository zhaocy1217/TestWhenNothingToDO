namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    internal class PVESettleView
    {
        private static float _coinFrom;
        private static LTDescr _coinLTD;
        private static COMDT_REWARD_MULTIPLE_DETAIL _coinMulti;
        private static float _coinTo;
        private static Text _coinTweenText;
        private static GameObject _continueBtn1;
        private static GameObject _continueBtn2;
        private static float _expFrom;
        private static LTDescr _expLTD;
        private static float _expTo;
        private static RectTransform _expTweenRect;
        private static uint _lvUpGrade;
        [CompilerGenerated]
        private static Action<float> <>f__am$cacheC;
        [CompilerGenerated]
        private static Action<float> <>f__am$cacheD;
        private const float expBarWidth = 260f;
        public const string REWARD_ANIM_1_NAME = "Box_Show_2";
        public const string REWARD_ANIM_2_NAME = "AppearThePrizes_2";
        public const string STAR_WIN_ANIM_NAME = "Win_Show";
        private const float TweenTime = 2f;

        private static void DoCoinAndExpTween()
        {
            if ((_expTweenRect != null) && (_expTweenRect.get_gameObject() != null))
            {
                if (<>f__am$cacheC == null)
                {
                    <>f__am$cacheC = delegate (float value) {
                        if ((_expTweenRect != null) && (_expTweenRect.get_gameObject() != null))
                        {
                            _expTweenRect.set_sizeDelta(new Vector2(value * 260f, _expTweenRect.get_sizeDelta().y));
                            if (value >= _expTo)
                            {
                                DoExpTweenEnd();
                            }
                        }
                    };
                }
                _expLTD = LeanTween.value(_expTweenRect.get_gameObject(), <>f__am$cacheC, _expFrom, _expTo, 2f);
            }
            if ((_coinTweenText != null) && (_coinTweenText.get_gameObject() != null))
            {
                if (<>f__am$cacheD == null)
                {
                    <>f__am$cacheD = delegate (float value) {
                        if ((_coinTweenText != null) && (_coinTweenText.get_gameObject() != null))
                        {
                            _coinTweenText.set_text(string.Format("+{0}", value.ToString("N0")));
                            if (value >= _coinTo)
                            {
                                DoCoinTweenEnd();
                            }
                        }
                    };
                }
                _coinLTD = LeanTween.value(_coinTweenText.get_gameObject(), <>f__am$cacheD, _coinFrom, _coinTo, 2f);
            }
        }

        public static void DoCoinTweenEnd()
        {
            if ((_coinLTD != null) && (_coinTweenText != null))
            {
                _coinTweenText.set_text(string.Format("+{0}", _coinTo.ToString("N0")));
                if ((_coinMulti != null) && (Singleton<BattleStatistic>.GetInstance().acntInfo != null))
                {
                    COMDT_ACNT_INFO acntInfo = Singleton<BattleStatistic>.GetInstance().acntInfo;
                    CUICommonSystem.AppendMultipleText(_coinTweenText, CUseable.GetMultiple(acntInfo.dwPvpSettleBaseCoin, ref _coinMulti, 0, -1));
                }
                _coinLTD.cancel();
                _coinLTD = null;
                _coinTweenText = null;
                _coinMulti = null;
            }
        }

        private static void DoExpTweenEnd()
        {
            if ((_expTweenRect != null) && (_expLTD != null))
            {
                _expTweenRect.set_sizeDelta(new Vector2(_expTo * 260f, _expTweenRect.get_sizeDelta().y));
                _expLTD.cancel();
                _expLTD = null;
                _expTweenRect = null;
            }
            if (_continueBtn1 != null)
            {
                _continueBtn1.CustomSetActive(true);
                _continueBtn1 = null;
            }
            if (_continueBtn2 != null)
            {
                _continueBtn2.CustomSetActive(true);
                _continueBtn2 = null;
            }
            if (_lvUpGrade > 1)
            {
                CUIEvent event3 = new CUIEvent();
                event3.m_eventID = enUIEventID.Settle_OpenLvlUp;
                event3.m_eventParams.tag = ((int) _lvUpGrade) - 1;
                event3.m_eventParams.tag2 = (int) _lvUpGrade;
                CUIEvent uiEvent = event3;
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
            }
            _lvUpGrade = 0;
        }

        private static void GoToStarAnimState(CUIFormScript starForm, ref StarCondition[] starArr, string animSuffix = "")
        {
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            for (int i = 0; i < 3; i++)
            {
                if (starArr[i].bCompelete)
                {
                    if (i == 1)
                    {
                        flag2 = true;
                    }
                    if (i == 2)
                    {
                        flag3 = true;
                    }
                }
                if (!starArr[i].bCompelete && (i == 0))
                {
                    flag = true;
                }
            }
            GameObject target = starForm.get_transform().Find("Root").get_gameObject();
            if (flag)
            {
                CUICommonSystem.PlayAnimator(target, string.Format("Star_3{0}", animSuffix));
            }
            else if (flag2)
            {
                if (flag3)
                {
                    CUICommonSystem.PlayAnimator(target, string.Format("Star_3{0}", animSuffix));
                }
                else
                {
                    CUICommonSystem.PlayAnimator(target, string.Format("Star_2{0}", animSuffix));
                }
            }
            else if (flag3)
            {
                CUICommonSystem.PlayAnimator(target, string.Format("Star_1_3{0}", animSuffix));
            }
            else
            {
                CUICommonSystem.PlayAnimator(target, string.Format("Star_1{0}", animSuffix));
            }
        }

        public static void OnStarWinAnimEnd(CUIFormScript starForm, ref StarCondition[] starArr)
        {
            GoToStarAnimState(starForm, ref starArr, string.Empty);
        }

        public static void SetExpFormData(CUIFormScript form, COMDT_SETTLE_RESULT_DETAIL settleData)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                new PVEPlayerItem(form.get_transform().Find("Root/Panel_Exp/Exp_Player").get_gameObject()).addExp(settleData.stAcntInfo.dwSettleExp);
                CUI3DImageScript component = form.get_transform().Find("Root/3DImage").get_gameObject().GetComponent<CUI3DImageScript>();
                DebugHelper.Assert(component != null);
                int num = 1;
                for (int i = 0; i < settleData.stHeroList.bNum; i++)
                {
                    CHeroInfo info2;
                    uint dwHeroConfID = settleData.stHeroList.astHeroList[i].dwHeroConfID;
                    GameObject heroItem = form.get_transform().Find(string.Format("Root/Panel_Exp/Exp_Hero{0}", num)).get_gameObject();
                    if (masterRoleInfo.GetHeroInfoDic().TryGetValue(dwHeroConfID, out info2))
                    {
                        ResHeroCfgInfo cfgInfo = info2.cfgInfo;
                        PVEHeroItem item2 = new PVEHeroItem(heroItem, cfgInfo.dwCfgID);
                        if (num <= settleData.stHeroList.bNum)
                        {
                            heroItem.CustomSetActive(true);
                            item2.addExp(settleData.stHeroList.astHeroList[num - 1].dwSettleExp);
                            int heroWearSkinId = (int) masterRoleInfo.GetHeroWearSkinId(cfgInfo.dwCfgID);
                            string objectName = CUICommonSystem.GetHeroPrefabPath(cfgInfo.dwCfgID, heroWearSkinId, true).ObjectName;
                            GameObject model = component.AddGameObjectToPath(objectName, false, string.Format("_root/Hero{0}", num));
                            CHeroAnimaSystem instance = Singleton<CHeroAnimaSystem>.GetInstance();
                            instance.Set3DModel(model);
                            instance.InitAnimatList();
                            instance.InitAnimatSoundList(cfgInfo.dwCfgID, (uint) heroWearSkinId);
                            instance.OnModePlayAnima("idleshow2");
                        }
                    }
                    num++;
                }
            }
        }

        public static void SetItemEtcCell(CUIFormScript form, GameObject item, Text name, COMDT_REWARD_INFO rewardInfo, COMDT_SETTLE_RESULT_DETAIL settleData)
        {
            CUseable itemUseable = null;
            switch (rewardInfo.bType)
            {
                case 1:
                {
                    itemUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, 0L, rewardInfo.stRewardInfo.stItem.dwItemID, (int) rewardInfo.stRewardInfo.stItem.dwCnt, 0);
                    itemUseable.SetMultiple(ref settleData.stMultipleDetail, true);
                    CUICommonSystem.SetItemCell(form, item, itemUseable, true, false, false, false);
                    ResPropInfo dataByKey = GameDataMgr.itemDatabin.GetDataByKey(rewardInfo.stRewardInfo.stItem.dwItemID);
                    if (dataByKey != null)
                    {
                        name.set_text(StringHelper.UTF8BytesToString(ref dataByKey.szName));
                    }
                    break;
                }
                case 3:
                    itemUseable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enDianQuan, (int) rewardInfo.stRewardInfo.dwCoupons);
                    itemUseable.SetMultiple(ref settleData.stMultipleDetail, true);
                    CUICommonSystem.SetItemCell(form, item, itemUseable, true, false, false, false);
                    name.set_text(itemUseable.m_name);
                    break;

                case 4:
                {
                    itemUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP, 0L, rewardInfo.stRewardInfo.stEquip.dwEquipID, (int) rewardInfo.stRewardInfo.stEquip.dwCnt, 0);
                    itemUseable.SetMultiple(ref settleData.stMultipleDetail, true);
                    CUICommonSystem.SetItemCell(form, item, itemUseable, true, false, false, false);
                    ResEquipInfo info2 = GameDataMgr.equipInfoDatabin.GetDataByKey(rewardInfo.stRewardInfo.stEquip.dwEquipID);
                    if (info2 != null)
                    {
                        name.set_text(StringHelper.UTF8BytesToString(ref info2.szName));
                    }
                    break;
                }
                case 5:
                {
                    itemUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_HERO, 0L, rewardInfo.stRewardInfo.stHero.dwHeroID, (int) rewardInfo.stRewardInfo.stHero.dwCnt, 0);
                    itemUseable.SetMultiple(ref settleData.stMultipleDetail, true);
                    CUICommonSystem.SetItemCell(form, item, itemUseable, true, false, false, false);
                    ResHeroCfgInfo info3 = GameDataMgr.heroDatabin.GetDataByKey(rewardInfo.stRewardInfo.stHero.dwHeroID);
                    if (info3 != null)
                    {
                        name.set_text(StringHelper.UTF8BytesToString(ref info3.szName));
                    }
                    break;
                }
                case 6:
                {
                    itemUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, 0L, rewardInfo.stRewardInfo.stSymbol.dwSymbolID, (int) rewardInfo.stRewardInfo.stSymbol.dwCnt, 0);
                    itemUseable.SetMultiple(ref settleData.stMultipleDetail, true);
                    CUICommonSystem.SetItemCell(form, item, itemUseable, true, false, false, false);
                    ResSymbolInfo info4 = GameDataMgr.symbolInfoDatabin.GetDataByKey(rewardInfo.stRewardInfo.stSymbol.dwSymbolID);
                    if (info4 != null)
                    {
                        name.set_text(StringHelper.UTF8BytesToString(ref info4.szName));
                    }
                    break;
                }
                case 0x10:
                    itemUseable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enDiamond, (int) rewardInfo.stRewardInfo.dwDiamond);
                    itemUseable.SetMultiple(ref settleData.stMultipleDetail, true);
                    CUICommonSystem.SetItemCell(form, item, itemUseable, true, false, false, false);
                    name.set_text(itemUseable.m_name);
                    break;
            }
        }

        public static void SetRewardFormData(CUIFormScript form, COMDT_SETTLE_RESULT_DETAIL settleData)
        {
            Singleton<CUIManager>.GetInstance().LoadUIScenePrefab(CUIUtility.s_heroSceneBgPath, form);
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if (curLvelContext != null)
            {
                GameObject obj2 = form.get_transform().Find("Root/Panel_Interactable/Button_Next").get_gameObject();
                if (curLvelContext.IsGameTypeActivity())
                {
                    obj2.CustomSetActive(false);
                }
                else
                {
                    int levelId = CAdventureSys.GetNextLevelId(curLvelContext.m_chapterNo, curLvelContext.m_levelNo, curLvelContext.m_levelDifficulty);
                    if (levelId != 0)
                    {
                        if (Singleton<CAdventureSys>.GetInstance().IsLevelOpen(levelId))
                        {
                            obj2.CustomSetActive(true);
                        }
                        else
                        {
                            obj2.CustomSetActive(false);
                        }
                    }
                    else
                    {
                        obj2.CustomSetActive(false);
                    }
                }
                obj2.CustomSetActive(false);
                Show3DModel(form);
                GameObject obj3 = form.get_transform().Find("Root/Panel_Award/Award/Panel_GuanKa/GuanKaDifficulty1").get_gameObject();
                GameObject obj4 = form.get_transform().Find("Root/Panel_Award/Award/Panel_GuanKa/GuanKaDifficulty2").get_gameObject();
                GameObject obj5 = form.get_transform().Find("Root/Panel_Award/Award/Panel_GuanKa/GuanKaDifficulty3").get_gameObject();
                Text component = form.get_transform().Find("Root/Panel_Award/Award/Panel_GuanKa/GuanKaName").get_gameObject().GetComponent<Text>();
                if (curLvelContext.m_levelDifficulty == 1)
                {
                    obj4.CustomSetActive(false);
                    obj5.CustomSetActive(false);
                }
                else if (curLvelContext.m_levelDifficulty == 2)
                {
                    obj3.CustomSetActive(false);
                    obj5.CustomSetActive(false);
                }
                else if (curLvelContext.m_levelDifficulty == 3)
                {
                    obj4.CustomSetActive(false);
                    obj3.CustomSetActive(false);
                }
                component.set_text(string.Format(curLvelContext.m_levelName, new object[0]));
                _continueBtn1 = form.get_transform().Find("Root/Panel_Interactable/Button_Once").get_gameObject();
                _continueBtn2 = form.get_transform().Find("Root/Panel_Interactable/Button_ReturnLobby").get_gameObject();
                _continueBtn1.CustomSetActive(true);
                _continueBtn2.CustomSetActive(true);
                ShowReward(form, settleData);
                CUICommonSystem.PlayAnimator(form.get_gameObject(), "Box_Show_2");
            }
        }

        public static void SetStarFormData(CUIFormScript form, COMDT_SETTLE_RESULT_DETAIL settleData, ref StarCondition[] starArr)
        {
            GameObject target = form.get_transform().Find("Root").get_gameObject();
            int num = 0;
            for (int i = 1; i < 4; i++)
            {
                GameObject obj3 = form.get_transform().Find(string.Format("Root/Condition{0}", i)).get_gameObject();
                Text component = obj3.get_transform().Find("Condition_text").get_gameObject().GetComponent<Text>();
                component.set_text(starArr[i - 1].ConditionName);
                if (!starArr[i - 1].bCompelete)
                {
                    string str = string.Empty;
                    if (i == 2)
                    {
                        str = "Condition_Star1";
                    }
                    else
                    {
                        str = "Condition_Star";
                    }
                    obj3.get_transform().Find(str).get_gameObject().CustomSetActive(false);
                    component.set_color(CUIUtility.s_Color_Grey);
                }
                else
                {
                    num++;
                }
            }
            for (int j = 1; j < 4; j++)
            {
                if (num < j)
                {
                    form.get_transform().Find(string.Format("Root/Panel_Star/Star{0}", j)).get_gameObject().CustomSetActive(false);
                }
            }
            CUICommonSystem.PlayAnimator(target, "Win_Show");
        }

        private static void Show3DModel(CUIFormScript belongForm)
        {
            CUI3DImageScript component = null;
            Transform transform = belongForm.get_transform().Find("Root/Panel_Award/3DImage");
            if (transform != null)
            {
                component = transform.GetComponent<CUI3DImageScript>();
            }
            if (component != null)
            {
                PlayerKDA hostKDA = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat.GetHostKDA();
                if (hostKDA != null)
                {
                    ListView<HeroKDA>.Enumerator enumerator = hostKDA.GetEnumerator();
                    uint heroId = 0;
                    while (enumerator.MoveNext())
                    {
                        HeroKDA current = enumerator.Current;
                        if (current != null)
                        {
                            heroId = (uint) current.HeroId;
                            break;
                        }
                    }
                    int heroWearSkinId = (int) Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetHeroWearSkinId(heroId);
                    ObjNameData data = CUICommonSystem.GetHeroPrefabPath(heroId, heroWearSkinId, true);
                    GameObject model = component.AddGameObject(data.ObjectName, false, false);
                    CHeroAnimaSystem instance = Singleton<CHeroAnimaSystem>.GetInstance();
                    instance.Set3DModel(model);
                    if (model != null)
                    {
                        instance.InitAnimatList();
                        instance.InitAnimatSoundList(heroId, (uint) heroWearSkinId);
                    }
                }
            }
        }

        public static void ShowPlayerLevelUp(CUIFormScript form, int oldLvl, int newLvl)
        {
            <ShowPlayerLevelUp>c__AnonStorey65 storey = new <ShowPlayerLevelUp>c__AnonStorey65();
            storey.newLvl = newLvl;
            if (form != null)
            {
                <ShowPlayerLevelUp>c__AnonStorey66 storey2 = new <ShowPlayerLevelUp>c__AnonStorey66();
                storey2.<>f__ref$101 = storey;
                GameObject obj2 = form.get_transform().Find("PlayerLvlUp").get_gameObject();
                obj2.get_transform().Find("bg/TxtPlayerLvl").get_gameObject().GetComponent<Text>().set_text(storey.newLvl.ToString());
                obj2.get_transform().Find("bg/TxtPlayerBeforeLvl").get_gameObject().GetComponent<Text>().set_text(oldLvl.ToString());
                object[] inParameters = new object[] { oldLvl };
                DebugHelper.Assert(GameDataMgr.acntExpDatabin.GetDataByKey((uint) oldLvl) != null, "Can't find acnt exp config -- level {0}", inParameters);
                object[] objArray2 = new object[] { storey.newLvl };
                DebugHelper.Assert(GameDataMgr.acntExpDatabin.GetDataByKey((uint) storey.newLvl) != null, "Can't find acnt exp config -- level {0}", objArray2);
                Transform transform = obj2.get_transform().Find("Panel/groupPanel/symbolPosPanel");
                int symbolPosOpenCnt = CSymbolInfo.GetSymbolPosOpenCnt(oldLvl);
                int num2 = CSymbolInfo.GetSymbolPosOpenCnt(storey.newLvl);
                storey2.hasBuy = false;
                storey2.master = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
                if ((storey2.master != null) && (symbolPosOpenCnt < num2))
                {
                    GameDataMgr.symbolPosDatabin.Accept(new Action<ResSymbolPos>(storey2.<>m__6B));
                }
                transform.get_gameObject().CustomSetActive(!storey2.hasBuy && (num2 > symbolPosOpenCnt));
                if (!storey2.hasBuy && (num2 > symbolPosOpenCnt))
                {
                    transform.Find("curCntText").get_gameObject().GetComponent<Text>().set_text(symbolPosOpenCnt.ToString());
                    transform.Find("levelUpCntText").get_gameObject().GetComponent<Text>().set_text(num2.ToString());
                }
                Transform transform2 = obj2.get_transform().Find("Panel/groupPanel/symbolLevelPanel");
                int symbolLvlLimit = CSymbolInfo.GetSymbolLvlLimit(oldLvl);
                int num4 = CSymbolInfo.GetSymbolLvlLimit(storey.newLvl);
                transform2.get_gameObject().CustomSetActive(num4 > symbolLvlLimit);
                if (num4 > symbolLvlLimit)
                {
                    Text component = transform2.Find("curCntText").get_gameObject().GetComponent<Text>();
                    Text text6 = transform2.Find("levelUpCntText").get_gameObject().GetComponent<Text>();
                    component.set_text(symbolLvlLimit.ToString());
                    text6.set_text(num4.ToString());
                }
                Transform transform3 = obj2.get_transform().Find("Panel/groupPanel/symbolPageCntPanel");
                ResHeroSymbolLvl dataByKey = GameDataMgr.heroSymbolLvlDatabin.GetDataByKey((long) storey.newLvl);
                if (dataByKey != null)
                {
                    transform3.get_gameObject().CustomSetActive(dataByKey.bPresentSymbolPage > 0);
                    CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                    if ((masterRoleInfo != null) && ((dataByKey.bPresentSymbolPage > 0) && (masterRoleInfo != null)))
                    {
                        Text text7 = transform3.Find("curCntText").get_gameObject().GetComponent<Text>();
                        Text text8 = transform3.Find("levelUpCntText").get_gameObject().GetComponent<Text>();
                        text7.set_text((masterRoleInfo.m_symbolInfo.m_pageCount - 1).ToString());
                        text8.set_text(masterRoleInfo.m_symbolInfo.m_pageCount.ToString());
                    }
                }
            }
        }

        private static void ShowReward(CUIFormScript belongForm, COMDT_SETTLE_RESULT_DETAIL settleData)
        {
            if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() != null)
            {
                COMDT_REWARD_INFO comdt_reward_info;
                GameObject obj2 = belongForm.get_transform().Find("Root/Panel_Award/Award/ItemAndCoin/Panel_Gold").get_gameObject();
                Text component = obj2.get_transform().Find("GoldNum").get_gameObject().GetComponent<Text>();
                GameObject obj3 = obj2.get_transform().Find("GoldMax").get_gameObject();
                if (settleData.stAcntInfo.bReachDailyLimit > 0)
                {
                    obj3.CustomSetActive(true);
                }
                else
                {
                    obj3.CustomSetActive(false);
                }
                component.set_text("0");
                COMDT_REWARD_DETAIL stReward = settleData.stReward;
                COMDT_ACNT_INFO stAcntInfo = settleData.stAcntInfo;
                if (stAcntInfo != null)
                {
                    GameObject obj4 = belongForm.get_transform().FindChild("Root/Panel_Award/Award/Panel_PlayerExp/PvpExpNode").get_gameObject();
                    Text text2 = obj4.get_transform().FindChild("PvpExpTxt").get_gameObject().GetComponent<Text>();
                    Text target = obj4.get_transform().FindChild("AddPvpExpTxt").get_gameObject().GetComponent<Text>();
                    RectTransform transform = obj4.get_transform().FindChild("PvpExpSliderBg/BasePvpExpSlider").get_gameObject().GetComponent<RectTransform>();
                    RectTransform transform2 = obj4.get_transform().FindChild("PvpExpSliderBg/AddPvpExpSlider").get_gameObject().GetComponent<RectTransform>();
                    Text text4 = obj4.get_transform().FindChild("PlayerName").get_gameObject().GetComponent<Text>();
                    CUIHttpImageScript script = obj4.get_transform().FindChild("HeadImage").get_gameObject().GetComponent<CUIHttpImageScript>();
                    Text text5 = obj4.get_transform().FindChild("PvpLevelTxt").get_gameObject().GetComponent<Text>();
                    Image image = obj4.get_transform().FindChild("NobeIcon").get_gameObject().GetComponent<Image>();
                    MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(image, (int) Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwCurLevel, false);
                    Image image2 = obj4.get_transform().FindChild("HeadFrame").get_gameObject().GetComponent<Image>();
                    MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(image2, (int) Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwHeadIconId);
                    text5.set_text(string.Format("Lv.{0}", stAcntInfo.dwPvpLv.ToString()));
                    ResAcntPvpExpInfo dataByKey = GameDataMgr.acntPvpExpDatabin.GetDataByKey((uint) ((byte) stAcntInfo.dwPvpLv));
                    GameObject obj5 = obj4.get_transform().FindChild("ExpMax").get_gameObject();
                    if (stAcntInfo.bExpDailyLimit == 0)
                    {
                        obj5.CustomSetActive(false);
                    }
                    text2.set_text(string.Format("{0}/{1}", stAcntInfo.dwPvpExp, dataByKey.dwNeedExp));
                    target.set_text(string.Format("+{0}", stAcntInfo.dwPvpSettleExp));
                    CUICommonSystem.AppendMultipleText(target, CUseable.GetMultiple(stAcntInfo.dwPvpSettleBaseExp, ref settleData.stMultipleDetail, 15, -1));
                    text4.set_text(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().Name);
                    string headUrl = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().HeadUrl;
                    if (!CSysDynamicBlock.bLobbyEntryBlocked)
                    {
                        script.SetImageUrl(headUrl);
                    }
                    if (stAcntInfo.dwPvpSettleExp > 0)
                    {
                        Singleton<CSoundManager>.GetInstance().PostEvent("UI_count_jingyan", null);
                    }
                    float num = 0f;
                    if (stAcntInfo.dwPvpExp < stAcntInfo.dwPvpSettleExp)
                    {
                        transform.set_sizeDelta(new Vector2(num * 260f, transform.get_sizeDelta().y));
                        _lvUpGrade = stAcntInfo.dwPvpLv;
                    }
                    else
                    {
                        num = ((float) (stAcntInfo.dwPvpExp - stAcntInfo.dwPvpSettleExp)) / ((float) dataByKey.dwNeedExp);
                        transform.set_sizeDelta(new Vector2(num * 260f, transform.get_sizeDelta().y));
                        _lvUpGrade = 0;
                    }
                    float num2 = ((float) stAcntInfo.dwPvpExp) / ((float) dataByKey.dwNeedExp);
                    _expFrom = num;
                    _expTo = num2;
                    transform2.set_sizeDelta(new Vector2(num * 260f, transform2.get_sizeDelta().y));
                    _expTweenRect = transform2;
                    _coinFrom = 0f;
                    _coinTo = 0f;
                    for (int j = 0; j < stReward.bNum; j++)
                    {
                        comdt_reward_info = stReward.astRewardDetail[j];
                        if (comdt_reward_info.bType == 11)
                        {
                            _coinTo = comdt_reward_info.stRewardInfo.dwPvpCoin;
                            _coinMulti = settleData.stMultipleDetail;
                        }
                    }
                    _coinTweenText = component;
                    DoCoinAndExpTween();
                }
                ListView<COMDT_REWARD_INFO> view = new ListView<COMDT_REWARD_INFO>();
                GameObject obj6 = belongForm.get_transform().Find("Root/Panel_Award/Award/Panel_QQVIPGold").get_gameObject();
                if (obj6 != null)
                {
                    obj6.CustomSetActive(false);
                }
                GameObject obj7 = belongForm.get_transform().Find("Root/Panel_Award/Award/ItemAndCoin/FirstGain").get_gameObject();
                if (obj7 != null)
                {
                    obj7.CustomSetActive(false);
                }
                for (int i = 0; i < stReward.bNum; i++)
                {
                    comdt_reward_info = stReward.astRewardDetail[i];
                    switch (comdt_reward_info.bType)
                    {
                        case 6:
                            view.Add(stReward.astRewardDetail[i]);
                            if (obj7 != null)
                            {
                                obj7.CustomSetActive(false);
                            }
                            break;

                        case 11:
                            CUICommonSystem.AppendMultipleText(component, CUseable.GetMultiple(stAcntInfo.dwPvpSettleBaseCoin, ref settleData.stMultipleDetail, 0, -1));
                            if (obj6 != null)
                            {
                                obj6.CustomSetActive(false);
                                Text text6 = obj6.get_transform().FindChild("Text_Value").get_gameObject().GetComponent<Text>();
                                GameObject obj8 = obj6.get_transform().FindChild("Icon_QQVIP").get_gameObject();
                                GameObject obj9 = obj6.get_transform().FindChild("Icon_QQSVIP").get_gameObject();
                                obj8.CustomSetActive(false);
                                obj9.CustomSetActive(false);
                                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                                uint num5 = CUseable.GetQqVipExtraCoin(comdt_reward_info.stRewardInfo.dwPvpCoin, ref settleData.stMultipleDetail, 0);
                                if ((masterRoleInfo != null) && (num5 > 0))
                                {
                                    text6.set_text(string.Format("+{0}", num5));
                                    if (masterRoleInfo.HasVip(0x10))
                                    {
                                        obj6.CustomSetActive(true);
                                        obj9.CustomSetActive(true);
                                    }
                                    else if (masterRoleInfo.HasVip(1))
                                    {
                                        obj6.CustomSetActive(true);
                                        obj8.CustomSetActive(true);
                                    }
                                }
                                obj6.CustomSetActive(false);
                            }
                            break;
                    }
                }
                GameObject obj10 = belongForm.get_transform().Find("Root/Panel_Award/Award/ItemAndCoin/itemCell").get_gameObject();
                obj10.CustomSetActive(false);
                if (view.Count > 0)
                {
                    Text name = obj10.get_transform().FindChild("ItemName").get_gameObject().GetComponent<Text>();
                    obj10.CustomSetActive(true);
                    comdt_reward_info = view[0];
                    SetItemEtcCell(belongForm, obj10, name, comdt_reward_info, settleData);
                }
            }
        }

        public static void StopExpAnim(CUIFormScript expForm)
        {
            if (expForm.get_transform().Find("Root/EscapeAnim").get_gameObject().get_activeSelf())
            {
                expForm.get_transform().Find("Root/EscapeAnim").get_gameObject().CustomSetActive(false);
                expForm.get_transform().Find("Root/Panel_Interactable").get_gameObject().CustomSetActive(true);
            }
        }

        public static void StopRewardAnim(CUIFormScript rewardForm)
        {
            if ((rewardForm != null) && rewardForm.get_transform().Find("Root/EscapeAnim").get_gameObject().get_activeSelf())
            {
                rewardForm.get_transform().Find("Root/EscapeAnim").get_gameObject().CustomSetActive(false);
                rewardForm.get_transform().Find("Root/Panel_Interactable").get_gameObject().CustomSetActive(true);
                rewardForm.get_gameObject().GetComponent<Animator>().Play("AppearThePrizes_2_Done");
                Singleton<PVESettleSys>.instance.OnAwardDisplayEnd();
            }
        }

        public static void StopStarAnim(CUIFormScript starForm)
        {
            if (starForm.get_transform().Find("EscapeAnim").get_gameObject().get_activeSelf())
            {
                starForm.get_transform().Find("EscapeAnim").get_gameObject().CustomSetActive(false);
                starForm.get_transform().Find("Panel_Interactable").get_gameObject().CustomSetActive(true);
                StarCondition[] condition = Singleton<PVESettleSys>.GetInstance().GetCondition();
                GoToStarAnimState(starForm, ref condition, "_Done");
            }
        }

        [CompilerGenerated]
        private sealed class <ShowPlayerLevelUp>c__AnonStorey65
        {
            internal int newLvl;
        }

        [CompilerGenerated]
        private sealed class <ShowPlayerLevelUp>c__AnonStorey66
        {
            internal PVESettleView.<ShowPlayerLevelUp>c__AnonStorey65 <>f__ref$101;
            internal bool hasBuy;
            internal CRoleInfo master;

            internal void <>m__6B(ResSymbolPos rule)
            {
                if ((rule != null) && (rule.wOpenLevel == this.<>f__ref$101.newLvl))
                {
                    this.hasBuy = this.master.m_symbolInfo.IsGridPosHasBuy(rule.bSymbolPos);
                }
            }
        }

        public enum AwardWidgets
        {
            ItemDetailPanel = 1,
            None = -1,
            Reserve = 0
        }
    }
}

