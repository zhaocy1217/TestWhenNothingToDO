namespace Assets.Scripts.GameLogic
{
    using ResData;
    using System;
    using System.Runtime.CompilerServices;

    public class SkillBaseDetectionAttribute : Attribute
    {
        [CompilerGenerated]
        private SkillUseRule <UseRule>k__BackingField;

        public SkillBaseDetectionAttribute(SkillUseRule _rule)
        {
            this.UseRule = _rule;
        }

        public SkillUseRule UseRule
        {
            [CompilerGenerated]
            get
            {
                return this.<UseRule>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<UseRule>k__BackingField = value;
            }
        }
    }
}

