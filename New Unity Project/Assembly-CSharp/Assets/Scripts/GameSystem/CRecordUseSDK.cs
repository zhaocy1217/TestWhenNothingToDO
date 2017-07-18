namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class CRecordUseSDK : Singleton<CRecordUseSDK>
    {
        private const string ASSISTNUM = "ASSISTNUM";
        private const string DEADNUM = "DEADNUM";
        private const string GRADENAME = "GRADE";
        private const string HERONAME = "HERO";
        private const string KILLNUM = "KILLNUM";
        private bool m_bIsCallGameJoyGenerate;
        private bool m_bIsCallStopGameJoyRecord;
        private bool m_bIsMvp;
        private bool m_bIsRecordMomentsEnable;
        private bool m_bIsStartRecordOk;
        private RECORD_EVENT_PRIORITY m_enLastEventPriority;
        private CHECK_PERMISSION_STUTAS m_enmCheckPermissionRes = CHECK_PERMISSION_STUTAS.CHECK_PERMISSION_STUTAS_TYPE_WITHOUTRESULT;
        private CHECK_WHITELIST_STATUS m_enmCheckWhiteListStatus;
        private PoolObjHandle<ActorRoot> m_hostActor;
        private int m_iContinuKillMaxNum = -1;
        private int m_iHostPlaterAssistNum;
        private int m_iHostPlaterDeadNum;
        private int m_iHostPlaterKillNum;
        private long m_lGameEndTime;
        private long m_lGameStartTime;
        private long m_lLastEventEndTime;
        private long m_lLastEventStartTime;
        private long m_lVideoTimeLen;
        private GameObject m_objKingBar;
        private Transform m_RecorderPanel;
        private Dictionary<RECORD_EVENT_PRIORITY, SortedList<long, long>> m_stRecordInfo = new Dictionary<RECORD_EVENT_PRIORITY, SortedList<long, long>>();
        private string m_strHostPlayerName;
        private uint m_ui210KillEventTotalTime = 0xea60;
        private uint m_ui543KillEventTotalTime = 0x15f90;
        private uint m_uiEventEndTimeInterval = 0x2710;
        private uint m_uiEventNumMax = 5;
        private uint m_uiEventStartTimeInterval = 0x1388;
        private uint m_uiMinSpaceLimit = 200;
        private uint m_uiOnceDoubleEventTimeIntervalReduce = 0x1388;
        private uint m_uiWarningSpaceLimit = 500;
        private const string MODENAME = "MODE";
        private const string MULTIKILL = "MULTIKILL";

        private void AddRecordEvent(RECORD_EVENT_PRIORITY eventPriority, long lStartTime, long lEndTime)
        {
            if (this.m_stRecordInfo != null)
            {
                SortedList<long, long> list = null;
                if (!this.m_stRecordInfo.TryGetValue(eventPriority, out list))
                {
                    list = new SortedList<long, long>();
                    this.m_stRecordInfo.Add(eventPriority, list);
                }
                if ((list != null) && !list.ContainsKey(lStartTime))
                {
                    list.Add(lStartTime, lEndTime);
                }
                this.m_enLastEventPriority = RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_INVALID;
            }
        }

        public void CallGameJoyGenerateWithNothing()
        {
            if ((this.m_bIsRecordMomentsEnable && this.m_bIsStartRecordOk) && !this.m_bIsCallGameJoyGenerate)
            {
                this.m_bIsCallGameJoyGenerate = true;
                Singleton<GameJoy>.instance.GenerateMomentsVideo(null, null, null);
            }
        }

        private void CheckPermission()
        {
            GameJoy.checkSDKPermission();
        }

        private bool CheckStorage()
        {
            bool flag = true;
            long num = 0L;
            object[] objArray1 = new object[] { Application.get_persistentDataPath() };
            using (AndroidJavaObject obj2 = new AndroidJavaObject("android.os.StatFs", objArray1))
            {
                num = ((((long) obj2.Call<int>("getBlockSize", new object[0])) * ((long) obj2.Call<int>("getAvailableBlocks", new object[0]))) / 0x400L) / 0x400L;
            }
            if (num < this.m_uiMinSpaceLimit)
            {
                this.SetKingBarSliderState(false);
                Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("ReplayKit_Disk_Space_Limit"), false, 1.5f, null, new object[0]);
                flag = false;
            }
            return flag;
        }

        private bool CheckStorageAndPermission()
        {
            if (!this.CheckStorage())
            {
                return false;
            }
            this.CheckPermission();
            return true;
        }

        private void CheckWhiteList()
        {
            GameJoy.CheckRecorderAvailability();
        }

        private void ChooseTopEvent()
        {
            int num = 0;
            int num2 = 0;
            bool flag = false;
            SortedList<long, RECORD_INFO> list = new SortedList<long, RECORD_INFO>();
            SortedList<long, long> list2 = null;
            for (RECORD_EVENT_PRIORITY record_event_priority = RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_PENTAKILL; record_event_priority > RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_DOUBLEKILL; record_event_priority -= 1)
            {
                if (((this.m_stRecordInfo != null) && this.m_stRecordInfo.TryGetValue(record_event_priority, out list2)) && (list2 != null))
                {
                    IEnumerator<KeyValuePair<long, long>> enumerator = list2.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        num2++;
                        if (num2 > this.m_uiEventNumMax)
                        {
                            flag = true;
                            break;
                        }
                        KeyValuePair<long, long> current = enumerator.Current;
                        long key = (current.Key >= this.m_uiEventStartTimeInterval) ? (enumerator.Current.Key - this.m_uiEventStartTimeInterval) : 0L;
                        KeyValuePair<long, long> pair3 = enumerator.Current;
                        long num4 = pair3.Value + this.m_uiEventEndTimeInterval;
                        num4 = (num4 <= this.m_lGameEndTime) ? num4 : this.m_lGameEndTime;
                        num += (int) (num4 - key);
                        if (num > this.m_ui543KillEventTotalTime)
                        {
                            flag = true;
                            break;
                        }
                        if (!list.ContainsKey(key))
                        {
                            list.Add(key, new RECORD_INFO(record_event_priority, num4));
                        }
                    }
                }
                if (flag)
                {
                    break;
                }
            }
            if (!flag && (num < this.m_ui210KillEventTotalTime))
            {
                bool flag2 = false;
                SortedList<int, SortedList<long, long>> assistInfo = null;
                SortedList<long, long> list4 = null;
                if ((this.m_stRecordInfo != null) && this.m_stRecordInfo.TryGetValue(RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_ASSIST, out list4))
                {
                    flag2 = true;
                }
                for (RECORD_EVENT_PRIORITY record_event_priority2 = RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_DOUBLEKILL; record_event_priority2 > RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_ASSIST; record_event_priority2 -= 1)
                {
                    if (((this.m_stRecordInfo != null) && this.m_stRecordInfo.TryGetValue(record_event_priority2, out list2)) && (list2 != null))
                    {
                        IEnumerator<KeyValuePair<long, long>> enumerator2 = list2.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            KeyValuePair<long, long> pair4 = enumerator2.Current;
                            long num5 = (pair4.Key >= this.m_uiEventStartTimeInterval) ? (enumerator2.Current.Key - this.m_uiEventStartTimeInterval) : 0L;
                            KeyValuePair<long, long> pair6 = enumerator2.Current;
                            long num6 = (pair6.Value + this.m_uiEventEndTimeInterval) - this.m_uiOnceDoubleEventTimeIntervalReduce;
                            num6 = (num6 <= this.m_lGameEndTime) ? num6 : this.m_lGameEndTime;
                            if (!flag2)
                            {
                                num2++;
                                if (num2 > this.m_uiEventNumMax)
                                {
                                    flag = true;
                                    break;
                                }
                                num += (int) (num6 - num5);
                                if (num > this.m_ui210KillEventTotalTime)
                                {
                                    flag = true;
                                    break;
                                }
                                if (!list.ContainsKey(num5))
                                {
                                    list.Add(num5, new RECORD_INFO(record_event_priority2, num6));
                                }
                            }
                            else
                            {
                                int assistCountWithTime = this.GetAssistCountWithTime((float) num5, (float) num6);
                                this.InsertAssistInfo(ref assistInfo, assistCountWithTime, num5, num6);
                            }
                        }
                        if ((flag2 && (assistInfo != null)) && (assistInfo.Count > 0))
                        {
                            for (int i = assistInfo.Count - 1; i >= 0; i--)
                            {
                                list2 = assistInfo.Values[i];
                                if (list2 != null)
                                {
                                    IEnumerator<KeyValuePair<long, long>> enumerator3 = list2.GetEnumerator();
                                    while (enumerator3.MoveNext())
                                    {
                                        KeyValuePair<long, long> pair7 = enumerator3.Current;
                                        long num10 = pair7.Key;
                                        KeyValuePair<long, long> pair8 = enumerator3.Current;
                                        long num11 = pair8.Value;
                                        num2++;
                                        if (num2 > this.m_uiEventNumMax)
                                        {
                                            flag = true;
                                            break;
                                        }
                                        num += (int) (num11 - num10);
                                        if (num > this.m_ui210KillEventTotalTime)
                                        {
                                            flag = true;
                                            break;
                                        }
                                        if (!list.ContainsKey(num10))
                                        {
                                            list.Add(num10, new RECORD_INFO(record_event_priority2, num11));
                                        }
                                    }
                                }
                            }
                            assistInfo.Clear();
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
                if ((!flag && (this.m_stRecordInfo != null)) && (this.m_stRecordInfo.TryGetValue(RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_ASSIST, out list2) && (list2 != null)))
                {
                    IEnumerator<KeyValuePair<long, long>> enumerator4 = list2.GetEnumerator();
                    while (enumerator4.MoveNext())
                    {
                        KeyValuePair<long, long> pair9 = enumerator4.Current;
                        long num12 = (pair9.Key >= this.m_uiEventStartTimeInterval) ? (enumerator4.Current.Key - this.m_uiEventStartTimeInterval) : 0L;
                        KeyValuePair<long, long> pair11 = enumerator4.Current;
                        long num13 = (pair11.Value + this.m_uiEventEndTimeInterval) - this.m_uiOnceDoubleEventTimeIntervalReduce;
                        num13 = (num13 <= this.m_lGameEndTime) ? num13 : this.m_lGameEndTime;
                        num2++;
                        if (num2 > this.m_uiEventNumMax)
                        {
                            break;
                        }
                        num += (int) (num13 - num12);
                        if (num > this.m_ui210KillEventTotalTime)
                        {
                            break;
                        }
                        if (!list.ContainsKey(num12))
                        {
                            list.Add(num12, new RECORD_INFO(RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_ASSIST, num13));
                        }
                    }
                }
            }
            List<TimeStamp> timeStampList = new List<TimeStamp>();
            long endTime = 0L;
            long startTime = 0L;
            IEnumerator<KeyValuePair<long, RECORD_INFO>> enumerator5 = list.GetEnumerator();
            while (enumerator5.MoveNext())
            {
                KeyValuePair<long, RECORD_INFO> pair12 = enumerator5.Current;
                long num16 = pair12.Key;
                KeyValuePair<long, RECORD_INFO> pair13 = enumerator5.Current;
                long lEndTime = pair13.Value.lEndTime;
                if ((timeStampList.Count > 0) && (endTime > num16))
                {
                    timeStampList.RemoveAt(timeStampList.Count - 1);
                    num -= (int) (endTime - startTime);
                    endTime = (endTime + num16) / 2L;
                    num16 = endTime;
                    num += (int) (endTime - startTime);
                    timeStampList.Add(new TimeStamp(startTime, endTime));
                }
                startTime = num16;
                endTime = lEndTime;
                timeStampList.Add(new TimeStamp(num16, lEndTime));
            }
            this.m_bIsCallGameJoyGenerate = true;
            Singleton<GameJoy>.instance.GenerateMomentsVideo(timeStampList, this.GetVideoName(), this.GetExtraInfos());
        }

        private void CloseRecorderPanel()
        {
            if (this.m_RecorderPanel != null)
            {
                Transform transform = this.m_RecorderPanel.FindChild("Extra");
                if (transform != null)
                {
                    transform.get_gameObject().CustomSetActive(false);
                }
            }
        }

        private int ConvertMaxMultiKillPriorityToResDef()
        {
            int num = -1;
            if (this.m_stRecordInfo.Count > 0)
            {
                if (this.m_stRecordInfo.ContainsKey(RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_PENTAKILL))
                {
                    return 6;
                }
                if (this.m_stRecordInfo.ContainsKey(RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_QUATARYKILL))
                {
                    return 5;
                }
                if (this.m_stRecordInfo.ContainsKey(RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_TRIPLEKILL))
                {
                    return 4;
                }
                if (this.m_stRecordInfo.ContainsKey(RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_DOUBLEKILL))
                {
                    return 3;
                }
                if (this.m_stRecordInfo.ContainsKey(RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_ONCEKILL))
                {
                    return 2;
                }
                if (this.m_stRecordInfo.ContainsKey(RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_ASSIST))
                {
                    num = 0;
                }
            }
            return num;
        }

        public void DoFightOver()
        {
            if (this.m_bIsRecordMomentsEnable && this.m_bIsStartRecordOk)
            {
                this.UpdateRecordEvent(new PoolObjHandle<ActorRoot>(), RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_INVALID);
                this.m_lGameEndTime = GameJoy.getSystemCurrentTimeMillis - this.m_lGameStartTime;
                if (this.m_hostActor != 0)
                {
                    this.m_strHostPlayerName = this.m_hostActor.handle.name;
                    uint mvpPlayer = Singleton<BattleStatistic>.instance.GetMvpPlayer(this.m_hostActor.handle.TheActorMeta.ActorCamp, true);
                    if ((mvpPlayer != 0) && (mvpPlayer == this.m_hostActor.handle.TheActorMeta.PlayerId))
                    {
                        this.m_bIsMvp = true;
                    }
                }
                PlayerKDA hostKDA = null;
                if ((Singleton<BattleLogic>.GetInstance().battleStat != null) && (Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat != null))
                {
                    hostKDA = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat.GetHostKDA();
                    if (hostKDA != null)
                    {
                        this.m_iHostPlaterKillNum = hostKDA.numKill;
                        this.m_iHostPlaterDeadNum = hostKDA.numDead;
                        this.m_iHostPlaterAssistNum = hostKDA.numAssist;
                    }
                }
                this.StopMomentsRecording();
            }
        }

        private int GetAssistCountWithTime(float fStartTime, float fEndTime)
        {
            int num = 0;
            SortedList<long, long> list = null;
            if (((this.m_stRecordInfo != null) && this.m_stRecordInfo.TryGetValue(RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_ASSIST, out list)) && (list != null))
            {
                IEnumerator<KeyValuePair<long, long>> enumerator = list.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<long, long> current = enumerator.Current;
                    if (fStartTime <= ((float) current.Key))
                    {
                        KeyValuePair<long, long> pair2 = enumerator.Current;
                        if (((float) pair2.Value) <= fEndTime)
                        {
                            num++;
                            continue;
                        }
                    }
                    KeyValuePair<long, long> pair3 = enumerator.Current;
                    if (((float) pair3.Value) > fEndTime)
                    {
                        return num;
                    }
                }
            }
            return num;
        }

        private Dictionary<string, string> GetExtraInfos()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if (curLvelContext != null)
            {
                dictionary.Add("MODE", curLvelContext.m_gameMatchName);
            }
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                ResRankGradeConf dataByKey = GameDataMgr.rankGradeDatabin.GetDataByKey((uint) masterRoleInfo.m_rankGrade);
                if (dataByKey != null)
                {
                    dictionary.Add("GRADE", dataByKey.szGradeDesc);
                }
            }
            if (!string.IsNullOrEmpty(this.m_strHostPlayerName))
            {
                int index = this.m_strHostPlayerName.IndexOf('(');
                string str = this.m_strHostPlayerName.Substring(index + 1, (this.m_strHostPlayerName.Length - index) - 2);
                dictionary.Add("HERO", str);
            }
            dictionary.Add("KILLNUM", this.m_iHostPlaterKillNum.ToString());
            dictionary.Add("DEADNUM", this.m_iHostPlaterDeadNum.ToString());
            dictionary.Add("ASSISTNUM", this.m_iHostPlaterAssistNum.ToString());
            int num2 = this.ConvertMaxMultiKillPriorityToResDef();
            if (num2 > 2)
            {
                ResMultiKill kill = GameDataMgr.multiKillDatabin.GetDataByKey((long) num2);
                if (kill != null)
                {
                    dictionary.Add("MULTIKILL", kill.szAchievementName);
                }
            }
            return dictionary;
        }

        public bool GetRecorderGlobalCfgEnableFlag()
        {
            bool flag = false;
            if ((GameDataMgr.svr2CltCfgDict != null) && GameDataMgr.svr2CltCfgDict.ContainsKey(12))
            {
                ResGlobalInfo info = new ResGlobalInfo();
                if (GameDataMgr.svr2CltCfgDict.TryGetValue(12, out info))
                {
                    flag = info.dwConfValue > 0;
                }
            }
            return flag;
        }

        private Vector3 GetRecorderPosition()
        {
            Vector3 vector = Vector3.get_zero();
            if (this.m_RecorderPanel != null)
            {
                Transform transform = this.m_RecorderPanel.FindChild("Record");
                if (transform == null)
                {
                    return vector;
                }
                Camera camera = Camera.get_current();
                if (camera == null)
                {
                    camera = Camera.get_allCameras()[0];
                }
                if (camera != null)
                {
                    vector = camera.WorldToViewportPoint(transform.get_transform().get_position());
                }
            }
            return vector;
        }

        private string GetVideoName()
        {
            string text = Singleton<CTextManager>.GetInstance().GetText("RecordMomentVideoNameHeader");
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if (curLvelContext != null)
            {
                text = text + curLvelContext.m_levelName;
                if (curLvelContext.IsGameTypeLadder())
                {
                    text = text + curLvelContext.m_gameMatchName;
                }
            }
            if (this.m_bIsMvp)
            {
                text = text + "MVP";
            }
            if (!string.IsNullOrEmpty(this.m_strHostPlayerName))
            {
                int index = this.m_strHostPlayerName.IndexOf('(');
                string str2 = this.m_strHostPlayerName.Substring(index + 1, (this.m_strHostPlayerName.Length - index) - 2);
                text = text + str2;
            }
            int num2 = this.ConvertMaxMultiKillPriorityToResDef();
            if (num2 > 2)
            {
                ResMultiKill dataByKey = GameDataMgr.multiKillDatabin.GetDataByKey((long) num2);
                if (dataByKey != null)
                {
                    text = text + dataByKey.szAchievementName;
                }
            }
            return (text + Singleton<CTextManager>.GetInstance().GetText("RecordMomentVideoNameTail"));
        }

        public override void Init()
        {
            base.Init();
            this.Reset();
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightPrepare, new RefAction<DefaultGameEventParam>(this.OnFightPrepare));
            Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_DoubleKill, new RefAction<DefaultGameEventParam>(this.OnActorDoubleKill));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_TripleKill, new RefAction<DefaultGameEventParam>(this.OnActorTripleKill));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_QuataryKill, new RefAction<DefaultGameEventParam>(this.OnActorQuataryKill));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_PentaKill, new RefAction<DefaultGameEventParam>(this.OnActorPentaKill));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Record_Save_Moment_Video, new CUIEventManager.OnUIEventHandler(this.OnSaveMomentVideo));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Record_Save_Moment_Video_Cancel, new CUIEventManager.OnUIEventHandler(this.OnSaveMomentVideoCancel));
            Singleton<EventRouter>.GetInstance().AddEventHandler<bool>(EventID.GAMEJOY_STARTRECORDING_RESULT, new Action<bool>(this.OnGameJoyStartRecordResult));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.OB_Video_Btn_VideoMgr_Click, new CUIEventManager.OnUIEventHandler(this.OnBtnVideoMgrClick));
            Singleton<EventRouter>.GetInstance().AddEventHandler<bool>(EventID.GAMEJOY_SDK_PERMISSION_CHECK_RESULT, new Action<bool>(this.OnGameJoyCheckPermissionResult));
            Singleton<EventRouter>.GetInstance().AddEventHandler<bool>(EventID.GAMEJOY_AVAILABILITY_CHECK_RESULT, new Action<bool>(this.OnGameJoyCheckAvailabilityResult));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Record_Check_WhiteList_TimeUp, new CUIEventManager.OnUIEventHandler(this.OnCheckWhiteListTimeUp));
            Singleton<EventRouter>.GetInstance().AddEventHandler<long>(EventID.GAMEJOY_STOPRECORDING_RESULT, new Action<long>(this.OnGameJoyStopRecordResult));
            if (GameSettings.EnableKingTimeMode)
            {
                this.m_enmCheckWhiteListStatus = CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_AUTOCHECK;
                this.CheckWhiteList();
            }
        }

        private void InsertAssistInfo(ref SortedList<int, SortedList<long, long>> assistInfo, int iAssistCount, long lStartTime, long lEndTime)
        {
            if (assistInfo == null)
            {
                assistInfo = new SortedList<int, SortedList<long, long>>();
            }
            SortedList<long, long> list = null;
            if (!assistInfo.TryGetValue(iAssistCount, out list))
            {
                list = new SortedList<long, long>();
                assistInfo.Add(iAssistCount, list);
            }
            if ((list != null) && !list.ContainsKey(lStartTime))
            {
                list.Add(lStartTime, lEndTime);
            }
        }

        private void OnActorDead(ref GameDeadEventParam prm)
        {
            if ((this.m_bIsRecordMomentsEnable && this.m_bIsStartRecordOk) && ((prm.src != 0) && (prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)))
            {
                this.UpdateRecordEvent(prm.atker, RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_ONCEKILL);
                if (((Singleton<GamePlayerCenter>.instance != null) && (Singleton<GamePlayerCenter>.instance.GetHostPlayer() != null)) && (prm.atker != Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain))
                {
                    if ((prm.src != 0) && (prm.src.handle.ActorControl != null))
                    {
                        List<KeyValuePair<uint, ulong>>.Enumerator enumerator = prm.src.handle.ActorControl.hurtSelfActorList.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            KeyValuePair<uint, ulong> current = enumerator.Current;
                            if (current.Key == Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain.handle.ObjID)
                            {
                                this.UpdateRecordEvent(Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain, RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_ASSIST);
                                return;
                            }
                        }
                    }
                    if ((prm.atker != 0) && (prm.atker.handle.ActorControl != null))
                    {
                        List<KeyValuePair<uint, ulong>>.Enumerator enumerator2 = prm.atker.handle.ActorControl.helpSelfActorList.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            KeyValuePair<uint, ulong> pair2 = enumerator2.Current;
                            if (pair2.Key == Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain.handle.ObjID)
                            {
                                this.UpdateRecordEvent(Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain, RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_ASSIST);
                                return;
                            }
                        }
                    }
                }
                else if (((Singleton<GamePlayerCenter>.instance != null) && (Singleton<GamePlayerCenter>.instance.GetHostPlayer() != null)) && (prm.atker == Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain))
                {
                    HeroWrapper actorControl = prm.orignalAtker.handle.ActorControl as HeroWrapper;
                    if ((actorControl != null) && (actorControl.ContiKillNum > this.m_iContinuKillMaxNum))
                    {
                        this.m_iContinuKillMaxNum = actorControl.ContiKillNum;
                    }
                }
            }
        }

        private void OnActorDoubleKill(ref DefaultGameEventParam prm)
        {
            this.UpdateRecordEvent(prm.atker, RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_DOUBLEKILL);
        }

        private void OnActorPentaKill(ref DefaultGameEventParam prm)
        {
            this.UpdateRecordEvent(prm.atker, RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_PENTAKILL);
        }

        private void OnActorQuataryKill(ref DefaultGameEventParam prm)
        {
            this.UpdateRecordEvent(prm.atker, RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_QUATARYKILL);
        }

        private void OnActorTripleKill(ref DefaultGameEventParam prm)
        {
            this.UpdateRecordEvent(prm.atker, RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_TRIPLEKILL);
        }

        public void OnBadGameEnd()
        {
            if (this.m_bIsStartRecordOk && this.m_bIsRecordMomentsEnable)
            {
                CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(SettlementSystem.SettlementFormName);
                if ((form == null) || !form.get_gameObject().get_activeSelf())
                {
                    this.StopMomentsRecording();
                    this.CallGameJoyGenerateWithNothing();
                }
            }
        }

        private void OnBtnVideoMgrClick(CUIEvent cuiEvent)
        {
            this.m_enmCheckWhiteListStatus = CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_VIDEOMGRCLICK;
            Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(null, 3, enUIEventID.Record_Check_WhiteList_TimeUp);
            this.CheckWhiteList();
        }

        private void OnCheckWhiteListTimeUp(CUIEvent uiEvent)
        {
            if (this.m_enmCheckWhiteListStatus == CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_INVALID)
            {
                this.SetKingBarSliderState(false);
            }
            this.m_enmCheckWhiteListStatus = CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_TIMEUP;
        }

        private void OnFightPrepare(ref DefaultGameEventParam prm)
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
            if (curLvelContext == null)
            {
                this.m_bIsRecordMomentsEnable = false;
            }
            else
            {
                this.m_bIsRecordMomentsEnable = ((GameSettings.EnableKingTimeMode && this.GetRecorderGlobalCfgEnableFlag()) && !Singleton<WatchController>.GetInstance().IsWatching) && curLvelContext.IsMobaModeWithOutGuide();
                if (this.m_bIsRecordMomentsEnable)
                {
                    if (Singleton<LobbyLogic>.instance.reconnGameInfo != null)
                    {
                        this.m_bIsRecordMomentsEnable = false;
                        Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("RecordMomentSuspendRecord"), false, 1.5f, null, new object[0]);
                    }
                    else
                    {
                        this.Reset();
                        this.m_hostActor = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain;
                        Singleton<GameJoy>.instance.StartMomentsRecording();
                    }
                }
            }
        }

        private void OnGameJoyCheckAvailabilityResult(bool bRes)
        {
            if ((this.m_enmCheckWhiteListStatus != CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_TIMEUP) && (this.m_enmCheckWhiteListStatus != CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_AUTOCHECK))
            {
                Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
                if (!bRes)
                {
                    Singleton<CUIManager>.instance.OpenTips("GamejoyCheckAvailabilityFailed", true, 1.5f, null, new object[0]);
                    this.SetKingBarSliderState(false);
                }
                if (this.m_enmCheckWhiteListStatus == CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_VIDEOMGRCLICK)
                {
                    if (bRes)
                    {
                        Singleton<GameJoy>.instance.ShowVideoListDialog();
                    }
                    this.m_enmCheckWhiteListStatus = CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_INVALID;
                }
                else
                {
                    this.m_enmCheckWhiteListStatus = !bRes ? CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_RESULTFAILED : CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_RESULTOK;
                    if (this.m_enmCheckWhiteListStatus == CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_RESULTOK)
                    {
                        this.CheckStorageAndPermission();
                    }
                }
            }
            else if (this.m_enmCheckWhiteListStatus == CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_AUTOCHECK)
            {
                this.m_enmCheckWhiteListStatus = !bRes ? CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_RESULTFAILED : CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_RESULTOK;
            }
        }

        private void OnGameJoyCheckPermissionResult(bool bRes)
        {
            this.m_enmCheckPermissionRes = !bRes ? CHECK_PERMISSION_STUTAS.CHECK_PERMISSION_STUTAS_TYPE_NOPERMISSION : CHECK_PERMISSION_STUTAS.CHECK_PERMISSION_STUTAS_TYPE_PERMISSIONOK;
            if ((this.m_objKingBar != null) && !bRes)
            {
                this.SetKingBarSliderState(false);
                Singleton<CUIManager>.instance.OpenTips("GameJoyCheckPermissionFailed", true, 1.5f, null, new object[0]);
            }
        }

        private void OnGameJoyStartRecordResult(bool bRes)
        {
            this.m_bIsStartRecordOk = bRes;
            if (bRes)
            {
                this.m_lGameStartTime = GameJoy.getSystemCurrentTimeMillis;
            }
        }

        private void OnGameJoyStopRecordResult(long lDuration)
        {
            this.m_lVideoTimeLen = lDuration;
        }

        private void OnSaveMomentVideo(CUIEvent uiEvent)
        {
            Vector3 recorderPosition = this.GetRecorderPosition();
            Singleton<GameJoy>.instance.SetDefaultUploadShareDialogPosition(recorderPosition.x, recorderPosition.y);
            this.CloseRecorderPanel();
            this.ChooseTopEvent();
        }

        private void OnSaveMomentVideoCancel(CUIEvent uiEvent)
        {
            this.CloseRecorderPanel();
            this.CallGameJoyGenerateWithNothing();
        }

        public void OpenMsgBoxForMomentRecorder(Transform container)
        {
            if ((this.m_bIsRecordMomentsEnable && (container != null)) && ((this.m_stRecordInfo != null) && (this.m_stRecordInfo.Count != 0)))
            {
                if (this.m_lVideoTimeLen <= 0L)
                {
                    Singleton<CUIManager>.instance.OpenTips("RecordMoment_EndGame_Tips_NoRecorderExist", true, 1.5f, null, new object[0]);
                }
                else
                {
                    this.m_RecorderPanel = container;
                    if (GameSettings.EnableKingTimeMode && this.m_bIsStartRecordOk)
                    {
                        Transform transform = container.FindChild("Extra/Image/Image/Text");
                        if ((transform != null) && (transform.get_gameObject() != null))
                        {
                            Text component = transform.get_gameObject().GetComponent<Text>();
                            if (component != null)
                            {
                                component.set_text(Singleton<CTextManager>.GetInstance().GetText("RecordSaveMomentVideo"));
                            }
                        }
                        container.get_gameObject().CustomSetActive(true);
                    }
                }
            }
        }

        public bool OpenRecorderCheck(GameObject KingBar)
        {
            this.m_objKingBar = KingBar;
            this.m_enmCheckWhiteListStatus = CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_INVALID;
            Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(null, 3, enUIEventID.Record_Check_WhiteList_TimeUp);
            this.CheckWhiteList();
            return false;
        }

        private void Reset()
        {
            this.m_enLastEventPriority = RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_INVALID;
            this.m_lLastEventStartTime = 0L;
            this.m_lLastEventEndTime = 0L;
            this.m_lGameStartTime = 0L;
            this.m_lGameEndTime = 0L;
            this.m_iContinuKillMaxNum = -1;
            this.m_bIsStartRecordOk = false;
            this.m_bIsMvp = false;
            this.m_iHostPlaterKillNum = 0;
            this.m_iHostPlaterDeadNum = 0;
            this.m_iHostPlaterAssistNum = 0;
            this.m_bIsCallGameJoyGenerate = false;
            this.m_bIsCallStopGameJoyRecord = false;
            this.m_lVideoTimeLen = 0L;
            if (this.m_hostActor != 0)
            {
                this.m_hostActor.Release();
            }
            if (this.m_stRecordInfo != null)
            {
                this.m_stRecordInfo.Clear();
            }
            this.m_uiEventStartTimeInterval = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_RECORDER_KINGTIME_EVENTSTARTTIMEINTERVAL) * 0x3e8;
            this.m_uiEventEndTimeInterval = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_RECORDER_KINGTIME_EVENTENDTIMEINTERVAL) * 0x3e8;
            this.m_uiEventNumMax = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_RECORDER_KINGTIME_EVENTNUMMAX);
            this.m_ui543KillEventTotalTime = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_RECORDER_KINGTIME_VIDEO543KILLTOTALTIME) * 0x3e8;
            this.m_ui210KillEventTotalTime = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_RECORDER_KINGTIME_VIDEO210KILLTOTALTIME) * 0x3e8;
            this.m_uiMinSpaceLimit = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_RECORDER_KINGTIME_ANDROIDMINSPACELIMIT);
            this.m_uiOnceDoubleEventTimeIntervalReduce = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_RECORDER_KINGTIME_ONCEDOUBLEEVENTTIMEINTERVAL) * 0x3e8;
        }

        public void SetKingBarSliderState(bool bIsOpen)
        {
            if (this.m_objKingBar != null)
            {
                Transform transform = this.m_objKingBar.get_transform().FindChild("Slider");
                if (transform != null)
                {
                    CUISliderEventScript component = transform.GetComponent<CUISliderEventScript>();
                    int num = !bIsOpen ? 0 : 1;
                    if (((int) component.value) != num)
                    {
                        component.value = num;
                    }
                }
                else if (GameSettings.EnableKingTimeMode != bIsOpen)
                {
                    GameSettings.EnableKingTimeMode = bIsOpen;
                }
            }
            else if (GameSettings.EnableKingTimeMode != bIsOpen)
            {
                GameSettings.EnableKingTimeMode = bIsOpen;
            }
        }

        public void StopMomentsRecording()
        {
            if ((this.m_bIsRecordMomentsEnable && this.m_bIsStartRecordOk) && !this.m_bIsCallStopGameJoyRecord)
            {
                this.m_bIsCallStopGameJoyRecord = true;
                Singleton<GameJoy>.instance.EndMomentsRecording();
            }
        }

        public override void UnInit()
        {
            base.UnInit();
            if (this.m_RecorderPanel != null)
            {
                this.m_stRecordInfo.Clear();
            }
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightPrepare, new RefAction<DefaultGameEventParam>(this.OnFightPrepare));
            Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_DoubleKill, new RefAction<DefaultGameEventParam>(this.OnActorDoubleKill));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_TripleKill, new RefAction<DefaultGameEventParam>(this.OnActorTripleKill));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_QuataryKill, new RefAction<DefaultGameEventParam>(this.OnActorQuataryKill));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_PentaKill, new RefAction<DefaultGameEventParam>(this.OnActorPentaKill));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Record_Save_Moment_Video, new CUIEventManager.OnUIEventHandler(this.OnSaveMomentVideo));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Record_Save_Moment_Video_Cancel, new CUIEventManager.OnUIEventHandler(this.OnSaveMomentVideoCancel));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<bool>(EventID.GAMEJOY_STARTRECORDING_RESULT, new Action<bool>(this.OnGameJoyStartRecordResult));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.OB_Video_Btn_VideoMgr_Click, new CUIEventManager.OnUIEventHandler(this.OnBtnVideoMgrClick));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<bool>(EventID.GAMEJOY_SDK_PERMISSION_CHECK_RESULT, new Action<bool>(this.OnGameJoyCheckPermissionResult));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<bool>(EventID.GAMEJOY_AVAILABILITY_CHECK_RESULT, new Action<bool>(this.OnGameJoyCheckAvailabilityResult));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Record_Check_WhiteList_TimeUp, new CUIEventManager.OnUIEventHandler(this.OnCheckWhiteListTimeUp));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<long>(EventID.GAMEJOY_STOPRECORDING_RESULT, new Action<long>(this.OnGameJoyStopRecordResult));
        }

        private void UpdateRecordEvent(PoolObjHandle<ActorRoot> eventActor, RECORD_EVENT_PRIORITY eventPriority)
        {
            if ((this.m_bIsRecordMomentsEnable && this.m_bIsStartRecordOk) && ((eventPriority == RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_INVALID) || (Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain == eventActor)))
            {
                long num = GameJoy.getSystemCurrentTimeMillis - this.m_lGameStartTime;
                bool flag = false;
                if ((eventActor != 0) && (eventActor.handle.ActorControl != null))
                {
                    HeroWrapper actorControl = eventActor.handle.ActorControl as HeroWrapper;
                    if (actorControl != null)
                    {
                        flag = actorControl.IsInMultiKill();
                    }
                }
                if (!flag || (eventPriority == RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_INVALID))
                {
                    if (this.m_enLastEventPriority > RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_INVALID)
                    {
                        this.AddRecordEvent(this.m_enLastEventPriority, this.m_lLastEventStartTime, this.m_lLastEventEndTime);
                    }
                    this.m_enLastEventPriority = eventPriority;
                    this.m_lLastEventStartTime = num;
                    this.m_lLastEventEndTime = num;
                }
                else if ((this.m_enLastEventPriority == RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_INVALID) || (eventPriority > this.m_enLastEventPriority))
                {
                    if (this.m_enLastEventPriority <= RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_ASSIST)
                    {
                        this.m_lLastEventStartTime = num;
                    }
                    this.m_enLastEventPriority = eventPriority;
                    this.m_lLastEventEndTime = num;
                }
            }
        }

        public enum CHECK_PERMISSION_STUTAS
        {
            CHECK_PERMISSION_STUTAS_TYPE_NOPERMISSION = 0,
            CHECK_PERMISSION_STUTAS_TYPE_PERMISSIONOK = 1,
            CHECK_PERMISSION_STUTAS_TYPE_WITHOUTRESULT = -1
        }

        public enum CHECK_WHITELIST_STATUS
        {
            CHECK_WHITELIST_STATUS_TYPE_INVALID,
            CHECK_WHITELIST_STATUS_TYPE_AUTOCHECK,
            CHECK_WHITELIST_STATUS_TYPE_TIMEUP,
            CHECK_WHITELIST_STATUS_TYPE_RESULTOK,
            CHECK_WHITELIST_STATUS_TYPE_RESULTFAILED,
            CHECK_WHITELIST_STATUS_TYPE_VIDEOMGRCLICK
        }

        public enum RECORD_EVENT_PRIORITY
        {
            RECORD_EVENT_TYPE_INVALID,
            RECORD_EVENT_TYPE_ASSIST,
            RECORD_EVENT_TYPE_ONCEKILL,
            RECORD_EVENT_TYPE_DOUBLEKILL,
            RECORD_EVENT_TYPE_TRIPLEKILL,
            RECORD_EVENT_TYPE_QUATARYKILL,
            RECORD_EVENT_TYPE_PENTAKILL
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECORD_INFO
        {
            public CRecordUseSDK.RECORD_EVENT_PRIORITY eventPriority;
            public long lEndTime;
            public RECORD_INFO(CRecordUseSDK.RECORD_EVENT_PRIORITY _eventPriority, long _lEndTime)
            {
                this.eventPriority = _eventPriority;
                this.lEndTime = _lEndTime;
            }
        }
    }
}

