namespace AGE
{
    using Assets.Scripts.Common;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public abstract class BaseEvent : PooledClassObject
    {
        public int time;
        public Track track;
        public Dictionary<int, bool> waitForConditions = new Dictionary<int, bool>();

        public BaseEvent()
        {
            base.bChkReset = false;
        }

        public bool CheckConditions(Action _action)
        {
            if (this.waitForConditions != null)
            {
                Dictionary<int, bool>.Enumerator enumerator = this.waitForConditions.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<int, bool> current = enumerator.Current;
                    int key = current.Key;
                    if ((key >= 0) && (key < _action.GetConditionCount()))
                    {
                        KeyValuePair<int, bool> pair2 = enumerator.Current;
                        if (_action.GetCondition(_action.GetTrack(key)) != pair2.Value)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public abstract BaseEvent Clone();
        protected virtual void CopyData(BaseEvent src)
        {
            this.time = src.time;
            this.waitForConditions = src.waitForConditions;
        }

        public virtual List<string> GetAssociatedAction()
        {
            List<string> list = new List<string>();
            for (Type type = base.GetType(); (type == typeof(BaseEvent)) || type.IsSubclassOf(typeof(BaseEvent)); type = type.BaseType)
            {
                foreach (FieldInfo info in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    if ((info.FieldType == typeof(string)) && Attribute.IsDefined(info, typeof(ActionReference)))
                    {
                        string item = info.GetValue(this) as string;
                        if ((item.Length > 0) && !list.Contains(item))
                        {
                            list.Add(item);
                        }
                    }
                }
            }
            return list;
        }

        public virtual Dictionary<string, bool> GetAssociatedResources()
        {
            Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
            Type inType = base.GetType();
            Type type2 = typeof(string);
            List<AssetReferenceMeta> associatedResourcesMeta = Singleton<ActionMetaRepository>.instance.GetAssociatedResourcesMeta(inType);
            if (associatedResourcesMeta != null)
            {
                for (int i = 0; i < associatedResourcesMeta.Count; i++)
                {
                    AssetReferenceMeta meta = associatedResourcesMeta[i];
                    if (meta.MetaFieldInfo.FieldType == type2)
                    {
                        dictionary.Add(meta.MetaFieldInfo.GetValue(this) as string, true);
                    }
                }
            }
            return dictionary;
        }

        public virtual void GetAssociatedResources(Dictionary<object, AssetRefType> results, int markID)
        {
            Type inType = base.GetType();
            Type type2 = typeof(string);
            Type type3 = typeof(int);
            Type type4 = typeof(uint);
            List<AssetReferenceMeta> associatedResourcesMeta = Singleton<ActionMetaRepository>.instance.GetAssociatedResourcesMeta(inType);
            if (associatedResourcesMeta != null)
            {
                for (int i = 0; i < associatedResourcesMeta.Count; i++)
                {
                    AssetReferenceMeta meta = associatedResourcesMeta[i];
                    switch (meta.Reference.RefType)
                    {
                        case AssetRefType.Action:
                        case AssetRefType.Prefab:
                        case AssetRefType.Particle:
                        case AssetRefType.Sound:
                            if (meta.MetaFieldInfo.FieldType == type2)
                            {
                                string key = meta.MetaFieldInfo.GetValue(this) as string;
                                if (((key != null) && (key.Length > 0)) && !results.ContainsKey(key))
                                {
                                    results.Add(key, meta.Reference.RefType);
                                }
                            }
                            break;

                        case AssetRefType.SkillID:
                        case AssetRefType.SkillCombine:
                        case AssetRefType.MonsterConfigId:
                            if ((meta.MetaFieldInfo.FieldType == type3) || (meta.MetaFieldInfo.FieldType == type4))
                            {
                                int num2 = (int) meta.MetaFieldInfo.GetValue(this);
                                if (num2 != 0)
                                {
                                    ulong num4 = ((ulong) meta.Reference.RefType) << 0x20;
                                    ulong num3 = ((ulong) num2) | num4;
                                    if (!results.ContainsKey(num3))
                                    {
                                        results.Add(num3, meta.Reference.RefType);
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }

        public virtual void OnLoaded()
        {
        }

        public override void OnUse()
        {
            base.OnUse();
            this.time = 0;
            this.waitForConditions = null;
            this.track = null;
        }

        public virtual bool SupportEditMode()
        {
            return false;
        }

        public virtual bool bScaleStart
        {
            get
            {
                return true;
            }
        }

        public float timeSec
        {
            get
            {
                return ActionUtility.MsToSec(this.time);
            }
        }
    }
}

