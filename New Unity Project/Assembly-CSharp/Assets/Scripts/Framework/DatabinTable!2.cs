namespace Assets.Scripts.Framework
{
    using System;
    using System.Collections.Generic;

    public class DatabinTable<T, K> : DatabinTableBase
    {
        public DatabinTable(string InName, string InKey) : base(typeof(T))
        {
            base.DataName = InName;
            base.KeyName = InKey;
            base.isDoubleKey = false;
            base.mapItems.Clear();
            base.bLoaded = false;
            Singleton<ResourceLoader>.GetInstance().LoadDatabin(InName, new ResourceLoader.BinLoadCompletedDelegate(this.onRecordLoaded));
        }

        public DatabinTable(string InName, string InKey1, string InKey2) : base(typeof(T))
        {
            base.DataName = InName;
            base.KeyName1 = InKey1;
            base.KeyName2 = InKey2;
            base.isDoubleKey = true;
            base.mapItems.Clear();
            base.bLoaded = false;
            Singleton<ResourceLoader>.GetInstance().LoadDatabin(InName, new ResourceLoader.BinLoadCompletedDelegate(this.onRecordLoaded));
        }

        public void Accept(Action<T> InVisitor)
        {
            base.Reload();
            DebugHelper.Assert(base.isLoaded, "you can't visit databin when it is not loaded.");
            if (base.isLoaded)
            {
                Dictionary<long, object>.Enumerator enumerator = base.mapItems.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<long, object> current = enumerator.Current;
                    InVisitor((T) current.Value);
                }
            }
        }

        public void CopyTo(ref T[] InArrayRef)
        {
            base.Reload();
            DebugHelper.Assert(InArrayRef.Length == base.mapItems.Count, "Failed Databin CopyTo,size miss.");
            int num = 0;
            Dictionary<long, object>.Enumerator enumerator = base.mapItems.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<long, object> current = enumerator.Current;
                InArrayRef[num++] = (T) current.Value;
            }
        }

        public T FindIf(Func<T, bool> InFunc)
        {
            base.Reload();
            if (base.isLoaded)
            {
                Dictionary<long, object>.Enumerator enumerator = base.mapItems.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<long, object> current = enumerator.Current;
                    if (InFunc.Invoke((T) current.Value))
                    {
                        KeyValuePair<long, object> pair2 = enumerator.Current;
                        return (T) pair2.Value;
                    }
                }
            }
            return default(T);
        }

        public T GetAnyData()
        {
            base.Reload();
            if (base.isLoaded && (base.mapItems.Count > 0))
            {
                Dictionary<long, object>.Enumerator enumerator = base.mapItems.GetEnumerator();
                enumerator.MoveNext();
                KeyValuePair<long, object> current = enumerator.Current;
                return (T) current.Value;
            }
            return default(T);
        }

        public T GetDataByIndex(int id)
        {
            base.Reload();
            if (base.isLoaded)
            {
                Dictionary<long, object>.Enumerator enumerator = base.mapItems.GetEnumerator();
                for (int i = 0; enumerator.MoveNext(); i++)
                {
                    if (i == id)
                    {
                        KeyValuePair<long, object> current = enumerator.Current;
                        return (T) current.Value;
                    }
                }
            }
            return default(T);
        }

        public T GetDataByKey(long key)
        {
            base.Reload();
            T dataByKeyInner = (T) base.GetDataByKeyInner(key);
            if (((dataByKeyInner == null) && base.bSimple) && (key != 0))
            {
                base.Unload();
                base.Reload();
                base.bSimple = false;
                dataByKeyInner = (T) base.GetDataByKeyInner(key);
            }
            return dataByKeyInner;
        }

        public T GetDataByKey(uint key)
        {
            return this.GetDataByKey((long) key);
        }

        public void ReduceDatabin(List<long> dataList)
        {
            base.Reload();
            if (base.isLoaded)
            {
                Dictionary<long, object>.Enumerator enumerator = base.mapItems.GetEnumerator();
                List<long> list = new List<long>();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<long, object> current = enumerator.Current;
                    if (!dataList.Contains(current.Key))
                    {
                        KeyValuePair<long, object> pair2 = enumerator.Current;
                        list.Add(pair2.Key);
                    }
                }
                for (int i = 0; i < list.Count; i++)
                {
                    base.mapItems.Remove(list[i]);
                }
                base.bSimple = true;
            }
        }

        public void UpdataData(uint key, T data)
        {
            this.UpdateData((long) key, data);
        }

        private void UpdateData(long key, T data)
        {
            base.Reload();
            if (base.mapItems.ContainsKey(key))
            {
                base.mapItems[key] = data;
            }
            else
            {
                base.mapItems.Add(key, data);
            }
        }
    }
}

