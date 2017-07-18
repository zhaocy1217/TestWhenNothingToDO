namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class CExploreView
    {
        private static float lastScrollX = 0f;
        public static readonly enUIEventID[] s_eventIDs = new enUIEventID[] { enUIEventID.Arena_OpenForm, enUIEventID.Adv_OpenChapterForm, enUIEventID.Burn_OpenForm };
        public static readonly Color[] s_exploreColors;
        public static readonly string[] s_exploreTypes;
        public static readonly RES_SPECIALFUNCUNLOCK_TYPE[] s_unlockTypes;

        static CExploreView()
        {
            RES_SPECIALFUNCUNLOCK_TYPE[] res_specialfuncunlock_typeArray1 = new RES_SPECIALFUNCUNLOCK_TYPE[3];
            res_specialfuncunlock_typeArray1[0] = RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ARENA;
            res_specialfuncunlock_typeArray1[2] = RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_LIUGUOYUANZHENG;
            s_unlockTypes = res_specialfuncunlock_typeArray1;
            s_exploreTypes = new string[] { "Explore_Common_Type_2", "Explore_Common_Type_1", "Explore_Common_Type_3" };
            s_exploreColors = new Color[] { new Color(1f, 0f, 0.8470588f), new Color(0f, 0.627451f, 1f), new Color(1f, 0f, 0.04313726f) };
        }

        public static void InitExloreList(CUIFormScript form)
        {
            if (form != null)
            {
                int length = s_eventIDs.Length;
                CUIListElementScript elemenet = null;
                CUIStepListScript component = form.get_transform().Find("ExploreList").get_gameObject().GetComponent<CUIStepListScript>();
                component.SetElementAmount(length);
                for (int i = 0; i < length; i++)
                {
                    elemenet = component.GetElemenet(i);
                    elemenet.GetComponent<CUIEventScript>().m_onClickEventID = s_eventIDs[i];
                    elemenet.get_gameObject().get_transform().Find("TitleBg/ExlporeNameText").GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText(s_exploreTypes[i]));
                    elemenet.get_gameObject().get_transform().Find("TitleBg/Image").GetComponent<Image>().set_color(s_exploreColors[i]);
                    Image image2 = elemenet.get_gameObject().get_transform().Find("Icon").get_gameObject().GetComponent<Image>();
                    GameObject prefab = CUIUtility.GetSpritePrefeb(CUIUtility.s_Sprite_Dynamic_Adventure_Dir + (i + 1), false, false);
                    if (prefab != null)
                    {
                        image2.SetSprite(prefab, false);
                    }
                    GameObject obj3 = elemenet.get_transform().FindChild("Lock").get_gameObject();
                    GameObject obj4 = elemenet.get_transform().FindChild("Unlock").get_gameObject();
                    RES_SPECIALFUNCUNLOCK_TYPE type = s_unlockTypes[i];
                    if (Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(type))
                    {
                        image2.set_color(CUIUtility.s_Color_White);
                        obj3.CustomSetActive(false);
                    }
                    else
                    {
                        image2.set_color(CUIUtility.s_Color_GrayShader);
                        obj3.CustomSetActive(true);
                        ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey((uint) type);
                        if (dataByKey != null)
                        {
                            obj3.GetComponentInChildren<Text>().set_text(Utility.UTF8Convert(dataByKey.szLockedTip));
                        }
                    }
                    if (s_unlockTypes[i] == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_NONE)
                    {
                        int lastChapter = CAdventureSys.GetLastChapter(1);
                        ResChapterInfo info = GameDataMgr.chapterInfoDatabin.GetDataByKey((long) lastChapter);
                        if (info != null)
                        {
                            obj4.CustomSetActive(true);
                            obj4.GetComponentInChildren<Text>().set_text(string.Format(Singleton<CTextManager>.instance.GetText("Adventure_Chapter_Max_Tips"), Utility.UTF8Convert(info.szChapterName)));
                        }
                    }
                    else if (s_unlockTypes[i] == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ARENA)
                    {
                        if ((Singleton<CArenaSystem>.GetInstance().m_fightHeroInfoList == null) || (Singleton<CArenaSystem>.GetInstance().m_fightHeroInfoList.stArenaInfo.dwSelfRank == 0))
                        {
                            obj4.CustomSetActive(false);
                        }
                        else
                        {
                            string str = string.Empty;
                            str = string.Format(Singleton<CTextManager>.GetInstance().GetText("ExploreArenaRankText"), Singleton<CArenaSystem>.GetInstance().m_fightHeroInfoList.stArenaInfo.dwSelfRank);
                            obj4.get_gameObject().get_transform().FindChild("Text").get_gameObject().GetComponent<Text>().set_text(str);
                            obj4.CustomSetActive(true);
                        }
                    }
                    else if (s_unlockTypes[i] == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_LIUGUOYUANZHENG)
                    {
                        BurnExpeditionModel model = Singleton<BurnExpeditionController>.GetInstance().model;
                        if (model._data == null)
                        {
                            obj4.CustomSetActive(false);
                        }
                        else
                        {
                            string str2 = string.Empty;
                            if (model.IsAllCompelte())
                            {
                                str2 = string.Format(Singleton<CTextManager>.GetInstance().GetText("ExploreBurnFinishText"), new object[0]);
                            }
                            else
                            {
                                str2 = string.Format(Singleton<CTextManager>.GetInstance().GetText("ExploreBurnText"), Math.Max(1, model.Get_LastUnlockLevelIndex(model.curDifficultyType) + 1));
                            }
                            obj4.get_gameObject().get_transform().FindChild("Text").get_gameObject().GetComponent<Text>().set_text(str2);
                            obj4.CustomSetActive(true);
                        }
                    }
                }
                component.SelectElementImmediately(1);
                Text text2 = form.get_gameObject().get_transform().FindChild("AwardGroup/Name1").get_gameObject().GetComponent<Text>();
                Text text3 = form.get_gameObject().get_transform().FindChild("AwardGroup/Name2").get_gameObject().GetComponent<Text>();
                Image image = form.get_gameObject().get_transform().FindChild("AwardGroup/Icon1").get_gameObject().GetComponent<Image>();
                Image image4 = form.get_gameObject().get_transform().FindChild("AwardGroup/Icon2").get_gameObject().GetComponent<Image>();
                text2.get_gameObject().CustomSetActive(false);
                text3.get_gameObject().CustomSetActive(false);
                image.get_gameObject().CustomSetActive(false);
                image4.get_gameObject().CustomSetActive(false);
                uint key = 0;
                try
                {
                    key = uint.Parse(Singleton<CTextManager>.GetInstance().GetText("ArenaAwardHeroId"));
                }
                catch (Exception)
                {
                }
                if (key != 0)
                {
                    ResHeroCfgInfo info2 = GameDataMgr.heroDatabin.GetDataByKey(key);
                    if (!Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().IsHaveHero(key, false) && (info2 != null))
                    {
                        text2.get_gameObject().CustomSetActive(true);
                        text2.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("ArenaAwardHero"), info2.szName));
                        image.get_gameObject().CustomSetActive(true);
                        image.SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, CSkinInfo.GetHeroSkinPic(key, 0)), form, true, false, false, false);
                    }
                }
                key = 0;
                try
                {
                    key = uint.Parse(Singleton<CTextManager>.GetInstance().GetText("BurningAwardHeroId"));
                }
                catch (Exception)
                {
                }
                if (key != 0)
                {
                    ResHeroCfgInfo info3 = GameDataMgr.heroDatabin.GetDataByKey(key);
                    if (!Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().IsHaveHero(key, false) && (info3 != null))
                    {
                        text3.get_gameObject().CustomSetActive(true);
                        text3.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("BurningAwardHero"), info3.szName));
                        image4.get_gameObject().CustomSetActive(true);
                        image4.SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, CSkinInfo.GetHeroSkinPic(key, 0)), form, true, false, false, false);
                    }
                }
                if (CSysDynamicBlock.bLobbyEntryBlocked)
                {
                    Transform transform = form.get_transform().FindChild("AwardGroup");
                    if (transform != null)
                    {
                        transform.get_gameObject().CustomSetActive(false);
                    }
                }
            }
        }

        public static void OnExploreListScroll(GameObject root)
        {
            CUIListScript component = root.get_transform().Find("ExploreList").get_gameObject().GetComponent<CUIListScript>();
            if (component != null)
            {
                Vector2 contentSize = component.GetContentSize();
                Vector2 scrollAreaSize = component.GetScrollAreaSize();
                Vector2 contentPosition = component.GetContentPosition();
                Vector2 vector4 = Vector2.get_zero();
                vector4.x = (contentSize.x != scrollAreaSize.x) ? (contentPosition.x / (contentSize.x - scrollAreaSize.x)) : 0f;
                float num = vector4.x - lastScrollX;
                lastScrollX = vector4.x;
                Transform transform = root.get_transform().Find("FW_MovePanel/textureFrame");
                float num2 = (num != 0f) ? ((num / (1f / ((float) (CAdventureSys.CHAPTER_NUM - 1)))) * 120f) : 0f;
                transform.Rotate(0f, 0f, num2);
            }
        }

        public static void RefreshExploreList()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CAdventureSys.EXLPORE_FORM_PATH);
            if (form != null)
            {
                InitExloreList(form);
            }
        }
    }
}

