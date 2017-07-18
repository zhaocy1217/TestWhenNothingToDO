namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using TMPro;
    using UnityEngine;

    public class CBattleFloatDigitManager
    {
        private const uint DEFAULT_FONT_SIZE = 6;
        private const string FLOAT_TEXT_PREFAB = "Text/FloatText/FloatText.prefab";
        private const uint HP_RECOVER_SHOW_THRESHOLD = 50;
        private List<FloatDigitInfo> m_floatDigitInfoList;
        private static string[][] s_battleFloatDigitAnimatorStates;
        private static string[] s_otherFloatTextAnimatorStates;
        private static string[] s_otherFloatTextKeys;
        private static string s_restrictTextAnimatorState;
        private static string[] s_restrictTextKeys;

        static CBattleFloatDigitManager()
        {
            string[][] textArrayArray1 = new string[14][];
            textArrayArray1[0] = new string[] { string.Empty };
            textArrayArray1[1] = new string[] { "Physics_Right", "Physics_Left" };
            textArrayArray1[2] = new string[] { "Physics_RightCrit", "Physics_LeftCrit" };
            textArrayArray1[3] = new string[] { "Magic_Right", "Magic_Left" };
            textArrayArray1[4] = new string[] { "Magic_RightCrit", "Magic_LeftCrit" };
            textArrayArray1[5] = new string[] { "ZhenShi_Right", "ZhenShi_Left" };
            textArrayArray1[6] = new string[] { "ZhenShi_RightCrit", "ZhenShi_LeftCrit" };
            textArrayArray1[7] = new string[] { "SufferPhysicalDamage" };
            textArrayArray1[8] = new string[] { "SufferMagicDamage" };
            textArrayArray1[9] = new string[] { "SufferRealDamage" };
            textArrayArray1[10] = new string[] { "ReviveHp" };
            textArrayArray1[11] = new string[] { "Exp" };
            textArrayArray1[12] = new string[] { "Gold" };
            textArrayArray1[13] = new string[] { "LastHitGold" };
            s_battleFloatDigitAnimatorStates = textArrayArray1;
            s_restrictTextKeys = new string[] { 
                "Restrict_None", "Restrict_Dizzy", "Restrict_SlowDown", "Restrict_Taunt", "Restrict_Fear", "Restrict_Frozen", "Restrict_Floating", "Restrict_Slient", "Restrict_Stone", "SkillBuff_Custom_Type_1", "SkillBuff_Custom_Type_2", "SkillBuff_Custom_Type_3", "SkillBuff_Custom_Type_4", "SkillBuff_Custom_Type_5", "SkillBuff_Custom_Type_6", "SkillBuff_Custom_Type_7", 
                "SkillBuff_Custom_Type_8", "SkillBuff_Custom_Type_9", "SkillBuff_Custom_Type_10", "SkillBuff_Custom_Type_11", "SkillBuff_Custom_Type_12", "SkillBuff_Custom_Type_13", "SkillBuff_Custom_Type_14", "SkillBuff_Custom_Type_15", "SkillBuff_Custom_Type_16", "SkillBuff_Custom_Type_17", "SkillBuff_Custom_Type_18", "SkillBuff_Custom_Type_19", "SkillBuff_Custom_Type_20", "SkillBuff_Custom_Type_21", "SkillBuff_Custom_Type_22", "SkillBuff_Custom_Type_23", 
                "SkillBuff_Custom_Type_24", "SkillBuff_Custom_Type_25", "SkillBuff_Custom_Type_26", "SkillBuff_Custom_Type_27", "SkillBuff_Custom_Type_28", "SkillBuff_Custom_Type_29", "SkillBuff_Custom_Type_30", "SkillBuff_Custom_Type_31", "SkillBuff_Custom_Type_32", "SkillBuff_Custom_Type_33", "SkillBuff_Custom_Type_34", "SkillBuff_Custom_Type_35", "SkillBuff_Custom_Type_36", "SkillBuff_Custom_Type_37", "SkillBuff_Custom_Type_38", "SkillBuff_Custom_Type_39", 
                "SkillBuff_Custom_Type_40"
             };
            s_restrictTextAnimatorState = "RestrictText_Anim";
            s_otherFloatTextKeys = new string[] { 
                "Accept_Task", "Complete_Task", "Level_Up", "Talent_Open", "Talent_Learn", "DragonBuff_Get1", "DragonBuff_Get2", "DragonBuff_Get3", "Battle_Absorb", "Battle_ShieldDisappear", "Battle_Immunity", "Battle_InCooldown", "Battle_NoTarget", "Battle_MagicShortage", "Battle_Blindess", "Battle_MadnessShortage", 
                "Battle_EnergyShortage", "Battle_FuryShortage", "Battle_BeanShortage"
             };
            s_otherFloatTextAnimatorStates = new string[] { 
                "Other_Anim", "Other_Anim", "Other_Anim", "Other_Anim", "Other_Anim", "Other_Anim", "Other_Anim", "Other_Anim", "Other_Anim", "Other_Anim", "Other_Anim", "Other_Anim", "Other_Anim", "Other_Anim", "Other_Anim", "Other_Anim", 
                "Other_Anim", "Other_Anim", "Other_Anim"
             };
        }

        private bool CanMergeToCritText(ref DIGIT_TYPE type1, DIGIT_TYPE type2)
        {
            if (((((type1 != DIGIT_TYPE.PhysicalAttackNormal) || (type2 != DIGIT_TYPE.PhysicalAttackCrit)) && ((type1 != DIGIT_TYPE.PhysicalAttackCrit) || (type2 != DIGIT_TYPE.PhysicalAttackNormal))) && (((type1 != DIGIT_TYPE.MagicAttackNormal) || (type2 != DIGIT_TYPE.MagicAttackCrit)) && ((type1 != DIGIT_TYPE.MagicAttackCrit) || (type2 != DIGIT_TYPE.MagicAttackNormal)))) && (((type1 != DIGIT_TYPE.RealAttackNormal) || (type2 != DIGIT_TYPE.RealAttackCrit)) && ((type1 != DIGIT_TYPE.RealAttackCrit) || (type2 != DIGIT_TYPE.RealAttackNormal))))
            {
                return false;
            }
            if (type1 < type2)
            {
                type1 = type2;
            }
            return true;
        }

        public void ClearAllBattleFloatText()
        {
            if (this.m_floatDigitInfoList != null)
            {
                this.m_floatDigitInfoList.Clear();
                this.m_floatDigitInfoList = null;
            }
        }

        public void ClearBattleFloatText(CUIAnimatorScript animatorScript)
        {
        }

        public void CollectFloatDigitInSingleFrame(PoolObjHandle<ActorRoot> attacker, PoolObjHandle<ActorRoot> target, DIGIT_TYPE digitType, int value)
        {
            if (!MonoSingleton<Reconnection>.GetInstance().isProcessingRelayRecover)
            {
                FloatDigitInfo info;
                if (this.m_floatDigitInfoList == null)
                {
                    this.m_floatDigitInfoList = new List<FloatDigitInfo>();
                }
                for (int i = 0; i < this.m_floatDigitInfoList.Count; i++)
                {
                    info = this.m_floatDigitInfoList[i];
                    if (((info.m_attacker == attacker) && (info.m_target == target)) && ((info.m_digitType == digitType) || this.CanMergeToCritText(ref info.m_digitType, digitType)))
                    {
                        info.m_value += value;
                        this.m_floatDigitInfoList[i] = info;
                        return;
                    }
                }
                info = new FloatDigitInfo(attacker, target, digitType, value);
                this.m_floatDigitInfoList.Add(info);
            }
        }

        public void CreateBattleFloatDigit(int digitValue, DIGIT_TYPE digitType, ref Vector3 worldPosition)
        {
            if (((((GameSettings.RenderQuality != SGameRenderQuality.Low) || (digitType == DIGIT_TYPE.MagicAttackCrit)) || ((digitType == DIGIT_TYPE.PhysicalAttackCrit) || (digitType == DIGIT_TYPE.RealAttackCrit))) || ((digitType == DIGIT_TYPE.ReceiveGoldCoinInBattle) || (digitType == DIGIT_TYPE.ReceiveLastHitGoldCoinInBattle))) && ((digitType != DIGIT_TYPE.ReviveHp) || (digitValue >= 50L)))
            {
                string[] strArray = s_battleFloatDigitAnimatorStates[(int) digitType];
                if (strArray.Length > 0)
                {
                    string content = (((((digitType != DIGIT_TYPE.ReviveHp) && (digitType != DIGIT_TYPE.ReceiveSpirit)) && (digitType != DIGIT_TYPE.ReceiveGoldCoinInBattle)) || (digitValue <= 0)) ? string.Empty : "+") + Mathf.Abs(digitValue).ToString();
                    if (digitType == DIGIT_TYPE.ReceiveSpirit)
                    {
                        content = content + "xp";
                    }
                    else
                    {
                        if ((digitType == DIGIT_TYPE.ReceiveGoldCoinInBattle) || (digitType == DIGIT_TYPE.ReceiveLastHitGoldCoinInBattle))
                        {
                            content = content + "g";
                        }
                        this.CreateBattleFloatText(content, ref worldPosition, strArray[Random.Range(0, strArray.Length)], 0);
                    }
                }
            }
        }

        public void CreateBattleFloatDigit(int digitValue, DIGIT_TYPE digitType, ref Vector3 worldPosition, int animatIndex)
        {
            if ((((GameSettings.RenderQuality != SGameRenderQuality.Low) || (digitType == DIGIT_TYPE.MagicAttackCrit)) || ((digitType == DIGIT_TYPE.PhysicalAttackCrit) || (digitType == DIGIT_TYPE.RealAttackCrit))) || (digitType == DIGIT_TYPE.ReceiveGoldCoinInBattle))
            {
                string[] strArray = s_battleFloatDigitAnimatorStates[(int) digitType];
                if (((strArray.Length > 0) && (animatIndex >= 0)) && (animatIndex < strArray.Length))
                {
                    string content = (((digitType != DIGIT_TYPE.ReceiveSpirit) || (digitValue <= 0)) ? string.Empty : "+") + SimpleNumericString.GetNumeric(Mathf.Abs(digitValue));
                    this.CreateBattleFloatText(content, ref worldPosition, strArray[animatIndex], 0);
                }
            }
        }

        private void CreateBattleFloatText(string content, ref Vector3 worldPosition, string animatorState, uint fontSize = 0)
        {
            if (!MonoSingleton<Reconnection>.GetInstance().isProcessingRelayRecover && (!string.IsNullOrEmpty(content) && !string.IsNullOrEmpty(animatorState)))
            {
                GameObject gameObject = Singleton<CGameObjectPool>.GetInstance().GetGameObject("Text/FloatText/FloatText.prefab", enResourceType.BattleScene);
                BattleFloatTextComponent cachedComponent = Singleton<CGameObjectPool>.GetInstance().GetCachedComponent<BattleFloatTextComponent>(gameObject, false);
                if ((gameObject != null) && (null != Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera()))
                {
                    int num;
                    gameObject.get_transform().set_parent(Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera().get_transform());
                    gameObject.get_transform().set_localRotation(Quaternion.get_identity());
                    TextMeshPro texMeshPro = cachedComponent.texMeshPro;
                    Animator anim = cachedComponent.anim;
                    Vector3 vector = Camera.get_main().WorldToScreenPoint(worldPosition);
                    vector.Set(vector.x, vector.y, 30f);
                    gameObject.get_transform().set_position(Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera().ScreenToWorldPoint(vector));
                    if ((animatorState.IndexOf("Crit") != -1) && (cachedComponent.iconTrans != null))
                    {
                        Vector3 vector2 = cachedComponent.iconTrans.get_localPosition();
                        if (animatorState.IndexOf("Left") != -1)
                        {
                            vector2.x = -0.3f * (content.Length + 1);
                        }
                        else
                        {
                            vector2.x = -0.3f * (content.Length + 1);
                        }
                        cachedComponent.iconTrans.set_localPosition(vector2);
                    }
                    if ((animatorState.IndexOf("LastHit") != -1) && (cachedComponent.iconTrans != null))
                    {
                        Vector3 vector3 = cachedComponent.iconTrans.get_localPosition();
                        vector3.x = -0.24f * (content.Length + 1);
                        cachedComponent.iconTrans.set_localPosition(vector3);
                    }
                    if (int.TryParse(content, out num))
                    {
                        Vector3 vector4 = gameObject.get_transform().get_localScale();
                        if (num > 0x5dc)
                        {
                            vector4.x = 1.2f;
                            vector4.y = 1.2f;
                        }
                        else if (num > 600)
                        {
                            vector4.x = 1.1f;
                            vector4.y = 1.1f;
                        }
                        else if (num > 300)
                        {
                            vector4.x = 1f;
                            vector4.y = 1f;
                        }
                        else if (num > 100)
                        {
                            vector4.x = 0.8f;
                            vector4.y = 0.8f;
                        }
                        else
                        {
                            vector4.x = 0.7f;
                            vector4.y = 0.7f;
                        }
                        gameObject.get_transform().set_localScale(vector4);
                    }
                    if (anim != null)
                    {
                        anim.Play(animatorState);
                    }
                    if (texMeshPro != null)
                    {
                        texMeshPro.text = content;
                        texMeshPro.fontSize = (fontSize <= 0) ? ((float) 6) : ((float) fontSize);
                    }
                    Singleton<CGameObjectPool>.GetInstance().RecycleGameObjectDelay(gameObject, 0x7d0, null);
                }
            }
        }

        public void CreateOtherFloatText(enOtherFloatTextContent otherFloatTextContent, ref Vector3 worldPosition, params string[] args)
        {
            if (GameSettings.RenderQuality != SGameRenderQuality.Low)
            {
                string text = Singleton<CTextManager>.GetInstance().GetText(s_otherFloatTextKeys[(int) otherFloatTextContent], args);
                this.CreateBattleFloatText(text, ref worldPosition, s_otherFloatTextAnimatorStates[(int) otherFloatTextContent], 0);
            }
        }

        public void CreateRestrictFloatText(RESTRICT_TYPE restrictType, ref Vector3 worldPosition)
        {
            string text = Singleton<CTextManager>.GetInstance().GetText(s_restrictTextKeys[(int) restrictType]);
            this.CreateBattleFloatText(text, ref worldPosition, s_restrictTextAnimatorState, 0);
        }

        public void CreateSpecifiedFloatText(uint floatTextID, ref Vector3 worldPosition)
        {
            ResBattleFloatText dataByKey = GameDataMgr.floatTextDatabin.GetDataByKey(floatTextID);
            if (dataByKey != null)
            {
                string animatorState = (dataByKey.szAnimation.Length <= 0) ? s_restrictTextAnimatorState : dataByKey.szAnimation;
                this.CreateBattleFloatText(dataByKey.szText, ref worldPosition, animatorState, dataByKey.dwFontsize);
            }
        }

        public void LateUpdate()
        {
            this.updateFloatDigitInLastFrame();
        }

        public static void Preload(ref ActorPreloadTab preloadTab)
        {
            preloadTab.AddParticle("Text/FloatText/FloatText.prefab");
        }

        private void updateFloatDigitInLastFrame()
        {
            if (!MonoSingleton<Reconnection>.GetInstance().isProcessingRelayRecover && ((this.m_floatDigitInfoList != null) && (this.m_floatDigitInfoList.Count != 0)))
            {
                for (int i = 0; i < this.m_floatDigitInfoList.Count; i++)
                {
                    FloatDigitInfo info = this.m_floatDigitInfoList[i];
                    if ((info.m_attacker != 0) && (info.m_target != 0))
                    {
                        Vector3 vector;
                        if (info.m_digitType == DIGIT_TYPE.ReviveHp)
                        {
                            vector = info.m_target.handle.myTransform.get_position();
                            this.CreateBattleFloatDigit(info.m_value, info.m_digitType, ref vector);
                        }
                        else
                        {
                            Vector3 vector4;
                            Vector3 vector5;
                            float num2;
                            float num3;
                            Vector3 vector2 = info.m_target.handle.myTransform.get_position();
                            Vector3 vector3 = info.m_attacker.handle.myTransform.get_position();
                            SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
                            if ((curLvelContext != null) && curLvelContext.m_isCameraFlip)
                            {
                                vector4 = vector2;
                                vector5 = vector3;
                                num2 = Random.Range(0.5f, 1f);
                                num3 = Random.Range(-1f, -0.5f);
                            }
                            else
                            {
                                vector4 = vector3;
                                vector5 = vector2;
                                num2 = Random.Range(-1f, -0.5f);
                                num3 = Random.Range(0.5f, 1f);
                            }
                            Vector3 vector6 = vector4 - vector5;
                            if (vector6.x > 0f)
                            {
                                vector = new Vector3(vector2.x + num2, vector2.y + Math.Abs(num2), vector2.z);
                                this.CreateBattleFloatDigit(info.m_value, info.m_digitType, ref vector, 1);
                            }
                            else
                            {
                                vector = new Vector3(vector2.x + num3, vector2.y + Math.Abs(num3), vector2.z);
                                this.CreateBattleFloatDigit(info.m_value, info.m_digitType, ref vector, 0);
                            }
                        }
                    }
                }
                this.m_floatDigitInfoList.Clear();
            }
        }
    }
}

