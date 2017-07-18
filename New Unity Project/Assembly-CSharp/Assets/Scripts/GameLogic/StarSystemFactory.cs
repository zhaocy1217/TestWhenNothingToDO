namespace Assets.Scripts.GameLogic
{
    using System;

    public class StarSystemFactory
    {
        private DictionaryView<int, Type> Factories = new DictionaryView<int, Type>();

        public StarSystemFactory(Type InAttributeType, Type InInterfaceType)
        {
            DebugHelper.Assert((InAttributeType != null) && (InInterfaceType != null));
            Type[] types = InInterfaceType.Assembly.GetTypes();
            for (int i = 0; (types != null) && (i < types.Length); i++)
            {
                Type inType = types[i];
                object[] customAttributes = inType.GetCustomAttributes(InAttributeType, true);
                if (customAttributes != null)
                {
                    for (int j = 0; j < customAttributes.Length; j++)
                    {
                        IIdentifierAttribute<int> attribute = customAttributes[j] as IIdentifierAttribute<int>;
                        if (attribute != null)
                        {
                            this.RegisterType(attribute.ID, inType);
                        }
                    }
                }
            }
        }

        public object Create(int InKeyType)
        {
            Type type = null;
            if (this.Factories.TryGetValue(InKeyType, out type))
            {
                DebugHelper.Assert(type != null);
                return Activator.CreateInstance(type);
            }
            return null;
        }

        private void RegisterType(int InKey, Type InType)
        {
            DebugHelper.Assert(!this.Factories.ContainsKey(InKey));
            this.Factories.Add(InKey, InType);
        }
    }
}

