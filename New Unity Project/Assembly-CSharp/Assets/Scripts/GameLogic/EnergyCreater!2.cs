namespace Assets.Scripts.GameLogic
{
    using System;

    public class EnergyCreater<CreateType, CreateTypeAttribute> : Singleton<EnergyCreater<CreateType, CreateTypeAttribute>> where CreateTypeAttribute: EnergyAttribute
    {
        private DictionaryView<int, Type> energyTypeSet;

        public EnergyCreater()
        {
            this.energyTypeSet = new DictionaryView<int, Type>();
        }

        public CreateType Create(int _type)
        {
            Type type;
            if (this.energyTypeSet.TryGetValue(_type, out type))
            {
                return (CreateType) Activator.CreateInstance(type);
            }
            return default(CreateType);
        }

        public override void Init()
        {
            ClassEnumerator enumerator = new ClassEnumerator(typeof(CreateTypeAttribute), typeof(CreateType), typeof(CreateTypeAttribute).Assembly, true, false, false);
            foreach (Type type in enumerator.results)
            {
                Attribute customAttribute = Attribute.GetCustomAttribute(type, typeof(CreateTypeAttribute));
                this.energyTypeSet.Add((customAttribute as CreateTypeAttribute).attributeType, type);
            }
        }
    }
}

