namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class CHostHeroDeadInfo
    {
        private bool m_bIsMobaMode;
        private CUIFormScript m_heroDeadInfoForm;
        private PoolObjHandle<ActorRoot> m_hostActor;
        private static string[] m_Skill_HurtType_Bg_ImgName = new string[] { "Common_Bg_Physicalbg", "Common_Bg_Spellbg", "Common_Bg_Realbg", "Common_Bg_blend" };
        private string[] m_Skill_HurtType_Name = new string[] { "physical", "magic", "real", "blend" };
        private static string[] m_Skill_HurtValue_Bg_ImgName = new string[] { "ImgDamge_physicalbg", "ImgDamge_spellbg", "ImgDamge_realbg", "ImgDamge_blend" };
        private string m_strAtkSkill0Name;
        public static string m_strDragonBig = (CUIUtility.s_Sprite_Dynamic_Signal_Dir + "Dragon_big");
        private const string m_strHeroDeadInfoForm = "UGUI/Form/Battle/Part/Form_Battle_Part_HeroDeadInfo.prefab";
        private string m_strHeroEquipSkillName;
        private string m_strHeroPassiveSkillName;

        private string GetCondtionText(int iCondId, int iIndex = 0)
        {
            int num = 0;
            int count = GameDataMgr.deadInfoTextDatabin.count;
            for (int i = 0; i < count; i++)
            {
                ResDeadInfoText dataByKey = GameDataMgr.deadInfoTextDatabin.GetDataByKey((long) i);
                if (dataByKey.bConditionId == iCondId)
                {
                    if (num == iIndex)
                    {
                        return dataByKey.szText;
                    }
                    num++;
                }
            }
            return null;
        }

        private int GetCondtionTextCount(int iCondId)
        {
            int num = 0;
            int count = GameDataMgr.deadInfoTextDatabin.count;
            for (int i = 0; i < count; i++)
            {
                if (GameDataMgr.deadInfoTextDatabin.GetDataByKey((long) i).bConditionId == iCondId)
                {
                    num++;
                }
            }
            return num;
        }

        public void Init()
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
            if ((curLvelContext != null) && curLvelContext.IsMobaMode())
            {
                this.m_bIsMobaMode = true;
                this.m_hostActor = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain;
                for (int i = 0; i < 4; i++)
                {
                    this.m_Skill_HurtType_Name[i] = Singleton<CTextManager>.instance.GetText("Skill_Common_Effect_Type_" + (i + 1));
                }
                this.m_strHeroPassiveSkillName = Singleton<CTextManager>.instance.GetText("HeroDeadInfo_PassiveSkill_Name");
                this.m_strHeroEquipSkillName = Singleton<CTextManager>.instance.GetText("HeroDeadInfo_EquipSkill_Name");
                this.m_strAtkSkill0Name = Singleton<CTextManager>.instance.GetText("HeroDeadInfo_Skill0_Name");
                Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorRevive, new RefAction<DefaultGameEventParam>(this.OnActorRevive));
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_DeadInfo_Click, new CUIEventManager.OnUIEventHandler(this.OnDeadInfoFormOpen));
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_DeadInfoForm_Close_Click, new CUIEventManager.OnUIEventHandler(this.OnDeadInfoFormClose));
            }
        }

        private void InitDeadInfoPanelBottomText(ulong ulDeadTime, uint[] arrDiffTypeHurtValue, uint uiTotalDamage, int iSpecailType)
        {
            int count = GameDataMgr.deadInfoConditionDatabin.count;
            int iCondId = 1;
            iCondId = 2;
            while (iCondId <= count)
            {
                ResDeadInfoCondition dataByKey = GameDataMgr.deadInfoConditionDatabin.GetDataByKey((long) iCondId);
                if (this.MatchRule(dataByKey, ulDeadTime, arrDiffTypeHurtValue, uiTotalDamage, iSpecailType))
                {
                    break;
                }
                iCondId++;
            }
            if (iCondId > count)
            {
                iCondId = 1;
            }
            if (iCondId <= count)
            {
                int condtionTextCount = this.GetCondtionTextCount(iCondId);
                if (condtionTextCount > 0)
                {
                    int iIndex = (int) (Singleton<FrameSynchr>.instance.LogicFrameTick % ((long) condtionTextCount));
                    string condtionText = this.GetCondtionText(iCondId, iIndex);
                    if (condtionText != null)
                    {
                        Transform transform = this.m_heroDeadInfoForm.get_transform().FindChild("PanelDeadInfo/TxtBottom");
                        if (transform != null)
                        {
                            Text component = transform.GetComponent<Text>();
                            if (component != null)
                            {
                                component.set_text(condtionText);
                            }
                        }
                    }
                }
            }
        }

        private void InitHeroDeadInfoForm()
        {
            this.m_hostActor = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain;
            if (this.m_hostActor != 0)
            {
                PlayerKDA playerKDA = Singleton<BattleStatistic>.GetInstance().m_playerKDAStat.GetPlayerKDA(Singleton<GamePlayerCenter>.instance.HostPlayerId);
                if (playerKDA != null)
                {
                    ListView<CHostHeroDamage>.Enumerator enumerator = playerKDA.m_hostHeroDamage.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        if ((enumerator.Current != null) && (this.m_hostActor.handle.ObjID == enumerator.Current.GetHostHeroObjId()))
                        {
                            uint[] arrDiffTypeHurtValue = new uint[4];
                            uint[] arrObjId = new uint[2];
                            uint uiAllTotalDamage = 0;
                            int num2 = enumerator.Current.GetAllActorsTotalDamageAndTopActorId(ref arrObjId, 2, ref uiAllTotalDamage, ref arrDiffTypeHurtValue);
                            uint uiObjId = 0;
                            ActorTypeDef invalid = ActorTypeDef.Invalid;
                            enumerator.Current.GetKillerObjId(ref uiObjId, ref invalid);
                            DOUBLE_INT_INFO[] arrDamageInfo = new DOUBLE_INT_INFO[12];
                            int[] numArray3 = new int[3];
                            for (int i = 0; i < (1 + num2); i++)
                            {
                                if (i == 0)
                                {
                                    enumerator.Current.GetActorDamage(uiObjId, ref arrDamageInfo);
                                }
                                else
                                {
                                    enumerator.Current.GetActorDamage(arrObjId[i - 1], ref arrDamageInfo);
                                }
                                for (int k = 0; k <= 11; k++)
                                {
                                    numArray3[i] += arrDamageInfo[k].iValue;
                                }
                            }
                            int num6 = (numArray3[0] <= numArray3[1]) ? numArray3[1] : numArray3[0];
                            num6 = (num6 <= numArray3[2]) ? numArray3[2] : num6;
                            for (int j = 0; j < (1 + num2); j++)
                            {
                                if (j == 0)
                                {
                                    Transform trHeroPanel = this.m_heroDeadInfoForm.get_transform().FindChild("PanelDeadInfo/KillerGounp/PanelKiller");
                                    this.InitHeroPanelInfo(uiObjId, trHeroPanel, numArray3[j], uiAllTotalDamage, numArray3[j] == num6, enumerator.Current);
                                }
                                else if (num2 == 2)
                                {
                                    Transform transform2 = this.m_heroDeadInfoForm.get_transform().FindChild("PanelDeadInfo/KillerGounp/PanelAssister" + (j - 1));
                                    this.InitHeroPanelInfo(arrObjId[j - 1], transform2, numArray3[j], uiAllTotalDamage, numArray3[j] == num6, enumerator.Current);
                                }
                                else
                                {
                                    Transform transform3 = this.m_heroDeadInfoForm.get_transform().FindChild("PanelDeadInfo/KillerGounp/PanelAssister" + j);
                                    this.InitHeroPanelInfo(arrObjId[j - 1], transform3, numArray3[j], uiAllTotalDamage, numArray3[j] == num6, enumerator.Current);
                                }
                            }
                            int iSpecailType = 0;
                            ulong hostHeroDeadTime = enumerator.Current.GetHostHeroDeadTime();
                            if (hostHeroDeadTime == Singleton<BattleStatistic>.instance.m_battleDeadStat.m_uiFBTime)
                            {
                                iSpecailType |= 2;
                            }
                            if (invalid == ActorTypeDef.Actor_Type_Organ)
                            {
                                iSpecailType |= 4;
                            }
                            this.InitDeadInfoPanelBottomText(hostHeroDeadTime, arrDiffTypeHurtValue, uiAllTotalDamage, iSpecailType);
                            this.ResetFormSize(1 + num2);
                            break;
                        }
                    }
                }
            }
        }

        private void InitHeroPanelHeroInfo(uint uiObjId, Transform trHeroPanel, CHostHeroDamage objHostHeroDamage)
        {
            if ((trHeroPanel != null) && (objHostHeroDamage != null))
            {
                Transform transform = trHeroPanel.FindChild("PanelTop");
                if (transform != null)
                {
                    int iConfigId = 0;
                    string actorName = null;
                    string playerName = null;
                    byte actorSubType = 0;
                    byte bMonsterType = 0;
                    ActorTypeDef invalid = ActorTypeDef.Invalid;
                    if (objHostHeroDamage.GetDamageActorInfo(uiObjId, ref actorName, ref playerName, ref invalid, ref iConfigId, ref actorSubType, ref bMonsterType))
                    {
                        Transform transform2 = transform.FindChild("Imghead");
                        if (transform2 != null)
                        {
                            Transform transform3 = transform2.FindChild("head");
                            if (transform3 != null)
                            {
                                Image component = transform3.GetComponent<Image>();
                                if (component != null)
                                {
                                    string szBossIcon;
                                    switch (invalid)
                                    {
                                        case ActorTypeDef.Actor_Type_Hero:
                                        {
                                            string heroSkinPic = CSkinInfo.GetHeroSkinPic((uint) iConfigId, 0);
                                            szBossIcon = CUIUtility.s_Sprite_Dynamic_Icon_Dir + heroSkinPic;
                                            break;
                                        }
                                        case ActorTypeDef.Actor_Type_Organ:
                                            szBossIcon = KillNotify.building_icon;
                                            break;

                                        case ActorTypeDef.Actor_Type_Monster:
                                        {
                                            ResMonsterCfgInfo dataCfgInfoByCurLevelDiff = MonsterDataHelper.GetDataCfgInfoByCurLevelDiff(iConfigId);
                                            if ((dataCfgInfoByCurLevelDiff != null) && !string.IsNullOrEmpty(dataCfgInfoByCurLevelDiff.szBossIcon))
                                            {
                                                szBossIcon = dataCfgInfoByCurLevelDiff.szBossIcon;
                                            }
                                            else if (bMonsterType == 1)
                                            {
                                                szBossIcon = KillNotify.monster_icon;
                                            }
                                            else
                                            {
                                                switch (actorSubType)
                                                {
                                                    case 7:
                                                    case 9:
                                                        szBossIcon = KillNotify.dragon_icon;
                                                        break;
                                                }
                                                if (actorSubType == 8)
                                                {
                                                    szBossIcon = m_strDragonBig;
                                                }
                                                else
                                                {
                                                    szBossIcon = KillNotify.yeguai_icon;
                                                }
                                            }
                                            break;
                                        }
                                        default:
                                            szBossIcon = KillNotify.monster_icon;
                                            break;
                                    }
                                    component.SetSprite(szBossIcon, this.m_heroDeadInfoForm, true, false, false, false);
                                }
                            }
                        }
                        Transform transform4 = transform.FindChild("heroName");
                        if (transform4 != null)
                        {
                            Text text = transform4.GetComponent<Text>();
                            if (text != null)
                            {
                                string str5 = actorName;
                                if (!string.IsNullOrEmpty(str5))
                                {
                                    int index = str5.IndexOf('(');
                                    string str6 = str5.Substring(index + 1, (str5.Length - index) - 2);
                                    text.set_text(str6);
                                }
                            }
                        }
                        Transform transform5 = transform.FindChild("playerName");
                        if (transform5 != null)
                        {
                            Text text2 = transform5.GetComponent<Text>();
                            if (text2 != null)
                            {
                                if (!string.IsNullOrEmpty(playerName))
                                {
                                    text2.set_text(playerName);
                                    transform5.get_gameObject().CustomSetActive(true);
                                }
                                else
                                {
                                    transform5.get_gameObject().CustomSetActive(false);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void InitHeroPanelInfo(uint uiObjId, Transform trHeroPanel, int iHeroTotalDamage, uint uiTotalDamage, bool bIsMaxTotalDamage, CHostHeroDamage objHostHeroDamage)
        {
            if ((trHeroPanel != null) && (uiObjId != 0))
            {
                this.InitHeroPanelHeroInfo(uiObjId, trHeroPanel, objHostHeroDamage);
                this.InitHeroPanelSkillInfo(uiObjId, trHeroPanel, uiTotalDamage, objHostHeroDamage);
                this.InitHeroPanelTotalDamage(trHeroPanel, iHeroTotalDamage, (uiTotalDamage != 0) ? (((float) iHeroTotalDamage) / ((float) uiTotalDamage)) : 1f, bIsMaxTotalDamage);
            }
        }

        private void InitHeroPanelSkillInfo(uint uiObjId, Transform trHeroPanel, uint uiTotalDamge, CHostHeroDamage objHostHeroDamage)
        {
            DOUBLE_INT_INFO[] arrDamageInfo = new DOUBLE_INT_INFO[12];
            objHostHeroDamage.GetActorDamage(uiObjId, ref arrDamageInfo);
            SkillSlot slot = null;
            for (int i = 0; i < 3; i++)
            {
                string str = "PanelSkill/Skill" + i;
                Transform transform = trHeroPanel.FindChild(str);
                if (transform != null)
                {
                    if (arrDamageInfo[i].iValue <= 0)
                    {
                        transform.get_gameObject().CustomSetActive(false);
                    }
                    else
                    {
                        transform.get_gameObject().CustomSetActive(true);
                        slot = null;
                        PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.instance.GetActor(uiObjId);
                        if (((arrDamageInfo[i].iKey < 10) && (actor != 0)) && (actor.handle.SkillControl != null))
                        {
                            if ((actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) && (arrDamageInfo[i].iKey > 0))
                            {
                                actor.handle.SkillControl.TryGetSkillSlot((SkillSlotType) arrDamageInfo[i].iKey, out slot);
                            }
                            else if (actor.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Hero)
                            {
                                actor.handle.SkillControl.TryGetSkillSlot((SkillSlotType) arrDamageInfo[i].iKey, out slot);
                            }
                        }
                        Transform transform2 = transform.FindChild("ImgSkill");
                        if (transform2 != null)
                        {
                            Image component = transform2.GetComponent<Image>();
                            if (component != null)
                            {
                                if (slot != null)
                                {
                                    if (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
                                    {
                                        component.SetSprite(CUIUtility.s_Sprite_Dynamic_Skill_Dir + slot.SkillObj.IconName, this.m_heroDeadInfoForm, true, false, false, false);
                                    }
                                    else
                                    {
                                        component.SetSprite(CUIUtility.s_Sprite_Dynamic_Skill_Dir + "1001", this.m_heroDeadInfoForm, true, false, false, false);
                                    }
                                }
                                else if (arrDamageInfo[i].iKey == 10)
                                {
                                    component.SetSprite(CUIUtility.s_Sprite_Dynamic_Skill_Dir + "1106", this.m_heroDeadInfoForm, true, false, false, false);
                                }
                                else
                                {
                                    component.SetSprite(CUIUtility.s_Sprite_Dynamic_Skill_Dir + "1001", this.m_heroDeadInfoForm, true, false, false, false);
                                }
                            }
                        }
                        int skillSlotHurtType = objHostHeroDamage.GetSkillSlotHurtType(uiObjId, (SkillSlotType) arrDamageInfo[i].iKey);
                        if ((skillSlotHurtType >= 0) && (skillSlotHurtType < 4))
                        {
                            Transform transform3 = transform.FindChild("TxtSkillTypeBg");
                            if (transform3 != null)
                            {
                                Image image = transform3.GetComponent<Image>();
                                if (image != null)
                                {
                                    image.SetSprite("UGUI/Sprite/Common/" + m_Skill_HurtType_Bg_ImgName[skillSlotHurtType], this.m_heroDeadInfoForm, true, false, false, false);
                                }
                            }
                            Transform transform4 = transform.FindChild("TxtSkillType");
                            if (transform4 != null)
                            {
                                Text text = transform4.GetComponent<Text>();
                                if (text != null)
                                {
                                    text.set_text(this.m_Skill_HurtType_Name[skillSlotHurtType]);
                                    text.set_color(CUIUtility.s_Text_Skill_HurtType_Color[skillSlotHurtType]);
                                }
                            }
                            Transform transform5 = transform.FindChild("TxtSkillName");
                            if (transform5 != null)
                            {
                                Text text2 = transform5.GetComponent<Text>();
                                if (text2 != null)
                                {
                                    if (slot != null)
                                    {
                                        text2.set_text(slot.SkillObj.cfgData.szSkillName);
                                    }
                                    else
                                    {
                                        ActorTypeDef invalid = ActorTypeDef.Invalid;
                                        if (actor == 0)
                                        {
                                            int iConfigId = 0;
                                            string actorName = null;
                                            string playerName = null;
                                            ActorTypeDef def2 = ActorTypeDef.Invalid;
                                            byte actorSubType = 0;
                                            byte bMonsterType = 0;
                                            if (objHostHeroDamage.GetDamageActorInfo(uiObjId, ref actorName, ref playerName, ref invalid, ref iConfigId, ref actorSubType, ref bMonsterType))
                                            {
                                                invalid = def2;
                                            }
                                        }
                                        else
                                        {
                                            invalid = actor.handle.TheActorMeta.ActorType;
                                        }
                                        if (invalid == ActorTypeDef.Actor_Type_Hero)
                                        {
                                            if (arrDamageInfo[i].iKey == 0)
                                            {
                                                text2.set_text(this.m_strAtkSkill0Name);
                                            }
                                            else if (arrDamageInfo[i].iKey == 10)
                                            {
                                                text2.set_text(this.m_strHeroEquipSkillName);
                                            }
                                            else
                                            {
                                                text2.set_text(this.m_strHeroPassiveSkillName);
                                            }
                                        }
                                        else
                                        {
                                            text2.set_text(this.m_strAtkSkill0Name);
                                        }
                                    }
                                    text2.set_color(CUIUtility.s_Text_SkillName_And_HurtValue_Color[skillSlotHurtType]);
                                }
                            }
                            float num6 = (uiTotalDamge != 0) ? (((float) arrDamageInfo[i].iValue) / ((float) uiTotalDamge)) : 1f;
                            Transform transform6 = transform.FindChild("Damage");
                            if (transform6 != null)
                            {
                                for (int j = 0; j < 4; j++)
                                {
                                    Transform transform7 = transform6.FindChild(m_Skill_HurtValue_Bg_ImgName[j]);
                                    if (transform7 != null)
                                    {
                                        transform7.get_gameObject().CustomSetActive(j == skillSlotHurtType);
                                        if (j == skillSlotHurtType)
                                        {
                                            Image image3 = transform7.GetComponent<Image>();
                                            if (image3 != null)
                                            {
                                                image3.CustomFillAmount(num6);
                                            }
                                        }
                                    }
                                }
                            }
                            Transform transform8 = transform.FindChild("TxtDamageValue");
                            if (transform8 != null)
                            {
                                Text text3 = transform8.GetComponent<Text>();
                                if (text3 != null)
                                {
                                    object[] objArray1 = new object[] { arrDamageInfo[i].iValue, "(", num6.ToString("P0"), ")" };
                                    string str4 = string.Concat(objArray1);
                                    text3.set_text(str4);
                                    text3.set_color(CUIUtility.s_Text_SkillName_And_HurtValue_Color[skillSlotHurtType]);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void InitHeroPanelTotalDamage(Transform trHeroPanel, int iHeroTotalDamage, float fTotalDamageRate, bool bIsMaxTotalDamage)
        {
            Transform transform = trHeroPanel.FindChild("PanelBottom");
            if (transform != null)
            {
                Transform transform2 = transform.FindChild("TxtTotalDamage");
                if (transform2 != null)
                {
                    Text component = transform2.GetComponent<Text>();
                    if (component != null)
                    {
                        component.set_color(CUIUtility.s_Text_Total_Damage_Text_Color[!bIsMaxTotalDamage ? 1 : 0]);
                    }
                    Outline outline = transform2.GetComponent<Outline>();
                    if (outline != null)
                    {
                        outline.set_effectColor(CUIUtility.s_Text_Total_Damage_Text_Outline_Color[!bIsMaxTotalDamage ? 1 : 0]);
                    }
                    Transform transform3 = transform2.FindChild("TxtValue");
                    if (transform3 != null)
                    {
                        Text text2 = transform3.GetComponent<Text>();
                        if (text2 != null)
                        {
                            text2.set_color(CUIUtility.s_Text_Total_Damage_Value_Color[!bIsMaxTotalDamage ? 1 : 0]);
                            object[] objArray1 = new object[] { iHeroTotalDamage, "(", fTotalDamageRate.ToString("P0"), ")" };
                            string str = string.Concat(objArray1);
                            text2.set_text(str);
                        }
                    }
                    Transform transform4 = transform2.FindChild("Tag");
                    if (transform4 != null)
                    {
                        transform4.get_gameObject().CustomSetActive(bIsMaxTotalDamage);
                    }
                }
            }
        }

        private bool MatchRule(ResDeadInfoCondition resCond, ulong ulDeadTime, uint[] arrDiffTypeHurtValue, uint uiTotalDamage, int iSpecailType)
        {
            if (resCond == null)
            {
                return false;
            }
            if ((resCond.ullEndTime != 0) && ((ulDeadTime < resCond.ullStartTime) || (ulDeadTime > resCond.ullEndTime)))
            {
                return false;
            }
            if (resCond.bHurtType != 0)
            {
                if (((resCond.bHurtType != 1) || (resCond.bHurtType != 2)) && (resCond.dwHurtValue >= arrDiffTypeHurtValue[resCond.bHurtType - 1]))
                {
                    uint num = (uiTotalDamage != 0) ? ((arrDiffTypeHurtValue[resCond.bHurtType - 1] * 100) / uiTotalDamage) : 100;
                    if (num < resCond.dwHurtRate)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            if ((resCond.bSpecailType != 0) && ((iSpecailType & (((int) 1) << resCond.bSpecailType)) == 0))
            {
                return false;
            }
            return true;
        }

        public void OnActorRevive(ref DefaultGameEventParam prm)
        {
            uint hostPlayerId = Singleton<GamePlayerCenter>.instance.HostPlayerId;
            if ((prm.src != 0) && (prm.src.handle.TheActorMeta.PlayerId == hostPlayerId))
            {
                PlayerKDA playerKDA = Singleton<BattleStatistic>.GetInstance().m_playerKDAStat.GetPlayerKDA(hostPlayerId);
                if (playerKDA != null)
                {
                    ListView<CHostHeroDamage>.Enumerator enumerator = playerKDA.m_hostHeroDamage.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current != null)
                        {
                            enumerator.Current.OnActorRevive(ref prm);
                        }
                    }
                    if (prm.src == this.m_hostActor)
                    {
                        this.OnDeadInfoFormClose(null);
                    }
                }
            }
        }

        public void OnDeadInfoFormClose(CUIEvent uiEvent)
        {
            if ((!Singleton<WatchController>.instance.IsWatching && (Singleton<CBattleSystem>.instance.FightForm != null)) && (this.m_heroDeadInfoForm != null))
            {
                Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/Battle/Part/Form_Battle_Part_HeroDeadInfo.prefab");
                this.m_heroDeadInfoForm = null;
            }
        }

        public void OnDeadInfoFormOpen(CUIEvent uiEvent)
        {
            if (!Singleton<WatchController>.instance.IsWatching && (Singleton<CBattleSystem>.instance.FightForm != null))
            {
                this.m_heroDeadInfoForm = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/Battle/Part/Form_Battle_Part_HeroDeadInfo.prefab", false, true);
                if (this.m_heroDeadInfoForm != null)
                {
                    this.InitHeroDeadInfoForm();
                }
            }
        }

        private void ResetFormSize(int iHeroPanelCount)
        {
            if ((iHeroPanelCount >= 1) && (iHeroPanelCount <= 3))
            {
                Animation component = this.m_heroDeadInfoForm.get_transform().GetComponent<Animation>();
                if (component != null)
                {
                    float num = component.get_Item("HeroDeadInfo_Bg_Anim").get_length();
                    component.Stop("HeroDeadInfo_Bg_Anim");
                    float num2 = 0f;
                    if (iHeroPanelCount != 3)
                    {
                        num2 = (((3 - iHeroPanelCount) + 1) * num) / 3f;
                    }
                    component.get_Item("HeroDeadInfo_Bg_Anim").set_time(num2);
                    component.get_Item("HeroDeadInfo_Bg_Anim").set_speed(0f);
                    component.Play("HeroDeadInfo_Bg_Anim");
                }
            }
        }

        public void UnInit()
        {
            if (this.m_bIsMobaMode)
            {
                Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorRevive, new RefAction<DefaultGameEventParam>(this.OnActorRevive));
                Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_DeadInfo_Click, new CUIEventManager.OnUIEventHandler(this.OnDeadInfoFormOpen));
                Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_DeadInfoForm_Close_Click, new CUIEventManager.OnUIEventHandler(this.OnDeadInfoFormClose));
                if (this.m_hostActor != 0)
                {
                    this.m_hostActor.Release();
                }
                this.OnDeadInfoFormClose(null);
            }
        }
    }
}

