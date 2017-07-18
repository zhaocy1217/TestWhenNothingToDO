namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class CAchieveShareComponent
    {
        private const string AchievementShareFormPrefabPath = "UGUI/Form/System/Achieve/Form_Achievement_Share.prefab";
        private float m_achievePointsFrom;
        private float m_achievePointsTo = 1f;
        private float m_containerWidth = 260f;
        private CAchieveItem2 m_curAchieveItem;
        private CTrophyRewardInfo m_curTrophyRewardInfo;
        private uint m_endPoint;
        private bool m_isNewTrophy;
        private bool m_isShowing;
        private uint m_nextPoint;
        private CUIFormScript m_shareForm;
        private uint m_startPoint;
        private static LTDescr m_trophyLevelLTD;
        private static LTDescr m_TrophyPointsAddLTD;
        private static LTDescr m_trophyPointsProgressLTD;
        private const float TweenTime = 2f;

        public CAchieveShareComponent()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Achievement_Close_Share_Form, new CUIEventManager.OnUIEventHandler(this.OnContinueProcessDoneAchievements));
        }

        private void DoTrophyPointsAddTweenEnd()
        {
            Text component = this.m_shareForm.GetWidget(0x11).GetComponent<Text>();
            if ((m_TrophyPointsAddLTD != null) && (component != null))
            {
                component.set_text(string.Format("+{0}", this.m_curAchieveItem.Cfg.dwPoint.ToString("N0")));
                m_TrophyPointsAddLTD.cancel();
                m_TrophyPointsAddLTD = null;
            }
        }

        private void DoTrophyPointsProgressTweenEnd(float value)
        {
            RectTransform component = this.m_shareForm.GetWidget(14).GetComponent<RectTransform>();
            if ((m_trophyPointsProgressLTD != null) && (component != null))
            {
                component.set_sizeDelta(new Vector2(value * this.m_containerWidth, component.get_sizeDelta().y));
                m_trophyPointsProgressLTD.cancel();
                m_trophyPointsProgressLTD = null;
            }
        }

        private void DoTrophyTween()
        {
            if (this.m_shareForm != null)
            {
                GameObject widget = this.m_shareForm.GetWidget(13);
                RectTransform component = this.m_shareForm.GetWidget(15).GetComponent<RectTransform>();
                RectTransform transform2 = this.m_shareForm.GetWidget(14).GetComponent<RectTransform>();
                Text text = this.m_shareForm.GetWidget(0x11).GetComponent<Text>();
                RectTransform transform3 = widget.GetComponent<RectTransform>();
                if (transform3 != null)
                {
                    this.m_containerWidth = transform3.get_rect().get_width();
                }
                component.set_sizeDelta(new Vector2(this.m_containerWidth * this.m_achievePointsFrom, component.get_sizeDelta().y));
                if (transform2 != null)
                {
                    m_trophyPointsProgressLTD = LeanTween.value(transform2.get_gameObject(), new Action<float>(this.TrophyPointsProgressTween), this.m_achievePointsFrom, this.m_achievePointsTo, 2f);
                }
                if (text != null)
                {
                    m_TrophyPointsAddLTD = LeanTween.value(text.get_gameObject(), new Action<float>(this.TrophyPointsAddTween), 0f, (float) this.m_curAchieveItem.Cfg.dwPoint, 2f);
                }
            }
        }

        private void OnContinueProcessDoneAchievements(CUIEvent uiEvent)
        {
            this.m_isShowing = false;
            CAchieveInfo2 masterAchieveInfo = CAchieveInfo2.GetMasterAchieveInfo();
            if (masterAchieveInfo.MostLatelyDoneAchievements.Count != 0)
            {
                masterAchieveInfo.MostLatelyDoneAchievements.RemoveAt(0);
                Singleton<CTimerManager>.GetInstance().AddTimer(200, 1, delegate (int sequence) {
                    this.Process(false);
                });
            }
        }

        private void OpenShareForm(uint achievementId)
        {
            this.m_shareForm = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Achieve/Form_Achievement_Share.prefab", false, true);
            if (this.m_shareForm != null)
            {
                this.m_isShowing = true;
                this.RefreshData(achievementId);
                this.RefreshShareForm();
            }
        }

        public void Process(bool force = false)
        {
            if (!this.m_isShowing)
            {
                CAchieveInfo2 masterAchieveInfo = CAchieveInfo2.GetMasterAchieveInfo();
                if ((masterAchieveInfo.MostLatelyDoneAchievements.Count != 0) && ((Singleton<CLobbySystem>.GetInstance().IsInLobbyForm() && !Singleton<CMatchingSystem>.GetInstance().IsInMatching) && (!Singleton<CMatchingSystem>.GetInstance().IsInMatchingTeam && (Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_CONFIRMBOX) == null))))
                {
                    if (!force)
                    {
                        string[] strArray = new string[] { Singleton<CMallSystem>.GetInstance().sMallFormPath, "Form_NewHeroOrSkin.prefab" };
                        for (int i = 0; i < strArray.Length; i++)
                        {
                            CUIFormScript script = Singleton<CUIManager>.GetInstance().GetForm(strArray[i]);
                            if ((script != null) && !script.IsClosed())
                            {
                                return;
                            }
                        }
                    }
                    CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("Form_NobeLevelUp.prefab");
                    if ((form == null) || form.IsClosed())
                    {
                        uint key = masterAchieveInfo.MostLatelyDoneAchievements[0];
                        if (masterAchieveInfo.m_AchiveItemDic.ContainsKey(key))
                        {
                            this.OpenShareForm(key);
                        }
                    }
                }
            }
        }

        private void RefreshData(uint achievementId)
        {
            this.ResetData();
            CAchieveInfo2 masterAchieveInfo = CAchieveInfo2.GetMasterAchieveInfo();
            this.m_curAchieveItem = masterAchieveInfo.m_AchiveItemDic[achievementId];
            if (this.m_curAchieveItem.GetHead() == this.m_curAchieveItem)
            {
                this.m_isNewTrophy = true;
            }
            else
            {
                this.m_isNewTrophy = false;
            }
            uint cur = 0;
            masterAchieveInfo.GetTrophyProgress(ref cur, ref this.m_nextPoint);
            uint num2 = 0;
            for (int i = 0; i < masterAchieveInfo.MostLatelyDoneAchievements.Count; i++)
            {
                CAchieveItem2 item2 = masterAchieveInfo.m_AchiveItemDic[masterAchieveInfo.MostLatelyDoneAchievements[i]];
                num2 += item2.Cfg.dwPoint;
            }
            this.m_startPoint = cur - num2;
            this.m_endPoint = this.m_startPoint + this.m_curAchieveItem.Cfg.dwPoint;
            CTrophyRewardInfo trophyRewardInfoByPoint = masterAchieveInfo.GetTrophyRewardInfoByPoint(this.m_startPoint);
            CTrophyRewardInfo info3 = this.m_curTrophyRewardInfo = masterAchieveInfo.GetTrophyRewardInfoByPoint(this.m_endPoint);
            CTrophyRewardInfo trophyRewardInfoByIndex = masterAchieveInfo.GetTrophyRewardInfoByIndex(info3.Index + 1);
            if (trophyRewardInfoByPoint.Cfg.dwTrophyLvl == info3.Cfg.dwTrophyLvl)
            {
                this.m_achievePointsFrom = Utility.Divide(this.m_startPoint - trophyRewardInfoByIndex.MinPoint, trophyRewardInfoByIndex.GetPointStep());
                this.m_achievePointsTo = Utility.Divide(this.m_endPoint - trophyRewardInfoByIndex.MinPoint, trophyRewardInfoByIndex.GetPointStep());
            }
            else
            {
                this.m_achievePointsFrom = 0f;
                this.m_achievePointsTo = Utility.Divide(this.m_endPoint - trophyRewardInfoByIndex.MinPoint, trophyRewardInfoByIndex.GetPointStep());
            }
        }

        private void RefreshShareForm()
        {
            if (this.m_curTrophyRewardInfo != null)
            {
                if (this.m_shareForm == null)
                {
                    this.m_shareForm = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Achieve/Form_Achievement_Share.prefab");
                }
                if (this.m_shareForm != null)
                {
                    CAchieveInfo2 masterAchieveInfo = CAchieveInfo2.GetMasterAchieveInfo();
                    CTrophyRewardInfo trophyRewardInfoByIndex = masterAchieveInfo.GetTrophyRewardInfoByIndex(this.m_curTrophyRewardInfo.Index + 1);
                    if (this.m_isNewTrophy)
                    {
                    }
                    this.m_shareForm.GetWidget(0).GetComponent<Text>().set_text(this.m_curAchieveItem.Cfg.szName);
                    this.m_shareForm.GetWidget(5).GetComponent<Text>().set_text(this.m_curAchieveItem.GetAchievementDesc());
                    this.m_shareForm.GetWidget(7).GetComponent<Text>().set_text(this.m_curAchieveItem.GetAchievementTips());
                    this.m_shareForm.GetWidget(6).GetComponent<Image>().SetSprite(this.m_curAchieveItem.GetAchieveImagePath(), this.m_shareForm, true, false, false, false);
                    CAchievementSystem.SetAchieveBaseIcon(this.m_shareForm.GetWidget(0x12).get_transform(), this.m_curAchieveItem, this.m_shareForm);
                    Image component = this.m_shareForm.GetWidget(20).GetComponent<Image>();
                    if (masterAchieveInfo.LastDoneTrophyRewardInfo != null)
                    {
                        component.SetSprite(masterAchieveInfo.LastDoneTrophyRewardInfo.GetTrophyImagePath(), this.m_shareForm, true, false, false, false);
                    }
                    GameObject widget = this.m_shareForm.GetWidget(12);
                    GameObject obj5 = this.m_shareForm.GetWidget(0x13);
                    Text text4 = this.m_shareForm.GetWidget(11).GetComponent<Text>();
                    Text text5 = widget.GetComponent<Text>();
                    this.m_shareForm.GetWidget(0x10).GetComponent<Text>().set_text(string.Format("{0}/{1}", this.m_endPoint - trophyRewardInfoByIndex.MinPoint, trophyRewardInfoByIndex.GetPointStep()));
                    text4.set_text(this.m_curTrophyRewardInfo.Cfg.dwTrophyLvl.ToString());
                    if (masterAchieveInfo.GetWorldRank() == 0)
                    {
                        obj5.CustomSetActive(true);
                        widget.CustomSetActive(false);
                    }
                    else
                    {
                        widget.CustomSetActive(true);
                        text5.set_text(masterAchieveInfo.GetWorldRank().ToString());
                        obj5.CustomSetActive(false);
                    }
                    this.DoTrophyTween();
                    ShareSys.SetSharePlatfText(this.m_shareForm.GetWidget(10).GetComponent<Text>());
                    if (CSysDynamicBlock.bSocialBlocked)
                    {
                        Transform transform = this.m_shareForm.get_transform().Find("Panel_ShareAchievement_Btn");
                        if (transform != null)
                        {
                            transform.get_gameObject().CustomSetActive(false);
                        }
                        Transform transform2 = this.m_shareForm.get_transform().Find("Panel_NewAchievement_Btn/Btn_Share");
                        if (transform2 != null)
                        {
                            transform2.get_gameObject().CustomSetActive(false);
                        }
                    }
                }
            }
        }

        public void ResetData()
        {
            this.m_curAchieveItem = null;
            this.m_curTrophyRewardInfo = null;
            this.m_nextPoint = 0;
            this.m_startPoint = 0;
            this.m_endPoint = 0;
            this.m_achievePointsFrom = 0f;
            this.m_achievePointsTo = 1f;
            this.m_isNewTrophy = false;
            this.m_curAchieveItem = null;
            this.m_curTrophyRewardInfo = null;
        }

        private void TrophyPointsAddTween(float value)
        {
            if (this.m_shareForm != null)
            {
                Text component = this.m_shareForm.GetWidget(0x11).GetComponent<Text>();
                if (component != null)
                {
                    component.set_text(string.Format("+{0}", value.ToString("N0")));
                    if (value >= this.m_curAchieveItem.Cfg.dwPoint)
                    {
                        this.DoTrophyPointsAddTweenEnd();
                    }
                }
            }
        }

        private void TrophyPointsProgressTween(float value)
        {
            if (this.m_shareForm != null)
            {
                RectTransform component = this.m_shareForm.GetWidget(14).GetComponent<RectTransform>();
                if (component.get_gameObject() != null)
                {
                    if (value > 1f)
                    {
                        this.DoTrophyPointsProgressTweenEnd(value);
                    }
                    component.set_sizeDelta(new Vector2(value * this.m_containerWidth, component.get_sizeDelta().y));
                }
            }
        }
    }
}

