namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [MessageHandlerClass]
    public class CPlayerPvpInfoController : Singleton<CPlayerPvpInfoController>
    {
        private float[] m_ability = new float[6];
        private uint m_baseValueAlive;
        private uint m_baseValueBattle;
        private uint m_baseValueGold;
        private uint m_baseValueHurtHero;
        private uint m_baseValueKDA;
        private bool m_initedBaseValue;
        private static string sCacheSubTabStr = "sgame_{0}_playerinfo_pvpinfo_detailmode";
        private static string[] sMainTitles = new string[2];
        private static string[] sSubTitles = new string[7];
        private static Vector3[] sTemVector;

        private static void ConvertAbilityData(CPlayerProfile profile, MainTab mainTab, SubTab subTab, float[] ability)
        {
            for (int i = 0; i < ability.Length; i++)
            {
                ability[i] = 0f;
            }
            CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO stTotal = null;
            if (profile.pvpAbilityDetail != null)
            {
                if (mainTab == MainTab.MatchAll)
                {
                    if (subTab == SubTab.MatchAll)
                    {
                        stTotal = profile.pvpAbilityDetail.stTotal;
                    }
                    else if (subTab == SubTab.Match5V5)
                    {
                        stTotal = profile.pvpAbilityDetail.stTotal5v5;
                    }
                    else if (subTab == SubTab.MatchRank)
                    {
                        stTotal = profile.pvpAbilityDetail.stTotalLadder;
                    }
                    else if (subTab == SubTab.MatchGuild)
                    {
                        stTotal = profile.pvpAbilityDetail.stTotalGuild;
                    }
                }
                else if (mainTab == MainTab.Match100)
                {
                    if (subTab == SubTab.MatchAll)
                    {
                        stTotal = profile.pvpAbilityDetail.stRecentTotal;
                    }
                    else if (subTab == SubTab.Match5V5)
                    {
                        stTotal = profile.pvpAbilityDetail.stRecent5v5;
                    }
                    else if (subTab == SubTab.MatchRank)
                    {
                        stTotal = profile.pvpAbilityDetail.stRecentLadder;
                    }
                    else if (subTab == SubTab.MatchGuild)
                    {
                        stTotal = profile.pvpAbilityDetail.stRecentGuild;
                    }
                }
                if (stTotal != null)
                {
                    ability[0] = ((float) stTotal.dwKDA) / 100f;
                    ability[1] = ((float) stTotal.dwTotal) / 100f;
                    ability[2] = ((float) stTotal.dwHurtHero) / 100f;
                    ability[3] = ((float) stTotal.dwGrow) / 100f;
                    ability[4] = ((float) stTotal.dwBattle) / 100f;
                    ability[5] = ((float) stTotal.dwSurvive) / 100f;
                }
            }
        }

        private static void ConvertAverageData(CPlayerProfile profile, MainTab mainTab, SubTab subTab, out float kda, out float joinFight, out int totalCoinGet, out int totalHurtToEnemy, out int totalBeHurt, out int totalHurtToOrgan, out float totalDead)
        {
            kda = 0f;
            joinFight = 0f;
            totalCoinGet = 0;
            totalHurtToEnemy = 0;
            totalBeHurt = 0;
            totalHurtToOrgan = 0;
            totalDead = 0f;
            if (mainTab == MainTab.MatchAll)
            {
                if (((subTab == SubTab.Match5V5) || (subTab == SubTab.MatchRank)) || (subTab == SubTab.MatchGuild))
                {
                    COMDT_HERO_STATISTIC_INFO stLadder = null;
                    if (subTab == SubTab.Match5V5)
                    {
                        stLadder = profile.pvpExtraDetail.st5v5;
                    }
                    else if (subTab == SubTab.MatchRank)
                    {
                        stLadder = profile.pvpExtraDetail.stLadder;
                    }
                    else if (subTab == SubTab.MatchGuild)
                    {
                        stLadder = profile.pvpExtraDetail.stGuildMatch;
                    }
                    uint b = stLadder.dwWinNum + stLadder.dwLoseNum;
                    kda = CPlayerProfile.Divide(stLadder.ullKDAPct, b) / 100f;
                    joinFight = CPlayerProfile.Divide(stLadder.dwBattleRatioPct, b) / 100f;
                    totalCoinGet = (int) CPlayerProfile.Divide(stLadder.dwGPM, b);
                    totalHurtToEnemy = (int) CPlayerProfile.Divide(stLadder.dwHurtPM, b);
                    totalBeHurt = (int) CPlayerProfile.Divide(stLadder.ullTotalBeHurt, b);
                    totalHurtToOrgan = (int) CPlayerProfile.Divide(stLadder.ullTotalHurtOrgan, b);
                    totalDead = CPlayerProfile.Divide(stLadder.dwDead, b);
                }
                else if (subTab == SubTab.MatchAll)
                {
                    uint num2 = profile.pvpExtraDetail.st5v5.dwWinNum + profile.pvpExtraDetail.st5v5.dwLoseNum;
                    ulong ullKDAPct = profile.pvpExtraDetail.st5v5.ullKDAPct;
                    uint dwBattleRatioPct = profile.pvpExtraDetail.st5v5.dwBattleRatioPct;
                    uint dwGPM = profile.pvpExtraDetail.st5v5.dwGPM;
                    uint dwHurtPM = profile.pvpExtraDetail.st5v5.dwHurtPM;
                    ulong ullTotalBeHurt = profile.pvpExtraDetail.st5v5.ullTotalBeHurt;
                    ulong ullTotalHurtOrgan = profile.pvpExtraDetail.st5v5.ullTotalHurtOrgan;
                    uint dwDead = profile.pvpExtraDetail.st5v5.dwDead;
                    uint num10 = profile.pvpExtraDetail.stLadder.dwWinNum + profile.pvpExtraDetail.stLadder.dwLoseNum;
                    ulong num11 = profile.pvpExtraDetail.stLadder.ullKDAPct;
                    uint num12 = profile.pvpExtraDetail.stLadder.dwBattleRatioPct;
                    uint num13 = profile.pvpExtraDetail.stLadder.dwGPM;
                    uint num14 = profile.pvpExtraDetail.stLadder.dwHurtPM;
                    ulong num15 = profile.pvpExtraDetail.stLadder.ullTotalBeHurt;
                    ulong num16 = profile.pvpExtraDetail.stLadder.ullTotalHurtOrgan;
                    uint num17 = profile.pvpExtraDetail.stLadder.dwDead;
                    uint num18 = profile.pvpExtraDetail.stGuildMatch.dwWinNum + profile.pvpExtraDetail.stGuildMatch.dwLoseNum;
                    ulong num19 = profile.pvpExtraDetail.stGuildMatch.ullKDAPct;
                    uint num20 = profile.pvpExtraDetail.stGuildMatch.dwBattleRatioPct;
                    uint num21 = profile.pvpExtraDetail.stGuildMatch.dwGPM;
                    uint num22 = profile.pvpExtraDetail.stGuildMatch.dwHurtPM;
                    ulong num23 = profile.pvpExtraDetail.stGuildMatch.ullTotalBeHurt;
                    ulong num24 = profile.pvpExtraDetail.stGuildMatch.ullTotalHurtOrgan;
                    uint num25 = profile.pvpExtraDetail.stGuildMatch.dwDead;
                    uint num26 = (num2 + num10) + num18;
                    if (num26 == 0)
                    {
                        num26 = 1;
                    }
                    kda = CPlayerProfile.Divide((ulong) ((ullKDAPct + num11) + num19), num26) / 100f;
                    joinFight = CPlayerProfile.Divide((uint) ((dwBattleRatioPct + num12) + num20), num26) / 100f;
                    totalCoinGet = (int) (((dwGPM + num13) + num21) / num26);
                    totalHurtToEnemy = (int) (((dwHurtPM + num14) + num22) / num26);
                    totalBeHurt = (int) (((ullTotalBeHurt + num15) + num23) / ((ulong) num26));
                    totalHurtToOrgan = (int) (((ullTotalHurtOrgan + num16) + num24) / ((ulong) num26));
                    totalDead = CPlayerProfile.Divide((uint) ((dwDead + num17) + num25), num26);
                }
            }
            else if (mainTab == MainTab.Match100)
            {
                uint num27 = 0;
                uint num28 = 0;
                byte gameType = 0;
                byte acntNum = 0;
                GetGameTypeAcntNum(subTab, out gameType, out acntNum);
                ulong a = 0L;
                ulong num32 = 0L;
                ulong num33 = 0L;
                ulong num34 = 0L;
                ulong num35 = 0L;
                ulong num36 = 0L;
                ulong num37 = 0L;
                for (int i = 0; i < profile.pvpExtraDetail.dwRecentNum; i++)
                {
                    if ((((gameType == 0) || ((profile.pvpExtraDetail.astRecentDetail[i].bGameType == gameType) && (acntNum == profile.pvpExtraDetail.astRecentDetail[i].bMapAcntNum))) && ((profile.pvpExtraDetail.astRecentDetail[i].bGameType != 5) || (profile.pvpExtraDetail.astRecentDetail[i].bMapAcntNum >= 10))) && (profile.pvpExtraDetail.astRecentDetail[i].bGameType != 9))
                    {
                        a += profile.pvpExtraDetail.astRecentDetail[i].ullKDAPct;
                        num32 += profile.pvpExtraDetail.astRecentDetail[i].dwBattleRatioPct;
                        num28 += profile.pvpExtraDetail.astRecentDetail[i].dwCampKill;
                        num33 += profile.pvpExtraDetail.astRecentDetail[i].dwGPM;
                        num34 += profile.pvpExtraDetail.astRecentDetail[i].dwHurtPM;
                        num35 += profile.pvpExtraDetail.astRecentDetail[i].ullTotalBeHurt;
                        num36 += profile.pvpExtraDetail.astRecentDetail[i].ullTotalHurtOrgan;
                        num37 += profile.pvpExtraDetail.astRecentDetail[i].dwDead;
                        num27++;
                    }
                }
                kda = CPlayerProfile.Divide(a, num27) / 100f;
                joinFight = CPlayerProfile.Divide(num32, num27) / 100f;
                totalCoinGet = (int) CPlayerProfile.Divide(num33, num27);
                totalHurtToEnemy = (int) CPlayerProfile.Divide(num34, num27);
                totalBeHurt = (int) CPlayerProfile.Divide(num35, num27);
                totalHurtToOrgan = (int) CPlayerProfile.Divide(num36, num27);
                totalDead = CPlayerProfile.Divide(num37, num27);
            }
        }

        private static void ConvertBaseData(CPlayerProfile profile, MainTab mainTab, SubTab subTab, out uint winMvp, out uint loseMvp, out uint godLike, out uint tripleKill, out uint quataryKill, out uint pentaKill)
        {
            winMvp = 0;
            loseMvp = 0;
            godLike = 0;
            tripleKill = 0;
            quataryKill = 0;
            pentaKill = 0;
            if (mainTab == MainTab.MatchAll)
            {
                if (subTab == SubTab.MatchAll)
                {
                    winMvp = profile.MVPCnt();
                    loseMvp = profile.LoseSoulCnt();
                    godLike = profile.HolyShit();
                    tripleKill = profile.TripleKill();
                    quataryKill = profile.QuataryKill();
                    pentaKill = profile.PentaKill();
                }
                else
                {
                    COMDT_HERO_STATISTIC_INFO stLadder = null;
                    if (subTab == SubTab.Match5V5)
                    {
                        stLadder = profile.pvpExtraDetail.st5v5;
                    }
                    else if (subTab == SubTab.MatchRank)
                    {
                        stLadder = profile.pvpExtraDetail.stLadder;
                    }
                    else if (subTab == SubTab.MatchGuild)
                    {
                        stLadder = profile.pvpExtraDetail.stGuildMatch;
                    }
                    else if (subTab == SubTab.Match3V3)
                    {
                        stLadder = profile.pvpExtraDetail.st3v3;
                    }
                    else if (subTab == SubTab.Match1V1)
                    {
                        stLadder = profile.pvpExtraDetail.st1v1;
                    }
                    else if (subTab == SubTab.MatchEntertainment)
                    {
                        stLadder = profile.pvpExtraDetail.stEntertainment;
                    }
                    winMvp = stLadder.dwMvp;
                    loseMvp = stLadder.dwLoseSoul;
                    godLike = stLadder.dwGodLike;
                    tripleKill = stLadder.dwTripleKill;
                    quataryKill = stLadder.dwUltraKill;
                    pentaKill = stLadder.dwRampage;
                }
            }
            else if (mainTab == MainTab.Match100)
            {
                byte gameType = 0;
                byte acntNum = 0;
                GetGameTypeAcntNum(subTab, out gameType, out acntNum);
                for (int i = 0; i < profile.pvpExtraDetail.dwRecentNum; i++)
                {
                    if ((gameType == 0) || ((profile.pvpExtraDetail.astRecentDetail[i].bGameType == gameType) && (profile.pvpExtraDetail.astRecentDetail[i].bMapAcntNum == acntNum)))
                    {
                        winMvp += profile.pvpExtraDetail.astRecentDetail[i].dwMvp;
                        loseMvp += profile.pvpExtraDetail.astRecentDetail[i].dwLoseSoul;
                        godLike += profile.pvpExtraDetail.astRecentDetail[i].dwGodLike;
                        tripleKill += profile.pvpExtraDetail.astRecentDetail[i].dwTripleKill;
                        quataryKill += profile.pvpExtraDetail.astRecentDetail[i].dwUltraKill;
                        pentaKill += profile.pvpExtraDetail.astRecentDetail[i].dwRampage;
                    }
                }
            }
        }

        private static void ConvertComplexData(CPlayerProfile profile, MainTab mainTab, SubTab subTab, out int gameCnt, out float gameWins)
        {
            int num = 0;
            gameCnt = 0;
            gameWins = 0f;
            if (mainTab == MainTab.MatchAll)
            {
                if (subTab == SubTab.MatchAll)
                {
                    gameCnt = ((((profile.Pvp1V1TotalGameCnt() + profile.Pvp3V3TotalGameCnt()) + profile.Pvp5V5TotalGameCnt()) + profile.EntertainmentTotalGameCnt()) + profile.PvpGuildTotalGameCnt()) + profile.RankTotalGameCnt();
                    num = ((((profile.Pvp1V1WinGameCnt() + profile.Pvp3V3WinGameCnt()) + profile.Pvp5V5WinGameCnt()) + profile.EntertainmentWinGameCnt()) + profile.PvpGuildWinGameCnt()) + profile.RankWinGameCnt();
                    gameWins = CPlayerProfile.Divide((uint) num, (uint) gameCnt);
                }
                else if (subTab == SubTab.Match1V1)
                {
                    gameCnt = profile.Pvp1V1TotalGameCnt();
                    gameWins = profile.Pvp1V1Wins();
                }
                else if (subTab == SubTab.Match3V3)
                {
                    gameCnt = profile.Pvp3V3TotalGameCnt();
                    gameWins = profile.Pvp3V3Wins();
                }
                else if (subTab == SubTab.Match5V5)
                {
                    gameCnt = profile.Pvp5V5TotalGameCnt();
                    gameWins = profile.Pvp5V5Wins();
                }
                else if (subTab == SubTab.MatchEntertainment)
                {
                    gameCnt = profile.EntertainmentTotalGameCnt();
                    gameWins = profile.EntertainmentWins();
                }
                else if (subTab == SubTab.MatchGuild)
                {
                    gameCnt = profile.PvpGuildTotalGameCnt();
                    gameWins = profile.PvpGuildWins();
                }
                else if (subTab == SubTab.MatchRank)
                {
                    gameCnt = profile.RankTotalGameCnt();
                    gameWins = profile.RankWins();
                }
            }
            else if (mainTab == MainTab.Match100)
            {
                byte gameType = 0;
                byte acntNum = 0;
                GetGameTypeAcntNum(subTab, out gameType, out acntNum);
                for (int i = 0; i < profile.pvpExtraDetail.dwRecentNum; i++)
                {
                    if ((gameType == 0) || ((profile.pvpExtraDetail.astRecentDetail[i].bGameType == gameType) && (profile.pvpExtraDetail.astRecentDetail[i].bMapAcntNum == acntNum)))
                    {
                        num += (int) profile.pvpExtraDetail.astRecentDetail[i].dwWinNum;
                        gameCnt++;
                    }
                }
                gameWins = CPlayerProfile.Divide((uint) num, (uint) gameCnt);
            }
        }

        private static void GetGameTypeAcntNum(SubTab subTab, out byte gameType, out byte acntNum)
        {
            gameType = 0xff;
            acntNum = 0;
            if (subTab == SubTab.MatchAll)
            {
                gameType = 0;
                acntNum = 0;
            }
            else if (subTab == SubTab.Match5V5)
            {
                gameType = 5;
                acntNum = 10;
            }
            else if (subTab == SubTab.MatchRank)
            {
                gameType = 4;
                acntNum = 10;
            }
            else if (subTab == SubTab.MatchGuild)
            {
                gameType = 11;
                acntNum = 10;
            }
            else if (subTab == SubTab.Match3V3)
            {
                gameType = 5;
                acntNum = 6;
            }
            else if (subTab == SubTab.Match1V1)
            {
                gameType = 5;
                acntNum = 2;
            }
            else if (subTab == SubTab.MatchEntertainment)
            {
                gameType = 9;
                acntNum = 10;
            }
        }

        public override void Init()
        {
            sMainTitles = new string[] { Singleton<CTextManager>.instance.GetText("Player_Info_Title_1"), Singleton<CTextManager>.instance.GetText("Player_Info_Title_2") };
            sSubTitles = new string[] { Singleton<CTextManager>.instance.GetText("Player_Info_Title_3"), Singleton<CTextManager>.instance.GetText("Player_Info_Title_4"), Singleton<CTextManager>.instance.GetText("Player_Info_Title_5"), Singleton<CTextManager>.instance.GetText("Player_Info_Title_6"), Singleton<CTextManager>.instance.GetText("Player_Info_Title_7"), Singleton<CTextManager>.instance.GetText("Player_Info_Title_8"), Singleton<CTextManager>.instance.GetText("Player_Info_Title_9") };
            Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Player_Info_Pvp_MainList_Click, new CUIEventManager.OnUIEventHandler(this.OnMainListClick));
            Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Player_Info_Pvp_SubList_Click, new CUIEventManager.OnUIEventHandler(this.OnSubListClick));
            Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Player_Info_Pvp_SubList_Show, new CUIEventManager.OnUIEventHandler(this.OnSubListShow));
            Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Player_Info_Pvp_Graph_Show, new CUIEventManager.OnUIEventHandler(this.OnGraphShow));
            Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Player_Info_Pvp_Detail_Show, new CUIEventManager.OnUIEventHandler(this.OnDetailShow));
        }

        private void InitBaseValue()
        {
            if (!this.m_initedBaseValue)
            {
                ResGlobalInfo info = null;
                GameDataMgr.svr2CltCfgDict.TryGetValue(0x10, out info);
                if (info != null)
                {
                    this.m_baseValueKDA = info.dwConfValue;
                    GameDataMgr.svr2CltCfgDict.TryGetValue(0x11, out info);
                    if (info != null)
                    {
                        this.m_baseValueGold = info.dwConfValue;
                        GameDataMgr.svr2CltCfgDict.TryGetValue(0x12, out info);
                        if (info != null)
                        {
                            this.m_baseValueBattle = info.dwConfValue;
                            GameDataMgr.svr2CltCfgDict.TryGetValue(0x13, out info);
                            if (info != null)
                            {
                                this.m_baseValueAlive = info.dwConfValue;
                                GameDataMgr.svr2CltCfgDict.TryGetValue(20, out info);
                                if (info != null)
                                {
                                    this.m_baseValueHurtHero = info.dwConfValue;
                                    this.m_initedBaseValue = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void InitTemVector()
        {
            if (sTemVector == null)
            {
                CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CPlayerInfoSystem.sPlayerInfoFormPath);
                if (form != null)
                {
                    GameObject widget = form.GetWidget(2);
                    sTemVector = new Vector3[7];
                    GameObject obj3 = null;
                    for (int i = 0; i <= 6; i++)
                    {
                        obj3 = Utility.FindChild(widget, string.Format("panelLeftGraph/Content/t{0}", i));
                        sTemVector[i] = obj3.get_transform().get_localPosition();
                    }
                }
            }
        }

        public void InitUI()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CPlayerInfoSystem.sPlayerInfoFormPath);
            if (form != null)
            {
                GameObject widget = form.GetWidget(2);
                CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(widget, "panelTop/MenuList");
                CUIListScript script3 = Utility.GetComponetInChild<CUIListScript>(widget, "panelTop/DropList/List");
                CUICommonSystem.InitMenuPanel(componetInChild.get_gameObject(), sMainTitles, 0, true);
                CUICommonSystem.InitMenuPanel(script3.get_gameObject(), sSubTitles, CacheSubTab, true);
                Utility.FindChild(widget, "btnGraph").CustomSetActive(false);
                Utility.FindChild(widget, "btnDetail").CustomSetActive(true);
            }
        }

        public void OnDetailShow(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CPlayerInfoSystem.sPlayerInfoFormPath);
            if (form != null)
            {
                GameObject widget = form.GetWidget(2);
                Utility.FindChild(widget, "btnGraph").CustomSetActive(true);
                Utility.FindChild(widget, "btnDetail").CustomSetActive(false);
                this.UpdateUI();
            }
        }

        public void OnGraphShow(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CPlayerInfoSystem.sPlayerInfoFormPath);
            if (form != null)
            {
                GameObject widget = form.GetWidget(2);
                Utility.FindChild(widget, "btnGraph").CustomSetActive(false);
                Utility.FindChild(widget, "btnDetail").CustomSetActive(true);
                this.UpdateUI();
            }
        }

        public void OnMainListClick(CUIEvent uiEvent)
        {
            this.UpdateUI();
        }

        public void OnSubListClick(CUIEvent uiEvent)
        {
            this.UpdateUI();
        }

        public void OnSubListShow(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CPlayerInfoSystem.sPlayerInfoFormPath);
            if (form != null)
            {
                CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(form.GetWidget(2), "panelTop/DropList/List");
                componetInChild.get_gameObject().CustomSetActive(!componetInChild.get_gameObject().get_activeSelf());
            }
        }

        public override void UnInit()
        {
            Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Player_Info_Pvp_MainList_Click, new CUIEventManager.OnUIEventHandler(this.OnMainListClick));
            Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Player_Info_Pvp_SubList_Click, new CUIEventManager.OnUIEventHandler(this.OnSubListClick));
            Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Player_Info_Pvp_SubList_Show, new CUIEventManager.OnUIEventHandler(this.OnSubListShow));
            Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Player_Info_Pvp_Graph_Show, new CUIEventManager.OnUIEventHandler(this.OnGraphShow));
            Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Player_Info_Pvp_Detail_Show, new CUIEventManager.OnUIEventHandler(this.OnDetailShow));
        }

        public void UpdateUI()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CPlayerInfoSystem.sPlayerInfoFormPath);
            if (form != null)
            {
                GameObject widget = form.GetWidget(2);
                CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(widget, "panelTop/MenuList");
                CUIListScript script3 = Utility.GetComponetInChild<CUIListScript>(widget, "panelTop/DropList/List");
                GameObject obj3 = Utility.FindChild(widget, "panelTop");
                GameObject p = Utility.FindChild(widget, "panelLeft");
                GameObject obj5 = Utility.FindChild(widget, "panelLeftGraph");
                GameObject obj6 = Utility.FindChild(widget, "panelRight");
                int selectedIndex = componetInChild.GetSelectedIndex();
                int num2 = script3.GetSelectedIndex();
                if (((selectedIndex >= 0) && (num2 >= 0)) && ((selectedIndex < Enum.GetValues(typeof(MainTab)).Length) && (num2 < Enum.GetValues(typeof(SubTab)).Length)))
                {
                    CacheSubTab = num2;
                    this.InitBaseValue();
                    MainTab mainTab = (MainTab) selectedIndex;
                    SubTab subTab = (SubTab) num2;
                    Utility.GetComponetInChild<Text>(widget, "panelTop/DropList/Button_Down/Text").set_text(sSubTitles[(int) subTab]);
                    float kda = 0f;
                    float joinFight = 0f;
                    int totalCoinGet = 0;
                    int totalHurtToEnemy = 0;
                    int totalBeHurt = 0;
                    int totalHurtToOrgan = 0;
                    float totalDead = 0f;
                    uint winMvp = 0;
                    uint loseMvp = 0;
                    uint godLike = 0;
                    uint tripleKill = 0;
                    uint quataryKill = 0;
                    uint pentaKill = 0;
                    int gameCnt = 0;
                    float gameWins = 0f;
                    CPlayerProfile profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
                    ConvertAverageData(profile, mainTab, subTab, out kda, out joinFight, out totalCoinGet, out totalHurtToEnemy, out totalBeHurt, out totalHurtToOrgan, out totalDead);
                    ConvertBaseData(profile, mainTab, subTab, out winMvp, out loseMvp, out godLike, out tripleKill, out quataryKill, out pentaKill);
                    ConvertComplexData(profile, mainTab, subTab, out gameCnt, out gameWins);
                    ConvertAbilityData(profile, mainTab, subTab, this.m_ability);
                    for (int i = 0; i < this.m_ability.Length; i++)
                    {
                        this.m_ability[i] = Mathf.Clamp(this.m_ability[i], 0f, 1f);
                    }
                    switch (subTab)
                    {
                        case SubTab.MatchAll:
                        case SubTab.Match5V5:
                        case SubTab.MatchRank:
                        case SubTab.MatchGuild:
                        {
                            Utility.GetComponetInChild<Text>(p, "Content/txt1").set_text(kda.ToString("F1"));
                            Utility.GetComponetInChild<Text>(p, "Content/txt2").set_text(joinFight.ToString("P0"));
                            Utility.GetComponetInChild<Text>(p, "Content/txt3").set_text(totalCoinGet.ToString());
                            Utility.GetComponetInChild<Text>(p, "Content/txt4").set_text(totalHurtToEnemy.ToString());
                            Utility.GetComponetInChild<Text>(p, "Content/txt5").set_text(totalBeHurt.ToString());
                            Utility.GetComponetInChild<Text>(p, "Content/txt6").set_text(totalHurtToOrgan.ToString());
                            float num20 = this.m_ability[1] * 100f;
                            Utility.GetComponetInChild<Text>(obj6, "DateBar3/txt9").set_text(num20.ToString("F1"));
                            break;
                        }
                        default:
                            Utility.GetComponetInChild<Text>(p, "Content/txt1").set_text("-");
                            Utility.GetComponetInChild<Text>(p, "Content/txt2").set_text("-");
                            Utility.GetComponetInChild<Text>(p, "Content/txt3").set_text("-");
                            Utility.GetComponetInChild<Text>(p, "Content/txt4").set_text("-");
                            Utility.GetComponetInChild<Text>(p, "Content/txt5").set_text("-");
                            Utility.GetComponetInChild<Text>(p, "Content/txt6").set_text("-");
                            Utility.GetComponetInChild<Text>(obj6, "DateBar3/txt9").set_text("-");
                            break;
                    }
                    if (subTab != SubTab.Match1V1)
                    {
                        Utility.GetComponetInChild<Text>(obj6, "txt1").set_text(winMvp.ToString());
                        Utility.GetComponetInChild<Text>(obj6, "txt2").set_text(loseMvp.ToString());
                        Utility.GetComponetInChild<Text>(obj6, "txt3").set_text(godLike.ToString());
                        Utility.GetComponetInChild<Text>(obj6, "txt4").set_text(tripleKill.ToString());
                        Utility.GetComponetInChild<Text>(obj6, "txt5").set_text(quataryKill.ToString());
                        Utility.GetComponetInChild<Text>(obj6, "txt6").set_text(pentaKill.ToString());
                    }
                    else
                    {
                        Utility.GetComponetInChild<Text>(obj6, "txt1").set_text("-");
                        Utility.GetComponetInChild<Text>(obj6, "txt2").set_text("-");
                        Utility.GetComponetInChild<Text>(obj6, "txt3").set_text("-");
                        Utility.GetComponetInChild<Text>(obj6, "txt4").set_text("-");
                        Utility.GetComponetInChild<Text>(obj6, "txt5").set_text("-");
                        Utility.GetComponetInChild<Text>(obj6, "txt6").set_text("-");
                    }
                    Utility.GetComponetInChild<Text>(obj6, "DateBar1/txt7").set_text(gameCnt.ToString());
                    Utility.GetComponetInChild<Text>(obj6, "DateBar2/txt8").set_text((gameWins != 1f) ? gameWins.ToString("P1") : gameWins.ToString("P0"));
                    Utility.GetComponetInChild<Image>(obj6, "DateBar2/Bar").set_fillAmount(gameWins);
                    Utility.GetComponetInChild<Image>(obj6, "DateBar3/Bar").set_fillAmount(CPlayerProfile.Divide(this.m_ability[1], 1f));
                    this.InitTemVector();
                    GameObject obj7 = null;
                    Vector3 vector = Vector3.get_zero();
                    Vector3[] vectorArray = new Vector3[6];
                    for (int j = 1; j <= 6; j++)
                    {
                        obj7 = Utility.FindChild(widget, string.Format("panelLeftGraph/Content/p{0}", j));
                        vector = sTemVector[j] - sTemVector[0];
                        vectorArray[j - 1] = sTemVector[0] + ((Vector3) (vector * this.m_ability[j - 1]));
                        obj7.get_transform().set_localPosition(vectorArray[j - 1]);
                    }
                    CUIPolygon polygon = Utility.GetComponetInChild<CUIPolygon>(obj5, "Content/Polygon");
                    polygon.vertexs = vectorArray;
                    polygon.SetAllDirty();
                    GameObject obj8 = Utility.FindChild(widget, "btnGraph");
                    GameObject obj9 = Utility.FindChild(widget, "btnDetail");
                    GameObject obj10 = Utility.FindChild(widget, "btnShare");
                    p.CustomSetActive(obj8.get_activeSelf());
                    obj5.CustomSetActive(obj9.get_activeSelf());
                    obj10.CustomSetActive(CPlayerInfoSystem.isSelf(Singleton<CPlayerInfoSystem>.instance.GetProfile().m_uuid));
                    script3.get_gameObject().CustomSetActive(false);
                    if (CSysDynamicBlock.bLobbyEntryBlocked)
                    {
                        CUICommonSystem.SetObjActive(form.get_transform().FindChild("pnlBg/pnlBody/pnlPvpInfo/btnShare"), false);
                    }
                }
            }
        }

        private static int CacheSubTab
        {
            get
            {
                return PlayerPrefs.GetInt(string.Format(sCacheSubTabStr, Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().playerUllUID.ToString()));
            }
            set
            {
                PlayerPrefs.SetInt(string.Format(sCacheSubTabStr, Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().playerUllUID.ToString()), value);
            }
        }
    }
}

