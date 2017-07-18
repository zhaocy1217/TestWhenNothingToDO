namespace Assets.Scripts.GameSystem
{
    using Apollo;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.DataCenter;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class SettlementSystem : Singleton<SettlementSystem>
    {
        private readonly string _achievementsTips = string.Format("{0}{1}", "UGUI/Form/System/", "PvP/Settlement/Form_SettleAchievement");
        private GameObject _cacheLastReportGo;
        private uint _camp1TotalDamage;
        private uint _camp1TotalKill;
        private uint _camp1TotalTakenDamage;
        private uint _camp1TotalToHeroDamage;
        private uint _camp2TotalDamage;
        private uint _camp2TotalKill;
        private uint _camp2TotalTakenDamage;
        private uint _camp2TotalToHeroDamage;
        private bool _changingGrage;
        private float _coinFrom;
        private LTDescr _coinLtd;
        private float _coinTo;
        private Text _coinTweenText;
        private ShowBtnType _curBtnType;
        private uint _curDian = 1;
        private uint _curGrade = 1;
        private int _curLeftIndex;
        private uint _curMaxScore = 3;
        private int _curRightIndex;
        private bool _doWangZheSpecial;
        private string _duration;
        private float _expFrom;
        private LTDescr _expLtd;
        private float _expTo;
        private RectTransform _expTweenRect;
        private bool _isBraveScoreIncreased;
        private bool _isDown;
        private bool _isLadderMatch;
        private bool _isSettle;
        private bool _isUp;
        private Animator _ladderAnimator;
        private CUIFormScript _ladderForm;
        private readonly string _ladderFormName = string.Format("{0}{1}", "UGUI/Form/System/", "PvP/Settlement/Form_LadderSettle");
        private GameObject _ladderRoot;
        private bool _lastLadderWin;
        private CUIListScript _leftListScript;
        private static uint _lvUpGrade = 0;
        private COM_PLAYERCAMP _myCamp = COM_PLAYERCAMP.COM_PLAYERCAMP_1;
        private bool _neutral;
        private uint _newGrade = 1;
        private uint _newMaxScore = 3;
        private uint _newScore = 1;
        private uint _oldGrade = 1;
        private uint _oldMaxScore = 3;
        private uint _oldScore = 1;
        public readonly string _profitFormName = string.Format("{0}{1}", "UGUI/Form/System/", "PvP/Settlement/Form_PvpNewProfit.prefab");
        private CUIFormScript _profitFormScript;
        private ulong _reportUid;
        private int _reportWordId;
        private CUIListScript _rightListScript;
        private CUIFormScript _settleFormScript;
        private string _startTime;
        private uint _startTimeInt;
        private float _symbolCoinFrom;
        private LTDescr _symbolCoinLtd;
        private float _symbolCoinTo;
        private Text _symbolCoinTweenText;
        private bool _win;
        [CompilerGenerated]
        private int <HostPlayerHeroId>k__BackingField;
        private const string ColorStarGameObjectSubPath = "greyStar/colorStar";
        private const float ExpBarWidth = 220.3f;
        private bool m_bBackShowTimeLine;
        private bool m_bGrade;
        private bool m_bIsDetail = true;
        private bool m_bLastAddFriendBtnState;
        private bool m_bLastDataBtnState;
        private bool m_bLastOverViewBtnState;
        private bool m_bLastReprotBtnState;
        private int m_bLegaendary;
        private int m_bMvp;
        private int m_bPENTAKILL;
        private int m_bQUATARYKIL;
        private bool m_bSendRedBag;
        private bool m_bShareDataSucc;
        private bool m_bShareOverView;
        private Transform m_BtnTimeLine;
        private Transform m_btnVictotyTips;
        private int m_bTRIPLEKILL;
        private int m_bWin;
        private Transform m_ChatNode;
        private bool m_isRisingStarAnimationStarted;
        private Transform m_logIcon;
        private Transform m_PVPBtnGroup;
        private Transform m_PVPShareBtnClose;
        private Transform m_PVPShareDataBtnGroup;
        private Transform m_PVPSwitchOverview;
        private Transform m_PVPSwitchStatistics;
        private Transform m_PVPSwtichAddFriend;
        private GameObject m_ShareDataBtn;
        private PvpAchievementForm m_sharePVPAchieventForm;
        private Text m_timeLineText;
        private Text m_TxtBtnShareCaption;
        private CUIFormScript m_UpdateGradeForm;
        private const int MaxAchievement = 8;
        private int playerNum;
        private static string PlayerWinTimesStr = "PlayerWinTimes";
        private const float ProficientBarWidth = 159.45f;
        public static readonly string SettlementFormName = string.Format("{0}{1}", "UGUI/Form/System/", "PvP/Settlement/Form_PvpNewSettlement.prefab");
        public const string SHARE_UPDATE_GRADE_FORM = "UGUI/Form/System/ShareUI/Form_SharePVPLadder.prefab";
        private static readonly string[] StrHelper = new string[20];
        private static readonly string[] StrHelper2 = new string[20];
        private const int StrHelperLength = 20;
        private const float TweenTime = 2f;

        private void ChangeSharePVPDataBtnState(bool bShowShare)
        {
            if (this.m_logIcon != null)
            {
                this.m_logIcon.get_gameObject().CustomSetActive(!bShowShare);
            }
            if (this.m_ChatNode != null)
            {
                this.m_ChatNode.get_gameObject().CustomSetActive(!bShowShare);
            }
            if (this.m_btnVictotyTips != null)
            {
                this.m_btnVictotyTips.get_gameObject().CustomSetActive(!bShowShare);
            }
            if (this.m_PVPBtnGroup != null)
            {
                this.m_PVPBtnGroup.get_gameObject().SetActive(!bShowShare);
            }
            if (this.m_PVPSwtichAddFriend != null)
            {
                if (bShowShare)
                {
                    this.m_bLastAddFriendBtnState = this.m_PVPSwtichAddFriend.get_gameObject().get_activeSelf();
                    this.m_PVPSwtichAddFriend.get_gameObject().SetActive(false);
                }
                else
                {
                    this.m_PVPSwtichAddFriend.get_gameObject().SetActive(this.m_bLastAddFriendBtnState);
                }
            }
            if (this.m_PVPSwitchStatistics != null)
            {
                if (bShowShare)
                {
                    this.m_bLastDataBtnState = this.m_PVPSwitchStatistics.get_gameObject().get_activeSelf();
                    this.m_PVPSwitchStatistics.get_gameObject().SetActive(false);
                }
                else
                {
                    this.m_PVPSwitchStatistics.get_gameObject().SetActive(this.m_bLastDataBtnState);
                }
            }
            if (this._settleFormScript != null)
            {
                this._settleFormScript.m_formWidgets[9].CustomSetActive(!bShowShare);
                this._settleFormScript.m_formWidgets[10].CustomSetActive(!bShowShare);
                this._settleFormScript.m_formWidgets[0x19].CustomSetActive(!bShowShare);
            }
            if (this.m_PVPSwitchOverview != null)
            {
                if (bShowShare)
                {
                    this.m_bLastOverViewBtnState = this.m_PVPSwitchOverview.get_gameObject().get_activeSelf();
                    this.m_PVPSwitchOverview.get_gameObject().SetActive(false);
                }
                else
                {
                    this.m_PVPSwitchOverview.get_gameObject().SetActive(this.m_bLastOverViewBtnState);
                }
            }
            if (this.m_bIsDetail)
            {
                if (this.m_bShareDataSucc)
                {
                    this.UpdateTimeBtnState(false);
                }
                else
                {
                    this.UpdateTimeBtnState(true);
                }
            }
            else if (this.m_bShareOverView)
            {
                this.UpdateTimeBtnState(false);
            }
            else
            {
                this.UpdateTimeBtnState(true);
            }
            if (this.m_PVPShareDataBtnGroup != null)
            {
                this.m_PVPShareDataBtnGroup.get_gameObject().SetActive(bShowShare);
            }
            if (this.m_PVPShareBtnClose != null)
            {
                this.m_PVPShareBtnClose.get_gameObject().SetActive(bShowShare);
            }
        }

        private void CheckPVPAchievement()
        {
            this.m_sharePVPAchieventForm.Init(this._win);
            if (this._win && this.m_sharePVPAchieventForm.CheckAchievement())
            {
                this.m_sharePVPAchieventForm.ShowVictory();
            }
            else
            {
                this.ShowSettlementPanel(false);
            }
        }

        private void ClearSendRedBag()
        {
            this.m_bMvp = 0;
            this.m_bLegaendary = 0;
            this.m_bPENTAKILL = 0;
            this.m_bQUATARYKIL = 0;
            this.m_bTRIPLEKILL = 0;
            this.m_bWin = 0;
            this.m_bSendRedBag = false;
        }

        private void ClearShareData()
        {
            this.m_bLastAddFriendBtnState = false;
            this.m_bLastReprotBtnState = false;
            this.m_bLastOverViewBtnState = false;
            this.m_bLastDataBtnState = false;
            this.m_bShareDataSucc = false;
            this.m_bShareOverView = false;
            this.m_bIsDetail = true;
            this.m_bBackShowTimeLine = false;
            this.m_PVPBtnGroup = null;
            this.m_PVPSwtichAddFriend = null;
            this.m_PVPSwitchStatistics = null;
            this.m_PVPSwitchOverview = null;
            this.m_PVPShareDataBtnGroup = null;
            this.m_PVPShareBtnClose = null;
            this.m_timeLineText = null;
            this.m_BtnTimeLine = null;
            this.m_TxtBtnShareCaption = null;
            this.m_ShareDataBtn = null;
            this.m_btnVictotyTips = null;
            this.m_ChatNode = null;
            this.m_logIcon = null;
        }

        public void ClosePersonalProfit()
        {
            this.DoCoinTweenEnd();
            this.DoExpTweenEnd();
            this._profitFormScript = null;
            Singleton<CUIManager>.GetInstance().CloseForm(this._profitFormName);
        }

        private void CloseSettlementPanel()
        {
            this._settleFormScript = null;
            Singleton<CUIManager>.GetInstance().CloseForm(SettlementFormName);
            Singleton<CResourceManager>.GetInstance().UnloadUnusedAssets();
            if (NetworkAccelerator.getAccelRecommendation())
            {
                Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("NetAccelRecommendation", null, true);
            }
            if (!PlayerPrefs.HasKey("NET_ACC_RECOMMENDED") && NetworkAccelerator.getAccelRecommendation())
            {
                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("NETWORK_ACCELERATOR_RECOMMENDED_MSGBOX"), enUIEventID.NetworkAccelerator_TurnOn, enUIEventID.NetworkAccelerator_Ignore, Singleton<CTextManager>.GetInstance().GetText("NETWORK_ACCELERATOR_RECOMMENDED_MSGBOX_OK"), Singleton<CTextManager>.GetInstance().GetText("NETWORK_ACCELERATOR_RECOMMENDED_MSGBOX_CANCEL"), false);
                PlayerPrefs.SetString("NET_ACC_RECOMMENDED", "Y");
            }
            MonoSingleton<ShareSys>.instance.m_bShowTimeline = false;
            MonoSingleton<PandroaSys>.GetInstance().StopRedBox();
        }

        private void CollectPlayerKda(PlayerKDA kda)
        {
            ListView<HeroKDA>.Enumerator enumerator = kda.GetEnumerator();
            while (enumerator.MoveNext())
            {
                HeroKDA current = enumerator.Current;
                if (current == null)
                {
                    continue;
                }
                COM_PLAYERCAMP playerCamp = kda.PlayerCamp;
                if (playerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
                {
                    this._camp1TotalKill += (uint) current.numKill;
                    this._camp1TotalDamage += (uint) current.hurtToEnemy;
                    this._camp1TotalTakenDamage += (uint) current.hurtTakenByEnemy;
                    this._camp1TotalToHeroDamage += (uint) current.hurtToHero;
                }
                else if (playerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
                {
                    goto Label_008A;
                }
                break;
            Label_008A:
                this._camp2TotalKill += (uint) current.numKill;
                this._camp2TotalDamage += (uint) current.hurtToEnemy;
                this._camp2TotalTakenDamage += (uint) current.hurtTakenByEnemy;
                this._camp2TotalToHeroDamage += (uint) current.hurtToHero;
                break;
            }
        }

        public void DianXing()
        {
            if (this.NeedDianXing())
            {
                uint num = this.NeedChangeGrade();
                if ((num > 0) && !this._changingGrage)
                {
                    this._changingGrage = true;
                    this._curMaxScore = num;
                    if (this._isUp)
                    {
                        this._curGrade++;
                        this._curDian = 0;
                    }
                    else
                    {
                        this._curGrade--;
                        this._curDian = this._curMaxScore;
                    }
                    if (this._isUp)
                    {
                        this.Ladder_PlayLevelUpStart();
                    }
                    else
                    {
                        this.Ladder_PlayLevelDownStart();
                    }
                }
                else if (!this._changingGrage)
                {
                    if (this._isUp)
                    {
                        this._curDian++;
                        this.PlayXingAnim(this._curDian, this._curMaxScore, false);
                    }
                    else
                    {
                        this.PlayXingAnim(this._curDian, this._curMaxScore, this._isDown);
                    }
                }
            }
            else if (((!this._doWangZheSpecial && (this._oldGrade == this._newGrade)) && ((this._newGrade == GameDataMgr.rankGradeDatabin.count) && (this._oldScore == this._newScore))) && (this._newScore == 0))
            {
                this._doWangZheSpecial = true;
                this.PlayXingAnim(this._curDian, this._curMaxScore, this._isDown);
            }
            else
            {
                this._doWangZheSpecial = false;
                this.LadderAllDisplayEnd();
            }
        }

        private void DoCoinAndExpTween()
        {
            try
            {
                if ((this._coinTweenText != null) && (this._coinTweenText.get_gameObject() != null))
                {
                    this._coinLtd = LeanTween.value(this._coinTweenText.get_gameObject(), delegate (float value) {
                        if ((this._coinTweenText != null) && (this._coinTweenText.get_gameObject() != null))
                        {
                            this._coinTweenText.set_text(string.Format("+{0}", value.ToString("N0")));
                            if (value >= this._coinTo)
                            {
                                this.DoCoinTweenEnd();
                            }
                        }
                    }, this._coinFrom, this._coinTo, 2f);
                }
                if ((this._symbolCoinTweenText != null) && (this._symbolCoinTweenText.get_gameObject() != null))
                {
                    this._symbolCoinLtd = LeanTween.value(this._symbolCoinTweenText.get_gameObject(), delegate (float value) {
                        if ((this._symbolCoinTweenText != null) && (this._symbolCoinTweenText.get_gameObject() != null))
                        {
                            this._symbolCoinTweenText.set_text(string.Format("+{0}", value.ToString("N0")));
                            if (value >= this._symbolCoinTo)
                            {
                                this.DoSymbolCoinTweenEnd();
                            }
                        }
                    }, this._symbolCoinFrom, this._symbolCoinTo, 2f);
                }
                if ((this._expTweenRect != null) && (this._expTweenRect.get_gameObject() != null))
                {
                    this._expLtd = LeanTween.value(this._expTweenRect.get_gameObject(), delegate (float value) {
                        if ((this._expTweenRect != null) && (this._expTweenRect.get_gameObject() != null))
                        {
                            this._expTweenRect.set_sizeDelta(new Vector2(value * 220.3f, this._expTweenRect.get_sizeDelta().y));
                            if (value >= this._expTo)
                            {
                                this.DoExpTweenEnd();
                            }
                        }
                    }, this._expFrom, this._expTo, 2f);
                }
            }
            catch (Exception exception)
            {
                object[] inParameters = new object[] { exception.Message };
                DebugHelper.Assert(false, "Exceptin in DoCoinAndExpTween, {0}", inParameters);
            }
        }

        public void DoCoinTweenEnd()
        {
            if ((this._coinLtd != null) && (this._coinTweenText != null))
            {
                this._coinTweenText.set_text(string.Format("+{0}", this._coinTo.ToString("N0")));
                COMDT_ACNT_INFO acntInfo = Singleton<BattleStatistic>.GetInstance().acntInfo;
                if (Singleton<BattleStatistic>.GetInstance().multiDetail != null)
                {
                    CUICommonSystem.AppendMultipleText(this._coinTweenText, CUseable.GetMultiple(acntInfo.dwPvpSettleBaseCoin, ref Singleton<BattleStatistic>.GetInstance().multiDetail, 0, -1));
                }
                this._coinLtd.cancel();
                this._coinLtd = null;
                this._coinTweenText = null;
            }
        }

        private void DoExpTweenEnd()
        {
            if ((this._expTweenRect != null) && (this._expLtd != null))
            {
                this._expTweenRect.set_sizeDelta(new Vector2(this._expTo * 220.3f, this._expTweenRect.get_sizeDelta().y));
                this._expLtd.cancel();
                this._expLtd = null;
                this._expTweenRect = null;
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

        public void DoSymbolCoinTweenEnd()
        {
            if ((this._symbolCoinLtd != null) && (this._symbolCoinTweenText != null))
            {
                this._symbolCoinTweenText.set_text(string.Format("+{0}", this._symbolCoinTo.ToString("N0")));
                COMDT_ACNT_INFO acntInfo = Singleton<BattleStatistic>.GetInstance().acntInfo;
                if (Singleton<BattleStatistic>.GetInstance().multiDetail != null)
                {
                    CUICommonSystem.AppendMultipleText(this._symbolCoinTweenText, CUseable.GetMultiple(acntInfo.dwPvpSettleBaseCoin, ref Singleton<BattleStatistic>.GetInstance().multiDetail, 14, -1));
                }
                this._symbolCoinLtd.cancel();
                this._symbolCoinLtd = null;
                this._symbolCoinTweenText = null;
            }
        }

        public static COMDT_SETTLE_HERO_RESULT_INFO GetHeroSettleInfo(uint heroId)
        {
            COMDT_SETTLE_HERO_RESULT_DETAIL heroSettleInfo = Singleton<BattleStatistic>.GetInstance().heroSettleInfo;
            if (heroSettleInfo != null)
            {
                for (int i = 0; i < heroSettleInfo.bNum; i++)
                {
                    if ((heroSettleInfo.astHeroList[i] != null) && (heroSettleInfo.astHeroList[i].dwHeroConfID == heroId))
                    {
                        return heroSettleInfo.astHeroList[i];
                    }
                }
            }
            return null;
        }

        private int GetHostPlayerHeroId(DictionaryView<uint, PlayerKDA>.Enumerator playerKdaEmr)
        {
            while (playerKdaEmr.MoveNext())
            {
                if (Utility.IsSelf(playerKdaEmr.Current.Value.PlayerUid, playerKdaEmr.Current.Value.WorldId))
                {
                    ListView<HeroKDA>.Enumerator enumerator = playerKdaEmr.Current.Value.GetEnumerator();
                    if (enumerator.MoveNext() && (enumerator.Current != null))
                    {
                        return enumerator.Current.HeroId;
                    }
                }
            }
            return -1;
        }

        private string GetLadderResultDesc()
        {
            uint selfRankClass = Singleton<CRoleInfoManager>.GetInstance().GetSelfRankClass();
            string rankName = CLadderView.GetRankName((byte) this._oldGrade, selfRankClass);
            string str3 = CLadderView.GetRankName((byte) this._newGrade, selfRankClass);
            if (this._newGrade > this._oldGrade)
            {
                string[] textArray1 = new string[] { rankName, str3 };
                return Singleton<CTextManager>.GetInstance().GetText("Ladder_Settle_Result_Up_Grade", textArray1);
            }
            if (this._newGrade < this._oldGrade)
            {
                string[] textArray2 = new string[] { rankName, str3 };
                return Singleton<CTextManager>.GetInstance().GetText("Ladder_Settle_Result_Down_Grade", textArray2);
            }
            if (this._newScore > this._oldScore)
            {
                uint num2 = this._newScore - this._oldScore;
                string[] textArray3 = new string[] { str3, num2.ToString() };
                return Singleton<CTextManager>.GetInstance().GetText("Ladder_Settle_Result_Up_Star", textArray3);
            }
            if (this._newScore < this._oldScore)
            {
                uint num3 = this._oldScore - this._newScore;
                string[] textArray4 = new string[] { str3, num3.ToString() };
                return Singleton<CTextManager>.GetInstance().GetText("Ladder_Settle_Result_Down_Star", textArray4);
            }
            string[] args = new string[] { str3 };
            return Singleton<CTextManager>.GetInstance().GetText("Ladder_Settle_Result_No_Grade_Change", args);
        }

        public int GetMvpScoreRankInCamp()
        {
            int serverRawMvpScore = 0;
            int num2 = 1;
            CPlayerKDAStat playerKDAStat = Singleton<BattleStatistic>.instance.m_playerKDAStat;
            if (playerKDAStat != null)
            {
                DictionaryView<uint, PlayerKDA>.Enumerator enumerator = playerKDAStat.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                    PlayerKDA rkda = current.Value;
                    if ((rkda != null) && (rkda.PlayerId == Singleton<GamePlayerCenter>.instance.HostPlayerId))
                    {
                        serverRawMvpScore = Singleton<BattleStatistic>.instance.GetServerRawMvpScore(rkda.PlayerId);
                        break;
                    }
                }
                enumerator.Reset();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<uint, PlayerKDA> pair2 = enumerator.Current;
                    PlayerKDA rkda2 = pair2.Value;
                    if (rkda2 != null)
                    {
                        int num4 = Singleton<BattleStatistic>.instance.GetServerRawMvpScore(rkda2.PlayerId);
                        if (((rkda2.PlayerCamp == Singleton<GamePlayerCenter>.instance.hostPlayerCamp) && (rkda2.PlayerId != Singleton<GamePlayerCenter>.instance.HostPlayerId)) && (num4 > serverRawMvpScore))
                        {
                            num2++;
                        }
                    }
                }
            }
            return num2;
        }

        public static string GetProficiencyLvTxt(int heroType, uint level)
        {
            ResHeroProficiency heroProficiency = CHeroInfo.GetHeroProficiency(heroType, (int) level);
            return ((heroProficiency == null) ? string.Empty : Utility.UTF8Convert(heroProficiency.szTitle));
        }

        private GameObject GetXing(uint targetScore, uint targetMax)
        {
            if (this._ladderRoot == null)
            {
                return null;
            }
            Transform transform = this._ladderRoot.get_transform().FindChild(string.Format("RankConNow/ScoreCon/Con{0}Star", targetMax));
            if (transform == null)
            {
                return null;
            }
            GameObject obj2 = transform.get_gameObject();
            if (obj2 == null)
            {
                return null;
            }
            Transform transform2 = obj2.get_transform().FindChild(string.Format("Xing{0}", targetScore));
            return ((transform2 == null) ? null : transform2.get_gameObject());
        }

        private void ImpAddFriend(CUIEvent uiEvent)
        {
            Singleton<CFriendContoller>.instance.Open_Friend_Verify(uiEvent.m_eventParams.commonUInt64Param1, (uint) uiEvent.m_eventParams.commonUInt64Param2, false, COM_ADD_FRIEND_TYPE.COM_ADD_FRIEND_PVP, uiEvent.m_eventParams.tag, true);
            uiEvent.m_srcWidget.CustomSetActive(false);
        }

        protected void ImpCloseReport(CUIEvent uiEvent)
        {
            if ((this._settleFormScript != null) && (this._settleFormScript.get_gameObject() != null))
            {
                this._cacheLastReportGo = null;
                this._reportUid = 0L;
                this._reportWordId = 0;
                this._settleFormScript.m_formWidgets[3].CustomSetActive(false);
            }
        }

        protected void ImpDoReport(CUIEvent uiEvent)
        {
            if ((this._settleFormScript != null) && (this._settleFormScript.get_gameObject() != null))
            {
                GameObject obj2 = this._settleFormScript.m_formWidgets[3];
                obj2.CustomSetActive(false);
                Singleton<CUIManager>.instance.OpenTips(Singleton<CTextManager>.GetInstance().GetText("Report_Report"), false, 1.5f, null, new object[0]);
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x10d1);
                msg.stPkgData.stUserComplaintReq.dwComplaintReason = 1;
                msg.stPkgData.stUserComplaintReq.ullComplaintUserUid = this._reportUid;
                msg.stPkgData.stUserComplaintReq.iComplaintLogicWorldID = this._reportWordId;
                DictionaryView<uint, PlayerKDA>.Enumerator enumerator = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat.GetEnumerator();
                int num = 0;
                while (enumerator.MoveNext())
                {
                    KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                    PlayerKDA rkda = current.Value;
                    if (((rkda != null) && (rkda.PlayerUid == this._reportUid)) && (rkda.WorldId == this._reportWordId))
                    {
                        if (!string.IsNullOrEmpty(rkda.PlayerOpenId))
                        {
                            Utility.StringToByteArray(rkda.PlayerOpenId, ref msg.stPkgData.stUserComplaintReq.szComplaintUserOpenId);
                        }
                        byte[] sourceArray = Utility.BytesConvert(rkda.PlayerName);
                        byte[] szComplaintPlayerName = msg.stPkgData.stUserComplaintReq.szComplaintPlayerName;
                        Array.Copy(sourceArray, szComplaintPlayerName, Math.Min(sourceArray.Length, szComplaintPlayerName.Length));
                        szComplaintPlayerName[szComplaintPlayerName.Length - 1] = 0;
                        msg.stPkgData.stUserComplaintReq.iComplaintPlayerCamp = (rkda.PlayerCamp != this._myCamp) ? 2 : 1;
                    }
                    num++;
                }
                GameObject obj3 = obj2.get_transform().FindChild("ReportToggle").get_gameObject();
                if (obj3.get_transform().FindChild("ReportGuaJi").get_gameObject().GetComponent<Toggle>().get_isOn())
                {
                    msg.stPkgData.stUserComplaintReq.dwComplaintReason = 1;
                }
                else if (obj3.get_transform().FindChild("ReportSong").get_gameObject().GetComponent<Toggle>().get_isOn())
                {
                    msg.stPkgData.stUserComplaintReq.dwComplaintReason = 2;
                }
                else if (obj3.get_transform().FindChild("ReportXiaoJi").get_gameObject().GetComponent<Toggle>().get_isOn())
                {
                    msg.stPkgData.stUserComplaintReq.dwComplaintReason = 3;
                }
                else if (obj3.get_transform().FindChild("ReportMaRen").get_gameObject().GetComponent<Toggle>().get_isOn())
                {
                    msg.stPkgData.stUserComplaintReq.dwComplaintReason = 4;
                }
                else if (obj3.get_transform().FindChild("ReportYanYuan").get_gameObject().GetComponent<Toggle>().get_isOn())
                {
                    msg.stPkgData.stUserComplaintReq.dwComplaintReason = 5;
                }
                else if (obj3.get_transform().FindChild("ReportGua").get_gameObject().GetComponent<Toggle>().get_isOn())
                {
                    msg.stPkgData.stUserComplaintReq.dwComplaintReason = 6;
                }
                Utility.StringToByteArray(CUIUtility.RemoveEmoji(obj3.get_transform().FindChild("InputField").get_gameObject().GetComponent<InputField>().get_text()), ref msg.stPkgData.stUserComplaintReq.szComplaintRemark);
                msg.stPkgData.stUserComplaintReq.dwClientStartTime = this._startTimeInt;
                msg.stPkgData.stUserComplaintReq.iBattlePlayerNumber = num;
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
                this._reportUid = 0L;
                this._reportWordId = 0;
                if (this._cacheLastReportGo != null)
                {
                    this._cacheLastReportGo.CustomSetActive(false);
                    this._cacheLastReportGo = null;
                }
            }
        }

        private void ImpSettlementTimerEnd()
        {
            Singleton<GameBuilder>.instance.EndGame();
            Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharShow");
            Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharIcon");
            if (this._settleFormScript != null)
            {
                this._settleFormScript.m_formWidgets[2].CustomSetActive(false);
                this._settleFormScript.m_formWidgets[0x10].CustomSetActive(false);
                this._settleFormScript.m_formWidgets[1].CustomSetActive(true);
                this._settleFormScript.m_formWidgets[4].CustomSetActive(true);
                this._settleFormScript.m_formWidgets[5].CustomSetActive(true);
                this._settleFormScript.m_formWidgets[6].CustomSetActive(true);
                this._settleFormScript.m_formWidgets[15].CustomSetActive(true);
                this._settleFormScript.m_formWidgets[11].CustomSetActive(false);
                this._settleFormScript.m_formWidgets[12].CustomSetActive(true);
                this._settleFormScript.m_formWidgets[0x13].CustomSetActive(false);
                this._settleFormScript.m_formWidgets[20].CustomSetActive(false);
                this._settleFormScript.m_formWidgets[0x11].CustomSetActive(true);
                this._settleFormScript.m_formWidgets[0x12].CustomSetActive(true);
                this._settleFormScript.m_formWidgets[0x16].CustomSetActive(!this._neutral);
                this._settleFormScript.m_formWidgets[0x1b].CustomSetActive(!this._neutral);
                this._settleFormScript.m_formWidgets[9].CustomSetActive(!this._neutral);
                this._settleFormScript.m_formWidgets[10].CustomSetActive(!this._neutral);
                this._settleFormScript.m_formWidgets[0x19].CustomSetActive(!this._neutral);
                uint[] param = new uint[] { !this._win ? 2 : 1, this.playerNum / 2 };
                MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.PvPShowKDA, param);
                SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
                if (((curLvelContext != null) && curLvelContext.IsMultilModeWithWarmBattle()) && !this._neutral)
                {
                    Singleton<CChatController>.instance.model.channelMgr.Clear(EChatChannel.Settle, 0L, 0);
                    Singleton<CChatController>.instance.model.channelMgr.SetChatTab(CChatChannelMgr.EChatTab.Settle);
                    Singleton<CChatController>.instance.ShowPanel(true, false);
                    Singleton<CChatController>.instance.view.UpView(true);
                    Singleton<CChatController>.instance.model.sysData.ClearEntryText();
                    Singleton<CChatController>.GetInstance().BuildWarmBattlePlayerLeaveRoomSystemMsg();
                    Singleton<CChatController>.GetInstance().BuildCachedPlayerLeaveRoomSystemMsg();
                    Singleton<CChatController>.GetInstance().ClearCachedPlayerLeaveRoomSystemMsg();
                    Singleton<CChatController>.GetInstance().SetEntryVisible(true);
                }
                else
                {
                    Singleton<CChatController>.GetInstance().SetEntryVisible(false);
                }
                GameObject obj2 = this._settleFormScript.m_formWidgets[0x1a];
                if (obj2 != null)
                {
                    Singleton<CRecordUseSDK>.GetInstance().OpenMsgBoxForMomentRecorder(obj2.get_transform());
                }
                if (this.m_bSendRedBag)
                {
                    MonoSingleton<PandroaSys>.GetInstance().StartOpenRedBox(this.m_bWin, this.m_bMvp, this.m_bLegaendary, this.m_bPENTAKILL, this.m_bQUATARYKIL, this.m_bTRIPLEKILL);
                    this.ClearSendRedBag();
                }
            }
        }

        protected void ImpShowReport(CUIEvent uiEvent)
        {
            if ((this._settleFormScript != null) && (this._settleFormScript.get_gameObject() != null))
            {
                GameObject obj2 = this._settleFormScript.m_formWidgets[3];
                obj2.CustomSetActive(true);
                this._cacheLastReportGo = uiEvent.m_srcWidget;
                this._reportUid = uiEvent.m_eventParams.commonUInt64Param1;
                this._reportWordId = (int) uiEvent.m_eventParams.commonUInt64Param2;
                CPlayerKDAStat playerKDAStat = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat;
                string playerName = null;
                if (playerKDAStat != null)
                {
                    DictionaryView<uint, PlayerKDA>.Enumerator enumerator = playerKDAStat.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                        if (current.Value.PlayerUid == this._reportUid)
                        {
                            KeyValuePair<uint, PlayerKDA> pair2 = enumerator.Current;
                            if (pair2.Value.WorldId == this._reportWordId)
                            {
                                KeyValuePair<uint, PlayerKDA> pair3 = enumerator.Current;
                                playerName = pair3.Value.PlayerName;
                                break;
                            }
                        }
                    }
                }
                obj2.get_transform().FindChild("ReportToggle/ReportName").get_gameObject().GetComponent<Text>().set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Report_PlayerName"), playerName));
            }
        }

        private void ImpSwitchAddFriendReportLaHeiDianZan(ShowBtnType btnType)
        {
            if (this._settleFormScript != null)
            {
                Transform transform = this._settleFormScript.m_formWidgets[9].get_transform().FindChild("light");
                if (transform != null)
                {
                    transform.get_gameObject().CustomSetActive((btnType == ShowBtnType.AddFriend) && !this._neutral);
                }
                transform = this._settleFormScript.m_formWidgets[10].get_transform().FindChild("light");
                if (transform != null)
                {
                    transform.get_gameObject().CustomSetActive((btnType == ShowBtnType.Report) && !this._neutral);
                }
                transform = this._settleFormScript.m_formWidgets[0x19].get_transform().FindChild("light");
                if (transform != null)
                {
                    transform.get_gameObject().CustomSetActive((btnType == ShowBtnType.LaHeiDianZan) && !this._neutral);
                }
                if (this._leftListScript != null)
                {
                    int elementAmount = this._leftListScript.GetElementAmount();
                    for (int i = 0; i < elementAmount; i++)
                    {
                        SettlementHelper component = this._leftListScript.GetElemenet(i).get_gameObject().GetComponent<SettlementHelper>();
                        component.AddFriendRoot.CustomSetActive((btnType == ShowBtnType.AddFriend) && !this._neutral);
                        component.ReportRoot.CustomSetActive((btnType == ShowBtnType.Report) && !this._neutral);
                        component.DianZanLaHeiRoot.CustomSetActive((btnType == ShowBtnType.LaHeiDianZan) && !this._neutral);
                    }
                }
                if (this._rightListScript != null)
                {
                    int num3 = this._rightListScript.GetElementAmount();
                    for (int j = 0; j < num3; j++)
                    {
                        SettlementHelper helper2 = this._rightListScript.GetElemenet(j).get_gameObject().GetComponent<SettlementHelper>();
                        helper2.AddFriendRoot.CustomSetActive((btnType == ShowBtnType.AddFriend) && !this._neutral);
                        helper2.ReportRoot.CustomSetActive((btnType == ShowBtnType.Report) && !this._neutral);
                        helper2.DianZanLaHeiRoot.CustomSetActive((btnType == ShowBtnType.LaHeiDianZan) && !this._neutral);
                    }
                }
                this._curBtnType = btnType;
            }
        }

        private void ImpSwitchStatistics()
        {
            if (this._settleFormScript != null)
            {
                bool bActive = true;
                if (this._leftListScript != null)
                {
                    int elementAmount = this._leftListScript.GetElementAmount();
                    for (int i = 0; i < elementAmount; i++)
                    {
                        SettlementHelper component = this._leftListScript.GetElemenet(i).get_gameObject().GetComponent<SettlementHelper>();
                        if (component.Detail.get_activeSelf())
                        {
                            component.Detail.CustomSetActive(false);
                            component.Damage.CustomSetActive(true);
                            bActive = false;
                        }
                        else
                        {
                            component.Detail.CustomSetActive(true);
                            component.Damage.CustomSetActive(false);
                            bActive = true;
                        }
                    }
                }
                if (this._rightListScript != null)
                {
                    int num3 = this._rightListScript.GetElementAmount();
                    for (int j = 0; j < num3; j++)
                    {
                        SettlementHelper helper2 = this._rightListScript.GetElemenet(j).get_gameObject().GetComponent<SettlementHelper>();
                        if (helper2.Detail.get_activeSelf())
                        {
                            helper2.Detail.CustomSetActive(false);
                            helper2.Damage.CustomSetActive(true);
                        }
                        else
                        {
                            helper2.Detail.CustomSetActive(true);
                            helper2.Damage.CustomSetActive(false);
                        }
                    }
                }
                this._settleFormScript.m_formWidgets[0x11].CustomSetActive(bActive);
                this._settleFormScript.m_formWidgets[0x12].CustomSetActive(bActive);
                this._settleFormScript.m_formWidgets[0x13].CustomSetActive(!bActive);
                this._settleFormScript.m_formWidgets[20].CustomSetActive(!bActive);
                this._settleFormScript.m_formWidgets[11].CustomSetActive(!bActive);
                this._settleFormScript.m_formWidgets[12].CustomSetActive(bActive);
                this.UpdateSharePVPDataCaption(bActive);
            }
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ProfitContinue, new CUIEventManager.OnUIEventHandler(this.OnClickProfitContinue));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_TimerEnd, new CUIEventManager.OnUIEventHandler(this.OnSettlementTimerEnd));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ClickBack, new CUIEventManager.OnUIEventHandler(this.OnClickBack));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ClickAgain, new CUIEventManager.OnUIEventHandler(this.OnClickBattleAgain));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_SaveReplay, new CUIEventManager.OnUIEventHandler(this.OnClickSaveReplay));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ClickStatistics, new CUIEventManager.OnUIEventHandler(this.OnSwitchStatistics));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ShowReport, new CUIEventManager.OnUIEventHandler(this.OnShowReport));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_CloseReport, new CUIEventManager.OnUIEventHandler(this.OnCloseReport));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_DoReport, new CUIEventManager.OnUIEventHandler(this.OnDoReport));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ClickAddFriend, new CUIEventManager.OnUIEventHandler(this.OnAddFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ClickDianLa, new CUIEventManager.OnUIEventHandler(this.OnClickDianZanLaHei));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_OnCloseProfit, new CUIEventManager.OnUIEventHandler(this.OnCloseProfit));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_OnCloseSettlement, new CUIEventManager.OnUIEventHandler(this.OnCloseSettlement));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_SwitchAddFriendReport, new CUIEventManager.OnUIEventHandler(this.OnSwitchAddFriendReport));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ClickLadderContinue, new CUIEventManager.OnUIEventHandler(this.OnLadderClickContinue));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ClickShowAchievements, new CUIEventManager.OnUIEventHandler(this.OnShowAchievements));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_OpenSharePVPDefeat, new CUIEventManager.OnUIEventHandler(this.OnShowDefeatShare));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ClickItemDisplay, new CUIEventManager.OnUIEventHandler(this.OnClickItemDisplay));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_ShowPVPSettleData, new CUIEventManager.OnUIEventHandler(this.OnShowPVPSettleData));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_ShowPVPSettleDataClose, new CUIEventManager.OnUIEventHandler(this.OnShowPVPSettleDataClose));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_ShowUpdateGradeShare, new CUIEventManager.OnUIEventHandler(this.OnShareUpdateGradShare));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_ShowUpdateGradeShareClose, new CUIEventManager.OnUIEventHandler(this.OnShareUpdateGradShareClose));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_Ladder_OnAnimatiorBravePanelShowInEnd, new CUIEventManager.OnUIEventHandler(this.OnAnimatiorBravePanelShowInEnd));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_Ladder_OnAnimatiorBraveProgressFillEnd, new CUIEventManager.OnUIEventHandler(this.OnAnimatiorBraveProgressFillEnd));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_Ladder_OnAnimatiorBraveDigitalJitterEnd, new CUIEventManager.OnUIEventHandler(this.OnAnimatiorBraveDigitalJitterEnd));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_Ladder_OnAnimatiorBraveDigitalReductionEnd, new CUIEventManager.OnUIEventHandler(this.OnAnimatiorBraveeDigitalReductionEnd));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_Ladder_OnAnimatiorBraveDigitalRisingStarEnd, new CUIEventManager.OnUIEventHandler(this.OnAnimatiorBraveDigitalRisingStarEnd));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_Ladder_OnAnimatiorBraveProgressFillFull, new CUIEventManager.OnUIEventHandler(this.OnAnimatiorBraveProgressFillFull));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ShowAddFriendBtn, new CUIEventManager.OnUIEventHandler(this.OnShowFriendBtn));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ShowReportBtn, new CUIEventManager.OnUIEventHandler(this.OnShowReportBtn));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ShowDianLaBtn, new CUIEventManager.OnUIEventHandler(this.OnShowLaHeiDianZanBtn));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_OpenPlayerDetailInfo, new CUIEventManager.OnUIEventHandler(this.OnOpenPlayerDetailInfo));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_GetMoreGold, new CUIEventManager.OnUIEventHandler(this.OnGetMoreGold));
            this.m_sharePVPAchieventForm = new PvpAchievementForm();
        }

        private void InitShareDataBtn(CUIFormScript form)
        {
            this.m_logIcon = form.get_gameObject().get_transform().FindChild("Panel/Logo");
            this.m_ChatNode = form.get_gameObject().get_transform().FindChild("Panel/entry_node");
            this.m_btnVictotyTips = form.get_gameObject().get_transform().FindChild("Panel/ButtonVictoryTips");
            this.m_PVPBtnGroup = form.get_gameObject().get_transform().FindChild("Panel/ButtonGrid");
            this.m_PVPSwtichAddFriend = form.get_gameObject().get_transform().FindChild("Panel/SwtichAddFriend");
            this.m_PVPSwitchStatistics = form.get_gameObject().get_transform().FindChild("Panel/SwitchStatistics");
            this.m_TxtBtnShareCaption = form.get_gameObject().get_transform().FindChild("Panel/ButtonGrid/ButtonShareData/Text").GetComponent<Text>();
            this.m_PVPSwitchOverview = form.get_gameObject().get_transform().FindChild("Panel/SwitchOverview");
            this.m_PVPShareDataBtnGroup = form.get_gameObject().get_transform().FindChild("Panel/ShareGroup");
            this.m_BtnTimeLine = form.get_gameObject().get_transform().FindChild("Panel/ShareGroup/Button_TimeLine");
            this.m_PVPShareBtnClose = form.get_gameObject().get_transform().FindChild("Panel/Btn_Share_PVP_DATA_CLOSE");
            this.m_timeLineText = form.get_gameObject().get_transform().FindChild("Panel/ShareGroup/Button_TimeLine/ClickText").GetComponent<Text>();
            ShareSys.SetSharePlatfText(this.m_timeLineText);
            this.UpdateSharePVPDataCaption(this.m_bIsDetail);
        }

        private bool IsBraveScoreDeduction()
        {
            COMDT_RANK_SETTLE_INFO rankInfo = Singleton<BattleStatistic>.GetInstance().rankInfo;
            if (rankInfo == null)
            {
                DebugHelper.Assert(false, "BattleStatistic.instance.rankInfo is null!!!");
                return false;
            }
            return ((((this._oldGrade == this._newGrade) && (this._oldScore == this._newScore)) && (rankInfo.dwOldAddStarScore != 0)) && (rankInfo.dwNowAddStarScore == 0));
        }

        private bool IsBraveScoreExchange()
        {
            COMDT_RANK_SETTLE_INFO rankInfo = Singleton<BattleStatistic>.GetInstance().rankInfo;
            if (rankInfo == null)
            {
                DebugHelper.Assert(false, "BattleStatistic.instance.rankInfo is null!!!");
                return false;
            }
            return (this._isBraveScoreIncreased && (rankInfo.dwOldAddStarScore >= rankInfo.dwNowAddStarScore));
        }

        public bool IsExistSettleForm()
        {
            bool flag = false;
            if ((((Singleton<CUIManager>.instance.GetForm(FightForm.s_battleUIForm) == null) && (Singleton<CUIManager>.instance.GetForm(WinLose.m_FormPath) == null)) && ((Singleton<CUIManager>.instance.GetForm(SettlementFormName) == null) && (Singleton<CUIManager>.instance.GetForm(this._profitFormName) == null))) && ((((Singleton<CUIManager>.instance.GetForm(SingleGameSettleMgr.PATH_BURNING_WINLOSE) == null) && (Singleton<CUIManager>.instance.GetForm(SingleGameSettleMgr.PATH_BURNING_SETTLE) == null)) && ((Singleton<CUIManager>.instance.GetForm(PVESettleSys.PATH_STAR) == null) && (Singleton<CUIManager>.instance.GetForm(PVESettleSys.PATH_EXP) == null))) && (((Singleton<CUIManager>.instance.GetForm(PVESettleSys.PATH_ITEM) == null) && (Singleton<CUIManager>.instance.GetForm(PVESettleSys.PATH_LOSE) == null)) && (Singleton<CUIManager>.instance.GetForm(PVESettleSys.PATH_LEVELUP) == null))))
            {
                return flag;
            }
            return true;
        }

        private bool IsGuildProfitGameType()
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
            if (curLvelContext == null)
            {
                return false;
            }
            return curLvelContext.IsGuildProfitGameType();
        }

        public bool IsInSettlementState()
        {
            return (Singleton<CUIManager>.GetInstance().GetForm(SettlementFormName) != null);
        }

        public void Ladder_PlayLevelDownEnd()
        {
            if (this._ladderAnimator != null)
            {
                this._ladderAnimator.Play("Base Layer.RankConNow_LevelDownEnd");
                Singleton<CSoundManager>.GetInstance().PostEvent("UI_paiwei_jiangji", null);
            }
        }

        public void Ladder_PlayLevelDownStart()
        {
            if (this._ladderAnimator != null)
            {
                this._ladderAnimator.Play("Base Layer.RankConNow_LevelDownStart");
            }
        }

        public void Ladder_PlayLevelUpEnd()
        {
            if (this._ladderAnimator != null)
            {
                this._ladderAnimator.Play("Base Layer.RankConNow_LevelUpEnd");
                Singleton<CSoundManager>.GetInstance().PostEvent("UI_paiwei_shengji", null);
            }
        }

        public void Ladder_PlayLevelUpStart()
        {
            if (this._ladderAnimator != null)
            {
                this._ladderAnimator.Play("Base Layer.RankConNow_LevelUpStart");
            }
        }

        public void Ladder_PlayShowIn()
        {
            if (this._ladderAnimator != null)
            {
                this._ladderAnimator.Play("Base Layer.RankConNow_ShowIn");
            }
        }

        private void LadderAllDisplayEnd()
        {
            if ((this._ladderForm != null) && (this._ladderForm.get_gameObject() != null))
            {
                this._ladderForm.GetWidget(5).GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Ladder_Inherit_Last_Season_Grade_Finish"));
                Transform transform = this._ladderForm.get_gameObject().get_transform().FindChild("ShareGroup/Btn_Continue");
                if ((transform != null) && (transform.get_gameObject() != null))
                {
                    transform.get_gameObject().CustomSetActive(true);
                }
                Transform transform2 = this._ladderForm.get_gameObject().get_transform().FindChild("ShareGroup/Btn_Share");
                if (transform2 != null)
                {
                    if (CSysDynamicBlock.bSocialBlocked)
                    {
                        this.m_bGrade = false;
                    }
                    if (this.m_bGrade)
                    {
                        transform2.get_gameObject().CustomSetActive(true);
                    }
                    else
                    {
                        transform2.get_gameObject().CustomSetActive(false);
                    }
                }
                if (this._isSettle)
                {
                    this.PlayResultAnimation();
                }
            }
        }

        protected void LadderDisplayProcess()
        {
            CLadderView.ShowRankDetail(this._ladderRoot.get_transform().FindChild("RankConNow").get_gameObject(), (byte) this._oldGrade, Singleton<CRoleInfoManager>.GetInstance().GetSelfRankClass(), false, false);
            this._ladderForm.GetWidget(5).CustomSetActive(!this._isSettle);
            this.ShowBraveScorePanel(this._isSettle);
            this.ShowLadderResultPanel(this._isSettle);
            this.ResetAllXing(this._curDian, this._curMaxScore, false);
            this.Ladder_PlayShowIn();
        }

        public uint NeedChangeGrade()
        {
            uint dwGradeUpNeedScore = 0;
            if (this._isUp && this.NeedDianXing())
            {
                if (this._curDian == GameDataMgr.rankGradeDatabin.GetDataByKey(this._curGrade).dwGradeUpNeedScore)
                {
                    dwGradeUpNeedScore = GameDataMgr.rankGradeDatabin.GetDataByKey((uint) (this._curGrade + 1)).dwGradeUpNeedScore;
                }
                return dwGradeUpNeedScore;
            }
            if ((this._isDown && this.NeedDianXing()) && (this._curDian == 0))
            {
                dwGradeUpNeedScore = GameDataMgr.rankGradeDatabin.GetDataByKey((uint) (this._curGrade - 1)).dwGradeUpNeedScore;
            }
            return dwGradeUpNeedScore;
        }

        public bool NeedDianXing()
        {
            if (this._isUp)
            {
                return ((this._curGrade < this._newGrade) || ((this._curGrade == this._newGrade) && (this._curDian < this._newScore)));
            }
            return ((this._curGrade > this._newGrade) || ((this._curGrade == this._newGrade) && (this._curDian > this._newScore)));
        }

        private void OnAddFriend(CUIEvent uiEvent)
        {
            this.ImpAddFriend(uiEvent);
        }

        private void OnAnimatiorBraveDigitalJitterEnd(CUIEvent uiEvent)
        {
            if (Singleton<BattleStatistic>.GetInstance().rankInfo == null)
            {
                DebugHelper.Assert(false, "BattleStatistic.instance.rankInfo is null!!!");
            }
            else
            {
                CUIAnimatorScript component = this._ladderForm.GetWidget(4).GetComponent<CUIAnimatorScript>();
                if (this.IsBraveScoreExchange())
                {
                    component.SetUIEvent(enAnimatorEventType.AnimatorEnd, enUIEventID.SettlementSys_Ladder_OnAnimatiorBraveDigitalRisingStarEnd, new stUIEventParams());
                    component.PlayAnimator("Rising_Star");
                    this.m_isRisingStarAnimationStarted = true;
                }
                else
                {
                    this.DianXing();
                }
            }
        }

        private void OnAnimatiorBraveDigitalRisingStarEnd(CUIEvent uiEvent)
        {
            this.DianXing();
        }

        private void OnAnimatiorBraveeDigitalReductionEnd(CUIEvent uiEvent)
        {
            this.DianXing();
        }

        private void OnAnimatiorBravePanelShowInEnd(CUIEvent uiEvent)
        {
            if (this._ladderForm == null)
            {
                DebugHelper.Assert(false, "_ladderForm is null!!!");
            }
            else
            {
                COMDT_RANK_SETTLE_INFO rankInfo = Singleton<BattleStatistic>.GetInstance().rankInfo;
                if (rankInfo == null)
                {
                    DebugHelper.Assert(false, "BattleStatistic.GetInstance().rankInfo is null!!!");
                }
                else
                {
                    uint braveScoreMax = Singleton<CLadderSystem>.GetInstance().GetBraveScoreMax(this._oldGrade);
                    uint dwNowAddStarScore = rankInfo.dwNowAddStarScore;
                    uint selfBraveScoreMax = Singleton<CLadderSystem>.GetInstance().GetSelfBraveScoreMax();
                    CUIProgressUpdaterScript component = this._ladderForm.GetWidget(11).GetComponent<CUIProgressUpdaterScript>();
                    if (this.IsBraveScoreExchange() && (dwNowAddStarScore > 0))
                    {
                        float processCircleFillAmount = CLadderView.GetProcessCircleFillAmount((int) braveScoreMax, (int) braveScoreMax);
                        component.m_fillEndEventID = enUIEventID.SettlementSys_Ladder_OnAnimatiorBraveProgressFillFull;
                        component.StartFill(processCircleFillAmount, CUIProgressUpdaterScript.enFillDirection.Clockwise, -1f);
                    }
                    else
                    {
                        float targetFillAmount = CLadderView.GetProcessCircleFillAmount((int) dwNowAddStarScore, (int) selfBraveScoreMax);
                        bool flag = this.IsBraveScoreDeduction();
                        component.m_fillEndEventID = enUIEventID.SettlementSys_Ladder_OnAnimatiorBraveProgressFillEnd;
                        component.StartFill(targetFillAmount, !flag ? CUIProgressUpdaterScript.enFillDirection.Clockwise : CUIProgressUpdaterScript.enFillDirection.CounterClockwise, -1f);
                    }
                    this.m_isRisingStarAnimationStarted = false;
                }
            }
        }

        private void OnAnimatiorBraveProgressFillEnd(CUIEvent uiEvent)
        {
            if (this._ladderForm == null)
            {
                DebugHelper.Assert(false, "_ladderForm is null!!!");
            }
            else
            {
                COMDT_RANK_SETTLE_INFO rankInfo = Singleton<BattleStatistic>.GetInstance().rankInfo;
                if (rankInfo == null)
                {
                    DebugHelper.Assert(false, "BattleStatistic.instance.rankInfo is null!!!");
                }
                else
                {
                    uint braveScoreMax = Singleton<CLadderSystem>.GetInstance().GetBraveScoreMax(this._oldGrade);
                    uint dwNowAddStarScore = rankInfo.dwNowAddStarScore;
                    uint selfBraveScoreMax = Singleton<CLadderSystem>.GetInstance().GetSelfBraveScoreMax();
                    string[] args = new string[] { selfBraveScoreMax.ToString() };
                    this._ladderForm.GetWidget(13).GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Ladder_Brave_Exchange_Tip", args));
                    this._ladderForm.GetWidget(12).GetComponent<Text>().set_text(dwNowAddStarScore + "/" + selfBraveScoreMax);
                    if (this.IsBraveScoreExchange() && this.m_isRisingStarAnimationStarted)
                    {
                        DebugHelper.Assert(this.m_isRisingStarAnimationStarted, "发生了积分兑换缺没有播飞星动画，请检查！！！");
                    }
                    else
                    {
                        CUIAnimatorScript component = this._ladderForm.GetWidget(4).GetComponent<CUIAnimatorScript>();
                        if (selfBraveScoreMax != braveScoreMax)
                        {
                            component.SetUIEvent(enAnimatorEventType.AnimatorEnd, enUIEventID.SettlementSys_Ladder_OnAnimatiorBraveDigitalJitterEnd, new stUIEventParams());
                            component.PlayAnimator("Digital_Jitter");
                        }
                        else if (this.IsBraveScoreDeduction())
                        {
                            component.SetUIEvent(enAnimatorEventType.AnimatorEnd, enUIEventID.SettlementSys_Ladder_OnAnimatiorBraveDigitalReductionEnd, new stUIEventParams());
                            component.PlayAnimator("Digital_reduction");
                        }
                        else
                        {
                            this.DianXing();
                        }
                    }
                }
            }
        }

        private void OnAnimatiorBraveProgressFillFull(CUIEvent uiEvent)
        {
            if (this.IsBraveScoreExchange())
            {
                if (this._ladderForm == null)
                {
                    DebugHelper.Assert(false, "_ladderForm is null!!!");
                }
                else
                {
                    COMDT_RANK_SETTLE_INFO rankInfo = Singleton<BattleStatistic>.GetInstance().rankInfo;
                    if (rankInfo == null)
                    {
                        DebugHelper.Assert(false, "BattleStatistic.GetInstance().rankInfo is null!!!");
                    }
                    else
                    {
                        uint braveScoreMax = Singleton<CLadderSystem>.GetInstance().GetBraveScoreMax(this._oldGrade);
                        uint selfBraveScoreMax = Singleton<CLadderSystem>.GetInstance().GetSelfBraveScoreMax();
                        CUIAnimatorScript component = this._ladderForm.GetWidget(4).GetComponent<CUIAnimatorScript>();
                        if (selfBraveScoreMax != braveScoreMax)
                        {
                            component.SetUIEvent(enAnimatorEventType.AnimatorEnd, enUIEventID.SettlementSys_Ladder_OnAnimatiorBraveDigitalJitterEnd, new stUIEventParams());
                            component.PlayAnimator("Digital_Jitter");
                        }
                        else
                        {
                            component.SetUIEvent(enAnimatorEventType.AnimatorEnd, enUIEventID.SettlementSys_Ladder_OnAnimatiorBraveDigitalRisingStarEnd, new stUIEventParams());
                            component.PlayAnimator("Rising_Star");
                            this.m_isRisingStarAnimationStarted = true;
                        }
                        uint dwNowAddStarScore = rankInfo.dwNowAddStarScore;
                        if (dwNowAddStarScore > 0)
                        {
                            float processCircleFillAmount = CLadderView.GetProcessCircleFillAmount((int) dwNowAddStarScore, (int) selfBraveScoreMax);
                            CUIProgressUpdaterScript script2 = this._ladderForm.GetWidget(11).GetComponent<CUIProgressUpdaterScript>();
                            float curFillAmount = CLadderView.GetProcessCircleFillAmount(0, (int) selfBraveScoreMax);
                            script2.m_fillEndEventID = enUIEventID.SettlementSys_Ladder_OnAnimatiorBraveProgressFillEnd;
                            script2.StartFill(processCircleFillAmount, CUIProgressUpdaterScript.enFillDirection.Clockwise, curFillAmount);
                        }
                        else
                        {
                            string[] args = new string[] { selfBraveScoreMax.ToString() };
                            this._ladderForm.GetWidget(13).GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Ladder_Brave_Exchange_Tip", args));
                            this._ladderForm.GetWidget(12).GetComponent<Text>().set_text(dwNowAddStarScore + "/" + selfBraveScoreMax);
                        }
                    }
                }
            }
        }

        private void OnClickBack(CUIEvent uiEvent)
        {
            this.CloseSettlementPanel();
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if ((curLvelContext != null) && ((!curLvelContext.IsGameTypeRewardMatch() && !curLvelContext.IsGameTypeLadder()) && !curLvelContext.IsGameTypeGuildMatch()))
            {
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Matching_OpenEntry);
            }
        }

        private void OnClickBattleAgain(CUIEvent uiEvent)
        {
            CUIEvent event5;
            this.CloseSettlementPanel();
            if (this._isLadderMatch)
            {
                event5 = new CUIEvent();
                event5.m_eventID = enUIEventID.Matching_OpenLadder;
                CUIEvent event2 = event5;
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event2);
            }
            else
            {
                SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                if ((curLvelContext != null) && curLvelContext.IsGameTypeGuildMatch())
                {
                    Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Guild_Match_OpenMatchForm);
                }
                else if ((curLvelContext != null) && !curLvelContext.IsGameTypeRewardMatch())
                {
                    Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Matching_OpenEntry);
                }
                else if ((curLvelContext != null) && curLvelContext.IsGameTypeRewardMatch())
                {
                    CUIEvent event3 = new CUIEvent();
                    event3.m_eventParams.tag = 0;
                    event3.m_eventID = enUIEventID.Union_Battle_BattleEntryGroup_Click;
                    Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event3);
                }
                else if ((Singleton<CMatchingSystem>.instance.cacheMathingInfo != null) && (Singleton<CMatchingSystem>.instance.cacheMathingInfo.uiEventId != enUIEventID.None))
                {
                    if (Singleton<CMatchingSystem>.instance.cacheMathingInfo.uiEventId == enUIEventID.Room_CreateRoom)
                    {
                        CRoomSystem.ReqCreateRoom(Singleton<CMatchingSystem>.instance.cacheMathingInfo.mapId, Singleton<CMatchingSystem>.instance.cacheMathingInfo.mapType, false);
                    }
                    else
                    {
                        event5 = new CUIEvent();
                        event5.m_eventID = Singleton<CMatchingSystem>.instance.cacheMathingInfo.uiEventId;
                        event5.m_eventParams.tagUInt = Singleton<CMatchingSystem>.instance.cacheMathingInfo.mapId;
                        event5.m_eventParams.tag = (int) Singleton<CMatchingSystem>.instance.cacheMathingInfo.AILevel;
                        CUIEvent event4 = event5;
                        Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event4);
                    }
                }
            }
        }

        private void OnClickDianZanLaHei(CUIEvent uiEvent)
        {
            uint num = uiEvent.m_eventParams.commonUInt32Param1;
            uint num2 = uiEvent.m_eventParams.commonUInt16Param1;
            ulong num3 = uiEvent.m_eventParams.commonUInt64Param1;
            int num4 = (int) uiEvent.m_eventParams.commonUInt64Param2;
            string playerName = string.Empty;
            DictionaryView<uint, PlayerKDA>.Enumerator enumerator = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                PlayerKDA rkda = current.Value;
                if (((rkda != null) && (rkda.PlayerUid == num3)) && (rkda.WorldId == num4))
                {
                    playerName = rkda.PlayerName;
                    break;
                }
            }
            if (num == num2)
            {
                Singleton<CUIManager>.instance.OpenTips(string.Format(Singleton<CTextManager>.GetInstance().GetText("ZanTeam"), playerName), false, 1.5f, null, new object[0]);
            }
            else
            {
                Singleton<CUIManager>.instance.OpenTips(string.Format(Singleton<CTextManager>.GetInstance().GetText("ZanEnemyTeam"), playerName), false, 1.5f, null, new object[0]);
            }
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x535);
            msg.stPkgData.stLikeReq.stAcntUin.ullUid = num3;
            msg.stPkgData.stLikeReq.stAcntUin.dwLogicWorldId = (uint) num4;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
            uiEvent.m_srcWidget.CustomSetActive(false);
        }

        public void OnClickItemDisplay(CUIEvent uiEvent)
        {
            this.DoCoinAndExpTween();
        }

        private void OnClickProfitContinue(CUIEvent uiEvent)
        {
            this.ClosePersonalProfit();
            this.CheckPVPAchievement();
            MonoSingleton<ShareSys>.GetInstance().m_bShowTimeline = false;
        }

        private void OnClickSaveReplay(CUIEvent uiEvent)
        {
            if (Singleton<GameReplayModule>.HasInstance())
            {
                if (Singleton<GameReplayModule>.GetInstance().FlushRecord())
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("replaySaved", true, 1.5f, null, new object[0]);
                }
                else
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("replaySaveFailed", true, 1.5f, null, new object[0]);
                }
            }
            if (this._settleFormScript != null)
            {
                CUICommonSystem.SetButtonEnable(Utility.GetComponetInChild<Button>(this._settleFormScript.get_gameObject(), "Panel/ButtonGrid/BtnSaveReplay"), false, false, true);
            }
        }

        private void OnCloseProfit(CUIEvent uiEvent)
        {
            this.DoCoinTweenEnd();
            this.DoExpTweenEnd();
            this._profitFormScript = null;
        }

        private void OnCloseReport(CUIEvent uiEvent)
        {
            this.ImpCloseReport(uiEvent);
        }

        private void OnCloseSettlement(CUIEvent uiEvent)
        {
            Singleton<CChatController>.instance.model.channelMgr.Clear(EChatChannel.Settle, 0L, 0);
            Singleton<CChatController>.instance.model.channelMgr.SetChatTab(CChatChannelMgr.EChatTab.Normal);
            Singleton<CChatController>.instance.view.UpView(false);
            Singleton<CChatController>.instance.model.sysData.ClearEntryText();
            Singleton<CChatController>.instance.ShowPanel(false, false);
            CChatNetUT.Send_Leave_Settle();
            this._settleFormScript = null;
            this._leftListScript = null;
            this._rightListScript = null;
            this._cacheLastReportGo = null;
            this._curBtnType = ShowBtnType.AddFriend;
            this.ClearShareData();
            Singleton<GameReplayModule>.GetInstance().ClearRecord();
            Singleton<CRecordUseSDK>.instance.CallGameJoyGenerateWithNothing();
        }

        private void OnDoReport(CUIEvent uiEvent)
        {
            this.ImpDoReport(uiEvent);
        }

        private void OnGetMoreGold(CUIEvent uiEvent)
        {
            ushort txtKey = 0x11;
            Singleton<CUIManager>.GetInstance().OpenInfoForm(txtKey);
        }

        protected void OnLadderClickContinue(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().CloseForm(this._ladderFormName);
            this._ladderForm = null;
            this._ladderAnimator = null;
            this._ladderRoot = null;
            if (this._isSettle)
            {
                this.ShowPersonalProfit(this._lastLadderWin);
                this._lastLadderWin = false;
            }
            Singleton<CLadderSystem>.GetInstance().PromptSkinRewardIfNeed();
        }

        public void OnLadderLevelDownEndOver()
        {
            if (this._ladderRoot != null)
            {
                this._changingGrage = false;
                this.DianXing();
            }
        }

        public void OnLadderLevelDownStartOver()
        {
            if (this._ladderRoot != null)
            {
                this.ResetAllXing(this._curMaxScore, this._curMaxScore, true);
                CLadderView.ShowRankDetail(this._ladderRoot.get_transform().FindChild("RankConNow").get_gameObject(), (byte) this._curGrade, Singleton<CRoleInfoManager>.GetInstance().GetSelfRankClass(), false, false);
                this.Ladder_PlayLevelDownEnd();
            }
        }

        public void OnLadderLevelUpEndOver()
        {
            this._changingGrage = false;
            this.DianXing();
        }

        public void OnLadderLevelUpStartOver()
        {
            if (this._ladderRoot != null)
            {
                this.ResetAllXing(0, this._curMaxScore, false);
                CLadderView.ShowRankDetail(this._ladderRoot.get_transform().FindChild("RankConNow").get_gameObject(), (byte) this._curGrade, Singleton<CRoleInfoManager>.GetInstance().GetSelfRankClass(), false, false);
                this.Ladder_PlayLevelUpEnd();
            }
        }

        public void OnLadderShowInOver()
        {
            if (this._isSettle)
            {
                this.PlayBraveScoreAnimation();
            }
            else
            {
                this.DianXing();
            }
        }

        public void OnLadderWangZheXingEndOver()
        {
            if (this._ladderRoot != null)
            {
                this.DianXing();
            }
        }

        public void OnLadderWangZheXingStartOver()
        {
        }

        public void OnLadderXingDownOver()
        {
            GameObject xing = this.GetXing(this._curDian, this._curMaxScore);
            if (xing != null)
            {
                xing.CustomSetActive(true);
                Animator component = xing.GetComponent<Animator>();
                if (component == null)
                {
                    return;
                }
                component.set_enabled(false);
            }
            this._curDian--;
            this.DianXing();
        }

        public void OnLadderXingUpOver()
        {
            GameObject xing = this.GetXing(this._curDian, this._curMaxScore);
            if (xing != null)
            {
                xing.CustomSetActive(true);
                Animator component = xing.GetComponent<Animator>();
                if (component == null)
                {
                    return;
                }
                component.set_enabled(false);
            }
            this.DianXing();
        }

        private void OnOpenPlayerDetailInfo(CUIEvent uiEvent)
        {
            ulong uid = uiEvent.m_eventParams.commonUInt64Param1;
            int tag = uiEvent.m_eventParams.tag;
            if (Utility.IsValidPlayer(uid, tag) && !Utility.IsSelf(uid, tag))
            {
                Singleton<CPlayerInfoSystem>.GetInstance().ShowPlayerDetailInfo(uid, tag, CPlayerInfoSystem.DetailPlayerInfoSource.DefaultOthers, true);
            }
        }

        private void OnSettlementTimerEnd(CUIEvent uiEvent)
        {
            this.ImpSettlementTimerEnd();
        }

        private void OnShareTimeLineSucc(Transform btn)
        {
            if ((this.m_BtnTimeLine != null) && (this.m_BtnTimeLine == btn))
            {
                if (this.m_bIsDetail)
                {
                    this.m_bShareDataSucc = true;
                }
                else
                {
                    this.m_bShareOverView = true;
                }
            }
        }

        private void OnShareUpdateGradShare(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.OpenForm("UGUI/Form/System/ShareUI/Form_SharePVPLadder.prefab", false, true);
            this.m_UpdateGradeForm = form;
            CLadderView.ShowRankDetail(form.get_transform().FindChild("ShareFrame/Ladder/RankConNow").get_gameObject(), (byte) this._curGrade, Singleton<CRoleInfoManager>.GetInstance().GetSelfRankClass(), false, false);
            MonoSingleton<ShareSys>.GetInstance().UpdateShareGradeForm(form);
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                CUIHttpImageScript componetInChild = Utility.GetComponetInChild<CUIHttpImageScript>(form.get_gameObject(), "ShareFrame/Ladder/RankConNow/PlayerHead");
                if (componetInChild != null)
                {
                    componetInChild.SetImageUrl(masterRoleInfo.HeadUrl);
                }
                Text text = Utility.GetComponetInChild<Text>(form.get_gameObject(), "ShareFrame/Ladder/RankConNow/PlayerName");
                if (text != null)
                {
                    text.set_text(masterRoleInfo.Name);
                }
            }
        }

        private void OnShareUpdateGradShareClose(CUIEvent uiEvent)
        {
            if (this.m_UpdateGradeForm != null)
            {
                Singleton<CUIManager>.instance.CloseForm(this.m_UpdateGradeForm);
            }
            this.m_UpdateGradeForm = null;
        }

        private void OnShowAchievements(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.instance.OpenForm(this._achievementsTips, false, true);
        }

        private void OnShowDefeatShare(CUIEvent uiEvent)
        {
            if (!this._win)
            {
                this.m_sharePVPAchieventForm.ShowDefeat();
            }
            Singleton<CChatController>.instance.ShowPanel(false, false);
        }

        private void OnShowFriendBtn(CUIEvent uiEvent)
        {
            this.ImpSwitchAddFriendReportLaHeiDianZan(ShowBtnType.AddFriend);
        }

        private void OnShowLaHeiDianZanBtn(CUIEvent uiEvent)
        {
            this.ImpSwitchAddFriendReportLaHeiDianZan(ShowBtnType.LaHeiDianZan);
        }

        private void OnShowPVPSettleData(CUIEvent uiEvnet)
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            uint num = 0;
            uint pvpPlayerNum = 0;
            if (curLvelContext != null)
            {
                num = (uint) (curLvelContext.GetGameType() + 1);
                if (curLvelContext.IsMobaModeWithOutGuide())
                {
                    pvpPlayerNum = (uint) curLvelContext.m_pvpPlayerNum;
                }
            }
            uint[] kShareParam = new uint[] { num, pvpPlayerNum };
            MonoSingleton<ShareSys>.GetInstance().m_ShareActivityParam.set(1, 2, kShareParam);
            this.m_bBackShowTimeLine = MonoSingleton<ShareSys>.GetInstance().m_bShowTimeline;
            this.ChangeSharePVPDataBtnState(true);
            Singleton<EventRouter>.instance.AddEventHandler<Transform>(EventID.SHARE_TIMELINE_SUCC, new Action<Transform>(this.OnShareTimeLineSucc));
            this.ShowElementAddFriendBtn(false);
            Singleton<CChatController>.instance.ShowPanel(false, false);
        }

        private void OnShowPVPSettleDataClose(CUIEvent uiEvnet)
        {
            Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.SHARE_PVP_SETTLEDATA_CLOSE);
            this.ChangeSharePVPDataBtnState(false);
            MonoSingleton<ShareSys>.GetInstance().m_bShowTimeline = this.m_bBackShowTimeLine;
            this.ShowElementAddFriendBtn(true);
            SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
            if ((curLvelContext != null) && curLvelContext.IsMultilModeWithWarmBattle())
            {
                Singleton<CChatController>.instance.ShowPanel(true, false);
                MonoSingleton<ShareSys>.GetInstance().m_ShareActivityParam.clear();
            }
        }

        private void OnShowReport(CUIEvent uiEvent)
        {
            this.ImpShowReport(uiEvent);
        }

        private void OnShowReportBtn(CUIEvent uiEvent)
        {
            this.ImpSwitchAddFriendReportLaHeiDianZan(ShowBtnType.Report);
        }

        private void OnSwitchAddFriendReport(CUIEvent uiEvent)
        {
        }

        private void OnSwitchStatistics(CUIEvent uiEvent)
        {
            this.ImpSwitchStatistics();
        }

        private void PlayBraveScoreAnimation()
        {
            if (this._ladderForm == null)
            {
                DebugHelper.Assert(false, "_ladderForm is null!!!");
            }
            else
            {
                this._ladderForm.GetWidget(4).GetComponent<CUIAnimatorScript>().PlayAnimator("BravePanel_ShowIn");
            }
        }

        private void PlayResultAnimation()
        {
            if (this._ladderForm == null)
            {
                DebugHelper.Assert(false, "_ladderForm is null!!!");
            }
            else
            {
                this._ladderForm.GetWidget(14).GetComponent<CUIAnimatorScript>().PlayAnimator("ResulltPanel_ShowIn");
            }
        }

        private void PlayXingAnim(uint targetScore, uint targetMax, bool disappear = false)
        {
            if (this._ladderRoot != null)
            {
                GameObject xing = this.GetXing(targetScore, targetMax);
                if ((xing == null) && (targetMax > 5))
                {
                    if (disappear && (this._curDian > 0))
                    {
                        this._curDian--;
                    }
                    GameObject obj3 = this._ladderRoot.get_transform().FindChild("RankConNow/WangZheXing").get_gameObject();
                    obj3.CustomSetActive(true);
                    obj3.get_transform().FindChild("XingNumTxt").get_gameObject().GetComponent<Text>().set_text(string.Format("X{0}", this._curDian));
                    obj3.GetComponent<Animator>().Play("Base Layer.wangzhe_starend");
                    Singleton<CSoundManager>.GetInstance().PostEvent(!disappear ? "UI_paiwei_dexing" : "UI_paiwei_diuxing", null);
                }
                else if (xing != null)
                {
                    xing.CustomSetActive(true);
                    Animator component = xing.GetComponent<Animator>();
                    if (component != null)
                    {
                        component.set_enabled(true);
                        xing.get_transform().FindChild("LiangXing").get_gameObject().CustomSetActive(true);
                        component.Play(!disappear ? "Base Layer.Start_ShowIn" : "Base Layer.Start_Disappear");
                        Singleton<CSoundManager>.GetInstance().PostEvent(!disappear ? "UI_paiwei_dexing" : "UI_paiwei_diuxing", null);
                    }
                }
            }
        }

        private void ResetAllXing(uint targetScore, uint targetMax, bool inverseShow = false)
        {
            if (this._ladderRoot != null)
            {
                GameObject obj2 = this._ladderRoot.get_transform().FindChild(string.Format("RankConNow/ScoreCon/Con{0}Star", 3)).get_gameObject();
                GameObject obj3 = this._ladderRoot.get_transform().FindChild(string.Format("RankConNow/ScoreCon/Con{0}Star", 4)).get_gameObject();
                GameObject obj4 = this._ladderRoot.get_transform().FindChild(string.Format("RankConNow/ScoreCon/Con{0}Star", 5)).get_gameObject();
                obj2.CustomSetActive(false);
                obj3.CustomSetActive(false);
                obj4.CustomSetActive(false);
                GameObject obj5 = this._ladderRoot.get_transform().FindChild("RankConNow/WangZheXing").get_gameObject();
                if (targetMax > 5)
                {
                    obj5.get_transform().FindChild("XingNumTxt").get_gameObject().GetComponent<Text>().set_text(string.Format("X{0}", this._curDian));
                }
                else
                {
                    obj5.CustomSetActive(false);
                }
                Transform transform = this._ladderRoot.get_transform().FindChild(string.Format("RankConNow/ScoreCon/Con{0}Star", targetMax));
                if (transform != null)
                {
                    GameObject obj6 = transform.get_gameObject();
                    obj6.CustomSetActive(true);
                    for (int i = 1; i <= 5; i++)
                    {
                        Transform transform2 = obj6.get_transform().FindChild(string.Format("Xing{0}", i));
                        Transform transform3 = obj6.get_transform().FindChild(string.Format("Xing{0}/LiangXing", i));
                        if ((transform2 != null) && (transform3 != null))
                        {
                            transform2.get_gameObject().GetComponent<Animator>().set_enabled(inverseShow);
                            transform3.get_gameObject().CustomSetActive(i <= targetScore);
                        }
                    }
                }
            }
        }

        private void SetAchievementIcon(GameObject achievements, PvpAchievement type, int index)
        {
            if ((index <= 8) && (achievements != null))
            {
                Transform transform = achievements.get_transform().FindChild(string.Format("Achievement{0}", index));
                if (transform != null)
                {
                    if (type == PvpAchievement.NULL)
                    {
                        transform.get_gameObject().CustomSetActive(false);
                    }
                    else
                    {
                        string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Pvp_Settle_Dir, type.ToString());
                        transform.get_gameObject().CustomSetActive(true);
                        transform.GetComponent<Image>().SetSprite(prefabPath, this._settleFormScript, true, false, false, false);
                    }
                }
            }
        }

        private void SetBpModeOpenTip(byte oldGrade, byte newGrade)
        {
            GameObject widget = this._ladderForm.GetWidget(6);
            if (widget != null)
            {
                widget.CustomSetActive(!Singleton<CLadderSystem>.GetInstance().IsUseBpMode(oldGrade) && Singleton<CLadderSystem>.GetInstance().IsUseBpMode(newGrade));
            }
        }

        private void SetCreditSettlement()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                COMDT_ACNT_INFO acntInfo = Singleton<BattleStatistic>.GetInstance().acntInfo;
                if (acntInfo != null)
                {
                    Text componetInChild = Utility.GetComponetInChild<Text>(this._settleFormScript.m_formWidgets[0x16], "Text");
                    if (acntInfo.iSettleCreditValue > 0)
                    {
                        string[] args = new string[] { masterRoleInfo.creditScore.ToString(), acntInfo.iSettleCreditValue.ToString() };
                        componetInChild.set_text(Singleton<CTextManager>.instance.GetText("Credit_Change_Tips_1", args));
                    }
                    else if (acntInfo.iSettleCreditValue < 0)
                    {
                        string[] textArray2 = new string[] { masterRoleInfo.creditScore.ToString(), acntInfo.iSettleCreditValue.ToString() };
                        componetInChild.set_text(Singleton<CTextManager>.instance.GetText("Credit_Change_Tips_2", textArray2));
                    }
                    else
                    {
                        componetInChild.set_text(masterRoleInfo.creditScore.ToString());
                    }
                }
            }
        }

        private void SetExpProfit()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                ResAcntPvpExpInfo dataByKey = GameDataMgr.acntPvpExpDatabin.GetDataByKey((uint) ((byte) masterRoleInfo.PvpLevel));
                if (dataByKey != null)
                {
                    COMDT_ACNT_INFO acntInfo = Singleton<BattleStatistic>.GetInstance().acntInfo;
                    if (acntInfo != null)
                    {
                        GameObject obj2 = this._profitFormScript.m_formWidgets[1];
                        obj2.get_transform().FindChild("PlayerName").get_gameObject().GetComponent<Text>().set_text(masterRoleInfo.Name);
                        obj2.get_transform().FindChild("PlayerLv").get_gameObject().GetComponent<Text>().set_text(string.Format("Lv.{0}", masterRoleInfo.PvpLevel));
                        obj2.get_transform().FindChild("ExpMaxTip").get_gameObject().GetComponent<Text>().set_text((acntInfo.bExpDailyLimit <= 0) ? string.Empty : Singleton<CTextManager>.GetInstance().GetText("GetExp_Limit"));
                        obj2.get_transform().FindChild("PvpExpTxt").get_gameObject().GetComponent<Text>().set_text(string.Format("{0}/{1}", acntInfo.dwPvpExp, dataByKey.dwNeedExp));
                        obj2.get_transform().FindChild("AddPvpExpTxt").get_gameObject().GetComponent<Text>().set_text((acntInfo.dwPvpSettleExp < 0) ? acntInfo.dwPvpSettleExp.ToString() : string.Format("+{0}", acntInfo.dwPvpSettleExp));
                        RectTransform component = obj2.get_transform().FindChild("PvpExpSliderBg/BasePvpExpSlider").get_gameObject().GetComponent<RectTransform>();
                        RectTransform transform2 = obj2.get_transform().FindChild("PvpExpSliderBg/AddPvpExpSlider").get_gameObject().GetComponent<RectTransform>();
                        if (acntInfo.dwPvpSettleExp > 0)
                        {
                            Singleton<CSoundManager>.GetInstance().PostEvent("UI_count_jingyan", null);
                        }
                        int num = (int) (acntInfo.dwPvpExp - acntInfo.dwPvpSettleExp);
                        _lvUpGrade = (num >= 0) ? 0 : acntInfo.dwPvpLv;
                        float num2 = Mathf.Max(0f, ((float) num) / ((float) dataByKey.dwNeedExp));
                        float num3 = Mathf.Max(0f, ((num >= 0) ? ((float) acntInfo.dwPvpSettleExp) : ((float) acntInfo.dwPvpExp)) / ((float) dataByKey.dwNeedExp));
                        component.set_sizeDelta(new Vector2(num2 * 220.3f, component.get_sizeDelta().y));
                        transform2.set_sizeDelta(new Vector2(num2 * 220.3f, transform2.get_sizeDelta().y));
                        this._expFrom = num2;
                        this._expTo = num2 + num3;
                        this._expTweenRect = transform2;
                        component.get_gameObject().CustomSetActive(num >= 0);
                        CUIHttpImageScript script = obj2.get_transform().FindChild("HeadImage").GetComponent<CUIHttpImageScript>();
                        Image image = obj2.get_transform().FindChild("NobeIcon").GetComponent<Image>();
                        Image image2 = obj2.get_transform().FindChild("HeadFrame").GetComponent<Image>();
                        if (!CSysDynamicBlock.bSocialBlocked)
                        {
                            string headUrl = masterRoleInfo.HeadUrl;
                            script.SetImageUrl(headUrl);
                            MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(image, (int) masterRoleInfo.GetNobeInfo().stGameVipClient.dwCurLevel, false);
                            MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(image2, (int) masterRoleInfo.GetNobeInfo().stGameVipClient.dwHeadIconId);
                        }
                        else
                        {
                            MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(image, 0, false);
                        }
                        GameObject obj3 = obj2.get_transform().FindChild("ExpGroup/DoubleExp").get_gameObject();
                        obj3.CustomSetActive(false);
                        COMDT_REWARD_MULTIPLE_DETAIL multiDetail = Singleton<BattleStatistic>.GetInstance().multiDetail;
                        for (int i = 0; i < StrHelper.Length; i++)
                        {
                            StrHelper[i] = null;
                        }
                        if (multiDetail != null)
                        {
                            GameObject obj4 = this._profitFormScript.m_formWidgets[8];
                            Text text4 = obj4.get_transform().GetComponent<Text>();
                            string key = (masterRoleInfo.m_expWeekCur < masterRoleInfo.m_expWeekLimit) ? "Settlement_WEEK_EXP_limited" : "Settlement_WEEK_EXP_limited_full";
                            string[] args = new string[] { masterRoleInfo.m_expWeekCur.ToString(), masterRoleInfo.m_expWeekLimit.ToString() };
                            text4.set_text(Singleton<CTextManager>.GetInstance().GetText(key, args));
                            GameObject obj5 = this._profitFormScript.m_formWidgets[9];
                            Text text5 = obj5.get_transform().GetComponent<Text>();
                            key = (masterRoleInfo.m_goldWeekCur < masterRoleInfo.m_goldWeekLimit) ? "Settlement_WEEK_GOLD_limited" : "Settlement_WEEK_GOLD_limited_full";
                            string[] textArray2 = new string[] { masterRoleInfo.m_goldWeekCur.ToString(), masterRoleInfo.m_goldWeekLimit.ToString() };
                            text5.set_text(Singleton<CTextManager>.GetInstance().GetText(key, textArray2));
                            int num5 = CUseable.GetMultiple(acntInfo.dwPvpSettleBaseExp, ref multiDetail, 15, -1);
                            COMDT_MULTIPLE_DATA[] multipleData = null;
                            uint num6 = CUseable.GetMultipleInfo(out multipleData, ref multiDetail, 15, -1);
                            acntInfo = Singleton<BattleStatistic>.GetInstance().acntInfo;
                            string str3 = string.Empty;
                            if (num5 > 0)
                            {
                                str3 = str3 + "+";
                            }
                            str3 = str3 + num5.ToString();
                            string[] textArray3 = new string[] { str3 };
                            StrHelper[0] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_1", textArray3);
                            if (multipleData != null)
                            {
                                for (int k = 0; k < num6; k++)
                                {
                                    string str4 = string.Empty;
                                    if ((acntInfo.dwPvpSettleBaseExp * multipleData[k].iValue) > 0L)
                                    {
                                        str4 = "+";
                                    }
                                    switch (multipleData[k].bOperator)
                                    {
                                        case 0:
                                            str4 = str4 + multipleData[k].iValue;
                                            break;

                                        case 1:
                                        {
                                            double num8 = (acntInfo.dwPvpSettleBaseExp * multipleData[k].iValue) / 10000.0;
                                            if (num8 > 0.0)
                                            {
                                                str4 = str4 + ((int) (num8 + 0.9999));
                                            }
                                            else if (num8 < 0.0)
                                            {
                                                str4 = str4 + ((int) (num8 - 0.9999));
                                            }
                                            else
                                            {
                                                str4 = "0";
                                            }
                                            break;
                                        }
                                        default:
                                            str4 = str4 + "0";
                                            break;
                                    }
                                    StrHelper2[k + 1] = string.Empty;
                                    int num9 = multipleData[k].iValue / 100;
                                    if (num9 >= 0)
                                    {
                                        StrHelper2[k + 1] = StrHelper2[k + 1] + "+";
                                    }
                                    StrHelper2[k + 1] = StrHelper2[k + 1] + num9;
                                    switch (multipleData[k].iType)
                                    {
                                        case 1:
                                        {
                                            string[] textArray4 = new string[] { str4 };
                                            StrHelper[k + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_6", textArray4);
                                            goto Label_0991;
                                        }
                                        case 2:
                                        {
                                            if (!masterRoleInfo.HasVip(0x10))
                                            {
                                                break;
                                            }
                                            string[] textArray5 = new string[] { str4 };
                                            StrHelper[k + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_9", textArray5);
                                            goto Label_0991;
                                        }
                                        case 3:
                                            StrHelper[k + 1] = string.Format(Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_4"), str4, masterRoleInfo.GetExpWinCount(), Math.Ceiling((double) (((float) masterRoleInfo.GetExpExpireHours()) / 24f)));
                                            goto Label_0991;

                                        case 4:
                                        {
                                            string[] textArray7 = new string[] { masterRoleInfo.dailyPvpCnt.ToString(), str4 };
                                            StrHelper[k + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_2", textArray7);
                                            goto Label_0991;
                                        }
                                        case 5:
                                        {
                                            string[] textArray8 = new string[] { str4 };
                                            StrHelper[k + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_13", textArray8);
                                            goto Label_0991;
                                        }
                                        case 6:
                                        {
                                            string[] textArray9 = new string[] { str4 };
                                            StrHelper[k + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_15", textArray9);
                                            goto Label_0991;
                                        }
                                        case 7:
                                        {
                                            string[] textArray10 = new string[] { str4 };
                                            StrHelper[k + 1] = Singleton<CTextManager>.instance.GetText("Daily_Quest_FirstVictoryName", textArray10);
                                            goto Label_0991;
                                        }
                                        case 8:
                                        {
                                            string[] textArray11 = new string[] { str4 };
                                            StrHelper[k + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_16", textArray11);
                                            goto Label_0991;
                                        }
                                        case 9:
                                        {
                                            string[] textArray12 = new string[] { str4 };
                                            StrHelper[k + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_14", textArray12);
                                            goto Label_0991;
                                        }
                                        case 10:
                                        {
                                            string[] textArray13 = new string[] { str4 };
                                            StrHelper[k + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_21", textArray13);
                                            goto Label_0991;
                                        }
                                        case 11:
                                        {
                                            string[] textArray14 = new string[] { str4 };
                                            StrHelper[k + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_17", textArray14);
                                            goto Label_0991;
                                        }
                                        case 12:
                                        {
                                            string[] textArray15 = new string[] { str4 };
                                            StrHelper[k + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_18", textArray15);
                                            goto Label_0991;
                                        }
                                        case 13:
                                        {
                                            string[] textArray17 = new string[] { str4 };
                                            StrHelper[k + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_20", textArray17);
                                            goto Label_0991;
                                        }
                                        case 14:
                                        {
                                            string[] textArray16 = new string[] { str4 };
                                            StrHelper[k + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_19", textArray16);
                                            goto Label_0991;
                                        }
                                        default:
                                            goto Label_0991;
                                    }
                                    string[] textArray6 = new string[] { str4 };
                                    StrHelper[k + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_3", textArray6);
                                Label_0991:;
                                }
                            }
                            string str5 = StrHelper[0];
                            for (int j = 1; j < StrHelper.Length; j++)
                            {
                                if (!string.IsNullOrEmpty(StrHelper[j]))
                                {
                                    if ((multipleData[j].iType == 7) || string.IsNullOrEmpty(StrHelper2[j]))
                                    {
                                        str5 = string.Format("{0}\n{1}", str5, StrHelper[j]);
                                    }
                                    else
                                    {
                                        str5 = string.Format("{0}\n{1}({2}%)", str5, StrHelper[j], StrHelper2[j]);
                                    }
                                }
                            }
                            obj3.CustomSetActive(true);
                            if (num5 == 0)
                            {
                                obj3.CustomSetActive(false);
                            }
                            else
                            {
                                obj3.GetComponentInChildren<Text>().set_text(string.Format("{0}", str3));
                            }
                            CUICommonSystem.SetCommonTipsEvent(this._profitFormScript, obj3, str5, enUseableTipsPos.enTop);
                        }
                    }
                }
            }
        }

        private void SetGoldCoinProfit()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                COMDT_ACNT_INFO acntInfo = Singleton<BattleStatistic>.GetInstance().acntInfo;
                if (acntInfo != null)
                {
                    GameObject obj2 = this._profitFormScript.m_formWidgets[2];
                    Text component = obj2.get_transform().FindChild("goldTotalGroup/GoldNum").GetComponent<Text>();
                    component.set_text("+0");
                    this._coinFrom = 0f;
                    this._coinTo = acntInfo.dwPvpSettleCoin;
                    this._coinTweenText = component;
                    obj2.get_transform().FindChild("GoldMax").get_gameObject().CustomSetActive(acntInfo.bReachDailyLimit > 0);
                    GameObject obj4 = obj2.get_transform().FindChild("goldTotalGroup/goldGroup/DoubleCoin").get_gameObject();
                    obj4.CustomSetActive(false);
                    Transform transform = obj2.get_transform().FindChild("goldTotalGroup/goldGroup/QQVipIcon");
                    transform.get_gameObject().CustomSetActive(false);
                    for (int i = 0; i < StrHelper.Length; i++)
                    {
                        StrHelper[i] = null;
                    }
                    COMDT_REWARD_MULTIPLE_DETAIL multiDetail = Singleton<BattleStatistic>.GetInstance().multiDetail;
                    if (multiDetail != null)
                    {
                        int num2 = CUseable.GetMultiple(acntInfo.dwPvpSettleBaseCoin, ref multiDetail, 11, -1);
                        COMDT_MULTIPLE_DATA[] multipleData = null;
                        uint num3 = CUseable.GetMultipleInfo(out multipleData, ref multiDetail, 11, -1);
                        string str = string.Empty;
                        if (num2 > 0)
                        {
                            str = str + "+";
                        }
                        str = str + num2.ToString();
                        string[] args = new string[] { str };
                        StrHelper[0] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_7", args);
                        if (multipleData != null)
                        {
                            obj4.CustomSetActive(true);
                            bool flag = false;
                            bool flag2 = false;
                            for (int k = 0; k < num3; k++)
                            {
                                if (multipleData[k].iType == 2)
                                {
                                    flag = true;
                                }
                                if (multipleData[k].iType == 9)
                                {
                                    flag2 = true;
                                }
                            }
                            for (int m = 0; m < num3; m++)
                            {
                                Transform transform2;
                                Transform transform4;
                                Transform transform5;
                                string str2 = string.Empty;
                                if ((acntInfo.dwPvpSettleBaseCoin * multipleData[m].iValue) > 0L)
                                {
                                    str2 = "+";
                                }
                                switch (multipleData[m].bOperator)
                                {
                                    case 0:
                                        str2 = str2 + multipleData[m].iValue;
                                        break;

                                    case 1:
                                    {
                                        double num6 = (acntInfo.dwPvpSettleBaseCoin * multipleData[m].iValue) / 10000.0;
                                        if (num6 > 0.0)
                                        {
                                            str2 = str2 + ((int) (num6 + 0.9999));
                                        }
                                        else if (num6 < 0.0)
                                        {
                                            str2 = str2 + ((int) (num6 - 0.9999));
                                        }
                                        else
                                        {
                                            str2 = "0";
                                        }
                                        break;
                                    }
                                    default:
                                        str2 = str2 + "0";
                                        break;
                                }
                                StrHelper2[m + 1] = string.Empty;
                                int num7 = multipleData[m].iValue / 100;
                                if (num7 >= 0)
                                {
                                    StrHelper2[m + 1] = StrHelper2[m + 1] + "+";
                                }
                                StrHelper2[m + 1] = StrHelper2[m + 1] + num7;
                                switch (multipleData[m].iType)
                                {
                                    case 1:
                                    {
                                        string[] textArray2 = new string[] { str2 };
                                        StrHelper[m + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_6", textArray2);
                                        goto Label_07C3;
                                    }
                                    case 2:
                                    {
                                        if (!masterRoleInfo.HasVip(0x10))
                                        {
                                            break;
                                        }
                                        string[] textArray3 = new string[] { str2 };
                                        StrHelper[m + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_9", textArray3);
                                        goto Label_0417;
                                    }
                                    case 3:
                                        StrHelper[m + 1] = string.Format(Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_10"), str2, masterRoleInfo.GetCoinWinCount(), Math.Ceiling((double) (((float) masterRoleInfo.GetCoinExpireHours()) / 24f)));
                                        goto Label_07C3;

                                    case 4:
                                    {
                                        string[] textArray5 = new string[] { masterRoleInfo.dailyPvpCnt.ToString(), str2 };
                                        StrHelper[m + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_2", textArray5);
                                        goto Label_07C3;
                                    }
                                    case 5:
                                    {
                                        string[] textArray6 = new string[] { str2 };
                                        StrHelper[m + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_13", textArray6);
                                        Transform transform3 = obj2.get_transform().FindChild("goldTotalGroup/goldGroup/WXIcon");
                                        MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(transform3.get_gameObject(), masterRoleInfo.m_privilegeType, ApolloPlatform.Wechat, false, CSysDynamicBlock.bLobbyEntryBlocked);
                                        transform3.FindChild("Text").GetComponent<Text>().set_text(string.Format("{0}", str2));
                                        goto Label_07C3;
                                    }
                                    case 6:
                                    {
                                        string[] textArray7 = new string[] { str2 };
                                        StrHelper[m + 1] = Singleton<CTextManager>.instance.GetText("Guild_Settlement_Guild_Coin_Plus_Tip2", textArray7);
                                        goto Label_07C3;
                                    }
                                    case 7:
                                    {
                                        string[] textArray8 = new string[] { str2 };
                                        StrHelper[m + 1] = Singleton<CTextManager>.instance.GetText("Daily_Quest_FirstVictoryName", textArray8);
                                        goto Label_07C3;
                                    }
                                    case 8:
                                    {
                                        string[] textArray9 = new string[] { str2 };
                                        StrHelper[m + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_16", textArray9);
                                        goto Label_07C3;
                                    }
                                    case 9:
                                    {
                                        string[] textArray10 = new string[] { str2 };
                                        StrHelper[m + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_14", textArray10);
                                        transform4 = obj2.get_transform().FindChild("goldTotalGroup/goldGroup/QQGameCenterIcon");
                                        if (flag)
                                        {
                                            goto Label_0672;
                                        }
                                        MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(transform4.get_gameObject(), masterRoleInfo.m_privilegeType, ApolloPlatform.QQ, false, CSysDynamicBlock.bLobbyEntryBlocked);
                                        transform4.FindChild("Text").GetComponent<Text>().set_text(string.Format("{0}", str2));
                                        goto Label_07C3;
                                    }
                                    case 10:
                                    {
                                        string[] textArray11 = new string[] { str2 };
                                        StrHelper[m + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_21", textArray11);
                                        transform5 = obj2.get_transform().FindChild("goldTotalGroup/goldGroup/GuestGameCenterIcon");
                                        if (flag || flag2)
                                        {
                                            goto Label_070D;
                                        }
                                        MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(transform5.get_gameObject(), masterRoleInfo.m_privilegeType, ApolloPlatform.Guest, false, CSysDynamicBlock.bLobbyEntryBlocked);
                                        transform5.FindChild("Text").GetComponent<Text>().set_text(string.Format("{0}", str2));
                                        goto Label_07C3;
                                    }
                                    case 11:
                                    {
                                        string[] textArray12 = new string[] { str2 };
                                        StrHelper[m + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_17", textArray12);
                                        goto Label_07C3;
                                    }
                                    case 12:
                                    {
                                        string[] textArray13 = new string[] { str2 };
                                        StrHelper[m + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_18", textArray13);
                                        goto Label_07C3;
                                    }
                                    case 13:
                                    {
                                        string[] textArray15 = new string[] { str2 };
                                        StrHelper[m + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_20", textArray15);
                                        goto Label_07C3;
                                    }
                                    case 14:
                                    {
                                        string[] textArray14 = new string[] { str2 };
                                        StrHelper[m + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_19", textArray14);
                                        goto Label_07C3;
                                    }
                                    default:
                                        goto Label_07C3;
                                }
                                string[] textArray4 = new string[] { str2 };
                                StrHelper[m + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_3", textArray4);
                            Label_0417:
                                transform2 = transform.FindChild("Icon");
                                if (transform2 != null)
                                {
                                    MonoSingleton<NobeSys>.GetInstance().SetMyQQVipHead(transform2.GetComponent<Image>());
                                    transform.FindChild("Text").GetComponent<Text>().set_text(string.Format("{0}", str2));
                                    transform.get_gameObject().CustomSetActive(true);
                                }
                                goto Label_07C3;
                            Label_0672:
                                transform4.get_gameObject().CustomSetActive(false);
                                goto Label_07C3;
                            Label_070D:
                                transform5.get_gameObject().CustomSetActive(false);
                            Label_07C3:;
                            }
                        }
                        string str3 = StrHelper[0];
                        for (int j = 1; j < StrHelper.Length; j++)
                        {
                            if (!string.IsNullOrEmpty(StrHelper[j]))
                            {
                                if ((multipleData[j].iType == 7) || string.IsNullOrEmpty(StrHelper2[j]))
                                {
                                    str3 = string.Format("{0}\n{1}", str3, StrHelper[j]);
                                }
                                else
                                {
                                    str3 = string.Format("{0}\n{1}({2}%)", str3, StrHelper[j], StrHelper2[j]);
                                }
                            }
                        }
                        if (CSysDynamicBlock.bLobbyEntryBlocked)
                        {
                            obj4.CustomSetActive(false);
                        }
                        else
                        {
                            obj4.CustomSetActive(true);
                            if (num2 == 0)
                            {
                                obj4.CustomSetActive(false);
                            }
                            else
                            {
                                obj4.GetComponentInChildren<Text>().set_text(string.Format("{0}", str));
                            }
                            CUICommonSystem.SetCommonTipsEvent(this._profitFormScript, obj4, str3, enUseableTipsPos.enTop);
                        }
                    }
                }
            }
        }

        private void SetGuildInfo()
        {
            GameObject obj2 = this._profitFormScript.m_formWidgets[4];
            obj2.CustomSetActive(false);
            if (((Singleton<BattleStatistic>.GetInstance().acntInfo != null) && Singleton<CGuildSystem>.GetInstance().IsInNormalGuild()) && this.IsGuildProfitGameType())
            {
                GuildMemInfo playerGuildMemberInfo = CGuildHelper.GetPlayerGuildMemberInfo();
                if (playerGuildMemberInfo != null)
                {
                    GameObject obj3 = this._profitFormScript.m_formWidgets[7];
                    Text component = obj2.get_transform().FindChild("GuildPointTxt").GetComponent<Text>();
                    Transform transform = obj2.get_transform().FindChild("ImageIcon");
                    Transform transform2 = obj2.get_transform().FindChild("GuildText");
                    uint num = playerGuildMemberInfo.RankInfo.byGameRankPoint - CGuildSystem.s_lastByGameRankpoint;
                    object[] inParameters = new object[] { playerGuildMemberInfo.RankInfo.byGameRankPoint, CGuildSystem.s_lastByGameRankpoint };
                    DebugHelper.Assert(playerGuildMemberInfo.RankInfo.byGameRankPoint >= CGuildSystem.s_lastByGameRankpoint, "byGameRankPoint={0}, s_lastByGameRankpoint={1}", inParameters);
                    if (transform != null)
                    {
                        transform.get_gameObject().CustomSetActive(num > 0);
                    }
                    if (transform2 != null)
                    {
                        transform2.get_gameObject().CustomSetActive(num > 0);
                    }
                    if (num > 0)
                    {
                        obj3.CustomSetActive(false);
                        component.set_text(num.ToString(CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        obj3.CustomSetActive(CGuildSystem.s_lastByGameRankpoint >= CGuildSystem.s_rankpointProfitMax);
                        component.set_text(string.Empty);
                    }
                    CGuildSystem.s_lastByGameRankpoint = playerGuildMemberInfo.RankInfo.byGameRankPoint;
                    CUICommonSystem.SetCommonTipsEvent(this._profitFormScript, obj2, Singleton<CTextManager>.GetInstance().GetText("Guild_Settlement_Guild_Info_Tip"), enUseableTipsPos.enTop);
                    obj2.CustomSetActive(true);
                }
            }
        }

        public void SetLadderDisplayOldAndNewGrade(uint oldGrade, uint oldScore, uint newGrade, uint newScore)
        {
            this._oldGrade = Math.Max(oldGrade, 1);
            this._oldScore = oldScore;
            this._oldMaxScore = GameDataMgr.rankGradeDatabin.GetDataByKey(this._oldGrade).dwGradeUpNeedScore;
            this._newGrade = Math.Max(newGrade, 1);
            this._newScore = newScore;
            this._newMaxScore = GameDataMgr.rankGradeDatabin.GetDataByKey(this._oldGrade).dwGradeUpNeedScore;
            this._isUp = false;
            this._isDown = false;
            if (this._oldGrade < this._newGrade)
            {
                this.m_bGrade = true;
            }
            else
            {
                this.m_bGrade = false;
            }
            if ((this._oldGrade < this._newGrade) || ((this._oldGrade == this._newGrade) && (this._oldScore < this._newScore)))
            {
                this._isUp = true;
                this._isDown = false;
            }
            else
            {
                this._isDown = true;
                this._isUp = false;
            }
            this._curDian = this._oldScore;
            this._curGrade = this._oldGrade;
            this._curMaxScore = this._oldMaxScore;
        }

        private void SetLadderInfo()
        {
            GameObject obj2 = this._profitFormScript.m_formWidgets[5];
            obj2.CustomSetActive(false);
            this._isLadderMatch = false;
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if ((curLvelContext != null) && curLvelContext.IsGameTypeLadder())
            {
                COMDT_RANK_SETTLE_INFO rankInfo = Singleton<BattleStatistic>.GetInstance().rankInfo;
                if (rankInfo != null)
                {
                    obj2.CustomSetActive(true);
                    this._isLadderMatch = true;
                    Transform transform = obj2.get_transform().FindChild(string.Format("RankLevelName", new object[0]));
                    if (transform != null)
                    {
                        transform.get_gameObject().GetComponent<Text>().set_text(CLadderView.GetRankName(rankInfo.bNowGrade, Singleton<CRoleInfoManager>.GetInstance().GetSelfRankClass()));
                    }
                    if (obj2.get_transform().FindChild(string.Format("WangZheXingTxt", new object[0])) != null)
                    {
                        Text component = obj2.get_transform().FindChild(string.Format("WangZheXingTxt", new object[0])).get_gameObject().GetComponent<Text>();
                        if (rankInfo.bNowGrade == GameDataMgr.rankGradeDatabin.count)
                        {
                            Transform transform2 = obj2.get_transform().FindChild(string.Format("XingGrid/ImgScore{0}", 1));
                            if (transform2 != null)
                            {
                                transform2.get_gameObject().CustomSetActive(true);
                            }
                            component.get_gameObject().CustomSetActive(true);
                            component.set_text(string.Format("X{0}", rankInfo.dwNowScore));
                        }
                        else
                        {
                            component.get_gameObject().CustomSetActive(false);
                            for (int i = 1; i <= rankInfo.dwNowScore; i++)
                            {
                                Transform transform3 = obj2.get_transform().FindChild(string.Format("XingGrid/ImgScore{0}", i));
                                if (transform3 != null)
                                {
                                    transform3.get_gameObject().CustomSetActive(true);
                                }
                            }
                        }
                        this._profitFormScript.m_formWidgets[6].get_gameObject().CustomSetActive(false);
                    }
                }
            }
        }

        public void SetLastMatchDuration(string duration, string startTime, uint startTimeInt)
        {
            this._duration = duration;
            this._startTime = startTime;
            this._startTimeInt = startTimeInt;
        }

        private void SetMapInfo()
        {
            GameObject obj2 = this._profitFormScript.m_formWidgets[6];
            obj2.CustomSetActive(false);
            Text component = obj2.get_transform().FindChild("GameType").GetComponent<Text>();
            Text text2 = obj2.get_transform().FindChild("MapName").GetComponent<Text>();
            string text = Singleton<CTextManager>.instance.GetText("Battle_Settle_Game_Type_Single");
            string levelName = string.Empty;
            SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
            if (curLvelContext != null)
            {
                uint mapID = (uint) curLvelContext.m_mapID;
                if (curLvelContext.IsMobaMode())
                {
                    obj2.CustomSetActive(true);
                    levelName = curLvelContext.m_levelName;
                    if (curLvelContext.IsGameTypeRewardMatch())
                    {
                        text = curLvelContext.m_SecondName;
                    }
                    else
                    {
                        text = Singleton<CTextManager>.instance.GetText(string.Format("Battle_Settle_Game_Type{0}", curLvelContext.m_pvpPlayerNum / 2));
                    }
                }
                component.set_text(text);
                text2.set_text(levelName);
            }
        }

        private void SetPlayerSettlement()
        {
            CUIListScript script = null;
            CUIListScript component = null;
            component = this._settleFormScript.m_formWidgets[7].GetComponent<CUIListScript>();
            script = this._settleFormScript.m_formWidgets[8].GetComponent<CUIListScript>();
            int count = Singleton<GamePlayerCenter>.GetInstance().GetAllCampPlayers(COM_PLAYERCAMP.COM_PLAYERCAMP_1).Count;
            int amount = Singleton<GamePlayerCenter>.GetInstance().GetAllCampPlayers(COM_PLAYERCAMP.COM_PLAYERCAMP_2).Count;
            this.playerNum = count + amount;
            component.SetElementAmount(count);
            script.SetElementAmount(amount);
            this._curLeftIndex = 0;
            this._curRightIndex = 0;
            CPlayerKDAStat playerKDAStat = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat;
            if (playerKDAStat != null)
            {
                DictionaryView<uint, PlayerKDA>.Enumerator enumerator = playerKDAStat.GetEnumerator();
                this._camp1TotalDamage = 0;
                this._camp1TotalTakenDamage = 0;
                this._camp1TotalToHeroDamage = 0;
                this._camp2TotalDamage = 0;
                this._camp2TotalTakenDamage = 0;
                this._camp2TotalToHeroDamage = 0;
                this._camp1TotalKill = 0;
                this._camp2TotalKill = 0;
                this._myCamp = Singleton<GamePlayerCenter>.GetInstance().hostPlayerCamp;
                while (enumerator.MoveNext())
                {
                    KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                    PlayerKDA kda = current.Value;
                    this.CollectPlayerKda(kda);
                }
                GameObject obj2 = this._settleFormScript.m_formWidgets[4];
                obj2.get_transform().FindChild("LeftTotalKill").get_gameObject().GetComponent<Text>().set_text(this._camp1TotalKill.ToString(CultureInfo.InvariantCulture));
                obj2.get_transform().FindChild("RightTotalKill").get_gameObject().GetComponent<Text>().set_text(this._camp2TotalKill.ToString(CultureInfo.InvariantCulture));
                DictionaryView<uint, PlayerKDA>.Enumerator playerKdaEmr = playerKDAStat.GetEnumerator();
                this.HostPlayerHeroId = this.GetHostPlayerHeroId(playerKdaEmr);
                playerKdaEmr.Reset();
                while (playerKdaEmr.MoveNext())
                {
                    KeyValuePair<uint, PlayerKDA> pair2 = playerKdaEmr.Current;
                    PlayerKDA rkda2 = pair2.Value;
                    this.UpdatePlayerKda(rkda2);
                }
            }
        }

        private void SetProficiencyInfo()
        {
            GameObject obj2 = this._profitFormScript.m_formWidgets[3];
            obj2.CustomSetActive(false);
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                PlayerKDA hostKDA = null;
                if ((Singleton<BattleLogic>.GetInstance().battleStat != null) && (Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat != null))
                {
                    hostKDA = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat.GetHostKDA();
                }
                if (hostKDA != null)
                {
                    CHeroInfo info2;
                    RectTransform component = obj2.get_transform().FindChild("ProficiencySliderBg/BaseProficiencySlider").get_gameObject().GetComponent<RectTransform>();
                    RectTransform transform2 = obj2.get_transform().FindChild("ProficiencySliderBg/AddProficiencySlider").get_gameObject().GetComponent<RectTransform>();
                    Text text = obj2.get_transform().FindChild("HeroName").GetComponent<Text>();
                    Text text2 = obj2.get_transform().FindChild("ProficiencyLv").GetComponent<Text>();
                    Text text3 = obj2.get_transform().FindChild("ProficiencyTxt").GetComponent<Text>();
                    Text text4 = obj2.get_transform().FindChild("AddProficiencyTxt").GetComponent<Text>();
                    Image image = obj2.get_transform().FindChild("HeroInfo/HeroHeadIcon").GetComponent<Image>();
                    GameObject proficiencyIcon = obj2.get_transform().FindChild("heroProficiencyImg").get_gameObject();
                    text4.set_text(null);
                    text.set_text(string.Empty);
                    ListView<HeroKDA>.Enumerator enumerator = hostKDA.GetEnumerator();
                    uint id = 0;
                    uint skinId = 0;
                    while (enumerator.MoveNext())
                    {
                        HeroKDA current = enumerator.Current;
                        if (current != null)
                        {
                            id = (uint) current.HeroId;
                            skinId = current.SkinId;
                            break;
                        }
                    }
                    masterRoleInfo.GetHeroInfo(id, out info2, false);
                    ActorMeta meta2 = new ActorMeta();
                    meta2.PlayerId = hostKDA.PlayerId;
                    meta2.ConfigId = (int) id;
                    ActorMeta actorMeta = meta2;
                    ActorStaticData actorData = new ActorStaticData();
                    Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.StaticLobbyDataProvider).GetActorStaticData(ref actorMeta, ref actorData);
                    COMDT_SETTLE_HERO_RESULT_INFO heroSettleInfo = GetHeroSettleInfo(id);
                    if (heroSettleInfo != null)
                    {
                        ResHeroProficiency heroProficiency = CHeroInfo.GetHeroProficiency(actorData.TheHeroOnlyInfo.HeroCapability, (int) heroSettleInfo.dwProficiencyLv);
                        if ((heroProficiency != null) && (Singleton<BattleLogic>.GetInstance().GetCurLvelContext().IsMobaMode() && (info2 != null)))
                        {
                            obj2.CustomSetActive(true);
                            text.set_text(actorData.TheResInfo.Name);
                            text4.set_text((heroSettleInfo.dwSettleProficiency <= 0) ? null : string.Format("+{0}", heroSettleInfo.dwSettleProficiency));
                            if (heroSettleInfo.dwSettleProficiency == 0)
                            {
                            }
                            image.SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, CSkinInfo.GetHeroSkinPic(id, 0)), obj2.GetComponent<CUIFormScript>(), true, false, false, false);
                            float num3 = 0f;
                            float num4 = 0f;
                            if (CHeroInfo.GetMaxProficiency() == heroSettleInfo.dwProficiencyLv)
                            {
                                num3 = 1f;
                                num4 = 0f;
                                text3.set_text("MAX");
                            }
                            else
                            {
                                int num6 = (int) (heroSettleInfo.dwProficiency - heroSettleInfo.dwSettleProficiency);
                                num3 = Mathf.Max(0f, ((float) num6) / ((float) heroProficiency.dwTopPoint));
                                num4 = Mathf.Max(0f, ((num6 >= 0) ? ((float) heroSettleInfo.dwSettleProficiency) : ((float) heroSettleInfo.dwProficiency)) / ((float) heroProficiency.dwTopPoint));
                                text3.set_text(string.Format("{0} / {1}", heroSettleInfo.dwProficiency, heroProficiency.dwTopPoint));
                            }
                            text2.set_text(GetProficiencyLvTxt(actorData.TheHeroOnlyInfo.HeroCapability, heroSettleInfo.dwProficiencyLv));
                            CUICommonSystem.SetHeroProficiencyIconImage(this._profitFormScript, proficiencyIcon, (int) heroSettleInfo.dwProficiencyLv);
                            transform2.set_sizeDelta(new Vector2((num3 + num4) * 159.45f, transform2.get_sizeDelta().y));
                            component.set_sizeDelta(new Vector2(num3 * 159.45f, component.get_sizeDelta().y));
                            transform2.get_gameObject().CustomSetActive(num4 > 0f);
                        }
                    }
                }
            }
        }

        private void SetSettlementButton()
        {
            GameObject obj2 = this._settleFormScript.m_formWidgets[1];
            obj2.get_transform().FindChild("BtnBack").get_gameObject().CustomSetActive(true);
            obj2.get_transform().FindChild("BtnAgain").get_gameObject().CustomSetActive(!this._neutral);
            obj2.get_transform().FindChild("BtnSaveReplay").get_gameObject().CustomSetActive(Singleton<GameReplayModule>.GetInstance().HasRecord);
            obj2.get_transform().FindChild("ButtonShare").get_gameObject().CustomSetActive((this._win && !this._neutral) && !CSysDynamicBlock.bSocialBlocked);
            obj2.get_transform().FindChild("ButtonShit").get_gameObject().CustomSetActive((!this._win && !this._neutral) && !CSysDynamicBlock.bSocialBlocked);
            this.m_ShareDataBtn = obj2.get_transform().FindChild("ButtonShareData").get_gameObject();
            this.m_ShareDataBtn.CustomSetActive(!this._neutral && !CSysDynamicBlock.bSocialBlocked);
        }

        private void SetSettlementTitle()
        {
            if (this._settleFormScript != null)
            {
                GameObject obj2 = this._settleFormScript.m_formWidgets[0];
                obj2.get_transform().FindChild("Mid").get_gameObject().CustomSetActive(this._neutral);
                obj2.get_transform().FindChild("Win").get_gameObject().CustomSetActive(this._win && !this._neutral);
                obj2.get_transform().FindChild("Lose").get_gameObject().CustomSetActive(!this._win && !this._neutral);
                GameObject p = this._settleFormScript.m_formWidgets[4];
                Color color = (!this._win || this._neutral) ? new Color(1f, 1f, 1f, 0.75f) : Color.get_white();
                Utility.GetComponetInChild<Image>(p, "BgRed").set_color(color);
                Utility.GetComponetInChild<Image>(p, "BgBlue").set_color(color);
                Utility.FindChild(p, "LightBlue1").CustomSetActive(this._win && !this._neutral);
                Utility.FindChild(p, "LightBlue2").CustomSetActive(this._win && !this._neutral);
                Utility.FindChild(p, "LightRed1").CustomSetActive(this._win && !this._neutral);
            }
        }

        private void SetSpecialItem()
        {
            COMDT_PVPSPECITEM_OUTPUT specialItemInfo = Singleton<BattleStatistic>.GetInstance().SpecialItemInfo;
            COMDT_REWARD_DETAIL rewards = Singleton<BattleStatistic>.GetInstance().Rewards;
            CUseable[] items = new CUseable[specialItemInfo.bOutputCnt + rewards.bNum];
            int index = 0;
            if (rewards.bNum > 0)
            {
                ListView<CUseable> useableListFromReward = CUseableManager.GetUseableListFromReward(rewards);
                for (int i = 0; i < useableListFromReward.Count; i++)
                {
                    items[index] = useableListFromReward[i];
                    index++;
                }
            }
            if (specialItemInfo.bOutputCnt > 0)
            {
                DictionaryView<uint, ResPVPSpecItem> pvpSpecialItemDict = GameDataMgr.pvpSpecialItemDict;
                if (pvpSpecialItemDict != null)
                {
                    for (int j = 0; j < specialItemInfo.bOutputCnt; j++)
                    {
                        ResPVPSpecItem item = null;
                        if (pvpSpecialItemDict.TryGetValue(specialItemInfo.astOutputInfo[j].dwPVPSpecItemID, out item))
                        {
                            items[index] = CUseableManager.CreateUsableByServerType((RES_REWARDS_TYPE) item.wItemType, (int) specialItemInfo.astOutputInfo[j].dwPVPSpecItemCnt, item.dwItemID);
                        }
                        index++;
                    }
                }
            }
            string text = Singleton<CTextManager>.GetInstance().GetText("gotAward");
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if ((curLvelContext != null) && curLvelContext.IsGameTypeRewardMatch())
            {
                if (this._win)
                {
                    text = Singleton<CTextManager>.GetInstance().GetText("Union_Battle_Tips8");
                }
                else
                {
                    text = Singleton<CTextManager>.GetInstance().GetText("Union_Battle_Tips9");
                }
            }
            if (items.Length == 0)
            {
                this.DoCoinAndExpTween();
            }
            else if ((curLvelContext != null) && curLvelContext.IsGameTypeRewardMatch())
            {
                Singleton<CUIManager>.GetInstance().OpenAwardTip(items, text, true, enUIEventID.SettlementSys_ClickItemDisplay, false, true, "Form_AwardGold");
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenAwardTip(items, text, true, enUIEventID.SettlementSys_ClickItemDisplay, false, true, "Form_Award");
            }
        }

        private void SetSymbolCoinProfit()
        {
            if (Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo() != null)
            {
                COMDT_ACNT_INFO acntInfo = Singleton<BattleStatistic>.GetInstance().acntInfo;
                if (acntInfo != null)
                {
                    GameObject obj2 = this._profitFormScript.m_formWidgets[10];
                    Text component = obj2.get_transform().FindChild("CoinNum").GetComponent<Text>();
                    component.set_text("+0");
                    this._symbolCoinFrom = 0f;
                    this._symbolCoinTo = acntInfo.dwSymbolCoin;
                    this._symbolCoinTweenText = component;
                    obj2.get_transform().FindChild("CoinMax").get_gameObject().CustomSetActive(acntInfo.bReachDailyLimit > 0);
                }
            }
        }

        private void SetTitle()
        {
            GameObject obj2 = this._profitFormScript.m_formWidgets[0];
            obj2.get_transform().FindChild("Win").get_gameObject().CustomSetActive(this._win);
            obj2.get_transform().FindChild("Lose").get_gameObject().CustomSetActive(!this._win);
        }

        private void SetVictoryTipsBtnInfo(CUIFormScript form)
        {
            PlayerKDA hostKDA = null;
            if ((Singleton<BattleLogic>.GetInstance().battleStat != null) && (Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat != null))
            {
                hostKDA = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat.GetHostKDA();
            }
            if (hostKDA != null)
            {
                ListView<HeroKDA>.Enumerator enumerator = hostKDA.GetEnumerator();
                uint key = 0;
                while (enumerator.MoveNext())
                {
                    HeroKDA current = enumerator.Current;
                    if (current != null)
                    {
                        key = (uint) current.HeroId;
                        break;
                    }
                }
                if (form != null)
                {
                    GameObject widget = form.GetWidget(0x18);
                    if (CBattleGuideManager.EnableHeroVictoryTips() && !CSysDynamicBlock.bLobbyEntryBlocked)
                    {
                        widget.CustomSetActive(true);
                        Transform transform = widget.get_transform();
                        string[] args = new string[] { key.ToString() };
                        transform.FindChild("Btn").GetComponent<CUIEventScript>().m_onClickEventParams.tagStr = Singleton<CTextManager>.GetInstance().GetText("WinTrick_Url_Result", args);
                        if (!this._win && !MonoSingleton<NewbieGuideManager>.instance.isNewbieGuiding)
                        {
                            int num2 = PlayerPrefs.GetInt(PlayerWinTimesStr, 0) + 1;
                            uint globeValue = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_SHOW_WINTRICKTIPS_PVPLOSE_TIMES);
                            if (num2 >= globeValue)
                            {
                                string szName;
                                transform.FindChild("Panel_Guide").get_gameObject().CustomSetActive(true);
                                ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(key);
                                if (dataByKey != null)
                                {
                                    szName = dataByKey.szName;
                                }
                                else
                                {
                                    szName = Singleton<CTextManager>.GetInstance().GetText("WinTrick_Tips_DefaultHeroName");
                                }
                                string[] textArray2 = new string[] { szName };
                                transform.FindChild("Panel_Guide/Text").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("WinTrick_Tips_text", textArray2));
                                PlayerPrefs.SetInt(PlayerWinTimesStr, 0);
                            }
                            else
                            {
                                PlayerPrefs.SetInt(PlayerWinTimesStr, num2);
                                transform.FindChild("Panel_Guide").get_gameObject().CustomSetActive(false);
                            }
                        }
                        else
                        {
                            PlayerPrefs.SetInt(PlayerWinTimesStr, 0);
                            transform.FindChild("Panel_Guide").get_gameObject().CustomSetActive(false);
                        }
                    }
                    else
                    {
                        widget.CustomSetActive(false);
                    }
                    if (this._neutral)
                    {
                        widget.CustomSetActive(false);
                    }
                }
            }
        }

        private void ShowBraveScoreDetailPanel()
        {
            if (this._ladderForm == null)
            {
                DebugHelper.Assert(false, "_ladderForm is null!!!");
            }
            else
            {
                COMDT_RANK_SETTLE_INFO rankInfo = Singleton<BattleStatistic>.instance.rankInfo;
                if (rankInfo == null)
                {
                    DebugHelper.Assert(false, "BattleStatistic.instance.rankInfo is null!!!");
                }
                else
                {
                    uint dwGameScore = rankInfo.dwGameScore;
                    uint dwConWinScore = rankInfo.dwConWinScore;
                    uint dwMVPScore = rankInfo.dwMVPScore;
                    uint dwMMRScore = rankInfo.dwMMRScore;
                    uint num5 = ((dwGameScore + dwConWinScore) + dwMVPScore) + dwMMRScore;
                    GameObject widget = this._ladderForm.GetWidget(0);
                    GameObject obj3 = this._ladderForm.GetWidget(1);
                    Text component = this._ladderForm.GetWidget(2).GetComponent<Text>();
                    Text text2 = this._ladderForm.GetWidget(3).GetComponent<Text>();
                    if (num5 > 0)
                    {
                        if (this.IsBraveScoreDeduction())
                        {
                            this._isBraveScoreIncreased = false;
                            widget.CustomSetActive(false);
                            obj3.CustomSetActive(true);
                            component.set_text(Singleton<CTextManager>.GetInstance().GetText("Ladder_Brave_Clear_Score_Tip"));
                            text2.set_text("0");
                        }
                        else
                        {
                            this._isBraveScoreIncreased = true;
                            widget.CustomSetActive(true);
                            Text text3 = this._ladderForm.GetWidget(7).GetComponent<Text>();
                            Text text4 = this._ladderForm.GetWidget(8).GetComponent<Text>();
                            Text text5 = this._ladderForm.GetWidget(9).GetComponent<Text>();
                            Text text6 = this._ladderForm.GetWidget(10).GetComponent<Text>();
                            GameObject obj4 = this._ladderForm.GetWidget(0x12);
                            GameObject obj5 = this._ladderForm.GetWidget(0x13);
                            GameObject obj6 = this._ladderForm.GetWidget(20);
                            GameObject obj7 = this._ladderForm.GetWidget(0x15);
                            if (dwGameScore > 0)
                            {
                                obj4.CustomSetActive(true);
                                text3.get_gameObject().CustomSetActive(true);
                                text3.set_text("+" + dwGameScore);
                            }
                            else
                            {
                                obj4.CustomSetActive(false);
                                text3.get_gameObject().CustomSetActive(false);
                            }
                            if (dwConWinScore > 0)
                            {
                                obj5.CustomSetActive(true);
                                text4.get_gameObject().CustomSetActive(true);
                                text4.set_text("+" + dwConWinScore);
                                COMDT_RANKDETAIL currentRankDetail = Singleton<CLadderSystem>.GetInstance().GetCurrentRankDetail();
                                if (currentRankDetail != null)
                                {
                                    string[] args = new string[] { currentRankDetail.dwContinuousWin.ToString() };
                                    obj5.GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Ladder_Brave_Liansheng", args));
                                }
                            }
                            else
                            {
                                obj5.CustomSetActive(false);
                                text4.get_gameObject().CustomSetActive(false);
                            }
                            if (dwMVPScore > 0)
                            {
                                obj6.CustomSetActive(true);
                                text5.get_gameObject().CustomSetActive(true);
                                text5.set_text("+" + dwMVPScore);
                                string[] textArray2 = new string[] { this.GetMvpScoreRankInCamp().ToString() };
                                obj6.GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Ladder_Brave_Xianfeng", textArray2));
                            }
                            else
                            {
                                obj6.CustomSetActive(false);
                                text5.get_gameObject().CustomSetActive(false);
                            }
                            if (dwMMRScore > 0)
                            {
                                obj7.CustomSetActive(true);
                                text6.get_gameObject().CustomSetActive(true);
                                text6.set_text("+" + dwMMRScore);
                            }
                            else
                            {
                                obj7.CustomSetActive(false);
                                text6.get_gameObject().CustomSetActive(false);
                            }
                            obj3.CustomSetActive(false);
                            text2.set_text("+" + num5.ToString());
                        }
                    }
                    else
                    {
                        this._isBraveScoreIncreased = false;
                        widget.CustomSetActive(false);
                        obj3.CustomSetActive(true);
                        component.set_text(Singleton<CTextManager>.GetInstance().GetText("Ladder_Brave_Guaji_Tip"));
                        text2.set_text(string.Empty);
                    }
                }
            }
        }

        private void ShowBraveScorePanel(bool isSettle)
        {
            if (this._ladderForm == null)
            {
                DebugHelper.Assert(false, "_ladderForm is null!!!");
            }
            else
            {
                GameObject widget = this._ladderForm.GetWidget(4);
                if (isSettle)
                {
                    widget.CustomSetActive(true);
                    this.ShowBraveScoreProcessPanel();
                    this.ShowBraveScoreDetailPanel();
                }
                else
                {
                    widget.CustomSetActive(false);
                }
            }
        }

        private void ShowBraveScoreProcessPanel()
        {
            if (this._ladderForm == null)
            {
                DebugHelper.Assert(false, "_ladderForm is null!!!");
            }
            else
            {
                Image component = this._ladderForm.GetWidget(11).GetComponent<Image>();
                Text text = this._ladderForm.GetWidget(12).GetComponent<Text>();
                Text text2 = this._ladderForm.GetWidget(13).GetComponent<Text>();
                COMDT_RANK_SETTLE_INFO rankInfo = Singleton<BattleStatistic>.GetInstance().rankInfo;
                if (rankInfo == null)
                {
                    DebugHelper.Assert(false, "BattleStatistic.instance.rankInfo is null!!!");
                }
                else
                {
                    uint dwOldAddStarScore = rankInfo.dwOldAddStarScore;
                    uint braveScoreMax = Singleton<CLadderSystem>.GetInstance().GetBraveScoreMax(this._oldGrade);
                    component.set_fillAmount(CLadderView.GetProcessCircleFillAmount((int) dwOldAddStarScore, (int) braveScoreMax));
                    text.set_text(dwOldAddStarScore + "/" + braveScoreMax);
                    string[] args = new string[] { braveScoreMax.ToString() };
                    text2.set_text(Singleton<CTextManager>.GetInstance().GetText("Ladder_Brave_Exchange_Tip", args));
                }
            }
        }

        private void ShowElementAddFriendBtn(bool bShow)
        {
            if (!bShow)
            {
                if (this._leftListScript != null)
                {
                    int elementAmount = this._leftListScript.GetElementAmount();
                    for (int i = 0; i < elementAmount; i++)
                    {
                        SettlementHelper component = this._leftListScript.GetElemenet(i).get_gameObject().GetComponent<SettlementHelper>();
                        component.AddFriendRoot.CustomSetActive(false);
                        component.ReportRoot.CustomSetActive(false);
                        component.DianZanLaHeiRoot.CustomSetActive(false);
                    }
                }
                if (this._rightListScript != null)
                {
                    int num3 = this._rightListScript.GetElementAmount();
                    for (int j = 0; j < num3; j++)
                    {
                        SettlementHelper helper2 = this._rightListScript.GetElemenet(j).get_gameObject().GetComponent<SettlementHelper>();
                        helper2.AddFriendRoot.CustomSetActive(false);
                        helper2.ReportRoot.CustomSetActive(false);
                        helper2.DianZanLaHeiRoot.CustomSetActive(false);
                    }
                }
            }
            else
            {
                if (this._leftListScript != null)
                {
                    int num5 = this._leftListScript.GetElementAmount();
                    for (int k = 0; k < num5; k++)
                    {
                        SettlementHelper helper3 = this._leftListScript.GetElemenet(k).get_gameObject().GetComponent<SettlementHelper>();
                        helper3.AddFriendRoot.CustomSetActive((this._curBtnType == ShowBtnType.AddFriend) && !this._neutral);
                        helper3.ReportRoot.CustomSetActive((this._curBtnType == ShowBtnType.Report) && !this._neutral);
                        helper3.DianZanLaHeiRoot.CustomSetActive((this._curBtnType == ShowBtnType.LaHeiDianZan) && !this._neutral);
                    }
                }
                if (this._rightListScript != null)
                {
                    int num7 = this._rightListScript.GetElementAmount();
                    for (int m = 0; m < num7; m++)
                    {
                        SettlementHelper helper4 = this._rightListScript.GetElemenet(m).get_gameObject().GetComponent<SettlementHelper>();
                        helper4.AddFriendRoot.CustomSetActive((this._curBtnType == ShowBtnType.AddFriend) && !this._neutral);
                        helper4.ReportRoot.CustomSetActive((this._curBtnType == ShowBtnType.Report) && !this._neutral);
                        helper4.DianZanLaHeiRoot.CustomSetActive((this._curBtnType == ShowBtnType.LaHeiDianZan) && !this._neutral);
                    }
                }
            }
        }

        private void ShowLadderResultPanel(bool isSettle)
        {
            if (this._ladderForm == null)
            {
                DebugHelper.Assert(false, "_ladderForm is null!!!");
            }
            else
            {
                GameObject widget = this._ladderForm.GetWidget(14);
                if (isSettle)
                {
                    widget.CustomSetActive(true);
                    uint selfRankClass = Singleton<CRoleInfoManager>.GetInstance().GetSelfRankClass();
                    CLadderView.ShowRankDetail(this._ladderForm.GetWidget(15), (byte) this._oldGrade, selfRankClass, this._oldScore, true, false, false, false, true);
                    CLadderView.ShowRankDetail(this._ladderForm.GetWidget(0x10), (byte) this._newGrade, selfRankClass, this._newScore, true, false, false, false, true);
                    this._ladderForm.GetWidget(0x11).GetComponent<Text>().set_text(this.GetLadderResultDesc());
                }
                else
                {
                    widget.CustomSetActive(false);
                }
            }
        }

        public void ShowLadderSettleForm(bool win)
        {
            if (Singleton<CUIManager>.GetInstance().GetForm(this._ladderFormName) == null)
            {
                this._ladderForm = Singleton<CUIManager>.GetInstance().OpenForm(this._ladderFormName, false, true);
                this._ladderRoot = this._ladderForm.get_gameObject().get_transform().FindChild("Ladder").get_gameObject();
                this._ladderAnimator = this._ladderRoot.GetComponent<Animator>();
                this._lastLadderWin = win;
                Transform transform = this._ladderForm.get_gameObject().get_transform().FindChild("ShareGroup/Btn_Share");
                if (transform != null)
                {
                    transform.get_gameObject().CustomSetActive(false);
                }
                COMDT_RANK_SETTLE_INFO rankInfo = Singleton<BattleStatistic>.GetInstance().rankInfo;
                if (rankInfo != null)
                {
                    this.SetLadderDisplayOldAndNewGrade(rankInfo.bOldGrade, rankInfo.dwOldScore, rankInfo.bNowGrade, rankInfo.dwNowScore);
                    this.SetBpModeOpenTip(rankInfo.bOldGrade, rankInfo.bNowGrade);
                }
                this._isSettle = true;
                this.LadderDisplayProcess();
            }
        }

        public void ShowLadderSettleFormWithoutSettle()
        {
            if (Singleton<CUIManager>.GetInstance().GetForm(this._ladderFormName) == null)
            {
                this._ladderForm = Singleton<CUIManager>.GetInstance().OpenForm(this._ladderFormName, false, true);
                this._ladderRoot = this._ladderForm.get_gameObject().get_transform().FindChild("Ladder").get_gameObject();
                this._ladderAnimator = this._ladderRoot.GetComponent<Animator>();
                this._isSettle = false;
                this.LadderDisplayProcess();
            }
        }

        public void ShowPersonalProfit(bool win)
        {
            if (Singleton<CUIManager>.GetInstance().GetForm(this._profitFormName) == null)
            {
                this._win = win;
                this._profitFormScript = Singleton<CUIManager>.GetInstance().OpenForm(this._profitFormName, false, true);
                if (this._profitFormScript != null)
                {
                    this.SetTitle();
                    this.SetExpProfit();
                    this.SetGoldCoinProfit();
                    this.SetSymbolCoinProfit();
                    this.SetMapInfo();
                    this.SetProficiencyInfo();
                    this.SetGuildInfo();
                    this.SetLadderInfo();
                    this.SetSpecialItem();
                }
            }
        }

        public void ShowSettlementPanel(bool neutralShow = false)
        {
            if (Singleton<CUIManager>.GetInstance().GetForm(SettlementFormName) == null)
            {
                this.ClearSendRedBag();
                if (!neutralShow && Singleton<BattleLogic>.instance.GetCurLvelContext().IsRedBagMode())
                {
                    PlayerKDA hostKDA = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat.GetHostKDA();
                    if (hostKDA != null)
                    {
                        if (this._win)
                        {
                            this.m_bWin = 1;
                        }
                        uint mvpPlayer = Singleton<BattleStatistic>.instance.GetMvpPlayer(hostKDA.PlayerCamp, this._win);
                        if ((mvpPlayer != 0) && (mvpPlayer == hostKDA.PlayerId))
                        {
                            this.m_bMvp = 1;
                        }
                        ListView<HeroKDA>.Enumerator enumerator = hostKDA.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            HeroKDA current = enumerator.Current;
                            if (current != null)
                            {
                                if (current.LegendaryNum > 0)
                                {
                                    this.m_bLegaendary = 1;
                                }
                                if (current.PentaKillNum > 0)
                                {
                                    this.m_bPENTAKILL = 1;
                                }
                                if (current.QuataryKillNum > 0)
                                {
                                    this.m_bQUATARYKIL = 1;
                                }
                                if (current.TripleKillNum > 0)
                                {
                                    this.m_bTRIPLEKILL = 1;
                                }
                            }
                        }
                        this.m_bSendRedBag = true;
                    }
                }
                this._neutral = neutralShow;
                this._settleFormScript = Singleton<CUIManager>.GetInstance().OpenForm(SettlementFormName, false, true);
                this._settleFormScript.m_formWidgets[2].CustomSetActive(true);
                this._settleFormScript.m_formWidgets[0x10].CustomSetActive(true);
                this._settleFormScript.m_formWidgets[1].CustomSetActive(false);
                this._settleFormScript.m_formWidgets[4].CustomSetActive(true);
                this._settleFormScript.m_formWidgets[5].CustomSetActive(true);
                this._settleFormScript.m_formWidgets[6].CustomSetActive(true);
                this._settleFormScript.m_formWidgets[15].GetComponent<Text>().set_text(this._duration);
                this._settleFormScript.m_formWidgets[15].CustomSetActive(true);
                this._settleFormScript.m_formWidgets[0x15].GetComponent<Text>().set_text(this._startTime);
                this._settleFormScript.m_formWidgets[0x15].CustomSetActive(true);
                this._settleFormScript.m_formWidgets[11].CustomSetActive(false);
                this._settleFormScript.m_formWidgets[12].CustomSetActive(false);
                this._settleFormScript.m_formWidgets[20].CustomSetActive(false);
                this._settleFormScript.m_formWidgets[0x13].CustomSetActive(false);
                this._settleFormScript.m_formWidgets[0x12].CustomSetActive(true);
                this._settleFormScript.m_formWidgets[0x11].CustomSetActive(true);
                this._settleFormScript.m_formWidgets[0x16].CustomSetActive(!this._neutral);
                this._settleFormScript.m_formWidgets[0x1b].CustomSetActive(!this._neutral);
                this._settleFormScript.m_formWidgets[9].CustomSetActive(false);
                this._settleFormScript.m_formWidgets[10].CustomSetActive(false);
                this._settleFormScript.m_formWidgets[0x19].CustomSetActive(false);
                this._leftListScript = this._settleFormScript.m_formWidgets[7].GetComponent<CUIListScript>();
                this._rightListScript = this._settleFormScript.m_formWidgets[8].GetComponent<CUIListScript>();
                this.SetSettlementTitle();
                this.SetSettlementButton();
                this.SetPlayerSettlement();
                this.SetCreditSettlement();
                this.SetVictoryTipsBtnInfo(this._settleFormScript);
                GameObject obj2 = this._settleFormScript.m_formWidgets[0x17];
                if (obj2 != null)
                {
                    Singleton<CReplayKitSys>.GetInstance().InitReplayKitRecordBtn(obj2.get_transform());
                }
                if ((this.m_ShareDataBtn != null) && this.m_ShareDataBtn.get_activeSelf())
                {
                    this.InitShareDataBtn(this._settleFormScript);
                }
                uint[] param = new uint[] { !this._win ? 2 : 1, this.playerNum / 2 };
                MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.pvpFin, param);
                this._settleFormScript.m_formWidgets[0x1a].CustomSetActive(false);
            }
        }

        public void SnapScreenShotShowBtn(bool bClose)
        {
            if (this.m_PVPShareDataBtnGroup != null)
            {
                this.m_PVPShareDataBtnGroup.get_gameObject().SetActive(bClose);
            }
            if (this.m_PVPShareBtnClose != null)
            {
                this.m_PVPShareBtnClose.get_gameObject().SetActive(bClose);
            }
        }

        public override void UnInit()
        {
            base.UnInit();
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ProfitContinue, new CUIEventManager.OnUIEventHandler(this.OnClickProfitContinue));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_TimerEnd, new CUIEventManager.OnUIEventHandler(this.OnSettlementTimerEnd));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ClickBack, new CUIEventManager.OnUIEventHandler(this.OnClickBack));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ClickAgain, new CUIEventManager.OnUIEventHandler(this.OnClickBattleAgain));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_SaveReplay, new CUIEventManager.OnUIEventHandler(this.OnClickSaveReplay));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ClickStatistics, new CUIEventManager.OnUIEventHandler(this.OnSwitchStatistics));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ShowReport, new CUIEventManager.OnUIEventHandler(this.OnShowReport));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_CloseReport, new CUIEventManager.OnUIEventHandler(this.OnCloseReport));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_DoReport, new CUIEventManager.OnUIEventHandler(this.OnDoReport));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ClickAddFriend, new CUIEventManager.OnUIEventHandler(this.OnAddFriend));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ClickDianLa, new CUIEventManager.OnUIEventHandler(this.OnClickDianZanLaHei));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_OnCloseProfit, new CUIEventManager.OnUIEventHandler(this.OnCloseProfit));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_OnCloseSettlement, new CUIEventManager.OnUIEventHandler(this.OnCloseSettlement));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_SwitchAddFriendReport, new CUIEventManager.OnUIEventHandler(this.OnSwitchAddFriendReport));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ClickLadderContinue, new CUIEventManager.OnUIEventHandler(this.OnLadderClickContinue));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ClickShowAchievements, new CUIEventManager.OnUIEventHandler(this.OnShowAchievements));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_OpenSharePVPDefeat, new CUIEventManager.OnUIEventHandler(this.OnShowDefeatShare));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ClickItemDisplay, new CUIEventManager.OnUIEventHandler(this.OnClickItemDisplay));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Share_ShowPVPSettleData, new CUIEventManager.OnUIEventHandler(this.OnShowPVPSettleData));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Share_ShowPVPSettleDataClose, new CUIEventManager.OnUIEventHandler(this.OnShowPVPSettleDataClose));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Share_ShowUpdateGradeShare, new CUIEventManager.OnUIEventHandler(this.OnShareUpdateGradShare));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Share_ShowUpdateGradeShareClose, new CUIEventManager.OnUIEventHandler(this.OnShareUpdateGradShareClose));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ShowAddFriendBtn, new CUIEventManager.OnUIEventHandler(this.OnShowFriendBtn));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ShowReportBtn, new CUIEventManager.OnUIEventHandler(this.OnShowReportBtn));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ShowDianLaBtn, new CUIEventManager.OnUIEventHandler(this.OnShowLaHeiDianZanBtn));
        }

        private void UpdateAchievements(GameObject achievements, PlayerKDA kda)
        {
            int index = 1;
            ListView<HeroKDA>.Enumerator enumerator = kda.GetEnumerator();
            while (enumerator.MoveNext())
            {
                HeroKDA current = enumerator.Current;
                if (current != null)
                {
                    bool flag = false;
                    for (int i = 1; i < 13; i++)
                    {
                        switch (((PvpAchievement) i))
                        {
                            case PvpAchievement.Legendary:
                                if (current.LegendaryNum > 0)
                                {
                                    this.SetAchievementIcon(achievements, PvpAchievement.Legendary, index);
                                    index++;
                                }
                                break;

                            case PvpAchievement.PentaKill:
                                if ((current.PentaKillNum > 0) && !flag)
                                {
                                    this.SetAchievementIcon(achievements, PvpAchievement.PentaKill, index);
                                    index++;
                                    flag = true;
                                }
                                break;

                            case PvpAchievement.QuataryKill:
                                if ((current.QuataryKillNum > 0) && !flag)
                                {
                                    this.SetAchievementIcon(achievements, PvpAchievement.QuataryKill, index);
                                    index++;
                                    flag = true;
                                }
                                break;

                            case PvpAchievement.TripleKill:
                                if ((current.TripleKillNum > 0) && !flag)
                                {
                                    this.SetAchievementIcon(achievements, PvpAchievement.TripleKill, index);
                                    index++;
                                    flag = true;
                                }
                                break;

                            case PvpAchievement.DoubleKill:
                                if (current.DoubleKillNum <= 0)
                                {
                                }
                                break;

                            case PvpAchievement.KillMost:
                                if (current.bKillMost)
                                {
                                    this.SetAchievementIcon(achievements, PvpAchievement.KillMost, index);
                                    index++;
                                }
                                break;

                            case PvpAchievement.HurtMost:
                                if (current.bHurtMost)
                                {
                                    this.SetAchievementIcon(achievements, PvpAchievement.HurtMost, index);
                                    index++;
                                }
                                break;

                            case PvpAchievement.HurtTakenMost:
                                if (current.bHurtTakenMost)
                                {
                                    this.SetAchievementIcon(achievements, PvpAchievement.HurtTakenMost, index);
                                    index++;
                                }
                                break;

                            case PvpAchievement.AsssistMost:
                                if (current.bAsssistMost)
                                {
                                    this.SetAchievementIcon(achievements, PvpAchievement.AsssistMost, index);
                                    index++;
                                }
                                break;

                            case PvpAchievement.GetCoinMost:
                                if (current.bGetCoinMost)
                                {
                                    this.SetAchievementIcon(achievements, PvpAchievement.GetCoinMost, index);
                                    index++;
                                }
                                break;

                            case PvpAchievement.KillOrganMost:
                                if (current.bKillOrganMost)
                                {
                                    this.SetAchievementIcon(achievements, PvpAchievement.KillOrganMost, index);
                                    index++;
                                }
                                break;

                            case PvpAchievement.RunAway:
                                if ((kda.bRunaway || kda.bDisconnect) || kda.bHangup)
                                {
                                    this.SetAchievementIcon(achievements, PvpAchievement.RunAway, index);
                                    index++;
                                }
                                break;
                        }
                    }
                    for (int j = index; j <= 8; j++)
                    {
                        this.SetAchievementIcon(achievements, PvpAchievement.NULL, j);
                    }
                    break;
                }
            }
        }

        private void UpdateEquip(GameObject equip, PlayerKDA kda)
        {
            int num = 1;
            ListView<HeroKDA>.Enumerator enumerator = kda.GetEnumerator();
            while (enumerator.MoveNext())
            {
                HeroKDA current = enumerator.Current;
                if (current != null)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        ushort equipID = current.Equips[i].m_equipID;
                        Transform transform = equip.get_transform().FindChild(string.Format("TianFu{0}", num));
                        if ((equipID != 0) && (transform != null))
                        {
                            num++;
                            CUICommonSystem.SetEquipIcon(equipID, transform.get_gameObject(), this._settleFormScript);
                        }
                    }
                    for (int j = num; j <= 6; j++)
                    {
                        Transform transform2 = equip.get_transform().FindChild(string.Format("TianFu{0}", j));
                        if (transform2 != null)
                        {
                            transform2.get_gameObject().GetComponent<Image>().SetSprite(string.Format("{0}BattleSettle_EquipmentSpaceNew", CUIUtility.s_Sprite_Dynamic_Talent_Dir), this._settleFormScript, true, false, false, false);
                        }
                    }
                    break;
                }
            }
        }

        private void UpdatePlayerKda(PlayerKDA kda)
        {
            if (kda == null)
            {
                return;
            }
            CUIListScript script = null;
            int index = 0;
            SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
            if (curLvelContext == null)
            {
                return;
            }
            switch (kda.PlayerCamp)
            {
                case COM_PLAYERCAMP.COM_PLAYERCAMP_1:
                    script = this._leftListScript;
                    index = this._curLeftIndex++;
                    break;

                case COM_PLAYERCAMP.COM_PLAYERCAMP_2:
                    script = this._rightListScript;
                    index = this._curRightIndex++;
                    goto Label_007A;
            }
        Label_007A:
            if (script == null)
            {
                return;
            }
            CUIListElementScript elemenet = script.GetElemenet(index);
            if (elemenet != null)
            {
                SettlementHelper component = elemenet.get_gameObject().GetComponent<SettlementHelper>();
                this.UpdateEquip(component.Tianfu, kda);
                this.UpdateAchievements(component.Achievements, kda);
                bool flag = kda.PlayerCamp == Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp;
                bool bWin = (this._win && flag) || (!this._win && !flag);
                float score = 0f;
                if (!Singleton<BattleStatistic>.instance.GetServerMvpScore(kda.PlayerId, out score))
                {
                    score = !bWin ? (kda.MvpValue * (((float) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x128).dwConfValue) / 100f)) : kda.MvpValue;
                }
                component.mvpNode.CustomSetActive(!this._neutral);
                if (bWin)
                {
                    ResGlobalInfo info = null;
                    int dwConfValue = 1;
                    int num4 = 0;
                    if (GameDataMgr.svr2CltCfgDict.TryGetValue(0x19, out info))
                    {
                        dwConfValue = (int) info.dwConfValue;
                    }
                    if (GameDataMgr.svr2CltCfgDict.TryGetValue(0x1a, out info))
                    {
                        num4 = (int) info.dwConfValue;
                    }
                    score = (score * dwConfValue) + num4;
                }
                else
                {
                    ResGlobalInfo info2 = null;
                    int num5 = 1;
                    int num6 = 0;
                    if (GameDataMgr.svr2CltCfgDict.TryGetValue(0x1b, out info2))
                    {
                        num5 = (int) info2.dwConfValue;
                    }
                    if (GameDataMgr.svr2CltCfgDict.TryGetValue(0x1c, out info2))
                    {
                        num6 = (int) info2.dwConfValue;
                    }
                    score = (score * num5) + num6;
                }
                if (this.playerNum > 2)
                {
                    if (kda.PlayerId == Singleton<BattleStatistic>.instance.GetMvpPlayer(kda.PlayerCamp, bWin))
                    {
                        if (bWin)
                        {
                            component.WinMvpIcon.SetActive(true);
                            component.WinMvpTxt.get_gameObject().SetActive(true);
                            component.LoseMvpIcon.SetActive(false);
                            component.LoseMvpTxt.get_gameObject().SetActive(false);
                            component.NormalMvpIcon.SetActive(false);
                            component.NormalMvpTxt.get_gameObject().SetActive(false);
                            component.WinMvpTxt.set_text(Math.Max(score, 0f).ToString("F1"));
                        }
                        else
                        {
                            component.WinMvpIcon.SetActive(false);
                            component.WinMvpTxt.get_gameObject().SetActive(false);
                            component.LoseMvpIcon.SetActive(true);
                            component.LoseMvpTxt.get_gameObject().SetActive(true);
                            component.NormalMvpIcon.SetActive(false);
                            component.NormalMvpTxt.get_gameObject().SetActive(false);
                            component.LoseMvpTxt.set_text(Math.Max(score, 0f).ToString("F1"));
                        }
                    }
                    else
                    {
                        component.WinMvpIcon.SetActive(false);
                        component.WinMvpTxt.get_gameObject().SetActive(false);
                        component.LoseMvpIcon.SetActive(false);
                        component.LoseMvpTxt.get_gameObject().SetActive(false);
                        component.NormalMvpIcon.SetActive(true);
                        component.NormalMvpTxt.get_gameObject().SetActive(true);
                        component.NormalMvpTxt.set_text(Math.Max(score, 0f).ToString("F1"));
                    }
                }
                else
                {
                    component.WinMvpIcon.SetActive(false);
                    component.WinMvpTxt.get_gameObject().SetActive(false);
                    component.LoseMvpIcon.SetActive(false);
                    component.LoseMvpTxt.get_gameObject().SetActive(false);
                    component.NormalMvpIcon.SetActive(false);
                    component.NormalMvpTxt.get_gameObject().SetActive(false);
                }
                component.PlayerName.GetComponent<Text>().set_text(kda.PlayerName);
                component.PlayerLv.CustomSetActive(false);
                if (kda.PlayerId == Singleton<GamePlayerCenter>.GetInstance().HostPlayerId)
                {
                    component.PlayerName.GetComponent<Text>().set_color(CUIUtility.s_Text_Color_Self);
                    component.PlayerLv.GetComponent<Text>().set_color(CUIUtility.s_Text_Color_Self);
                    component.ItsMe.CustomSetActive(true);
                    MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component.HeroNobe.GetComponent<Image>(), (int) Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwCurLevel, false);
                }
                else
                {
                    component.ItsMe.CustomSetActive(false);
                    MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component.HeroNobe.GetComponent<Image>(), (int) kda.PlayerVipLv, false);
                }
                if (((kda.PlayerId == Singleton<GamePlayerCenter>.GetInstance().HostPlayerId) || Singleton<CFriendContoller>.instance.model.IsGameFriend(kda.PlayerUid, (uint) kda.WorldId)) || (kda.IsComputer && !curLvelContext.m_isWarmBattle))
                {
                    component.AddFriend.CustomSetActive(false);
                    component.Report.CustomSetActive(false);
                    component.m_AddfriendBtnShow = false;
                    component.m_ReportRootBtnShow = false;
                }
                else
                {
                    component.AddFriend.CustomSetActive(true);
                    component.Report.CustomSetActive(true);
                    component.m_AddfriendBtnShow = true;
                    component.m_ReportRootBtnShow = true;
                    component.AddFriend.GetComponent<CUIEventScript>().m_onClickEventParams.commonUInt64Param1 = kda.PlayerUid;
                    component.AddFriend.GetComponent<CUIEventScript>().m_onClickEventParams.commonUInt64Param2 = (ulong) kda.WorldId;
                    component.Report.GetComponent<CUIEventScript>().m_onClickEventParams.commonUInt64Param1 = kda.PlayerUid;
                    component.Report.GetComponent<CUIEventScript>().m_onClickEventParams.commonUInt64Param2 = (ulong) kda.WorldId;
                }
                if (((kda.PlayerId != Singleton<GamePlayerCenter>.GetInstance().HostPlayerId) && (!kda.IsComputer || curLvelContext.m_isWarmBattle)) && ((this.playerNum >= 6) && !curLvelContext.IsGameTypePvpRoom()))
                {
                    component.DianZanLaHei.CustomSetActive(true);
                    component.DianZanLaHei.GetComponent<CUIEventScript>().m_onClickEventParams.commonUInt64Param1 = kda.PlayerUid;
                    component.DianZanLaHei.GetComponent<CUIEventScript>().m_onClickEventParams.commonUInt64Param2 = (ulong) kda.WorldId;
                    component.DianZanLaHei.GetComponent<CUIEventScript>().m_onClickEventParams.commonUInt32Param1 = (uint) kda.PlayerCamp;
                    component.DianZanLaHei.GetComponent<CUIEventScript>().m_onClickEventParams.commonUInt16Param1 = (ushort) Singleton<GamePlayerCenter>.GetInstance().hostPlayerCamp;
                }
                else
                {
                    component.DianZanLaHei.CustomSetActive(false);
                }
                component.AddFriendRoot.CustomSetActive(!this._neutral);
                component.ReportRoot.CustomSetActive(false);
                component.DianZanLaHeiRoot.CustomSetActive(false);
                component.m_AddfriendBtnShow = true;
                component.m_ReportRootBtnShow = false;
                ListView<HeroKDA>.Enumerator enumerator = kda.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    HeroKDA current = enumerator.Current;
                    if (current != null)
                    {
                        component.HeroIcon.GetComponent<Image>().SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, CSkinInfo.GetHeroSkinPic((uint) current.HeroId, 0)), this._settleFormScript, true, false, false, false);
                        if (!Utility.IsSelf(kda.PlayerUid, kda.WorldId))
                        {
                            CUIEventScript script3 = component.HeroIcon.GetComponent<CUIEventScript>();
                            if (script3 != null)
                            {
                                script3.m_onClickEventParams.commonUInt64Param1 = kda.PlayerUid;
                                script3.m_onClickEventParams.tag = kda.WorldId;
                            }
                        }
                        component.HeroLv.GetComponent<Text>().set_text(string.Format("{0}", current.SoulLevel));
                        component.Kill.GetComponent<Text>().set_text(current.numKill.ToString(CultureInfo.InvariantCulture));
                        component.Death.GetComponent<Text>().set_text(current.numDead.ToString(CultureInfo.InvariantCulture));
                        component.Assist.GetComponent<Text>().set_text(current.numAssist.ToString(CultureInfo.InvariantCulture));
                        component.Coin.GetComponent<Text>().set_text(current.TotalCoin.ToString(CultureInfo.InvariantCulture));
                        uint num7 = 0;
                        uint num8 = 0;
                        uint num9 = 0;
                        if (kda.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
                        {
                            num7 = this._camp1TotalDamage;
                            num8 = this._camp1TotalTakenDamage;
                            num9 = this._camp1TotalToHeroDamage;
                        }
                        else
                        {
                            num7 = this._camp2TotalDamage;
                            num8 = this._camp2TotalTakenDamage;
                            num9 = this._camp2TotalToHeroDamage;
                        }
                        num7 = Math.Max(1, num7);
                        num8 = Math.Max(1, num8);
                        num9 = Math.Max(1, num9);
                        component.Damage.get_transform().FindChild("TotalDamageBg/TotalDamage").get_gameObject().GetComponent<Text>().set_text(current.hurtToEnemy.ToString(CultureInfo.InvariantCulture));
                        component.Damage.get_transform().FindChild("TotalDamageBg/TotalDamageBar").get_gameObject().GetComponent<Image>().set_fillAmount(((float) current.hurtToEnemy) / ((float) num7));
                        component.Damage.get_transform().FindChild("TotalDamageBg/Percent").get_gameObject().GetComponent<Text>().set_text(string.Format("{0:P1}", ((float) current.hurtToEnemy) / ((float) num7)));
                        component.Damage.get_transform().FindChild("TotalTakenDamageBg/TotalTakenDamage").get_gameObject().GetComponent<Text>().set_text(current.hurtTakenByEnemy.ToString(CultureInfo.InvariantCulture));
                        component.Damage.get_transform().FindChild("TotalTakenDamageBg/TotalTakenDamageBar").get_gameObject().GetComponent<Image>().set_fillAmount(((float) current.hurtTakenByEnemy) / ((float) num8));
                        component.Damage.get_transform().FindChild("TotalTakenDamageBg/Percent").get_gameObject().GetComponent<Text>().set_text(string.Format("{0:P1}", ((float) current.hurtTakenByEnemy) / ((float) num8)));
                        component.Damage.get_transform().FindChild("TotalDamageHeroBg/TotalDamageHero").get_gameObject().GetComponent<Text>().set_text(current.hurtToHero.ToString(CultureInfo.InvariantCulture));
                        component.Damage.get_transform().FindChild("TotalDamageHeroBg/TotalDamageHeroBar").get_gameObject().GetComponent<Image>().set_fillAmount(((float) current.hurtToHero) / ((float) num9));
                        component.Damage.get_transform().FindChild("TotalDamageHeroBg/Percent").get_gameObject().GetComponent<Text>().set_text(string.Format("{0:P1}", ((float) current.hurtToHero) / ((float) num9)));
                        component.AddFriend.GetComponent<CUIEventScript>().m_onClickEventParams.tag = this.HostPlayerHeroId;
                        break;
                    }
                }
                component.Detail.CustomSetActive(true);
                component.Damage.CustomSetActive(false);
            }
        }

        private void UpdateSharePVPDataCaption(bool isDetail)
        {
            if (!CSysDynamicBlock.bSocialBlocked && !this._neutral)
            {
                if (!isDetail)
                {
                    if (this.m_TxtBtnShareCaption != null)
                    {
                        this.m_TxtBtnShareCaption.set_text("分享数据");
                    }
                    if (this.m_ShareDataBtn != null)
                    {
                        this.m_ShareDataBtn.CustomSetActive(true);
                    }
                }
                else
                {
                    if (this.m_TxtBtnShareCaption != null)
                    {
                        this.m_TxtBtnShareCaption.set_text("分享战绩");
                    }
                    if (this.m_ShareDataBtn != null)
                    {
                        this.m_ShareDataBtn.CustomSetActive(true);
                    }
                }
                this.m_bIsDetail = !isDetail;
            }
        }

        private void UpdateTimeBtnState(bool bShow)
        {
            if (this.m_BtnTimeLine != null)
            {
                this.m_BtnTimeLine.GetComponent<CUIEventScript>().set_enabled(bShow);
                this.m_BtnTimeLine.GetComponent<Button>().set_interactable(bShow);
                float num = 0.37f;
                if (bShow)
                {
                    num = 1f;
                }
                this.m_BtnTimeLine.GetComponent<Image>().set_color(new Color(this.m_BtnTimeLine.GetComponent<Image>().get_color().r, this.m_BtnTimeLine.GetComponent<Image>().get_color().g, this.m_BtnTimeLine.GetComponent<Image>().get_color().b, num));
                if (this.m_timeLineText != null)
                {
                    this.m_timeLineText.set_color(new Color(this.m_timeLineText.get_color().r, this.m_timeLineText.get_color().g, this.m_timeLineText.get_color().b, num));
                }
            }
        }

        public int HostPlayerHeroId
        {
            [CompilerGenerated]
            get
            {
                return this.<HostPlayerHeroId>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<HostPlayerHeroId>k__BackingField = value;
            }
        }

        protected enum ProfitWidgets
        {
            CoinInfo = 2,
            ExpInfo = 1,
            ExpThisWeek = 8,
            GoldThisWeek = 9,
            GuildInfo = 4,
            GuildPointMaxTip = 7,
            LadderInfo = 5,
            None = -1,
            ProficiencyInfo = 3,
            PvpMapInfo = 6,
            SymbolCoinInfo = 10,
            WinLoseTitle = 0
        }

        public enum SettlementWidgets
        {
            AddFriendBtn = 9,
            BtnVictoryTips = 0x18,
            ButtonGrid = 1,
            CreditScore = 0x16,
            DamageBtn = 12,
            DamageCaption = 14,
            DetailBtn = 11,
            DetailCaption = 13,
            DianZanLaHeiBtn = 0x19,
            Duration = 15,
            LeftDamageTitle = 0x13,
            LeftOverViewTitle = 0x11,
            LeftPlayers = 5,
            LeftPlayersList = 7,
            MaxNum = 0x1c,
            None = -1,
            Recorder = 0x1a,
            ReplayKitRecord = 0x17,
            Report = 3,
            ReportBtn = 10,
            RightDamageTitle = 20,
            RightOverViewTitle = 0x12,
            RightPlayers = 6,
            RightPlayersList = 8,
            StartTime = 0x15,
            TimeNode = 0x1b,
            Timer = 2,
            TotalScore = 4,
            WaitNote = 0x10,
            WinLoseTitle = 0
        }

        private enum ShowBtnType
        {
            AddFriend,
            Report,
            LaHeiDianZan
        }
    }
}

