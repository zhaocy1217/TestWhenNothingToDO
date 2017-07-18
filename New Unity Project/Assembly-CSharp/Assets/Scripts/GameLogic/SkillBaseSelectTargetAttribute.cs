namespace Assets.Scripts.GameLogic
{
    using ResData;
    using System;
    using System.Runtime.CompilerServices;

    public class SkillBaseSelectTargetAttribute : Attribute
    {
        [CompilerGenerated]
        private SkillTargetRule <TargetRule>k__BackingField;

        public SkillBaseSelectTargetAttribute(SkillTargetRule _rule)
        {
            this.TargetRule = _rule;
        }

        public SkillTargetRule TargetRule
        {
            [CompilerGenerated]
            get
            {
                return this.<TargetRule>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<TargetRule>k__BackingField = value;
            }
        }
    }
}

