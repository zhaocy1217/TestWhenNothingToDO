namespace AGE
{
    using Assets.Scripts.Common;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class Track : PooledClassObject
    {
        public Action action;
        private List<DurationEvent> activeEvents;
        public Color color;
        public int curTime;
        public bool enabled;
        private Type eventType;
        public bool execOnActionCompleted;
        public bool execOnForceStopped;
        private bool isCondition;
        private bool isDurationEvent;
        private int preExcuteTime;
        public uint startCount;
        public bool started;
        private bool supportEditMode;
        public List<BaseEvent> trackEvents;
        public int trackIndex;
        public string trackName;
        public Dictionary<int, bool> waitForConditions;

        public Track()
        {
            this.trackEvents = new List<BaseEvent>();
            this.activeEvents = new List<DurationEvent>();
            this.trackIndex = -1;
            this.color = Color.get_red();
            this.trackName = string.Empty;
            base.bChkReset = false;
        }

        public Track(Action _action, Type _eventType)
        {
            this.trackEvents = new List<BaseEvent>();
            this.activeEvents = new List<DurationEvent>();
            this.trackIndex = -1;
            this.color = Color.get_red();
            this.trackName = string.Empty;
            this.CopyData(_action, _eventType);
        }

        public BaseEvent AddEvent(int _time, int _length)
        {
            BaseEvent item = (BaseEvent) Activator.CreateInstance(this.eventType);
            item.time = _time;
            if (this.isDurationEvent)
            {
                (item as DurationEvent).length = _length;
            }
            int count = 0;
            if (this.LocateInsertPos(_time, out count))
            {
                if (count > this.trackEvents.Count)
                {
                    count = this.trackEvents.Count;
                }
                this.trackEvents.Insert(count, item);
            }
            else
            {
                this.trackEvents.Add(item);
            }
            item.track = this;
            return item;
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

        protected bool CheckSkip(int _curTime, int _checkTime)
        {
            return ((_checkTime < _curTime) && (_checkTime >= this.preExcuteTime));
        }

        public Track Clone()
        {
            Track track = ClassObjPool<Track>.Get();
            track.CopyData(this);
            int count = this.trackEvents.Count;
            for (int i = 0; i < count; i++)
            {
                BaseEvent item = this.trackEvents[i].Clone();
                item.track = track;
                track.trackEvents.Add(item);
            }
            track.waitForConditions = this.waitForConditions;
            track.enabled = this.enabled;
            track.color = this.color;
            track.trackName = this.trackName;
            track.execOnActionCompleted = this.execOnActionCompleted;
            track.execOnForceStopped = this.execOnForceStopped;
            return track;
        }

        public void CopyData(Track src)
        {
            this.action = src.action;
            this.eventType = src.eventType;
            this.isDurationEvent = src.isDurationEvent;
            this.isCondition = src.isCondition;
            this.supportEditMode = src.supportEditMode;
            this.curTime = 0;
            this.preExcuteTime = 0;
        }

        public void CopyData(Action _action, Type _eventType)
        {
            this.action = _action;
            this.eventType = _eventType;
            if (this.eventType.IsSubclassOf(typeof(DurationEvent)))
            {
                this.isDurationEvent = true;
            }
            if (this.eventType.IsSubclassOf(typeof(TickCondition)) || this.eventType.IsSubclassOf(typeof(DurationCondition)))
            {
                this.isCondition = true;
            }
            this.supportEditMode = ((BaseEvent) Activator.CreateInstance(this.eventType)).SupportEditMode();
            this.curTime = 0;
            this.preExcuteTime = 0;
        }

        public void DoLoop()
        {
        }

        public Dictionary<string, bool> GetAssociatedResources()
        {
            Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
            foreach (BaseEvent event2 in this.trackEvents)
            {
                Dictionary<string, bool> associatedResources = event2.GetAssociatedResources();
                if (associatedResources != null)
                {
                    foreach (string str in associatedResources.Keys)
                    {
                        if (dictionary.ContainsKey(str))
                        {
                            Dictionary<string, bool> dictionary3;
                            string str2;
                            bool flag = dictionary3[str2];
                            (dictionary3 = dictionary)[str2 = str] = flag | associatedResources[str];
                        }
                        else
                        {
                            dictionary.Add(str, associatedResources[str]);
                        }
                    }
                    continue;
                }
            }
            return dictionary;
        }

        public void GetAssociatedResources(Dictionary<object, AssetRefType> results, int markID)
        {
            for (int i = 0; i < this.trackEvents.Count; i++)
            {
                BaseEvent event2 = this.trackEvents[i];
                if (event2 != null)
                {
                    event2.GetAssociatedResources(results, markID);
                }
            }
        }

        public BaseEvent GetEvent(int index)
        {
            if ((index >= 0) && (index < this.trackEvents.Count))
            {
                return this.trackEvents[index];
            }
            return null;
        }

        public int GetEventEndTime()
        {
            if (this.trackEvents.Count == 0)
            {
                return 0;
            }
            if (this.isDurationEvent)
            {
                return ((this.trackEvents[this.trackEvents.Count - 1] as DurationEvent).End + 0x21);
            }
            return ((this.trackEvents[this.trackEvents.Count - 1] as TickEvent).time + 0x21);
        }

        public int GetEventsCount()
        {
            return this.trackEvents.Count;
        }

        public int GetIndexOfEvent(BaseEvent _curEvent)
        {
            return this.trackEvents.LastIndexOf(_curEvent);
        }

        public BaseEvent GetOffsetEvent(BaseEvent _curEvent, int _offset)
        {
            int num = this.trackEvents.LastIndexOf(_curEvent);
            if (this.Loop)
            {
                int num2 = (num + _offset) % this.trackEvents.Count;
                if (num2 < 0)
                {
                    num2 += this.trackEvents.Count;
                }
                return this.trackEvents[num2];
            }
            int num3 = num + _offset;
            if ((num3 >= 0) && (num3 < this.trackEvents.Count))
            {
                return this.trackEvents[num3];
            }
            return null;
        }

        public bool LocateEvent(int _curTime, out int _result)
        {
            _result = 0;
            int length = this.Length;
            int count = this.trackEvents.Count;
            if (count == 0)
            {
                return false;
            }
            if (_curTime < 0)
            {
                _curTime = 0;
            }
            else if (_curTime > length)
            {
                _curTime = length;
            }
            int num3 = -1;
            int num4 = 0;
            int num5 = this.trackEvents.Count - 1;
            while (num4 != num5)
            {
                int num6 = ((num4 + num5) / 2) + 1;
                if (_curTime < this.trackEvents[num6].time)
                {
                    num5 = num6 - 1;
                }
                else
                {
                    num4 = num6;
                }
            }
            int time = this.trackEvents[0].time;
            int num8 = this.trackEvents[count - 1].time;
            if ((num4 == 0) && (_curTime < time))
            {
                num3 = -1;
            }
            else
            {
                num3 = num4;
            }
            if (num3 < 0)
            {
                _result = -1 + (_curTime / time);
            }
            else if (num3 == (count - 1))
            {
                _result = (count - 1) + ((_curTime - num8) / (length - num8));
            }
            else
            {
                int num9 = this.trackEvents[num3].time;
                int num10 = this.trackEvents[num3 + 1].time;
                _result = num3 + ((_curTime - num9) / (num10 - num9));
            }
            return true;
        }

        private bool LocateInsertPos(int _curTime, out int _result)
        {
            _result = 0;
            int length = this.Length;
            int count = this.trackEvents.Count;
            if (count == 0)
            {
                return false;
            }
            if (_curTime < 0)
            {
                _curTime = 0;
            }
            else if (_curTime > length)
            {
                _curTime = length;
            }
            int num3 = -1;
            int num4 = 0;
            int num5 = this.trackEvents.Count - 1;
            while (num4 != num5)
            {
                int num6 = ((num4 + num5) / 2) + 1;
                if (_curTime < this.trackEvents[num6].time)
                {
                    num5 = num6 - 1;
                }
                else
                {
                    num4 = num6;
                }
            }
            int time = this.trackEvents[0].time;
            if ((num4 == 0) && (_curTime < time))
            {
                num3 = -1;
            }
            else
            {
                num3 = num4;
            }
            if (num3 < 0)
            {
                _result = 0;
            }
            else if (num3 == (count - 1))
            {
                _result = count;
            }
            else
            {
                _result = num3;
            }
            return true;
        }

        public override void OnRelease()
        {
            int count = this.trackEvents.Count;
            for (int i = 0; i < count; i++)
            {
                this.trackEvents[i].Release();
            }
            this.trackEvents.Clear();
            this.activeEvents.Clear();
            this.waitForConditions = null;
            this.action = null;
        }

        public override void OnUse()
        {
            base.OnUse();
            this.waitForConditions = null;
            this.curTime = 0;
            this.preExcuteTime = 0;
            this.trackIndex = -1;
            this.trackName = string.Empty;
            this.color = Color.get_red();
            this.execOnActionCompleted = false;
            this.execOnForceStopped = false;
            this.eventType = null;
            this.isDurationEvent = false;
            this.isCondition = false;
            this.trackEvents.Clear();
            this.action = null;
            this.started = false;
            this.enabled = false;
            this.startCount = 0;
            this.supportEditMode = false;
            this.activeEvents.Clear();
        }

        public void Process(int _curTime)
        {
            this.preExcuteTime = this.curTime;
            this.curTime = _curTime;
            int num = 0;
            if (this.LocateEvent(_curTime, out num) && (num >= 0))
            {
                int length = this.Length;
                if (_curTime >= length)
                {
                    num = this.trackEvents.Count - 1;
                }
                int num3 = num - 1;
                if (num3 < 0)
                {
                    num3 = 0;
                }
                int num4 = num + 1;
                if (num4 >= this.trackEvents.Count)
                {
                    num4 = num;
                }
                if (this.isDurationEvent)
                {
                    for (int i = num3; i < this.trackEvents.Count; i++)
                    {
                        DurationEvent item = this.trackEvents[i] as DurationEvent;
                        if (this.CheckSkip(_curTime, item.Start) && item.CheckConditions(this.action))
                        {
                            if (this.activeEvents.Count == 0)
                            {
                                item.Enter(this.action, this);
                            }
                            else
                            {
                                DurationEvent event3 = this.activeEvents[0];
                                int num6 = event3.End - item.Start;
                                item.EnterBlend(this.action, this, event3, num6);
                            }
                            this.activeEvents.Add(item);
                        }
                        if (this.CheckSkip(_curTime, item.End) && this.activeEvents.Contains(item))
                        {
                            if (this.activeEvents.Count > 1)
                            {
                                DurationEvent event4 = this.activeEvents[1];
                                int num7 = item.End - event4.Start;
                                item.LeaveBlend(this.action, this, event4, num7);
                            }
                            else
                            {
                                item.Leave(this.action, this);
                            }
                            this.activeEvents.Remove(item);
                        }
                    }
                }
                else
                {
                    for (int j = num3; j < this.trackEvents.Count; j++)
                    {
                        TickEvent event5 = this.trackEvents[j] as TickEvent;
                        if (this.CheckSkip(_curTime, event5.time) && event5.CheckConditions(this.action))
                        {
                            event5.Process(this.action, this);
                        }
                    }
                    if (num != num4)
                    {
                        TickEvent event6 = this.trackEvents[num] as TickEvent;
                        TickEvent event7 = this.trackEvents[num4] as TickEvent;
                        float num9 = ((float) (_curTime - event6.time)) / ((float) (event7.time - event6.time));
                        event7.ProcessBlend(this.action, this, event6, num9);
                    }
                    else
                    {
                        TickEvent event8 = this.trackEvents[num] as TickEvent;
                        int num10 = _curTime - event8.time;
                        event8.PostProcess(this.action, this, num10);
                    }
                }
                if (this.activeEvents.Count == 1)
                {
                    DurationEvent event9 = this.activeEvents[0];
                    int num11 = 0;
                    if (_curTime >= event9.Start)
                    {
                        num11 = _curTime - event9.Start;
                    }
                    else
                    {
                        num11 = (_curTime + length) - event9.Start;
                    }
                    event9.Process(this.action, this, num11);
                }
                else if (this.activeEvents.Count == 2)
                {
                    DurationEvent event10 = this.activeEvents[0];
                    DurationEvent event11 = this.activeEvents[1];
                    if ((event10.Start < event11.Start) && (event10.End < length))
                    {
                        int num12 = _curTime - event11.Start;
                        int num13 = _curTime - event10.Start;
                        float num14 = ((float) (_curTime - event11.Start)) / ((float) (event10.End - event11.Start));
                        event11.ProcessBlend(this.action, this, num12, event10, num13, num14);
                    }
                    else if ((event10.Start < event11.Start) && (event10.End >= length))
                    {
                        if (_curTime >= event11.Start)
                        {
                            int num15 = _curTime - event11.Start;
                            int num16 = _curTime - event10.Start;
                            float num17 = ((float) (_curTime - event11.Start)) / ((float) (event10.End - event11.Start));
                            event11.ProcessBlend(this.action, this, num15, event10, num16, num17);
                        }
                        else
                        {
                            int num18 = (_curTime + length) - event11.Start;
                            int num19 = (_curTime + length) - event10.Start;
                            float num20 = ((float) ((_curTime + length) - event11.Start)) / ((float) (event10.End - event11.Start));
                            event11.ProcessBlend(this.action, this, num18, event10, num19, num20);
                        }
                    }
                    else
                    {
                        int num21 = _curTime - event11.Start;
                        int num22 = (_curTime + length) - event10.Start;
                        float num23 = ((float) (_curTime - event11.Start)) / ((float) ((event10.End - length) - event11.Start));
                        event11.ProcessBlend(this.action, this, num21, event10, num22, num23);
                    }
                }
            }
        }

        public void Start(Action _action)
        {
            if (this.enabled)
            {
                if (!this.isCondition)
                {
                    _action.SetCondition(this, true);
                }
                this.curTime = 0;
                this.preExcuteTime = 0;
                this.started = true;
                this.startCount++;
            }
        }

        public void Stop(Action _action)
        {
            if (this.started)
            {
                for (int i = 0; i < this.activeEvents.Count; i++)
                {
                    this.activeEvents[i].Leave(this.action, this);
                }
                this.activeEvents.Clear();
                if (!this.isCondition)
                {
                    _action.SetCondition(this, false);
                }
                this.started = false;
            }
        }

        public bool SupportEditMode()
        {
            return this.supportEditMode;
        }

        public Type EventType
        {
            get
            {
                return this.eventType;
            }
        }

        public bool IsDurationEvent
        {
            get
            {
                return this.isDurationEvent;
            }
        }

        public int Length
        {
            get
            {
                return this.action.length;
            }
        }

        public bool Loop
        {
            get
            {
                return this.action.loop;
            }
        }
    }
}

