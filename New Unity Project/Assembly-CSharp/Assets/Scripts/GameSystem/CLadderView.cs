namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEngine;
    using UnityEngine.UI;

    internal class CLadderView
    {
        public static string LADDER_IMG_PATH = "UGUI/Sprite/System/Ladder/";
        public static string LADDER_IMG_STAR = (LADDER_IMG_PATH + "Img_CompetitiveRace_Staropen.prefab");
        public static string LADDER_IMG_STAR_EMPTY = (LADDER_IMG_PATH + "Img_CompetitiveRace_Staroff.prefab");
        public const int MAX_MOST_USED_HERO_NUM = 4;
        public const int MAX_RECENT_GAME_SHOW_NUM = 10;
        public static string RANK_ICON_PATH = "UGUI/Sprite/Dynamic/Ladder/";
        public static string RANK_SMALL_ICON_PATH = "UGUI/Sprite/Dynamic/Ladder_Small/";
        public static Color s_Bg_Color_Expand = new Color(0.3176471f, 0.3803922f, 0.5607843f, 0.5f);
        public static Color s_Bg_Color_Shrink = new Color(0.1372549f, 0.1803922f, 0.282353f, 0.5f);

        private static int ComparisonHeroData(COMDT_RANK_COMMON_USED_HERO a, COMDT_RANK_COMMON_USED_HERO b)
        {
            if (a.dwFightCnt > b.dwFightCnt)
            {
                return -1;
            }
            if (a.dwFightCnt < b.dwFightCnt)
            {
                return 1;
            }
            return 0;
        }

        private static string GetGameTime(ref COMDT_RANK_CURSEASON_FIGHT_RECORD data)
        {
            DateTime time = Utility.ToUtcTime2Local((long) data.dwFightTime);
            object[] args = new object[] { time.Month.ToString("00"), time.Day.ToString("00"), time.Hour.ToString("00"), time.Minute.ToString("00") };
            return string.Format(Singleton<CTextManager>.GetInstance().GetText("GameTime_Template"), args);
        }

        public static string GetGradeParticleBgResName()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                ResRankGradeConf dataByKey = GameDataMgr.rankGradeDatabin.GetDataByKey((uint) masterRoleInfo.m_rankGrade);
                if (dataByKey != null)
                {
                    if (IsSuperKing(masterRoleInfo.m_rankGrade, masterRoleInfo.m_rankClass))
                    {
                        return dataByKey.szGradeParticleBgSuperMaster;
                    }
                    return dataByKey.szGradeParticleBg;
                }
            }
            return string.Empty;
        }

        public static float GetProcessCircleFillAmount(int target, int total)
        {
            if (total == 0)
            {
                return 0f;
            }
            double num = 0.085;
            double num2 = 1.0 - (num * 2.0);
            return (float) (num + ((((float) target) / ((float) total)) * num2));
        }

        public static string GetRankBigGradeName(byte rankBigGrade)
        {
            <GetRankBigGradeName>c__AnonStorey69 storey = new <GetRankBigGradeName>c__AnonStorey69();
            storey.rankBigGrade = rankBigGrade;
            ResRankGradeConf conf = GameDataMgr.rankGradeDatabin.FindIf(new Func<ResRankGradeConf, bool>(storey, (IntPtr) this.<>m__6F));
            if (conf == null)
            {
                return string.Empty;
            }
            if (storey.rankBigGrade == 6)
            {
                return Singleton<CTextManager>.GetInstance().GetText("Ladder_Super_Master");
            }
            return conf.szBigGradeName;
        }

        public static string GetRankFrameIconPath(byte rankGrade, uint rankClass)
        {
            ResRankGradeConf dataByKey = GameDataMgr.rankGradeDatabin.GetDataByKey((uint) rankGrade);
            if ((dataByKey == null) || string.IsNullOrEmpty(dataByKey.szGradeFramePicPath))
            {
                return string.Empty;
            }
            if (IsSuperMaster(rankGrade, rankClass))
            {
                return (RANK_ICON_PATH + dataByKey.szGradeFramePicPathSuperMaster);
            }
            return (RANK_ICON_PATH + dataByKey.szGradeFramePicPath);
        }

        public static string GetRankIconPath()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                return GetRankIconPath(masterRoleInfo.m_rankGrade, masterRoleInfo.m_rankClass);
            }
            return string.Empty;
        }

        public static string GetRankIconPath(byte rankGrade, uint rankClass)
        {
            ResRankGradeConf dataByKey = GameDataMgr.rankGradeDatabin.GetDataByKey((uint) rankGrade);
            if (dataByKey == null)
            {
                return string.Empty;
            }
            if (IsSuperMaster(rankGrade, rankClass))
            {
                return (RANK_ICON_PATH + StringHelper.UTF8BytesToString(ref dataByKey.szGradePicturePathSuperMaster));
            }
            return (RANK_ICON_PATH + StringHelper.UTF8BytesToString(ref dataByKey.szGradePicturePath));
        }

        private static string GetRankName(ref COMDT_RANK_PASTSEASON_FIGHT_RECORD data)
        {
            return GetRankName(data.bGrade, data.dwClassOfRank);
        }

        public static string GetRankName(byte rankGrade, uint rankClass)
        {
            ResRankGradeConf dataByKey = GameDataMgr.rankGradeDatabin.GetDataByKey((uint) rankGrade);
            if (dataByKey == null)
            {
                return string.Empty;
            }
            if (IsSuperMaster(rankGrade, rankClass))
            {
                return Singleton<CTextManager>.GetInstance().GetText("Ladder_Super_Master");
            }
            return StringHelper.UTF8BytesToString(ref dataByKey.szGradeDesc);
        }

        public static string GetRankSmallIconPath(byte rankGrade, uint rankClass)
        {
            ResRankGradeConf dataByKey = GameDataMgr.rankGradeDatabin.GetDataByKey((uint) rankGrade);
            if (dataByKey == null)
            {
                return string.Empty;
            }
            if (IsSuperMaster(rankGrade, rankClass))
            {
                return (RANK_SMALL_ICON_PATH + StringHelper.UTF8BytesToString(ref dataByKey.szGradePicturePathSuperMaster));
            }
            return (RANK_SMALL_ICON_PATH + StringHelper.UTF8BytesToString(ref dataByKey.szGradePicturePath));
        }

        private static string GetSeasonNameWithBracket(ulong time)
        {
            string ladderSeasonName = Singleton<CLadderSystem>.GetInstance().GetLadderSeasonName(time);
            return (!string.IsNullOrEmpty(ladderSeasonName) ? ("(" + ladderSeasonName + ")") : string.Empty);
        }

        private static string GetSeasonText(ref COMDT_RANK_PASTSEASON_FIGHT_RECORD data)
        {
            DateTime time = Utility.ToUtcTime2Local((long) data.dwSeaStartTime);
            DateTime time2 = Utility.ToUtcTime2Local((long) data.dwSeaEndTime);
            object[] args = new object[] { time.Year, time.Month, time2.Year, time2.Month };
            return string.Format(Singleton<CTextManager>.GetInstance().GetText("ladder_season_duration"), args);
        }

        private static string GetSeasonText(ref COMDT_RANKDETAIL data)
        {
            if (data.bState == 1)
            {
                DateTime time = Utility.ToUtcTime2Local((long) data.dwSeasonStartTime);
                DateTime time2 = Utility.ToUtcTime2Local((long) data.dwSeasonEndTime);
                object[] args = new object[] { time.Year, time.Month, time2.Year, time2.Month };
                return string.Format(Singleton<CTextManager>.GetInstance().GetText("ladder_season_duration"), args);
            }
            return "赛季还未开始";
        }

        public static string GetSubRankIconPath(byte rankGrade, uint rankClass)
        {
            ResRankGradeConf dataByKey = GameDataMgr.rankGradeDatabin.GetDataByKey((uint) rankGrade);
            if (dataByKey == null)
            {
                return string.Empty;
            }
            if (IsSuperMaster(rankGrade, rankClass))
            {
                return (RANK_ICON_PATH + StringHelper.UTF8BytesToString(ref dataByKey.szGradeSmallPicPathSuperMaster));
            }
            return (RANK_ICON_PATH + StringHelper.UTF8BytesToString(ref dataByKey.szGradeSmallPicPath));
        }

        public static string GetSubRankSmallIconPath(byte rankGrade, uint rankClass)
        {
            ResRankGradeConf dataByKey = GameDataMgr.rankGradeDatabin.GetDataByKey((uint) rankGrade);
            if (dataByKey == null)
            {
                return string.Empty;
            }
            if (IsSuperMaster(rankGrade, rankClass))
            {
                return (RANK_SMALL_ICON_PATH + StringHelper.UTF8BytesToString(ref dataByKey.szGradeSmallPicPathSuperMaster));
            }
            return (RANK_SMALL_ICON_PATH + StringHelper.UTF8BytesToString(ref dataByKey.szGradeSmallPicPath));
        }

        private static string GetTopUseHeroNames(ref COMDT_RANK_PASTSEASON_FIGHT_RECORD data, out List<COMDT_RANK_COMMON_USED_HERO> heroList)
        {
            heroList = new List<COMDT_RANK_COMMON_USED_HERO>();
            for (int i = 0; i < data.dwCommonUsedHeroNum; i++)
            {
                if (data.astCommonUsedHeroInfo[i].dwHeroId != 0)
                {
                    heroList.Add(data.astCommonUsedHeroInfo[i]);
                }
            }
            heroList.Sort(new Comparison<COMDT_RANK_COMMON_USED_HERO>(CLadderView.ComparisonHeroData));
            StringBuilder builder = new StringBuilder();
            int num2 = (heroList.Count <= 4) ? heroList.Count : 4;
            for (int j = 0; j < num2; j++)
            {
                ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroList[j].dwHeroId);
                if (dataByKey != null)
                {
                    builder.Append(StringHelper.UTF8BytesToString(ref dataByKey.szName));
                    builder.Append(" ");
                }
            }
            return builder.ToString();
        }

        public static void InitKingForm(CUIFormScript form, ref COMDT_RANKDETAIL data)
        {
            ShowRankDetail(form.get_transform().Find("RankCon").get_gameObject(), ref data, false);
        }

        public static void InitLadderEntry(CUIFormScript form, ref COMDT_RANKDETAIL data, bool isQualified)
        {
            Transform transform = form.get_transform().Find("MainPanel/BtnGroup/SingleStart");
            Transform transform2 = form.get_transform().Find("MainPanel/BtnGroup/DoubleStart");
            Transform transform3 = form.get_transform().Find("MainPanel/BtnGroup/FiveStart");
            Button btn = null;
            Button component = null;
            Button button3 = null;
            if (transform != null)
            {
                btn = transform.GetComponent<Button>();
            }
            if (transform2 != null)
            {
                component = transform2.GetComponent<Button>();
            }
            if (transform3 != null)
            {
                button3 = transform3.GetComponent<Button>();
            }
            form.GetWidget(11).CustomSetActive(isQualified);
            if (isQualified)
            {
                form.get_transform().Find("ReqPanel").get_gameObject().CustomSetActive(false);
                form.get_transform().Find("MainPanel/ImgLock").get_gameObject().CustomSetActive(false);
                form.get_transform().Find("MainPanel/RankCon").get_gameObject().CustomSetActive(true);
                GameObject go = form.get_transform().Find("MainPanel/RankCon").get_gameObject();
                if (data != null)
                {
                    if (btn != null)
                    {
                        CUICommonSystem.SetButtonEnableWithShader(btn, data.bState == 1, true);
                    }
                    if (component != null)
                    {
                        CUICommonSystem.SetButtonEnableWithShader(component, data.bState == 1, true);
                    }
                    if (button3 != null)
                    {
                        CUICommonSystem.SetButtonEnableWithShader(button3, data.bState == 1, true);
                    }
                    ShowRankDetail(go, ref data, false);
                    form.GetWidget(0x10).GetComponent<CUIParticleScript>().LoadRes(GetGradeParticleBgResName());
                }
                else
                {
                    if (btn != null)
                    {
                        CUICommonSystem.SetButtonEnableWithShader(btn, false, true);
                    }
                    if (component != null)
                    {
                        CUICommonSystem.SetButtonEnableWithShader(component, false, true);
                    }
                    if (button3 != null)
                    {
                        CUICommonSystem.SetButtonEnableWithShader(button3, false, true);
                    }
                }
            }
            else
            {
                if (btn != null)
                {
                    CUICommonSystem.SetButtonEnableWithShader(btn, false, true);
                }
                if (component != null)
                {
                    CUICommonSystem.SetButtonEnableWithShader(component, false, true);
                }
                if (button3 != null)
                {
                    CUICommonSystem.SetButtonEnableWithShader(button3, false, true);
                }
                form.get_transform().Find("ReqPanel").get_gameObject().CustomSetActive(true);
                form.get_transform().Find("MainPanel/ImgLock").get_gameObject().CustomSetActive(true);
                form.get_transform().Find("MainPanel/RankCon").get_gameObject().CustomSetActive(false);
                Text text = form.get_transform().Find("ReqPanel/txtHeroNum").GetComponent<Text>();
                Text text2 = form.get_transform().Find("ReqPanel/txtReqHeroNum").GetComponent<Text>();
                int haveHeroCount = 0;
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    haveHeroCount = masterRoleInfo.GetHaveHeroCount(false);
                }
                text.set_text(string.Format("{0}/{1}", haveHeroCount, CLadderSystem.REQ_HERO_NUM));
                text2.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Ladder_Req_Hero_Num"), CLadderSystem.REQ_HERO_NUM.ToString()));
            }
            ShowBraveScorePanel(form, data, isQualified);
            ShowRewardPanel(form, data);
            ShowBpModePanel(form);
            ShowSuperKingRankPanel(form);
            if (CSysDynamicBlock.bLobbyEntryBlocked)
            {
                form.get_transform().FindChild("MainPanel/pnlRankingBtn").get_gameObject().CustomSetActive(false);
            }
        }

        public static void InitLadderGameInfo(CUIFormScript gameInfoForm, COMDT_RANKDETAIL rankDetail, List<COMDT_RANK_CURSEASON_FIGHT_RECORD> dataList)
        {
            SetGameInfoRecentPanel(gameInfoForm, rankDetail, dataList);
            SetGameInfoSeasonPanel(gameInfoForm, rankDetail);
            SetGameInfoSeasonTimePanel(gameInfoForm, rankDetail);
        }

        public static void InitLadderHistory(CUIFormScript form, List<COMDT_RANK_PASTSEASON_FIGHT_RECORD> dataList)
        {
            CUIListScript component = form.get_transform().Find("ExpandList").GetComponent<CUIListScript>();
            if (dataList == null)
            {
                component.SetElementAmount(0);
            }
            else
            {
                component.SetElementAmount(dataList.Count);
                CUIListElementScript elemenet = null;
                for (int i = 0; i < dataList.Count; i++)
                {
                    elemenet = component.GetElemenet(i);
                    COMDT_RANK_PASTSEASON_FIGHT_RECORD data = dataList[i];
                    Text text = elemenet.get_transform().Find("Title/txtLeagueTime").GetComponent<Text>();
                    Text text2 = elemenet.get_transform().Find("Title/txtRankTitle").GetComponent<Text>();
                    Text text3 = elemenet.get_transform().Find("Title/txtHeroes").GetComponent<Text>();
                    Text text4 = elemenet.get_transform().Find("Expand/txtGameNum").GetComponent<Text>();
                    Text text5 = elemenet.get_transform().Find("Expand/txtWinNum").GetComponent<Text>();
                    Text text6 = elemenet.get_transform().Find("Expand/txtWinRate").GetComponent<Text>();
                    Text text7 = elemenet.get_transform().Find("Expand/txtContiWinNum").GetComponent<Text>();
                    text.set_text(GetSeasonNameWithBracket((ulong) data.dwSeaStartTime) + " " + GetSeasonText(ref data));
                    text2.set_text(GetRankName(ref data));
                    List<COMDT_RANK_COMMON_USED_HERO> heroList = new List<COMDT_RANK_COMMON_USED_HERO>();
                    text3.set_text(GetTopUseHeroNames(ref data, out heroList));
                    text4.set_text(data.dwTotalFightCnt.ToString());
                    text5.set_text(data.dwTotalWinCnt.ToString());
                    text6.set_text((data.dwTotalFightCnt <= 0) ? "0.00%" : string.Format("{0}%", ((data.dwTotalWinCnt * 100f) / ((float) data.dwTotalFightCnt)).ToString("0.00")));
                    text7.set_text(data.dwMaxContinuousWinCnt.ToString());
                    int num2 = (heroList.Count <= 4) ? heroList.Count : 4;
                    for (int j = 0; j < num2; j++)
                    {
                        Transform item = elemenet.get_transform().Find(string.Format("Expand/Hero{0}", j + 1));
                        item.get_gameObject().CustomSetActive(true);
                        COMDT_RANK_COMMON_USED_HERO comdt_rank_common_used_hero = heroList[j];
                        SetMostUsedHero(item, ref comdt_rank_common_used_hero, form);
                    }
                    for (int k = num2; k < 4; k++)
                    {
                        elemenet.get_transform().Find(string.Format("Expand/Hero{0}", k + 1)).get_gameObject().CustomSetActive(false);
                    }
                }
            }
        }

        public static void InitLadderRecent(CUIFormScript form, List<COMDT_RANK_CURSEASON_FIGHT_RECORD> dataList)
        {
            CUIListScript component = form.get_transform().Find("Root/List").GetComponent<CUIListScript>();
            if (dataList == null)
            {
                component.SetElementAmount(0);
            }
            else
            {
                int amount = (dataList.Count >= 10) ? 10 : dataList.Count;
                component.SetElementAmount(amount);
                CUIListElementScript elemenet = null;
                for (int i = 0; i < amount; i++)
                {
                    elemenet = component.GetElemenet(i);
                    COMDT_RANK_CURSEASON_FIGHT_RECORD data = dataList[i];
                    DebugHelper.Assert(data != null);
                    Image image = elemenet.get_transform().Find("imageIcon").GetComponent<Image>();
                    Text result = elemenet.get_transform().Find("txtResult").GetComponent<Text>();
                    Text text2 = elemenet.get_transform().Find("txtBraveScore").GetComponent<Text>();
                    Text text3 = elemenet.get_transform().Find("txtTime").GetComponent<Text>();
                    Text text4 = elemenet.get_transform().Find("txtKDA").GetComponent<Text>();
                    SetWinLose(result, ref data);
                    text2.set_text(data.dwAddStarScore.ToString());
                    text3.set_text(GetGameTime(ref data));
                    text4.set_text(string.Format("{0} / {1} / {2}", data.dwKillNum, data.dwDeadNum, data.dwAssistNum));
                    ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(data.dwHeroId);
                    if (dataByKey != null)
                    {
                        image.SetSprite(CUIUtility.s_Sprite_Dynamic_Icon_Dir + StringHelper.UTF8BytesToString(ref dataByKey.szImagePath), form, true, false, false, false);
                        Utility.FindChild(image.get_gameObject(), "Friend").CustomSetActive((data.bTeamerNum > 1) && (data.bTeamerNum < 5));
                        Utility.FindChild(image.get_gameObject(), "FiveFriend").CustomSetActive(data.bTeamerNum == 5);
                        Utility.FindChild(image.get_gameObject(), "Bp").CustomSetActive(Convert.ToBoolean(data.bIsBanPick));
                    }
                    for (int j = 0; j < 6; j++)
                    {
                        COMDT_INGAME_EQUIP_INFO comdt_ingame_equip_info = null;
                        if (j < data.bEquipNum)
                        {
                            comdt_ingame_equip_info = data.astEquipDetail[j];
                        }
                        int num4 = j + 1;
                        Image image2 = elemenet.get_transform().FindChild(string.Format("TianFu/TianFuIcon{0}", num4.ToString())).GetComponent<Image>();
                        if ((comdt_ingame_equip_info == null) || (comdt_ingame_equip_info.dwEquipID == 0))
                        {
                            image2.get_gameObject().CustomSetActive(false);
                        }
                        else
                        {
                            image2.get_gameObject().CustomSetActive(true);
                            CUICommonSystem.SetEquipIcon((ushort) comdt_ingame_equip_info.dwEquipID, image2.get_gameObject(), form);
                        }
                    }
                }
            }
        }

        public static void InitRewardForm(CUIFormScript form, ref COMDT_RANKDETAIL data)
        {
            ShowRankDetail(form.get_transform().Find("RankCon").get_gameObject(), ref data, true);
        }

        public static bool IsSuperKing(byte rankGrade, uint rankClass)
        {
            ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xcd);
            return ((dataByKey != null) && ((rankGrade == CLadderSystem.MAX_RANK_LEVEL) && ((0 < rankClass) && (rankClass <= dataByKey.dwConfValue))));
        }

        public static bool IsSuperMaster(byte rankGrade, uint rankClass)
        {
            ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xcd);
            return ((dataByKey != null) && ((rankGrade == CLadderSystem.MAX_RANK_LEVEL) && ((rankClass == 0) || (rankClass > dataByKey.dwConfValue))));
        }

        public static void OnHistoryItemChange(GameObject go, bool bExpand)
        {
            Image component = go.get_transform().Find("Bg").GetComponent<Image>();
            if (component != null)
            {
                component.set_color(!bExpand ? s_Bg_Color_Shrink : s_Bg_Color_Expand);
            }
            Transform transform = go.get_transform().Find("Title/Button");
            if (transform != null)
            {
                if (bExpand)
                {
                    (transform as RectTransform).set_rotation(Quaternion.Euler(0f, 0f, 180f));
                }
                else
                {
                    (transform as RectTransform).set_rotation(Quaternion.Euler(0f, 0f, 0f));
                }
            }
        }

        public static void SetGameInfoRecentPanel(CUIFormScript form, COMDT_RANKDETAIL rankDetail, List<COMDT_RANK_CURSEASON_FIGHT_RECORD> dataList)
        {
            GameObject widget = form.GetWidget(0);
            GameObject obj3 = form.GetWidget(14);
            if (((rankDetail != null) && (dataList != null)) && ((dataList.Count > 0) && (rankDetail.dwSeasonIdx == dataList[0].dwSeasonId)))
            {
                Text component = form.GetWidget(7).GetComponent<Text>();
                Text text2 = form.GetWidget(8).GetComponent<Text>();
                COMDT_RANK_CURSEASON_FIGHT_RECORD data = dataList[0];
                SetWinLose(component, ref data);
                text2.set_text(GetGameTime(ref data));
                ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(data.dwHeroId);
                if (dataByKey != null)
                {
                    form.GetWidget(3).GetComponent<Image>().SetSprite(CUIUtility.s_Sprite_Dynamic_Icon_Dir + StringHelper.UTF8BytesToString(ref dataByKey.szImagePath), form, true, false, false, false);
                    form.GetWidget(5).get_gameObject().CustomSetActive((data.bTeamerNum > 1) && (data.bTeamerNum < 5));
                    GameObject obj4 = form.GetWidget(6).get_gameObject();
                    if (obj4 != null)
                    {
                        obj4.CustomSetActive(data.bTeamerNum == 5);
                    }
                    form.GetWidget(4).get_gameObject().CustomSetActive(Convert.ToBoolean(data.bIsBanPick));
                }
                widget.CustomSetActive(true);
                obj3.CustomSetActive(false);
            }
            else
            {
                widget.CustomSetActive(false);
                obj3.CustomSetActive(true);
            }
        }

        public static void SetGameInfoSeasonPanel(CUIFormScript form, COMDT_RANKDETAIL rankdetail)
        {
            if (rankdetail != null)
            {
                Text component = form.GetWidget(10).GetComponent<Text>();
                Text text2 = form.GetWidget(9).GetComponent<Text>();
                Text text3 = form.GetWidget(11).GetComponent<Text>();
                component.set_text(rankdetail.dwTotalFightCnt.ToString());
                text2.set_text(rankdetail.dwTotalWinCnt.ToString());
                text3.set_text(rankdetail.dwMaxContinuousWinCnt.ToString());
            }
        }

        public static void SetGameInfoSeasonTimePanel(CUIFormScript form, COMDT_RANKDETAIL rankdetail)
        {
            Text component = form.GetWidget(15).GetComponent<Text>();
            component.set_text(component.get_text() + " " + GetSeasonNameWithBracket((ulong) rankdetail.dwSeasonStartTime));
            form.GetWidget(12).GetComponent<Text>().set_text(GetSeasonText(ref rankdetail));
        }

        private static void SetMostUsedHero(Transform item, ref COMDT_RANK_COMMON_USED_HERO data, CUIFormScript formScript)
        {
            Text component = item.Find("txtGameNum").GetComponent<Text>();
            Text text2 = item.Find("txtWinNum").GetComponent<Text>();
            component.set_text(data.dwFightCnt.ToString());
            text2.set_text(data.dwWinCnt.ToString());
            ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(data.dwHeroId);
            if (dataByKey != null)
            {
                item.Find("heroItemCell/imageIcon").GetComponent<Image>().SetSprite(CUIUtility.s_Sprite_Dynamic_Icon_Dir + StringHelper.UTF8BytesToString(ref dataByKey.szImagePath), formScript, true, false, false, false);
            }
        }

        private static void SetWinLose(Text Result, ref COMDT_RANK_CURSEASON_FIGHT_RECORD data)
        {
            if (data.dwGameResult == 1)
            {
                Result.set_text(Singleton<CTextManager>.GetInstance().GetText("GameResult_Win"));
            }
            else if (data.dwGameResult == 2)
            {
                Result.set_text(Singleton<CTextManager>.GetInstance().GetText("GameResult_Lose"));
            }
            else if (data.dwGameResult == 3)
            {
                Result.set_text(Singleton<CTextManager>.GetInstance().GetText("GameResult_Draw"));
            }
            else
            {
                Result.set_text(Singleton<CTextManager>.GetInstance().GetText("GameResult_Null"));
            }
        }

        private static void ShowBpModePanel(CUIFormScript form)
        {
            GameObject widget = form.GetWidget(7);
            GameObject obj3 = form.GetWidget(8);
            GameObject obj4 = form.GetWidget(9);
            GameObject obj5 = form.GetWidget(6);
            if (Singleton<CLadderSystem>.GetInstance().IsUseBpMode())
            {
                widget.CustomSetActive(true);
                obj3.CustomSetActive(true);
                obj4.CustomSetActive(true);
                obj5.CustomSetActive(true);
            }
            else
            {
                widget.CustomSetActive(false);
                obj3.CustomSetActive(false);
                obj4.CustomSetActive(false);
                obj5.CustomSetActive(false);
            }
        }

        private static void ShowBraveScorePanel(CUIFormScript form, COMDT_RANKDETAIL data, bool isShow)
        {
            GameObject widget = form.GetWidget(0);
            if (!isShow)
            {
                widget.CustomSetActive(false);
            }
            else
            {
                widget.CustomSetActive(true);
                if (data != null)
                {
                    Image component = form.GetWidget(1).GetComponent<Image>();
                    Text text = form.GetWidget(2).GetComponent<Text>();
                    Text text2 = form.GetWidget(3).GetComponent<Text>();
                    GameObject obj3 = form.GetWidget(15);
                    uint dwAddScoreOfConWinCnt = data.dwAddScoreOfConWinCnt;
                    uint selfBraveScoreMax = Singleton<CLadderSystem>.GetInstance().GetSelfBraveScoreMax();
                    component.set_fillAmount(GetProcessCircleFillAmount((int) dwAddScoreOfConWinCnt, (int) selfBraveScoreMax));
                    text.set_text(dwAddScoreOfConWinCnt + "/" + selfBraveScoreMax);
                    string[] args = new string[] { selfBraveScoreMax.ToString() };
                    text2.set_text(Singleton<CTextManager>.GetInstance().GetText("Ladder_Brave_Exchange_Tip", args));
                    if (data.dwContinuousWin > 0)
                    {
                        obj3.CustomSetActive(true);
                        obj3.GetComponent<Text>().set_text(data.dwContinuousWin + Singleton<CTextManager>.GetInstance().GetText("Common_Continues_Win"));
                    }
                    else
                    {
                        obj3.CustomSetActive(false);
                    }
                }
            }
        }

        public static void ShowRankButtonIn5(CUIFormScript form, bool show)
        {
            if (form != null)
            {
                Transform transform = form.get_transform().Find("MainPanel/BtnGroup/FiveStart");
                if (transform != null)
                {
                    transform.get_gameObject().CustomSetActive(show);
                }
            }
        }

        public static void ShowRankDetail(GameObject go, ref COMDT_RANKDETAIL rankDetail, bool isShowSeasonHighestGrade = false)
        {
            DebugHelper.Assert(rankDetail != null, "Rank Data must not be null!!!");
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if ((masterRoleInfo != null) && (rankDetail != null))
            {
                if (isShowSeasonHighestGrade)
                {
                    if (!Singleton<CLadderSystem>.GetInstance().IsValidGrade(masterRoleInfo.m_rankSeasonHighestGrade))
                    {
                        throw new Exception("Not valid rank highest season grade: " + masterRoleInfo.m_rankSeasonHighestGrade);
                    }
                    ShowRankDetail(go, masterRoleInfo.m_rankSeasonHighestGrade, masterRoleInfo.m_rankSeasonHighestClass, rankDetail.dwScore, true, false, true, false, true);
                }
                else
                {
                    ShowRankDetail(go, masterRoleInfo.m_rankGrade, masterRoleInfo.m_rankClass, rankDetail.dwScore, true, false, false, false, true);
                }
            }
        }

        public static void ShowRankDetail(GameObject go, byte rankGrade, uint rankClass, bool isGrey = false, bool isUseSmall = false)
        {
            DebugHelper.Assert(go != null, "GameObject is NULL!!!");
            DebugHelper.Assert(rankGrade > 0, "grade must be above 0!!!");
            Image image = (go.get_transform().Find("ImgRank") == null) ? null : go.get_transform().Find("ImgRank").GetComponent<Image>();
            Image image2 = (go.get_transform().Find("ImgSubRank") == null) ? null : go.get_transform().Find("ImgSubRank").GetComponent<Image>();
            Text graphic = (go.get_transform().Find("txtRankName") == null) ? null : go.get_transform().Find("txtRankName").GetComponent<Text>();
            if (image != null)
            {
                string rankIconPath = GetRankIconPath(rankGrade, rankClass);
                image.SetSprite(isUseSmall ? string.Format("{0}_small", rankIconPath) : rankIconPath, null, true, false, false, false);
                image.get_gameObject().CustomSetActive(true);
                CUIUtility.SetImageGrey(image, isGrey);
            }
            if (image2 != null)
            {
                image2.SetSprite(GetSubRankIconPath(rankGrade, rankClass), null, true, false, false, false);
                image2.get_gameObject().CustomSetActive(true);
                CUIUtility.SetImageGrey(image2, isGrey);
            }
            if (graphic != null)
            {
                graphic.set_text(GetRankName(rankGrade, rankClass));
                CUIUtility.SetImageGrey(graphic, isGrey);
            }
        }

        public static void ShowRankDetail(GameObject go, byte rankGrade, uint rankClass, uint score, bool bShowScore = true, bool useSmall = false, bool isLadderRewardForm = false, bool isUseSpecialColorWhenSuperKing = false, bool isImgSamll = true)
        {
            DebugHelper.Assert(go != null, "GameObject is NULL!!!");
            DebugHelper.Assert(rankGrade > 0, "grade must be above 0!!!");
            Image image = (go.get_transform().Find("ImgRank") == null) ? null : go.get_transform().Find("ImgRank").GetComponent<Image>();
            Image image2 = (go.get_transform().Find("ImgSubRank") == null) ? null : go.get_transform().Find("ImgSubRank").GetComponent<Image>();
            Text text = (go.get_transform().Find("txtRankName") == null) ? null : go.get_transform().Find("txtRankName").GetComponent<Text>();
            Text text2 = (go.get_transform().Find("txtTopGroupScore") == null) ? null : go.get_transform().Find("txtTopGroupScore").GetComponent<Text>();
            if (image != null)
            {
                string rankIconPath = GetRankIconPath(rankGrade, rankClass);
                string format = "{0}";
                if (isImgSamll)
                {
                    format = format + "_small";
                }
                image.SetSprite(useSmall ? string.Format(format, rankIconPath) : rankIconPath, null, true, false, false, false);
                image.get_gameObject().CustomSetActive(true);
            }
            if (image2 != null)
            {
                if (isLadderRewardForm && (rankGrade >= CLadderSystem.MAX_RANK_LEVEL))
                {
                    image2.get_gameObject().CustomSetActive(false);
                }
                else
                {
                    image2.SetSprite(GetSubRankIconPath(rankGrade, rankClass), null, true, false, false, false);
                    image2.get_gameObject().CustomSetActive(true);
                }
            }
            if (text != null)
            {
                text.set_text(GetRankName(rankGrade, rankClass));
                if (isUseSpecialColorWhenSuperKing && IsSuperKing(rankGrade, rankClass))
                {
                    text.set_text("<color=#feba29>" + text.get_text() + "</color>");
                }
            }
            if (text2 != null)
            {
                if (rankGrade >= CLadderSystem.MAX_RANK_LEVEL)
                {
                    text2.set_text(string.Format("x{0}", score));
                }
                text2.get_gameObject().CustomSetActive(rankGrade >= CLadderSystem.MAX_RANK_LEVEL);
            }
            Transform transform = go.get_transform().Find("ScoreCon");
            if (transform != null)
            {
                if ((rankGrade >= CLadderSystem.MAX_RANK_LEVEL) || !bShowScore)
                {
                    transform.get_gameObject().CustomSetActive(false);
                }
                else
                {
                    transform.Find("Con3Star").get_gameObject().CustomSetActive(false);
                    transform.Find("Con4Star").get_gameObject().CustomSetActive(false);
                    transform.Find("Con5Star").get_gameObject().CustomSetActive(false);
                    ResRankGradeConf dataByKey = GameDataMgr.rankGradeDatabin.GetDataByKey((uint) rankGrade);
                    if (dataByKey != null)
                    {
                        Transform transform2 = transform.Find(string.Format("Con{0}Star", dataByKey.dwGradeUpNeedScore));
                        if (transform2 != null)
                        {
                            transform2.get_gameObject().CustomSetActive(true);
                            for (int i = 1; i <= dataByKey.dwGradeUpNeedScore; i++)
                            {
                                Image component = transform2.Find(string.Format("ImgScore{0}", i)).GetComponent<Image>();
                                string prefabPath = (score < i) ? LADDER_IMG_STAR_EMPTY : LADDER_IMG_STAR;
                                if (component != null)
                                {
                                    component.SetSprite(prefabPath, null, true, false, false, false);
                                }
                            }
                        }
                    }
                    transform.get_gameObject().CustomSetActive(true);
                }
            }
        }

        private static void ShowRewardPanel(CUIFormScript form, COMDT_RANKDETAIL data)
        {
            if (data != null)
            {
                Text component = form.GetWidget(5).GetComponent<Text>();
                uint num = (data.bMaxSeasonGrade <= 0) ? 1 : ((uint) data.bMaxSeasonGrade);
                component.set_text(Singleton<CLadderSystem>.GetInstance().GetRewardDesc((byte) num));
                GameObject widget = form.GetWidget(12);
                CUseable skinRewardUseable = Singleton<CLadderSystem>.GetInstance().GetSkinRewardUseable();
                CUICommonSystem.SetItemCell(form, widget, skinRewardUseable, true, false, false, false);
                form.GetWidget(14).CustomSetActive(Singleton<CLadderSystem>.GetInstance().IsGotSkinReward());
                ShowSeasonEndRewardPanel(form);
            }
        }

        public static void ShowSeasonEndGetRewardForm(byte grade)
        {
            ResRankRewardConf dataByKey = GameDataMgr.rankRewardDatabin.GetDataByKey((uint) grade);
            if (dataByKey != null)
            {
                ListView<CUseable> inList = new ListView<CUseable>();
                for (int i = 0; i < dataByKey.astRewardDetail.Length; i++)
                {
                    ResDT_ChapterRewardInfo info = dataByKey.astRewardDetail[i];
                    if (info.bType != 0)
                    {
                        CUseable item = CUseableManager.CreateUsableByServerType((RES_REWARDS_TYPE) info.bType, (int) info.dwNum, info.dwID);
                        if (item != null)
                        {
                            inList.Add(item);
                        }
                    }
                }
                Singleton<CUIManager>.GetInstance().OpenAwardTip(LinqS.ToArray<CUseable>(inList), Singleton<CTextManager>.GetInstance().GetText("Ladder_Season_Reward"), false, enUIEventID.Ladder_ReqGetSeasonReward, false, false, "Form_Award");
            }
        }

        private static void ShowSeasonEndRewardPanel(CUIFormScript form)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                uint key = (masterRoleInfo.m_rankSeasonHighestGrade <= 0) ? 1 : ((uint) masterRoleInfo.m_rankSeasonHighestGrade);
                ResRankRewardConf dataByKey = GameDataMgr.rankRewardDatabin.GetDataByKey(key);
                if (dataByKey != null)
                {
                    ListView<CUseable> view = new ListView<CUseable>();
                    for (int i = 0; i < dataByKey.astRewardDetail.Length; i++)
                    {
                        ResDT_ChapterRewardInfo info2 = dataByKey.astRewardDetail[i];
                        if (info2.bType != 0)
                        {
                            CUseable item = CUseableManager.CreateUsableByServerType((RES_REWARDS_TYPE) info2.bType, (int) info2.dwNum, info2.dwID);
                            if (item != null)
                            {
                                view.Add(item);
                            }
                        }
                    }
                    if (view.Count > 0)
                    {
                        GameObject widget = form.GetWidget(13);
                        CUICommonSystem.SetItemCell(form, widget, view[0], true, false, false, false);
                    }
                }
            }
        }

        private static void ShowSuperKingRankPanel(CUIFormScript form)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            bool bActive = IsSuperKing(masterRoleInfo.m_rankGrade, masterRoleInfo.m_rankClass);
            GameObject widget = form.GetWidget(10);
            widget.CustomSetActive(bActive);
            if (bActive)
            {
                CUICommonSystem.SetRankDisplay(masterRoleInfo.m_rankClass, widget.get_transform());
            }
        }

        [CompilerGenerated]
        private sealed class <GetRankBigGradeName>c__AnonStorey69
        {
            internal byte rankBigGrade;

            internal bool <>m__6F(ResRankGradeConf x)
            {
                return (x.bBelongBigGrade == this.rankBigGrade);
            }
        }
    }
}

