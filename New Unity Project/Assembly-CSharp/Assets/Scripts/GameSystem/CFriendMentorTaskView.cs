namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class CFriendMentorTaskView
    {
        public List<TaskViewConditionData> taskViewConditionDataList = new List<TaskViewConditionData>();

        public List<TaskViewConditionData> CalcParam(CTask task, GameObject goto_obj, bool isMonthWeekCard = false)
        {
            if (task == null)
            {
                return null;
            }
            for (int i = 0; i < this.taskViewConditionDataList.Count; i++)
            {
                this.taskViewConditionDataList[i].Clear();
            }
            this.taskViewConditionDataList.Clear();
            if (isMonthWeekCard)
            {
                goto_obj.CustomSetActive(true);
                TaskViewConditionData item = new TaskViewConditionData();
                item.bValid = true;
                item.m_onClickEventID = enUIEventID.Mall_Open_Factory_Shop_Tab;
                this.taskViewConditionDataList.Add(item);
                item.bShowGoToBtn = true;
            }
            else
            {
                for (int j = 0; j < task.m_prerequisiteInfo.Length; j++)
                {
                    TaskViewConditionData data2 = new TaskViewConditionData();
                    data2.bValid = false;
                    data2.bFinish = task.m_prerequisiteInfo[j].m_isReach;
                    ResDT_PrerequisiteInTask task2 = task.m_resTask.astPrerequisiteArray[j];
                    data2.condition = task2.szPrerequisiteDesc;
                    RES_PERREQUISITE_TYPE conditionType = (RES_PERREQUISITE_TYPE) task.m_prerequisiteInfo[j].m_conditionType;
                    if (task.m_prerequisiteInfo[j].m_valueTarget > 0)
                    {
                        data2.bValid = true;
                        if ((conditionType != RES_PERREQUISITE_TYPE.RES_PERREQUISITE_ACNTLVL) && (conditionType != RES_PERREQUISITE_TYPE.RES_PERREQUISITE_PVPLVL))
                        {
                            object[] objArray1 = new object[] { task.m_prerequisiteInfo[j].m_value, "/", task.m_prerequisiteInfo[j].m_valueTarget, " " };
                            data2.progress = string.Concat(objArray1);
                        }
                        if (!task.m_prerequisiteInfo[j].m_isReach)
                        {
                            switch (conditionType)
                            {
                                case RES_PERREQUISITE_TYPE.RES_PERREQUISITE_STAGECLEARPVE:
                                {
                                    int iParam = task.m_resTask.astPrerequisiteArray[j].astPrerequisiteParam[3].iParam;
                                    if (iParam == Mathf.Pow(2f, 0f))
                                    {
                                        goto_obj.CustomSetActive(true);
                                        data2.bShowGoToBtn = true;
                                        data2.taskId = task.m_baseId;
                                        data2.tag = task.m_resTask.astPrerequisiteArray[j].astPrerequisiteParam[4].iParam;
                                        data2.m_onClickEventID = enUIEventID.Task_LinkPve;
                                    }
                                    else if (iParam == Mathf.Pow(2f, 7f))
                                    {
                                        goto_obj.CustomSetActive(true);
                                        data2.bShowGoToBtn = true;
                                        data2.m_onClickEventID = enUIEventID.Burn_OpenForm;
                                    }
                                    else if (iParam == Mathf.Pow(2f, 8f))
                                    {
                                        goto_obj.CustomSetActive(true);
                                        data2.bShowGoToBtn = true;
                                        data2.m_onClickEventID = enUIEventID.Arena_OpenForm;
                                    }
                                    goto Label_04D7;
                                }
                                case RES_PERREQUISITE_TYPE.RES_PERREQUISITE_STAGECLEARPVP:
                                case RES_PERREQUISITE_TYPE.RES_PERREQUISITE_PVPKILLCNT:
                                    goto_obj.CustomSetActive(true);
                                    data2.bShowGoToBtn = true;
                                    data2.m_onClickEventID = enUIEventID.Matching_OpenEntry;
                                    data2.tag = 0;
                                    goto Label_04D7;

                                case RES_PERREQUISITE_TYPE.RES_PERREQUISITE_FRIENDCNT:
                                    goto_obj.CustomSetActive(false);
                                    data2.bShowGoToBtn = false;
                                    data2.m_onClickEventID = enUIEventID.Friend_OpenForm;
                                    goto Label_04D7;

                                case RES_PERREQUISITE_TYPE.RES_PERREQUISITE_PVPLVL:
                                    goto_obj.CustomSetActive(true);
                                    data2.bShowGoToBtn = true;
                                    data2.m_onClickEventID = enUIEventID.Matching_OpenEntry;
                                    data2.tag = 0;
                                    goto Label_04D7;

                                case RES_PERREQUISITE_TYPE.RES_PERREQUISITE_GUILDOPT:
                                    if (task.m_resTask.astPrerequisiteArray[0].astPrerequisiteParam[1].iParam == 2L)
                                    {
                                        goto_obj.CustomSetActive(true);
                                        data2.bShowGoToBtn = true;
                                        data2.m_onClickEventID = enUIEventID.Matching_OpenEntry;
                                        data2.tag = 3;
                                    }
                                    else
                                    {
                                        goto_obj.CustomSetActive(false);
                                        data2.bShowGoToBtn = false;
                                    }
                                    goto Label_04D7;

                                case RES_PERREQUISITE_TYPE.RES_PERREQUISITE_ARENAOPT:
                                    goto_obj.CustomSetActive(true);
                                    data2.bShowGoToBtn = true;
                                    data2.m_onClickEventID = enUIEventID.Arena_OpenForm;
                                    goto Label_04D7;

                                case RES_PERREQUISITE_TYPE.RES_PERREQUISITE_SYMBOLCOMP:
                                    goto_obj.CustomSetActive(true);
                                    data2.bShowGoToBtn = true;
                                    data2.m_onClickEventID = enUIEventID.Symbol_OpenForm;
                                    goto Label_04D7;

                                case RES_PERREQUISITE_TYPE.RES_PERREQUISITE_BUYOPT:
                                    goto_obj.CustomSetActive(true);
                                    data2.bShowGoToBtn = true;
                                    data2.m_onClickEventID = enUIEventID.Mall_Open_Factory_Shop_Tab;
                                    goto Label_04D7;

                                case RES_PERREQUISITE_TYPE.RES_PERREQUISITE_OPENBOXCNT:
                                    goto_obj.CustomSetActive(true);
                                    data2.bShowGoToBtn = true;
                                    data2.m_onClickEventID = enUIEventID.Lottery_Open_Form;
                                    goto Label_04D7;

                                case RES_PERREQUISITE_TYPE.RES_PERREQUISITE_DUOBAO:
                                    goto_obj.CustomSetActive(true);
                                    data2.bShowGoToBtn = true;
                                    if (task.m_resTask.astPrerequisiteArray[0].astPrerequisiteParam[1].iParam == 1L)
                                    {
                                        data2.m_onClickEventID = enUIEventID.Mall_GotoDianmondTreasureTab;
                                    }
                                    else if (task.m_resTask.astPrerequisiteArray[0].astPrerequisiteParam[1].iParam == 2L)
                                    {
                                        data2.m_onClickEventID = enUIEventID.Mall_GotoCouponsTreasureTab;
                                    }
                                    goto Label_04D7;
                            }
                            if (conditionType == RES_PERREQUISITE_TYPE.RES_PERREQUISITE_RECALLFRIEND)
                            {
                                goto_obj.CustomSetActive(true);
                                data2.bShowGoToBtn = true;
                                data2.m_onClickEventID = enUIEventID.Friend_OpenForm;
                            }
                            else
                            {
                                goto_obj.CustomSetActive(false);
                                data2.bShowGoToBtn = false;
                            }
                        }
                        else
                        {
                            goto_obj.CustomSetActive(false);
                            data2.bShowGoToBtn = false;
                        }
                    }
                Label_04D7:
                    this.taskViewConditionDataList.Add(data2);
                }
            }
            return this.taskViewConditionDataList;
        }

        public void Clear()
        {
            Singleton<EventRouter>.instance.RemoveEventHandler("TaskUpdated", new Action(this, (IntPtr) this.OnMentorTaskUpdate));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Task_Mentor_GetReward, new CUIEventManager.OnUIEventHandler(this.On_Task_Submit));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Task_Mentor_Close, new CUIEventManager.OnUIEventHandler(this.On_Task_Mentor_Close));
            if (Singleton<CFriendContoller>.instance.view != null)
            {
                Singleton<CFriendContoller>.instance.view.Refresh_Tab();
            }
        }

        private void On_Task_Mentor_Close(CUIEvent uievent)
        {
            this.Clear();
        }

        private void On_Task_Submit(CUIEvent uiEvent)
        {
            uint tagUInt = uiEvent.m_eventParams.tagUInt;
            DebugHelper.Assert(tagUInt > 0, "---ctask Submit task, taskid should > 0");
            if (tagUInt > 0)
            {
                TaskNetUT.Send_SubmitTask(tagUInt);
            }
        }

        private void OnMentorTaskUpdate()
        {
            this.ShowMenu();
        }

        public void Open()
        {
            Singleton<EventRouter>.instance.AddEventHandler("TaskUpdated", new Action(this, (IntPtr) this.OnMentorTaskUpdate));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Task_Mentor_GetReward, new CUIEventManager.OnUIEventHandler(this.On_Task_Submit));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Task_Mentor_Close, new CUIEventManager.OnUIEventHandler(this.On_Task_Mentor_Close));
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(CFriendContoller.MentorTaskFormPath, false, true);
            this.ShowMenu();
        }

        public void ShowMenu()
        {
            EMentorTaskState mentorTaskState = Singleton<CTaskSys>.instance.MentorTaskState;
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CFriendContoller.MentorTaskFormPath);
            if (form != null)
            {
                GameObject obj2 = form.get_transform().FindChild("content/title").get_gameObject();
                GameObject obj3 = form.get_transform().FindChild("content/top").get_gameObject();
                GameObject obj4 = form.get_transform().FindChild("content/middle").get_gameObject();
                GameObject obj5 = form.get_transform().FindChild("content/bottom").get_gameObject();
                GameObject obj6 = form.get_transform().FindChild("content/info").get_gameObject();
                switch (mentorTaskState)
                {
                    case EMentorTaskState.Empty:
                        obj2.CustomSetActive(false);
                        obj3.CustomSetActive(false);
                        obj4.CustomSetActive(false);
                        obj5.CustomSetActive(false);
                        obj6.CustomSetActive(true);
                        obj6.get_transform().FindChild("txt").GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText("MTV_Empty"));
                        break;

                    case EMentorTaskState.Tudi_Task:
                        obj2.CustomSetActive(true);
                        obj3.CustomSetActive(true);
                        obj4.CustomSetActive(true);
                        obj5.CustomSetActive(true);
                        obj6.CustomSetActive(false);
                        this.showTask(Singleton<CTaskSys>.instance.apprenticeTaskID);
                        break;

                    case EMentorTaskState.TudiTaskFinish_No_MasterTask:
                        obj2.CustomSetActive(false);
                        obj3.CustomSetActive(false);
                        obj4.CustomSetActive(false);
                        obj5.CustomSetActive(false);
                        obj6.CustomSetActive(true);
                        obj6.get_transform().FindChild("txt").GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText("MTV_MiddleEmpty"));
                        break;

                    case EMentorTaskState.MasterTask:
                        obj2.CustomSetActive(true);
                        obj3.CustomSetActive(true);
                        obj4.CustomSetActive(true);
                        obj5.CustomSetActive(true);
                        obj6.CustomSetActive(false);
                        this.showTask(Singleton<CTaskSys>.instance.masterTaskID);
                        break;

                    case EMentorTaskState.AllFinish:
                        obj2.CustomSetActive(false);
                        obj3.CustomSetActive(false);
                        obj4.CustomSetActive(false);
                        obj5.CustomSetActive(false);
                        obj6.CustomSetActive(true);
                        obj6.get_transform().FindChild("txt").GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText("MTV_AllFinish"));
                        break;
                }
            }
        }

        public void showTask(uint taskid)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CFriendContoller.MentorTaskFormPath);
            if (form != null)
            {
                CTask task = Singleton<CTaskSys>.instance.model.GetTask(taskid);
                if (task != null)
                {
                    Utility.GetComponetInChild<Text>(form.get_gameObject(), "content/title/taskName").set_text(task.m_taskTitle);
                    form.GetWidget(1).GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText("MTV_TopNodeHeaderTxt"));
                    form.GetWidget(2).GetComponent<Text>().set_text(task.m_taskDesc);
                    form.GetWidget(4).GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText("MTV_MiddleNodeHeaderTxt"));
                    List<TaskViewConditionData> list = this.CalcParam(task, null, false);
                    int num = Math.Min(3, list.Count);
                    for (int i = 0; i < num; i++)
                    {
                        TaskViewConditionData data = list[i];
                        GameObject obj3 = form.GetWidget(6).get_transform().FindChild(string.Format("cond_{0}", i)).get_gameObject();
                        if (obj3 != null)
                        {
                            if (data.bValid)
                            {
                                obj3.CustomSetActive(true);
                                obj3.get_transform().FindChild("desc").GetComponent<Text>().set_text(data.condition);
                                obj3.get_transform().FindChild("progress").GetComponent<Text>().set_text(data.progress);
                                CUIEventScript component = obj3.get_transform().FindChild("btns/goto_btn").GetComponent<CUIEventScript>();
                                if (component != null)
                                {
                                    component.m_onClickEventID = data.m_onClickEventID;
                                    component.m_onClickEventParams.tag = data.tag;
                                    component.m_onClickEventParams.taskId = data.taskId;
                                }
                                if (data.bFinish)
                                {
                                    component.get_gameObject().CustomSetActive(false);
                                }
                                else
                                {
                                    component.get_gameObject().CustomSetActive(data.bShowGoToBtn);
                                }
                                obj3.get_transform().FindChild("btns/Text").get_gameObject().CustomSetActive(data.bFinish);
                            }
                            else
                            {
                                obj3.CustomSetActive(false);
                            }
                        }
                    }
                    CTaskView.CTaskUT.ShowTaskAward(form, task, form.GetWidget(11), 4);
                    if (task.m_taskState == 1)
                    {
                        GameObject obj4 = form.GetWidget(9).get_gameObject();
                        obj4.CustomSetActive(true);
                        obj4.GetComponent<CUIEventScript>().m_onClickEventParams.tagUInt = task.m_baseId;
                        form.GetWidget(10).get_gameObject().CustomSetActive(false);
                    }
                    else
                    {
                        form.GetWidget(9).get_gameObject().CustomSetActive(false);
                        form.GetWidget(10).get_gameObject().CustomSetActive(false);
                    }
                }
            }
        }

        public void ShowTask(uint taskID)
        {
        }

        public enum EMentorTaskState
        {
            None,
            Empty,
            Tudi_Task,
            TudiTaskFinish_No_MasterTask,
            MasterTask,
            AllFinish
        }

        private enum enMentorTaskViewWidget
        {
            BottomGetRewardBtn = 9,
            BottomNode = 7,
            BottomNodeHeaderTxt = 8,
            BottomRewardContainer = 11,
            BottomUnFinishInfo = 10,
            MiddleDoingNode = 6,
            MiddleNode = 3,
            MiddleNodeHeaderTxt = 4,
            MiddleUnlockNode = 5,
            None = -1,
            TopNode = 0,
            TopNodeDescTxt = 2,
            TopNodeHeaderTxt = 1
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TaskViewConditionData
        {
            public bool bValid;
            public bool bFinish;
            public string condition;
            public string progress;
            public uint taskId;
            public int tag;
            public enUIEventID m_onClickEventID;
            public bool bShowGoToBtn;
            public void Clear()
            {
                this.bValid = false;
            }
        }
    }
}

