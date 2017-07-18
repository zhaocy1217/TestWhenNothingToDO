using System;
using System.Runtime.CompilerServices;

public class ObjectTypeSerializerAttribute : Attribute
{
    [CompilerGenerated]
    private Type <type>k__BackingField;

    public ObjectTypeSerializerAttribute(Type serializeType)
    {
        this.type = serializeType;
    }

    public Type type
    {
        [CompilerGenerated]
        get
        {
            return this.<type>k__BackingField;
        }
        [CompilerGenerated]
        private set
        {
            this.<type>k__BackingField = value;
        }
    }
}

