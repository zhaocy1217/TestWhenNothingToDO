namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using ResData;
    using System;
    using System.Collections.Generic;

    public class BufferMarkRule
    {
        private BuffHolderComponent buffHolder;
        private DictionaryView<ulong, BufferMark> buffMarkSet = new DictionaryView<ulong, BufferMark>();

        public void AddBufferMark(PoolObjHandle<ActorRoot> _originator, int _markID, uint _markType, SkillUseContext inUseContext)
        {
            uint objID = 0;
            ulong key = 0L;
            if (_originator != 0)
            {
                objID = _originator.handle.ObjID;
                if (this.CheckDependMark(objID, _markID))
                {
                    BufferMark mark;
                    key = (ulong) (objID | (_markID << 0x20));
                    if (this.buffMarkSet.TryGetValue(key, out mark))
                    {
                        mark.AutoTrigger(_originator, inUseContext);
                    }
                    else
                    {
                        mark = new BufferMark(_markID);
                        if (mark.cfgData != null)
                        {
                            mark.Init(this.buffHolder, _originator, _markType);
                            this.buffMarkSet.Add(key, mark);
                        }
                    }
                }
            }
        }

        public bool CheckDependMark(uint _objID, int _markID)
        {
            BufferMark mark;
            ResSkillMarkCfgInfo dataByKey = GameDataMgr.skillMarkDatabin.GetDataByKey((long) _markID);
            if (dataByKey == null)
            {
                return false;
            }
            if (dataByKey.iDependCfgID == 0)
            {
                return true;
            }
            int iDependCfgID = dataByKey.iDependCfgID;
            ulong key = (ulong) (_objID | (iDependCfgID << 0x20));
            return ((this.buffMarkSet.TryGetValue(key, out mark) && (mark != null)) && (mark.GetCurLayer() > 0));
        }

        public void Clear()
        {
            DictionaryView<ulong, BufferMark>.Enumerator enumerator = this.buffMarkSet.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<ulong, BufferMark> current = enumerator.Current;
                current.Value.SetCurLayer(0);
            }
            this.buffMarkSet.Clear();
        }

        public void ClearBufferMark(int _typeMask)
        {
            DictionaryView<ulong, BufferMark>.Enumerator enumerator = this.buffMarkSet.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<ulong, BufferMark> current = enumerator.Current;
                BufferMark mark = current.Value;
                int markType = (int) mark.GetMarkType();
                if ((_typeMask & (((int) 1) << markType)) > 0)
                {
                    mark.SetCurLayer(0);
                }
            }
        }

        public void ClearBufferMark(PoolObjHandle<ActorRoot> _originator, int _markID)
        {
            ulong key = 0L;
            if (_originator != 0)
            {
                BufferMark mark;
                key = (ulong) (_originator.handle.ObjID | (_markID << 0x20));
                if (this.buffMarkSet.TryGetValue(key, out mark))
                {
                    mark.DecLayer(0);
                }
            }
        }

        public void Init(BuffHolderComponent _buffHolder)
        {
            this.buffHolder = _buffHolder;
            this.Clear();
        }

        public void RemoveBufferMark(PoolObjHandle<ActorRoot> _originator, int _markID)
        {
            ulong key = 0L;
            if (_originator != 0)
            {
                BufferMark mark;
                key = (ulong) (_originator.handle.ObjID | (_markID << 0x20));
                if (this.buffMarkSet.TryGetValue(key, out mark))
                {
                    mark.DecLayer(1);
                }
            }
        }

        public void TriggerBufferMark(PoolObjHandle<ActorRoot> _originator, int _markID, SkillUseContext inUseContext)
        {
            ulong key = 0L;
            if (_originator != 0)
            {
                BufferMark mark;
                key = (ulong) (_originator.handle.ObjID | (_markID << 0x20));
                if (this.buffMarkSet.TryGetValue(key, out mark))
                {
                    mark.UpperTrigger(_originator, inUseContext);
                }
            }
        }

        public void UpdateLogic(int nDelta)
        {
            DictionaryView<ulong, BufferMark>.Enumerator enumerator = this.buffMarkSet.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<ulong, BufferMark> current = enumerator.Current;
                current.Value.UpdateLogic(nDelta);
            }
        }
    }
}

