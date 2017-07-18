namespace AGE
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class ActionMetaRepository : Singleton<ActionMetaRepository>
    {
        public DictionaryView<Type, List<AssetReferenceMeta>> Repositories = new DictionaryView<Type, List<AssetReferenceMeta>>();

        public List<AssetReferenceMeta> GetAssociatedResourcesMeta(Type InType)
        {
            if ((InType != typeof(BaseEvent)) && !InType.IsSubclassOf(typeof(BaseEvent)))
            {
                return null;
            }
            List<AssetReferenceMeta> list = null;
            if (!this.Repositories.TryGetValue(InType, out list))
            {
                list = new List<AssetReferenceMeta>();
                this.Repositories.Add(InType, list);
                for (Type type = InType; (type == typeof(BaseEvent)) || type.IsSubclassOf(typeof(BaseEvent)); type = type.BaseType)
                {
                    FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                    if (fields != null)
                    {
                        for (int i = 0; i < fields.Length; i++)
                        {
                            FieldInfo element = fields[i];
                            AssetReference customAttribute = Attribute.GetCustomAttribute(element, typeof(AssetReference)) as AssetReference;
                            if (customAttribute != null)
                            {
                                AssetReferenceMeta item = new AssetReferenceMeta();
                                item.MetaFieldInfo = element;
                                item.Reference = customAttribute;
                                list.Add(item);
                            }
                        }
                    }
                }
            }
            return list;
        }
    }
}

