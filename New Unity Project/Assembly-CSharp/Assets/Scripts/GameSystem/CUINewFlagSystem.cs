namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class CUINewFlagSystem : Singleton<CUINewFlagSystem>
    {
        private static string NewFlagSetStr = "1";

        public void AddNewFlag(GameObject obj, enNewFlagKey flagKey, enNewFlagPos newFlagPos = 2, float scale = 1, float offsetX = 0, float offsetY = 0, enNewFlagType newFlagType = 0)
        {
            if (obj != null)
            {
                Transform transform = obj.get_transform().Find("redDotNew");
                string str = flagKey.ToString();
                if (transform != null)
                {
                    if (((flagKey > enNewFlagKey.New_None) && (flagKey < enNewFlagKey.New_Count)) && !PlayerPrefs.HasKey(str))
                    {
                        this.DelNewFlag(obj, flagKey, true);
                    }
                }
                else if (((flagKey > enNewFlagKey.New_None) && (flagKey < enNewFlagKey.New_Count)) && !PlayerPrefs.HasKey(str))
                {
                    string str2 = string.Empty;
                    if (newFlagType == enNewFlagType.enNewFlag)
                    {
                        str2 = "redDotNew";
                    }
                    else
                    {
                        str2 = "redDot";
                    }
                    GameObject obj2 = Object.Instantiate(CUIUtility.GetSpritePrefeb("UGUI/Form/Common/" + str2, false, false)) as GameObject;
                    if (obj2 != null)
                    {
                        obj2.get_transform().SetParent(obj.get_transform(), false);
                        obj2.get_transform().SetAsLastSibling();
                        RectTransform transform2 = obj2.get_transform() as RectTransform;
                        Vector2 vector = new Vector2();
                        Vector2 vector2 = new Vector2();
                        Vector2 vector3 = new Vector2();
                        switch (newFlagPos)
                        {
                            case enNewFlagPos.enTopLeft:
                                vector.x = 0f;
                                vector.y = 1f;
                                vector2.x = 0f;
                                vector2.y = 1f;
                                vector3.x = 0f;
                                vector3.y = 1f;
                                break;

                            case enNewFlagPos.enTopCenter:
                                vector.x = 0.5f;
                                vector.y = 1f;
                                vector2.x = 0.5f;
                                vector2.y = 1f;
                                vector3.x = 0.5f;
                                vector3.y = 1f;
                                break;

                            case enNewFlagPos.enTopRight:
                                vector.x = 1f;
                                vector.y = 1f;
                                vector2.x = 1f;
                                vector2.y = 1f;
                                vector3.x = 1f;
                                vector3.y = 1f;
                                break;

                            case enNewFlagPos.enMiddleLeft:
                                vector.x = 0f;
                                vector.y = 0.5f;
                                vector2.x = 0f;
                                vector2.y = 0.5f;
                                vector3.x = 0f;
                                vector3.y = 0.5f;
                                break;

                            case enNewFlagPos.enMiddleCenter:
                                vector.x = 0.5f;
                                vector.y = 0.5f;
                                vector2.x = 0.5f;
                                vector2.y = 0.5f;
                                vector3.x = 0.5f;
                                vector3.y = 0.5f;
                                break;

                            case enNewFlagPos.enMiddleRight:
                                vector.x = 1f;
                                vector.y = 0.5f;
                                vector2.x = 1f;
                                vector2.y = 0.5f;
                                vector3.x = 1f;
                                vector3.y = 0.5f;
                                break;

                            case enNewFlagPos.enBottomLeft:
                                vector.x = 0f;
                                vector.y = 0f;
                                vector2.x = 0f;
                                vector2.y = 0f;
                                vector3.x = 0f;
                                vector3.y = 0f;
                                break;

                            case enNewFlagPos.enBottomCenter:
                                vector.x = 0.5f;
                                vector.y = 0f;
                                vector2.x = 0.5f;
                                vector2.y = 0f;
                                vector3.x = 0.5f;
                                vector3.y = 0f;
                                break;

                            case enNewFlagPos.enBottomRight:
                                vector.x = 1f;
                                vector.y = 0f;
                                vector2.x = 1f;
                                vector2.y = 0f;
                                vector3.x = 1f;
                                vector3.y = 0f;
                                break;
                        }
                        transform2.set_pivot(vector3);
                        transform2.set_anchorMin(vector);
                        transform2.set_anchorMax(vector2);
                        if (scale != 1f)
                        {
                            transform2.set_localScale(new Vector3(scale, scale, scale));
                        }
                        transform2.set_anchoredPosition(new Vector2(offsetX, offsetY));
                    }
                }
            }
        }

        public void DelNewFlag(GameObject obj, enNewFlagKey flagKey, bool immediately = true)
        {
            if (obj != null)
            {
                Transform transform = obj.get_transform().Find("redDotNew(Clone)");
                if (transform == null)
                {
                    transform = obj.get_transform().Find("redDot(Clone)");
                }
                if (transform != null)
                {
                    string str = flagKey.ToString();
                    if (!PlayerPrefs.HasKey(str))
                    {
                        PlayerPrefs.SetString(str, NewFlagSetStr);
                        PlayerPrefs.Save();
                        if (immediately)
                        {
                            Object.Destroy(transform.get_gameObject());
                        }
                    }
                }
            }
        }

        public void HideNewFlagForAchievementEntry()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLobbySystem.LOBBY_FORM_PATH);
            if (form != null)
            {
                GameObject obj2 = form.get_transform().Find("LobbyBottom/SysEntry/AchievementBtn").get_gameObject();
                if (obj2 != null)
                {
                    this.DelNewFlag(obj2, enNewFlagKey.New_Achievement_V1, true);
                }
            }
        }

        public void HideNewFlagForBeizhanEntry()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLobbySystem.LOBBY_FORM_PATH);
            if (form != null)
            {
                GameObject obj2 = form.get_transform().Find("LobbyBottom/SysEntry/ChatBtn").get_gameObject();
                if (obj2 != null)
                {
                    this.DelNewFlag(obj2, enNewFlagKey.New_BeizhanEntryBtn_V3, true);
                }
            }
        }

        public void HideNewFlagForMishuEntry()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLobbySystem.LOBBY_FORM_PATH);
            if (form != null)
            {
                GameObject obj2 = form.get_transform().Find("LobbyBottom/Newbie").get_gameObject();
                if (obj2 != null)
                {
                    this.DelNewFlag(obj2, enNewFlagKey.New_MishuEntryBtn_V1, true);
                }
            }
        }

        public bool IsHaveNewFlagKey(enNewFlagKey newFlagKey)
        {
            return PlayerPrefs.HasKey(newFlagKey.ToString());
        }

        public void SetAllNewFlagKey()
        {
            for (int i = 0; i < 0x22; i++)
            {
                PlayerPrefs.SetString(((enNewFlagKey) i).ToString(), NewFlagSetStr);
            }
        }

        public void SetNewFlagForArenaRankBtn(bool bShow)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(RankingSystem.s_rankingForm);
            if (form != null)
            {
                GameObject obj2 = form.get_transform().Find("bg/AllRankType/AllRankSelectMenu/ListElement8").get_gameObject();
                if (obj2 != null)
                {
                    if (bShow)
                    {
                        this.AddNewFlag(obj2, enNewFlagKey.New_ArenaRank_V1, enNewFlagPos.enTopRight, 1f, 0f, 0f, enNewFlagType.enNewFlag);
                    }
                    else
                    {
                        this.DelNewFlag(obj2, enNewFlagKey.New_ArenaRank_V1, true);
                    }
                }
            }
        }

        public void SetNewFlagForFriendEntry(bool bShow)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLobbySystem.SYSENTRY_FORM_PATH);
            if (form != null)
            {
                GameObject obj2 = form.get_transform().Find("PlayerBtn/FriendBtn").get_gameObject();
                if (obj2 != null)
                {
                    if (bShow)
                    {
                        this.AddNewFlag(obj2, enNewFlagKey.New_Lobby_Friend_V1, enNewFlagPos.enTopLeft, 0.8f, 0f, -2f, enNewFlagType.enNewFlag);
                    }
                    else
                    {
                        this.DelNewFlag(obj2, enNewFlagKey.New_Lobby_Friend_V1, true);
                    }
                }
            }
        }

        public void SetNewFlagForGodRankBtn(bool bShow)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(RankingSystem.s_rankingForm);
            if (form != null)
            {
                GameObject obj2 = form.get_transform().Find("bg/AllRankType/AllRankSelectMenu/ListElement7").get_gameObject();
                if (obj2 != null)
                {
                    if (bShow)
                    {
                        this.AddNewFlag(obj2, enNewFlagKey.New_GodRank_V1, enNewFlagPos.enTopRight, 1f, 0f, 0f, enNewFlagType.enNewFlag);
                    }
                    else
                    {
                        this.DelNewFlag(obj2, enNewFlagKey.New_GodRank_V1, true);
                    }
                }
            }
        }

        public void SetNewFlagForMatch(bool bShow)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLobbySystem.LOBBY_FORM_PATH);
            if (form != null)
            {
                GameObject obj2 = form.get_transform().Find("Popup/BattleWebHome").get_gameObject();
                if (obj2 != null)
                {
                    if (bShow)
                    {
                        this.AddNewFlag(obj2, enNewFlagKey.New_Match_V1, enNewFlagPos.enTopLeft, 0.8f, 0f, 0f, enNewFlagType.enNewFlag);
                    }
                    else
                    {
                        this.DelNewFlag(obj2, enNewFlagKey.New_Match_V1, true);
                    }
                }
            }
        }

        public void SetNewFlagFormCustomEquipShow(bool bShow)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLobbySystem.LOBBY_FORM_PATH);
            if (form != null)
            {
                GameObject obj2 = form.get_transform().Find("LobbyBottom/SysEntry/ChatBtn_sub/Menu/PropBtn").get_gameObject();
                if (obj2 != null)
                {
                    if (bShow)
                    {
                        this.AddNewFlag(obj2, enNewFlagKey.New_CustomEqiup_V1, enNewFlagPos.enTopRight, 1f, 0f, 0f, enNewFlagType.enNewFlag);
                    }
                    else
                    {
                        this.DelNewFlag(obj2, enNewFlagKey.New_CustomEqiup_V1, true);
                    }
                }
            }
        }

        public void SetNewFlagForMessageBtnEntry(bool bShow)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLobbySystem.LOBBY_FORM_PATH);
            if (form != null)
            {
                GameObject obj2 = form.get_transform().Find("LobbyBottom/SysEntry/ChatBtn_sub/Menu/MessageBtn").get_gameObject();
                if (obj2 != null)
                {
                    if (bShow)
                    {
                        this.AddNewFlag(obj2, enNewFlagKey.New_MessageEntry_V1, enNewFlagPos.enTopRight, 1f, 0f, 0f, enNewFlagType.enNewFlag);
                    }
                    else
                    {
                        this.DelNewFlag(obj2, enNewFlagKey.New_MessageEntry_V1, true);
                    }
                }
            }
        }

        public void SetNewFlagForOBBtn(bool bShow)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLobbySystem.LOBBY_FORM_PATH);
            if (form != null)
            {
                GameObject obj2 = form.get_transform().Find("Popup/OBBtn").get_gameObject();
                if (obj2 != null)
                {
                    if (bShow)
                    {
                        this.AddNewFlag(obj2, enNewFlagKey.New_OBBtn_V1, enNewFlagPos.enTopLeft, 0.8f, 0f, 0f, enNewFlagType.enNewFlag);
                    }
                    else
                    {
                        this.DelNewFlag(obj2, enNewFlagKey.New_OBBtn_V1, true);
                    }
                }
            }
        }

        public void SetNewFlagForSettingEntry(bool bShow)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLobbySystem.SYSENTRY_FORM_PATH);
            if (form != null)
            {
                GameObject obj2 = form.get_transform().Find("PlayerBtn/SettingBtn").get_gameObject();
                if (obj2 != null)
                {
                    if (bShow)
                    {
                        this.AddNewFlag(obj2, enNewFlagKey.New_LobbySettingEntry_V2, enNewFlagPos.enTopRight, 0.8f, 0f, -2f, enNewFlagType.enNewFlag);
                    }
                    else
                    {
                        this.DelNewFlag(obj2, enNewFlagKey.New_LobbySettingEntry_V2, true);
                    }
                }
            }
        }

        public void ShowNewFlagForAchievementEntry()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLobbySystem.LOBBY_FORM_PATH);
            if (form != null)
            {
                GameObject obj2 = form.get_transform().Find("LobbyBottom/SysEntry/AchievementBtn").get_gameObject();
                if (obj2 != null)
                {
                    this.AddNewFlag(obj2, enNewFlagKey.New_Achievement_V1, enNewFlagPos.enTopLeft, 1f, 0f, 0f, enNewFlagType.enNewFlag);
                }
            }
        }

        public void ShowNewFlagForBeizhanEntry()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLobbySystem.LOBBY_FORM_PATH);
            if (form != null)
            {
                GameObject obj2 = form.get_transform().Find("LobbyBottom/SysEntry/ChatBtn").get_gameObject();
                if (obj2 != null)
                {
                    this.AddNewFlag(obj2, enNewFlagKey.New_BeizhanEntryBtn_V3, enNewFlagPos.enTopRight, 1f, 0f, 0f, enNewFlagType.enNewFlag);
                }
            }
        }

        public void ShowNewFlagForMishuEntry()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLobbySystem.LOBBY_FORM_PATH);
            if (form != null)
            {
                GameObject obj2 = form.get_transform().Find("LobbyBottom/Newbie").get_gameObject();
                if (obj2 != null)
                {
                    this.AddNewFlag(obj2, enNewFlagKey.New_MishuEntryBtn_V1, enNewFlagPos.enBottomRight, 1f, 0f, 0f, enNewFlagType.enNewFlag);
                }
            }
        }
    }
}

