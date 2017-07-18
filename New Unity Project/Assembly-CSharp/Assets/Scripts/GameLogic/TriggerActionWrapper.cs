namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Common;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class TriggerActionWrapper
    {
        [FriendlyName("起效时间")]
        public int ActiveTime;
        [FriendlyName("作用于事件肇事者")]
        public bool bAtker;
        [FriendlyName("打开")]
        public bool bEnable;
        [FriendlyName("作用于事件受害者")]
        public bool bSrc;
        [FriendlyName("离开时拔除")]
        public bool bStopWhenLeaving;
        [FriendlyName("进入时配置ID")]
        public int EnterUniqueId;
        [FriendlyName("离开时配置ID")]
        public int LeaveUniqueId;
        private TriggerActionBase m_internalAct;
        private int m_triggerId;
        [FriendlyName("偏移距离_x")]
        public int Offset_x;
        [FriendlyName("偏移距离_y")]
        public int Offset_y;
        public GameObject[] RefObjList;
        [SerializeField]
        public AreaEventTrigger.STimingAction[] TimingActionsInter;
        [FriendlyName("持续时间")]
        public int TotalTime;
        public EGlobalTriggerAct TriggerType;
        [FriendlyName("轮询探测时配置ID")]
        public int UpdateUniqueId;

        public TriggerActionWrapper()
        {
            this.RefObjList = new GameObject[0];
            this.TimingActionsInter = new AreaEventTrigger.STimingAction[0];
            this.bEnable = true;
            this.bEnable = true;
        }

        public TriggerActionWrapper(EGlobalTriggerAct inTriggerType)
        {
            this.RefObjList = new GameObject[0];
            this.TimingActionsInter = new AreaEventTrigger.STimingAction[0];
            this.bEnable = true;
            this.TriggerType = inTriggerType;
            this.bEnable = true;
        }

        public void Destroy()
        {
            if (this.m_internalAct != null)
            {
                this.m_internalAct.Destroy();
                this.m_internalAct = null;
            }
        }

        public TriggerActionBase GetActionInternal()
        {
            return this.m_internalAct;
        }

        public void Init(int inTriggerId)
        {
            this.m_triggerId = inTriggerId;
            if (this.m_internalAct == null)
            {
                switch (this.TriggerType)
                {
                    case EGlobalTriggerAct.Activate:
                        this.m_internalAct = new TriggerActionActivator(this, this.m_triggerId);
                        this.m_internalAct.bEnable = true;
                        break;

                    case EGlobalTriggerAct.Deactivate:
                        this.m_internalAct = new TriggerActionActivator(this, this.m_triggerId);
                        this.m_internalAct.bEnable = false;
                        break;

                    case EGlobalTriggerAct.TriggerBuff:
                        this.m_internalAct = new TriggerActionBuff(this, this.m_triggerId);
                        break;

                    case EGlobalTriggerAct.TriggerDialogue:
                        this.m_internalAct = new TriggerActionDialogue(this, this.m_triggerId);
                        break;

                    case EGlobalTriggerAct.TriggerSpawn:
                        this.m_internalAct = new TriggerActionSpawn(this, this.m_triggerId);
                        break;

                    case EGlobalTriggerAct.TriggerGuideTip:
                        this.m_internalAct = new TriggerActionGuideTip(this, this.m_triggerId);
                        break;

                    case EGlobalTriggerAct.TriggerDynamicBlock:
                        this.m_internalAct = new TriggerActionBlockSwitcher(this, this.m_triggerId);
                        break;

                    case EGlobalTriggerAct.TriggerAge:
                        this.m_internalAct = new TriggerActionAge(this, this.m_triggerId);
                        break;

                    case EGlobalTriggerAct.TriggerJungle:
                        this.m_internalAct = new TriggerActionJungle(this, this.m_triggerId);
                        break;

                    case EGlobalTriggerAct.TriggerBubbleText:
                        this.m_internalAct = new TriggerActionTextBubble(this, this.m_triggerId);
                        break;

                    case EGlobalTriggerAct.TriggerSkillHud:
                        this.m_internalAct = new TriggerActionSkillHud(this, this.m_triggerId);
                        break;

                    case EGlobalTriggerAct.TriggerBattleUi:
                        this.m_internalAct = new TriggerActionShowToggleAuto(this, this.m_triggerId);
                        break;

                    case EGlobalTriggerAct.TriggerNewbieForm:
                        this.m_internalAct = new TriggerActionNewbieForm(this, this.m_triggerId);
                        break;

                    case EGlobalTriggerAct.TriggerSoldierLine:
                        this.m_internalAct = new TriggerActionSoldierLine(this, this.m_triggerId);
                        break;

                    case EGlobalTriggerAct.TriggerPauseGame:
                        this.m_internalAct = new TriggerActionPauseGame(this, this.m_triggerId);
                        break;

                    case EGlobalTriggerAct.TriggerShenFu:
                        this.m_internalAct = new TriggerActionShenFu(this, this.m_triggerId);
                        break;

                    case EGlobalTriggerAct.TriggerBattleEquipLimit:
                        this.m_internalAct = new TriggerActionBattleEquipLimit(this, this.m_triggerId);
                        break;

                    case EGlobalTriggerAct.TriggerSetGlobalVariable:
                        this.m_internalAct = new TriggerActionSetGlobalVariable(this, this.m_triggerId);
                        break;

                    case EGlobalTriggerAct.TriggerAgeWithMobaLevel:
                        this.m_internalAct = new TriggerActionAgeWithMobaLevel(this, this.m_triggerId);
                        break;

                    default:
                        DebugHelper.Assert(false);
                        break;
                }
            }
        }

        public virtual void OnCoolDown(ITrigger inTrigger)
        {
            if (this.m_internalAct != null)
            {
                this.m_internalAct.OnCoolDown(inTrigger);
            }
        }

        public virtual void OnTriggerStart(ITrigger inTrigger)
        {
            if (this.m_internalAct != null)
            {
                this.m_internalAct.OnTriggerStart(inTrigger);
            }
        }

        public void PreLoadResource(ref ActorPreloadTab loadInfo, Dictionary<string, bool> ageCheckerSet, LoaderHelper loadHelper)
        {
            if (this.TimingActionsInter != null)
            {
                int length = this.TimingActionsInter.Length;
                for (int i = 0; i < length; i++)
                {
                    if (!string.IsNullOrEmpty(this.TimingActionsInter[i].ActionName) && !ageCheckerSet.ContainsKey(this.TimingActionsInter[i].ActionName))
                    {
                        AssetLoadBase item = new AssetLoadBase();
                        item.assetPath = this.TimingActionsInter[i].ActionName;
                        loadInfo.ageActions.Add(item);
                        ageCheckerSet.Add(this.TimingActionsInter[i].ActionName, true);
                    }
                }
            }
            if (this.TriggerType == EGlobalTriggerAct.TriggerBuff)
            {
                loadHelper.AnalyseSkillCombine(ref loadInfo, this.EnterUniqueId);
                loadHelper.AnalyseSkillCombine(ref loadInfo, this.LeaveUniqueId);
                loadHelper.AnalyseSkillCombine(ref loadInfo, this.UpdateUniqueId);
            }
        }

        public void Stop()
        {
            if (this.m_internalAct != null)
            {
                this.m_internalAct.Stop();
            }
        }

        public RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
        {
            if (this.m_internalAct == null)
            {
                return null;
            }
            return this.m_internalAct.TriggerEnter(src, atker, inTrigger);
        }

        public void TriggerLeave(PoolObjHandle<ActorRoot> src, ITrigger inTrigger)
        {
            if (this.m_internalAct != null)
            {
                this.m_internalAct.TriggerLeave(src, inTrigger);
            }
        }

        public void TriggerUpdate(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
        {
            if (this.m_internalAct != null)
            {
                this.m_internalAct.TriggerUpdate(src, atker, inTrigger);
            }
        }
    }
}

