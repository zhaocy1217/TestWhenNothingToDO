namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Text;
    using UnityEngine;
    using UnityEngine.UI;

    internal class CSettlementView
    {
        private static float _coinFrom;
        private static LTDescr _coinLTD;
        private static float _coinTo;
        private static Text _coinTweenText;
        private static GameObject _continueBtn;
        private static float _expFrom;
        private static LTDescr _expLTD;
        private static float _expTo;
        private static RectTransform _expTweenRect;
        private static uint _lvUpGrade;
        private const float expBarWidth = 327.6f;
        public const int MAX_ACHIEVEMENT = 6;
        private const float proficientBarWidth = 205f;
        private const float TweenTime = 2f;

        public static void DoCoinTweenEnd()
        {
            if ((_coinLTD != null) && (_coinTweenText != null))
            {
                _coinTweenText.set_text(string.Format("+{0}", _coinTo.ToString("N0")));
                COMDT_ACNT_INFO acntInfo = Singleton<BattleStatistic>.GetInstance().acntInfo;
                if (Singleton<BattleStatistic>.GetInstance().multiDetail != null)
                {
                    CUICommonSystem.AppendMultipleText(_coinTweenText, CUseable.GetMultiple(acntInfo.dwPvpSettleBaseCoin, ref Singleton<BattleStatistic>.GetInstance().multiDetail, 0, -1));
                }
                _coinLTD.cancel();
                _coinLTD = null;
                _coinTweenText = null;
            }
            if (_continueBtn != null)
            {
                _continueBtn.CustomSetActive(true);
                _continueBtn = null;
            }
        }

        private static void DoExpTweenEnd()
        {
            if ((_expTweenRect != null) && (_expLTD != null))
            {
                _expTweenRect.set_sizeDelta(new Vector2(_expTo * 327.6f, _expTweenRect.get_sizeDelta().y));
                _expLTD.cancel();
                _expLTD = null;
                _expTweenRect = null;
            }
            if (_continueBtn != null)
            {
                _continueBtn.CustomSetActive(true);
                _continueBtn = null;
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

        public static void HideData(CUIFormScript form)
        {
            form.get_gameObject().get_transform().Find("PanelB/StatCon").get_gameObject().CustomSetActive(false);
        }

        public static void SetAchievementIcon(CUIFormScript formScript, GameObject item, PvpAchievement type, int count)
        {
            if (count <= 6)
            {
                Image component = Utility.FindChild(item, string.Format("Achievement/Image{0}", count)).GetComponent<Image>();
                if (type == PvpAchievement.NULL)
                {
                    component.get_gameObject().CustomSetActive(false);
                }
                else
                {
                    string prefabPath = CUIUtility.s_Sprite_Dynamic_Pvp_Settle_Dir + type.ToString();
                    component.get_gameObject().CustomSetActive(true);
                    component.SetSprite(prefabPath, formScript, true, false, false, false);
                }
            }
        }

        private static void SetExpInfo(GameObject root, CUIFormScript formScript)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            DebugHelper.Assert(masterRoleInfo != null, "can't find roleinfo");
            if (masterRoleInfo != null)
            {
                ResAcntPvpExpInfo dataByKey = GameDataMgr.acntPvpExpDatabin.GetDataByKey((uint) ((byte) masterRoleInfo.PvpLevel));
                object[] inParameters = new object[] { masterRoleInfo.PvpLevel };
                DebugHelper.Assert(dataByKey != null, "can't find resexp id={0}", inParameters);
                if (dataByKey != null)
                {
                    root.get_transform().Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/PvpLevelTxt").GetComponent<Text>().set_text(string.Format("Lv.{0}", dataByKey.bLevel.ToString()));
                    Text component = root.get_transform().Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/PvpExpTxt").GetComponent<Text>();
                    Text text3 = root.get_transform().Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/ExpMax").GetComponent<Text>();
                    Text text4 = root.get_transform().Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/PlayerName").GetComponent<Text>();
                    CUIHttpImageScript script = root.get_transform().Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/HeadImage").GetComponent<CUIHttpImageScript>();
                    Image image = root.get_transform().Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/NobeIcon").GetComponent<Image>();
                    Image image2 = root.get_transform().Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/HeadFrame").GetComponent<Image>();
                    if (!CSysDynamicBlock.bSocialBlocked)
                    {
                        string headUrl = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().HeadUrl;
                        script.SetImageUrl(headUrl);
                        MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(image, (int) Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwCurLevel, false);
                        MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(image2, (int) Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwHeadIconId);
                    }
                    else
                    {
                        MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(image, 0, false);
                    }
                    SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                    DebugHelper.Assert(curLvelContext != null, "Battle Level Context is NULL!!");
                    GameObject obj2 = root.get_transform().Find("PanelA/Award/RankCon").get_gameObject();
                    obj2.CustomSetActive(false);
                    if (curLvelContext.IsGameTypeLadder())
                    {
                        COMDT_RANK_SETTLE_INFO rankInfo = Singleton<BattleStatistic>.GetInstance().rankInfo;
                        if (rankInfo != null)
                        {
                            obj2.CustomSetActive(true);
                            Text text5 = obj2.get_transform().FindChild(string.Format("txtRankName", new object[0])).get_gameObject().GetComponent<Text>();
                            Text text6 = obj2.get_transform().FindChild(string.Format("WangZheXingTxt", new object[0])).get_gameObject().GetComponent<Text>();
                            text5.set_text(StringHelper.UTF8BytesToString(ref GameDataMgr.rankGradeDatabin.GetDataByKey((uint) rankInfo.bNowGrade).szGradeDesc));
                            if (rankInfo.bNowGrade == GameDataMgr.rankGradeDatabin.count)
                            {
                                Transform transform = obj2.get_transform().FindChild(string.Format("XingGrid/ImgScore{0}", 1));
                                if (transform != null)
                                {
                                    transform.get_gameObject().CustomSetActive(true);
                                }
                                text6.get_gameObject().CustomSetActive(true);
                                text6.set_text(string.Format("X{0}", rankInfo.dwNowScore));
                            }
                            else
                            {
                                text6.get_gameObject().CustomSetActive(false);
                                for (int i = 1; i <= rankInfo.dwNowScore; i++)
                                {
                                    Transform transform2 = obj2.get_transform().FindChild(string.Format("XingGrid/ImgScore{0}", i));
                                    if (transform2 != null)
                                    {
                                        transform2.get_gameObject().CustomSetActive(true);
                                    }
                                }
                            }
                            root.get_transform().Find("PanelA/Award/Panel_PlayerExp/PvpLevelNode").get_gameObject().CustomSetActive(false);
                        }
                    }
                    Image image3 = root.get_transform().Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/QQVIPIcon").GetComponent<Image>();
                    MonoSingleton<NobeSys>.GetInstance().SetMyQQVipHead(image3);
                    COMDT_ACNT_INFO acntInfo = Singleton<BattleStatistic>.GetInstance().acntInfo;
                    COMDT_REWARD_MULTIPLE_DETAIL multiDetail = Singleton<BattleStatistic>.GetInstance().multiDetail;
                    if (multiDetail != null)
                    {
                        string[] strArray = null;
                        StringBuilder builder = new StringBuilder();
                        int num2 = CUseable.GetMultiple(acntInfo.dwPvpSettleBaseExp, ref multiDetail, 15, -1);
                        if (num2 > 0)
                        {
                            COMDT_MULTIPLE_DATA[] multipleData = null;
                            uint num3 = CUseable.GetMultipleInfo(out multipleData, ref multiDetail, 15, -1);
                            strArray = new string[num3 + 2];
                            string str2 = num2.ToString();
                            string[] args = new string[] { str2 };
                            strArray[0] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_1", args);
                            if (multipleData != null)
                            {
                                for (int k = 0; k < num3; k++)
                                {
                                    string str3 = string.Empty;
                                    if ((acntInfo.dwPvpSettleBaseExp * multipleData[k].iValue) > 0L)
                                    {
                                        str3 = "+";
                                    }
                                    switch (multipleData[k].bOperator)
                                    {
                                        case 0:
                                            str3 = str3 + multipleData[k].iValue;
                                            break;

                                        case 1:
                                            str3 = str3 + ((long) ((acntInfo.dwPvpSettleBaseExp * multipleData[k].iValue) / ((ulong) 0x2710L)));
                                            break;

                                        default:
                                            str3 = str3 + "0";
                                            break;
                                    }
                                    switch (multipleData[k].iType)
                                    {
                                        case 1:
                                        {
                                            string[] textArray2 = new string[] { str3 };
                                            strArray[k + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_6", textArray2);
                                            goto Label_0799;
                                        }
                                        case 2:
                                        {
                                            if (!masterRoleInfo.HasVip(0x10))
                                            {
                                                break;
                                            }
                                            string[] textArray3 = new string[] { str3 };
                                            strArray[k + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_9", textArray3);
                                            goto Label_0799;
                                        }
                                        case 3:
                                            strArray[k + 1] = string.Format(Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_4"), str3, masterRoleInfo.GetExpWinCount(), Math.Ceiling((double) (((float) masterRoleInfo.GetExpExpireHours()) / 24f)));
                                            goto Label_0799;

                                        case 4:
                                        {
                                            string[] textArray5 = new string[] { masterRoleInfo.dailyPvpCnt.ToString(), str3 };
                                            strArray[k + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_2", textArray5);
                                            goto Label_0799;
                                        }
                                        case 5:
                                        {
                                            string[] textArray6 = new string[] { str3 };
                                            strArray[k + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_13", textArray6);
                                            goto Label_0799;
                                        }
                                        case 6:
                                        {
                                            string[] textArray7 = new string[] { str3 };
                                            strArray[k + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_15", textArray7);
                                            goto Label_0799;
                                        }
                                        case 7:
                                        {
                                            string[] textArray8 = new string[] { str3 };
                                            strArray[k + 1] = Singleton<CTextManager>.instance.GetText("Daily_Quest_FirstVictoryName", textArray8);
                                            goto Label_0799;
                                        }
                                        case 8:
                                        {
                                            string[] textArray9 = new string[] { str3 };
                                            strArray[k + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_16", textArray9);
                                            goto Label_0799;
                                        }
                                        case 9:
                                        {
                                            string[] textArray10 = new string[] { str3 };
                                            strArray[k + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_14", textArray10);
                                            goto Label_0799;
                                        }
                                        case 10:
                                        {
                                            string[] textArray11 = new string[] { str3 };
                                            strArray[k + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_21", textArray11);
                                            goto Label_0799;
                                        }
                                        case 11:
                                        {
                                            string[] textArray12 = new string[] { str3 };
                                            strArray[k + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_17", textArray12);
                                            goto Label_0799;
                                        }
                                        case 12:
                                        {
                                            string[] textArray13 = new string[] { str3 };
                                            strArray[k + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_18", textArray13);
                                            goto Label_0799;
                                        }
                                        case 13:
                                        {
                                            string[] textArray15 = new string[] { str3 };
                                            strArray[k + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_20", textArray15);
                                            goto Label_0799;
                                        }
                                        case 14:
                                        {
                                            string[] textArray14 = new string[] { str3 };
                                            strArray[k + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_19", textArray14);
                                            goto Label_0799;
                                        }
                                        default:
                                            goto Label_0799;
                                    }
                                    string[] textArray4 = new string[] { str3 };
                                    strArray[k + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_3", textArray4);
                                Label_0799:;
                                }
                            }
                            builder.Append(strArray[0]);
                            for (int j = 1; j < strArray.Length; j++)
                            {
                                if (!string.IsNullOrEmpty(strArray[j]))
                                {
                                    builder.Append("\n");
                                    builder.Append(strArray[j]);
                                }
                            }
                            GameObject obj3 = root.get_transform().Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/DoubleExp").get_gameObject();
                            obj3.CustomSetActive(true);
                            obj3.GetComponentInChildren<Text>().set_text(string.Format("+{0}", str2));
                            CUICommonSystem.SetCommonTipsEvent(formScript, obj3, builder.ToString(), enUseableTipsPos.enLeft);
                        }
                        else
                        {
                            root.get_transform().Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/DoubleExp").get_gameObject().CustomSetActive(false);
                        }
                        GameObject obj5 = root.get_transform().Find("PanelA/Award/ItemAndCoin/Panel_Gold/GoldMax").get_gameObject();
                        if (Singleton<BattleStatistic>.GetInstance().acntInfo.bReachDailyLimit > 0)
                        {
                            obj5.CustomSetActive(true);
                        }
                        else
                        {
                            obj5.CustomSetActive(false);
                        }
                        int num6 = CUseable.GetMultiple(acntInfo.dwPvpSettleBaseCoin, ref multiDetail, 11, -1);
                        if (num6 > 0)
                        {
                            COMDT_MULTIPLE_DATA[] comdt_multiple_dataArray2 = null;
                            uint num7 = CUseable.GetMultipleInfo(out comdt_multiple_dataArray2, ref multiDetail, 11, -1);
                            strArray = new string[num7 + 2];
                            string str4 = num6.ToString();
                            string[] textArray16 = new string[] { str4 };
                            strArray[0] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_7", textArray16);
                            if (comdt_multiple_dataArray2 != null)
                            {
                                for (int num8 = 0; num8 < num7; num8++)
                                {
                                    string str5 = string.Empty;
                                    if ((acntInfo.dwPvpSettleBaseCoin * comdt_multiple_dataArray2[num8].iValue) > 0L)
                                    {
                                        str5 = "+";
                                    }
                                    switch (comdt_multiple_dataArray2[num8].bOperator)
                                    {
                                        case 0:
                                            str5 = str5 + comdt_multiple_dataArray2[num8].iValue;
                                            break;

                                        case 1:
                                            str5 = str5 + ((long) ((acntInfo.dwPvpSettleBaseCoin * comdt_multiple_dataArray2[num8].iValue) / ((ulong) 0x2710L)));
                                            break;

                                        default:
                                            str5 = str5 + "0";
                                            break;
                                    }
                                    switch (comdt_multiple_dataArray2[num8].iType)
                                    {
                                        case 1:
                                        {
                                            string[] textArray17 = new string[] { str5 };
                                            strArray[num8 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_6", textArray17);
                                            goto Label_0C84;
                                        }
                                        case 2:
                                        {
                                            if (!masterRoleInfo.HasVip(0x10))
                                            {
                                                break;
                                            }
                                            string[] textArray18 = new string[] { str5 };
                                            strArray[num8 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_9", textArray18);
                                            goto Label_0C84;
                                        }
                                        case 3:
                                            strArray[num8 + 1] = string.Format(Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_10"), str5, masterRoleInfo.GetCoinWinCount(), Math.Ceiling((double) (((float) masterRoleInfo.GetCoinExpireHours()) / 24f)));
                                            goto Label_0C84;

                                        case 4:
                                        {
                                            string[] textArray20 = new string[] { masterRoleInfo.dailyPvpCnt.ToString(), str5 };
                                            strArray[num8 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_2", textArray20);
                                            goto Label_0C84;
                                        }
                                        case 5:
                                        {
                                            string[] textArray21 = new string[] { str5 };
                                            strArray[num8 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_13", textArray21);
                                            goto Label_0C84;
                                        }
                                        case 6:
                                        {
                                            string[] textArray22 = new string[] { str5 };
                                            strArray[num8 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_15", textArray22);
                                            goto Label_0C84;
                                        }
                                        case 7:
                                        {
                                            string[] textArray23 = new string[] { str5 };
                                            strArray[num8 + 1] = Singleton<CTextManager>.instance.GetText("Daily_Quest_FirstVictoryName", textArray23);
                                            goto Label_0C84;
                                        }
                                        case 8:
                                        {
                                            string[] textArray24 = new string[] { str5 };
                                            strArray[num8 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_16", textArray24);
                                            goto Label_0C84;
                                        }
                                        case 9:
                                        {
                                            string[] textArray25 = new string[] { str5 };
                                            strArray[num8 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_14", textArray25);
                                            goto Label_0C84;
                                        }
                                        case 10:
                                        {
                                            string[] textArray26 = new string[] { str5 };
                                            strArray[num8 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_21", textArray26);
                                            goto Label_0C84;
                                        }
                                        case 11:
                                        {
                                            string[] textArray27 = new string[] { str5 };
                                            strArray[num8 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_17", textArray27);
                                            goto Label_0C84;
                                        }
                                        case 12:
                                        {
                                            string[] textArray28 = new string[] { str5 };
                                            strArray[num8 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_18", textArray28);
                                            goto Label_0C84;
                                        }
                                        case 13:
                                        {
                                            string[] textArray30 = new string[] { str5 };
                                            strArray[num8 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_20", textArray30);
                                            goto Label_0C84;
                                        }
                                        case 14:
                                        {
                                            string[] textArray29 = new string[] { str5 };
                                            strArray[num8 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_19", textArray29);
                                            goto Label_0C84;
                                        }
                                        default:
                                            goto Label_0C84;
                                    }
                                    string[] textArray19 = new string[] { str5 };
                                    strArray[num8 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_3", textArray19);
                                Label_0C84:;
                                }
                            }
                            builder.Append(strArray[0]);
                            for (int m = 1; m < strArray.Length; m++)
                            {
                                if (!string.IsNullOrEmpty(strArray[m]))
                                {
                                    builder.Append("\n");
                                    builder.Append(strArray[m]);
                                }
                            }
                            builder.Remove(0, builder.Length);
                            builder.Append(strArray[0]);
                            for (int n = 1; n < strArray.Length; n++)
                            {
                                if (!string.IsNullOrEmpty(strArray[n]))
                                {
                                    builder.Append("\n");
                                    builder.Append(strArray[n]);
                                }
                            }
                            GameObject obj6 = root.get_transform().Find("PanelA/Award/ItemAndCoin/Panel_Gold/DoubleCoin").get_gameObject();
                            obj6.CustomSetActive(true);
                            obj6.GetComponentInChildren<Text>().set_text(string.Format("+{0}", str4));
                            CUICommonSystem.SetCommonTipsEvent(formScript, obj6, builder.ToString(), enUseableTipsPos.enLeft);
                        }
                        else
                        {
                            root.get_transform().Find("PanelA/Award/ItemAndCoin/Panel_Gold/DoubleCoin").get_gameObject().CustomSetActive(false);
                        }
                    }
                    text4.set_text(masterRoleInfo.Name);
                    RectTransform transform3 = root.get_transform().Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/PvpExpSliderBg/BasePvpExpSlider").get_gameObject().GetComponent<RectTransform>();
                    RectTransform transform4 = root.get_transform().Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/PvpExpSliderBg/AddPvpExpSlider").get_gameObject().GetComponent<RectTransform>();
                    if (acntInfo != null)
                    {
                        if (acntInfo.dwPvpSettleExp > 0)
                        {
                            Singleton<CSoundManager>.GetInstance().PostEvent("UI_count_jingyan", null);
                        }
                        int num11 = (int) (acntInfo.dwPvpExp - acntInfo.dwPvpSettleExp);
                        if (num11 < 0)
                        {
                            _lvUpGrade = acntInfo.dwPvpLv;
                        }
                        else
                        {
                            _lvUpGrade = 0;
                        }
                        float num12 = Mathf.Max(0f, ((float) num11) / ((float) dataByKey.dwNeedExp));
                        float num13 = Mathf.Max(0f, ((num11 >= 0) ? ((float) acntInfo.dwPvpSettleExp) : ((float) acntInfo.dwPvpExp)) / ((float) dataByKey.dwNeedExp));
                        root.get_transform().FindChild("PanelA/Award/Panel_PlayerExp/PvpExpNode/AddPvpExpTxt").GetComponent<Text>().set_text((acntInfo.dwPvpSettleExp <= 0) ? string.Empty : string.Format("+{0}", acntInfo.dwPvpSettleExp).ToString());
                        if (acntInfo.dwPvpSettleExp == 0)
                        {
                            root.get_transform().FindChild("PanelA/Award/Panel_PlayerExp/PvpExpNode/Bar2").get_gameObject().CustomSetActive(false);
                        }
                        transform3.set_sizeDelta(new Vector2(num12 * 327.6f, transform3.get_sizeDelta().y));
                        transform4.set_sizeDelta(new Vector2(num12 * 327.6f, transform4.get_sizeDelta().y));
                        _expFrom = num12;
                        _expTo = num12 + num13;
                        _expTweenRect = transform4;
                        transform3.get_gameObject().CustomSetActive(num11 >= 0);
                        text3.set_text((acntInfo.bExpDailyLimit <= 0) ? string.Empty : Singleton<CTextManager>.GetInstance().GetText("GetExp_Limit"));
                        component.set_text(string.Format("{0}/{1}", acntInfo.dwPvpExp.ToString(), dataByKey.dwNeedExp.ToString()));
                    }
                }
            }
        }

        private static void SetHeroStat_Share(CUIFormScript formScript, GameObject item, HeroKDA kda, bool bSelf, bool bMvp, bool bWin)
        {
            Utility.GetComponetInChild<Text>(item, "Txt_PlayerLevel").set_text(string.Format("Lv.{0}", kda.SoulLevel.ToString()));
            ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey((uint) kda.HeroId);
            DebugHelper.Assert(dataByKey != null);
            item.get_transform().Find("Txt_HeroName").get_gameObject().GetComponent<Text>().set_text(StringHelper.UTF8BytesToString(ref dataByKey.szName));
            string str = (kda.numKill >= 10) ? kda.numKill.ToString() : string.Format(" {0} ", kda.numKill.ToString());
            string str2 = (kda.numDead >= 10) ? kda.numDead.ToString() : string.Format(" {0} ", kda.numDead.ToString());
            string str3 = (kda.numAssist >= 10) ? kda.numAssist.ToString() : string.Format(" {0}", kda.numAssist.ToString());
            item.get_transform().Find("Txt_KDA").get_gameObject().GetComponent<Text>().set_text(string.Format("{0} / {1} / {2}", str, str2, str3));
            item.get_transform().Find("KillerImg").get_gameObject().GetComponent<Image>().SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, CSkinInfo.GetHeroSkinPic((uint) kda.HeroId, 0)), formScript, true, false, false, false);
            GameObject obj2 = item.get_transform().Find("Mvp").get_gameObject();
            obj2.CustomSetActive(bMvp);
            if (bMvp)
            {
                Image component = obj2.GetComponent<Image>();
                if (bWin)
                {
                    component.SetSprite(CUIUtility.s_Sprite_Dynamic_Pvp_Settle_Dir + "Img_Icon_Red_Mvp", formScript, true, false, false, false);
                    component.get_gameObject().get_transform().set_localScale(new Vector3(0.7f, 0.7f, 1f));
                }
                else
                {
                    component.SetSprite(CUIUtility.s_Sprite_Dynamic_Pvp_Settle_Dir + "Img_Icon_Blue_Mvp", formScript, true, false, false, false);
                    component.get_gameObject().get_transform().set_localScale(new Vector3(0.6f, 0.6f, 1f));
                }
            }
            for (int i = 0; i < 5; i++)
            {
                uint dwTalentID = kda.TalentArr[i].dwTalentID;
                int num12 = i + 1;
                Image image = item.get_transform().FindChild(string.Format("TianFu/TianFuIcon{0}", num12.ToString())).GetComponent<Image>();
                if (dwTalentID == 0)
                {
                    image.get_gameObject().CustomSetActive(false);
                }
                else
                {
                    image.get_gameObject().CustomSetActive(true);
                    ResTalentLib lib = GameDataMgr.talentLib.GetDataByKey(dwTalentID);
                    image.SetSprite(CUIUtility.s_Sprite_Dynamic_Talent_Dir + lib.dwIcon, formScript, true, false, false, false);
                }
            }
            int count = 1;
            for (int j = 1; j < 13; j++)
            {
                switch (((PvpAchievement) j))
                {
                    case PvpAchievement.Legendary:
                        if (kda.LegendaryNum > 0)
                        {
                            SetAchievementIcon(formScript, item, PvpAchievement.Legendary, count);
                            count++;
                        }
                        break;

                    case PvpAchievement.PentaKill:
                        if (kda.PentaKillNum > 0)
                        {
                            SetAchievementIcon(formScript, item, PvpAchievement.PentaKill, count);
                            count++;
                        }
                        break;

                    case PvpAchievement.QuataryKill:
                        if (kda.QuataryKillNum > 0)
                        {
                            SetAchievementIcon(formScript, item, PvpAchievement.QuataryKill, count);
                            count++;
                        }
                        break;

                    case PvpAchievement.TripleKill:
                        if (kda.TripleKillNum > 0)
                        {
                            SetAchievementIcon(formScript, item, PvpAchievement.TripleKill, count);
                            count++;
                        }
                        break;

                    case PvpAchievement.DoubleKill:
                        if (kda.DoubleKillNum <= 0)
                        {
                        }
                        break;

                    case PvpAchievement.KillMost:
                        if (kda.bKillMost)
                        {
                            SetAchievementIcon(formScript, item, PvpAchievement.KillMost, count);
                            count++;
                        }
                        break;

                    case PvpAchievement.HurtMost:
                        if (kda.bHurtMost && (kda.hurtToEnemy > 0))
                        {
                            SetAchievementIcon(formScript, item, PvpAchievement.HurtMost, count);
                            count++;
                        }
                        break;

                    case PvpAchievement.HurtTakenMost:
                        if (kda.bHurtTakenMost && (kda.hurtTakenByEnemy > 0))
                        {
                            SetAchievementIcon(formScript, item, PvpAchievement.HurtTakenMost, count);
                            count++;
                        }
                        break;

                    case PvpAchievement.AsssistMost:
                        if (kda.bAsssistMost)
                        {
                            SetAchievementIcon(formScript, item, PvpAchievement.AsssistMost, count);
                            count++;
                        }
                        break;
                }
            }
            for (int k = count; k <= 6; k++)
            {
                SetAchievementIcon(formScript, item, PvpAchievement.NULL, k);
            }
        }

        public static void SetTab(int index, GameObject root)
        {
            if (index == 0)
            {
                Utility.FindChild(root, "PanelA").CustomSetActive(true);
                Utility.FindChild(root, "PanelB").CustomSetActive(false);
            }
            else if (index == 1)
            {
                DoCoinTweenEnd();
                DoExpTweenEnd();
                Utility.FindChild(root, "PanelA").CustomSetActive(false);
                Utility.FindChild(root, "PanelB").CustomSetActive(true);
                MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.pvpFin, new uint[0]);
            }
        }

        private static void SetWin(GameObject root, bool bWin)
        {
            Utility.FindChild(root, "PanelA/WinOrLoseTitle/win").CustomSetActive(bWin);
            Utility.FindChild(root, "PanelA/WinOrLoseTitle/lose").CustomSetActive(!bWin);
            Utility.FindChild(root, "PanelB/WinOrLoseTitle/win").CustomSetActive(bWin);
            Utility.FindChild(root, "PanelB/WinOrLoseTitle/lose").CustomSetActive(!bWin);
        }

        public static void ShowData(CUIFormScript form)
        {
            form.get_gameObject().get_transform().Find("PanelB/StatCon").get_gameObject().CustomSetActive(true);
        }
    }
}

