namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.GameSystem;
    using System;
    using System.Runtime.InteropServices;

    public static class EnergyCommon
    {
        private static enOtherFloatTextContent[] energyShortageFloatText;
        private static string[] energySpriteName = new string[] { "Battle_blueHp", "None", "Battle_whiteHp", "Battle_redHp", "Battle_redHp", "Battle_purpleHp" };
        private static string[][] s_energyShowText;

        static EnergyCommon()
        {
            string[][] textArrayArray1 = new string[3][];
            textArrayArray1[0] = new string[] { "Hero_Prop_MaxEp", "Hero_Prop_MaxEp", "Hero_Prop_MaxEnergyEp", "Hero_Prop_MaxAngerEp", "Hero_Prop_MaxMadnessEp", "Hero_Prop_MaxEp" };
            textArrayArray1[1] = new string[] { "Skill_Energy_Cost_Tips", "Skill_Energy_Cost_Tips", "Skill_EnergyEp_Cost_Tips", "Skill_Anger_Cost_Tips", "Skill_Madness_Cost_Tips", "Skill_Energy_Cost_Tips" };
            textArrayArray1[2] = new string[] { "Hero_Prop_EpRecover", "Hero_Prop_EpRecover", "Hero_Prop_Energy_EpRecover", "Hero_Prop_Anger_EpRecover", "Hero_Prop_Madness_EpRecover", "Hero_Prop_EpRecover" };
            s_energyShowText = textArrayArray1;
            energyShortageFloatText = new enOtherFloatTextContent[] { enOtherFloatTextContent.MagicShortage, enOtherFloatTextContent.MagicShortage, enOtherFloatTextContent.EnergyShortage, enOtherFloatTextContent.FuryShortage, enOtherFloatTextContent.MadnessShortage, enOtherFloatTextContent.MagicShortage };
        }

        public static string GetEnergyShowText(uint energyType, EnergyShowType showType = 1)
        {
            if ((energyType < 0) && (energyType >= s_energyShowText[(int) showType].Length))
            {
                energyType = 0;
            }
            return s_energyShowText[(int) showType][energyType];
        }

        public static enOtherFloatTextContent GetShortageText(int index)
        {
            if ((index < 0) || (index >= energyShortageFloatText.Length))
            {
                index = 0;
            }
            return energyShortageFloatText[index];
        }

        public static string GetSpriteName(int index)
        {
            if ((index < 0) || (index >= energySpriteName.Length))
            {
                index = 0;
            }
            return energySpriteName[index];
        }
    }
}

