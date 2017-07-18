namespace AGE
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class AssetReference : Attribute
    {
        [CompilerGenerated]
        private AssetRefType <RefType>k__BackingField;

        public AssetReference(AssetRefType refType)
        {
            this.RefType = refType;
        }

        public AssetRefType RefType
        {
            [CompilerGenerated]
            get
            {
                return this.<RefType>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<RefType>k__BackingField = value;
            }
        }
    }
}

