namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class CQualifyingSystem : Singleton<CQualifyingSystem>
    {
        public int m_areaIndex;
        public int m_menuIndex;
        public SCPKG_CLASSOFRANKDETAIL_NTF m_rankBaseInfo;
        public ListView<CSDT_CLASSOFRANKDETAIL> m_rankList = new ListView<CSDT_CLASSOFRANKDETAIL>();
        public static string s_qualifyingFormPath = "UGUI/Form/System/Qualifying/Form_Qualifying.prefab";

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Qualifying_OpenForm, new CUIEventManager.OnUIEventHandler(this.Qualifying_OpenForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Qualifying_MenuSelect, new CUIEventManager.OnUIEventHandler(this.Qualifying_MenuSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Qualifying_BattleAreaBtnDown, new CUIEventManager.OnUIEventHandler(this.Qualifying_BattleAreaBtnDown));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Qualifying_BattleAreaBtnUp, new CUIEventManager.OnUIEventHandler(this.Qualifying_BattleAreaBtnUp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Qualifying_RankListSelect, new CUIEventManager.OnUIEventHandler(this.Qualifying_RankListSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Qualifying_RankListElementInit, new CUIEventManager.OnUIEventHandler(this.Qualifying_RankListElementInit));
        }

        private void InitMenu()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_qualifyingFormPath);
            if (form != null)
            {
                CUIListScript component = form.get_gameObject().get_transform().Find("Panel/ListMenu").get_gameObject().GetComponent<CUIListScript>();
                string[] strArray = new string[] { Singleton<CTextManager>.GetInstance().GetText("Qualifying_Menu1"), Singleton<CTextManager>.GetInstance().GetText("Qualifying_Menu2") };
                component.SetElementAmount(strArray.Length);
                for (int i = 0; i < component.m_elementAmount; i++)
                {
                    component.GetElemenet(i).get_gameObject().get_transform().Find("Text").GetComponent<Text>().set_text(strArray[i]);
                }
                this.m_menuIndex = 0;
            }
        }

        private void Qualifying_BattleAreaBtnDown(CUIEvent uiEvent)
        {
        }

        private void Qualifying_BattleAreaBtnUp(CUIEvent uiEvent)
        {
        }

        private void Qualifying_MenuSelect(CUIEvent uiEvent)
        {
            this.m_menuIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
            GameObject obj2 = Singleton<CUIManager>.GetInstance().GetForm(s_qualifyingFormPath).get_gameObject();
            GameObject obj3 = obj2.get_transform().Find("Panel/Panel_HeroInfo").get_gameObject();
            GameObject obj4 = obj2.get_transform().Find("Panel/Panel_RankInfo").get_gameObject();
            obj3.CustomSetActive(false);
            obj4.CustomSetActive(false);
            if (this.m_menuIndex == 0)
            {
                obj3.CustomSetActive(true);
                this.RefreshHeroInfo();
            }
            else if (this.m_menuIndex == 1)
            {
                if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_rankGrade == 0)
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("You have not rank info", false, 1.5f, null, new object[0]);
                }
                else
                {
                    obj4.CustomSetActive(true);
                    this.RefreshRankInfo();
                }
            }
        }

        private void Qualifying_OpenForm(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().OpenForm(s_qualifyingFormPath, false, true);
            this.InitMenu();
            this.RefreshForm();
        }

        private void Qualifying_RankListElementInit(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            GameObject srcWidget = uiEvent.m_srcWidget;
            Text component = srcWidget.get_transform().Find("Cell/lblContent1").GetComponent<Text>();
            Text text2 = srcWidget.get_transform().Find("Cell/lblContent2").GetComponent<Text>();
            Text text3 = srcWidget.get_transform().Find("Cell/lblContent3").GetComponent<Text>();
            Text text4 = srcWidget.get_transform().Find("Cell/lblContent4").GetComponent<Text>();
            CSDT_CLASSOFRANKDETAIL csdt_classofrankdetail = this.m_rankList[srcWidgetIndexInBelongedList];
            component.set_text((srcWidgetIndexInBelongedList + 1).ToString());
            text2.set_text(StringHelper.UTF8BytesToString(ref csdt_classofrankdetail.stDetail.szAcntName));
            text3.set_text(csdt_classofrankdetail.stDetail.dwWinCnt.ToString());
            text4.set_text(csdt_classofrankdetail.stDetail.bScore.ToString());
        }

        private void Qualifying_RankListSelect(CUIEvent uiEvent)
        {
            int selectedIndex = uiEvent.m_srcWidget.get_gameObject().GetComponent<CUIListScript>().GetSelectedIndex();
            CUIListScript component = uiEvent.m_srcFormScript.get_gameObject().get_transform().Find("Panel/Panel_RankInfo").get_gameObject().get_transform().Find("Panel_Left/ListHeroIno").GetComponent<CUIListScript>();
            CSDT_CLASSOFRANKDETAIL csdt_classofrankdetail = this.m_rankList[selectedIndex];
            component.SetElementAmount(csdt_classofrankdetail.stDetail.astCommonUseHero.Length);
            for (int i = 0; i < csdt_classofrankdetail.stDetail.astCommonUseHero.Length; i++)
            {
                GameObject obj4 = component.GetElemenet(i).get_gameObject();
                Image image = obj4.get_transform().Find("heroInfo/imgRank").GetComponent<Image>();
                Text text = obj4.get_transform().Find("heroInfo/lblRank").GetComponent<Text>();
                if (csdt_classofrankdetail.stDetail.astCommonUseHero[i].dwHeroId == 0)
                {
                    obj4.CustomSetActive(false);
                }
                else
                {
                    image.SetSprite(CUIUtility.s_Sprite_System_Qualifying_Dir + "ranking_icon" + csdt_classofrankdetail.stDetail.astCommonUseHero[i].bHeroProficiencyLv, uiEvent.m_srcFormScript, true, false, false, false);
                    text.set_text(csdt_classofrankdetail.stDetail.astCommonUseHero[i].dwHeroProficiency.ToString());
                    obj4.CustomSetActive(true);
                }
            }
        }

        [MessageHandler(0xa28)]
        public static void ReciveRankAreaInfo(CSPkg msg)
        {
            SCPKG_CLASSOFRANKDETAIL_NTF stClassOfRankDetailNtf = msg.stPkgData.stClassOfRankDetailNtf;
            if ((Singleton<CQualifyingSystem>.GetInstance().m_rankBaseInfo != null) && (Singleton<CQualifyingSystem>.GetInstance().m_rankBaseInfo.bGrade != stClassOfRankDetailNtf.bGrade))
            {
                Singleton<CQualifyingSystem>.GetInstance().m_rankList.Clear();
            }
            Singleton<CQualifyingSystem>.GetInstance().m_rankBaseInfo = stClassOfRankDetailNtf;
            Singleton<CQualifyingSystem>.GetInstance().m_rankList.AddRange(stClassOfRankDetailNtf.astRecoed);
        }

        public void RefreshForm()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_qualifyingFormPath);
            if (form != null)
            {
                CUIListScript component = form.get_gameObject().get_transform().Find("Panel/ListMenu").get_gameObject().GetComponent<CUIListScript>();
                component.m_alwaysDispatchSelectedChangeEvent = true;
                component.SelectElement(this.m_menuIndex, true);
                component.m_alwaysDispatchSelectedChangeEvent = false;
            }
        }

        public void RefreshHeroInfo()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_qualifyingFormPath);
            if (form != null)
            {
                ListView<IHeroData> hostHeroList = CHeroDataFactory.GetHostHeroList(true, CMallSortHelper.HeroViewSortType.Name);
                GameObject obj2 = form.get_gameObject().get_transform().Find("Panel/Panel_HeroInfo").get_gameObject();
                Text component = obj2.get_transform().Find("lblProficiency").GetComponent<Text>();
                CUIListScript script2 = obj2.get_transform().Find("ListHeroIno").GetComponent<CUIListScript>();
                script2.SetElementAmount(hostHeroList.Count);
                for (int i = 0; i < hostHeroList.Count; i++)
                {
                    GameObject obj3 = script2.GetElemenet(i).get_gameObject();
                    Image image = obj3.get_transform().Find("heroInfo/imgRank").GetComponent<Image>();
                    Text text2 = obj3.get_transform().Find("heroInfo/lblRank").GetComponent<Text>();
                    image.SetSprite(CUIUtility.s_Sprite_System_Qualifying_Dir + "ranking_icon" + hostHeroList[i].proficiencyLV, form, true, false, false, false);
                    text2.set_text(hostHeroList[i].proficiency.ToString());
                }
                uint num2 = 0;
                for (int j = 0; j < hostHeroList.Count; j++)
                {
                    num2 += hostHeroList[j].proficiency;
                }
                string[] values = new string[] { num2.ToString() };
                component.set_text(CUIUtility.StringReplace(Singleton<CTextManager>.GetInstance().GetText("Qualifying_Title0"), values));
            }
        }

        public void RefreshRankInfo()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_qualifyingFormPath);
            if (form != null)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                GameObject obj2 = form.get_gameObject().get_transform().Find("Panel/Panel_RankInfo").get_gameObject();
                Text component = obj2.get_transform().Find("Panel_Top/lblName").GetComponent<Text>();
                Text text2 = obj2.get_transform().Find("Panel_Top/lblContent1").GetComponent<Text>();
                Text text3 = obj2.get_transform().Find("Panel_Top/lblContent2").GetComponent<Text>();
                Text text4 = obj2.get_transform().Find("Panel_Top/lblContent3").GetComponent<Text>();
                Text text5 = obj2.get_transform().Find("Panel_Top/lblContent4").GetComponent<Text>();
                Text text6 = obj2.get_transform().Find("Panel_Center/lblName").GetComponent<Text>();
                CUIListScript script2 = obj2.get_transform().Find("Panel_Center/List").GetComponent<CUIListScript>();
                component.set_text(Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().Name);
                ResRankGradeConf dataByKey = GameDataMgr.rankGradeDatabin.GetDataByKey((uint) masterRoleInfo.m_rankGrade);
                text2.set_text(StringHelper.UTF8BytesToString(ref dataByKey.szGradeDesc));
                text3.set_text(this.m_rankBaseInfo.dwSelfTotalFightCnt.ToString());
                text4.set_text(this.m_rankBaseInfo.dwSelfFightWinCnt.ToString());
                text5.set_text(this.m_rankBaseInfo.dwSelfScore.ToString());
                text6.set_text(this.m_rankBaseInfo.dwClass.ToString() + " Area");
                this.m_rankList.Sort(new Comparison<CSDT_CLASSOFRANKDETAIL>(this.SortCompare));
                script2.SetElementAmount(this.m_rankList.Count);
                int index = 0;
                for (int i = 0; i < this.m_rankList.Count; i++)
                {
                    if (this.m_rankList[i].stAcntUin.ullUid == masterRoleInfo.playerUllUID)
                    {
                        index = i;
                        break;
                    }
                }
                if (this.m_rankList.Count > 0)
                {
                    script2.SelectElement(index, true);
                    script2.MoveElementInScrollArea(index, true);
                }
            }
        }

        private int SortCompare(CSDT_CLASSOFRANKDETAIL info1, CSDT_CLASSOFRANKDETAIL info2)
        {
            return ((info2.stDetail.bScore <= info1.stDetail.bScore) ? 0 : 1);
        }
    }
}

