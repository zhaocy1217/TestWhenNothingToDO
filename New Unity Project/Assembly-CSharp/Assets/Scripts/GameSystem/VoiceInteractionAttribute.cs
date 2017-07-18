namespace Assets.Scripts.GameSystem
{
    using System;

    public class VoiceInteractionAttribute : AutoRegisterAttribute, IIdentifierAttribute<int>
    {
        public int KeyType;

        public VoiceInteractionAttribute(int InKeyType)
        {
            this.KeyType = InKeyType;
        }

        public int[] AdditionalIdList
        {
            get
            {
                return null;
            }
        }

        public int ID
        {
            get
            {
                return this.KeyType;
            }
        }
    }
}

