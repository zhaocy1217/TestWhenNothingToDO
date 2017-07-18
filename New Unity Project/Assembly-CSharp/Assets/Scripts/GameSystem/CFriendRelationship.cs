namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class CFriendRelationship
    {
        private uint _initmacyLimitTime;
        private COM_INTIMACY_STATE _state;
        [CompilerGenerated]
        private string <IntimRela_AleadyFristChoise>k__BackingField;
        [CompilerGenerated]
        private string <IntimRela_CD_CountDown>k__BackingField;
        [CompilerGenerated]
        private string <IntimRela_DoFristChoise>k__BackingField;
        [CompilerGenerated]
        private string <IntimRela_EmptyDataText>k__BackingField;
        [CompilerGenerated]
        private string <IntimRela_ReDelRelation>k__BackingField;
        [CompilerGenerated]
        private string <IntimRela_ReselectRelation>k__BackingField;
        [CompilerGenerated]
        private string <IntimRela_Tips_AlreadyHasGay>k__BackingField;
        [CompilerGenerated]
        private string <IntimRela_Tips_AlreadyHasLover>k__BackingField;
        [CompilerGenerated]
        private string <IntimRela_Tips_Cancle>k__BackingField;
        [CompilerGenerated]
        private string <IntimRela_Tips_DenyYourDelGay>k__BackingField;
        [CompilerGenerated]
        private string <IntimRela_Tips_DenyYourDelLover>k__BackingField;
        [CompilerGenerated]
        private string <IntimRela_Tips_DenyYourRequestGay>k__BackingField;
        [CompilerGenerated]
        private string <IntimRela_Tips_DenyYourRequestLover>k__BackingField;
        [CompilerGenerated]
        private string <IntimRela_Tips_MidText>k__BackingField;
        [CompilerGenerated]
        private string <IntimRela_Tips_OK>k__BackingField;
        [CompilerGenerated]
        private string <IntimRela_Tips_ReceiveOtherDelRela>k__BackingField;
        [CompilerGenerated]
        private string <IntimRela_Tips_ReceiveOtherReqRela>k__BackingField;
        [CompilerGenerated]
        private string <IntimRela_Tips_RelaHasDel>k__BackingField;
        [CompilerGenerated]
        private string <IntimRela_Tips_SelectRelation>k__BackingField;
        [CompilerGenerated]
        private string <IntimRela_Tips_SendDelGaySuccess>k__BackingField;
        [CompilerGenerated]
        private string <IntimRela_Tips_SendDelLoverSuccess>k__BackingField;
        [CompilerGenerated]
        private string <IntimRela_Tips_SendRequestGaySuccess>k__BackingField;
        [CompilerGenerated]
        private string <IntimRela_Tips_SendRequestLoverSuccess>k__BackingField;
        [CompilerGenerated]
        private string <IntimRela_Tips_Wait4TargetRspDelRela>k__BackingField;
        [CompilerGenerated]
        private string <IntimRela_Tips_Wait4TargetRspReqRela>k__BackingField;
        [CompilerGenerated]
        private string <IntimRela_Type_Gay>k__BackingField;
        [CompilerGenerated]
        private string <IntimRela_Type_Lover>k__BackingField;
        [CompilerGenerated]
        private string <IntimRela_TypeColor_Gay>k__BackingField;
        [CompilerGenerated]
        private string <IntimRela_TypeColor_Lover>k__BackingField;
        public ListView<FRConfig> frConfig_list = new ListView<FRConfig>();
        private ListView<CFR> m_cfrList = new ListView<CFR>();

        public void Add(ulong ulluid, uint worldId, COM_INTIMACY_STATE state, COM_INTIMACY_RELATION_CHG_TYPE op, uint timeStamp, bool bReceiveNtf = false)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if (((masterRoleInfo == null) || (masterRoleInfo.playerUllUID != ulluid)) || (masterRoleInfo.logicWorldID != worldId))
            {
                CFR cfr = this.GetCfr(ulluid, worldId);
                if (cfr == null)
                {
                    this.m_cfrList.Add(new CFR(ulluid, worldId, state, op, timeStamp, bReceiveNtf));
                }
                else
                {
                    cfr.SetData(ulluid, worldId, state, op, timeStamp, bReceiveNtf);
                }
                if (state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY)
                {
                    this.FindSetState(COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY_CONFIRM, COM_INTIMACY_STATE.COM_INTIMACY_STATE_VALUE_FULL);
                }
                if (state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER)
                {
                    this.FindSetState(COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER_CONFIRM, COM_INTIMACY_STATE.COM_INTIMACY_STATE_VALUE_FULL);
                }
                Singleton<EventRouter>.GetInstance().BroadCastEvent("FRDataChange");
            }
        }

        public void Add(ulong ulluid, uint worldId, byte state, byte op, uint timeStamp, bool bReceiveNtf = false)
        {
            this.Add(ulluid, worldId, (COM_INTIMACY_STATE) state, (COM_INTIMACY_RELATION_CHG_TYPE) op, timeStamp, bReceiveNtf);
        }

        public void Clear()
        {
            for (int i = 0; i < this.m_cfrList.Count; i++)
            {
                CFR cfr = this.m_cfrList[i];
                if (cfr != null)
                {
                    cfr.Clear();
                }
            }
            this.m_cfrList.Clear();
            this._initmacyLimitTime = 0;
        }

        public CFR FindFrist(COM_INTIMACY_STATE state)
        {
            for (int i = 0; i < this.m_cfrList.Count; i++)
            {
                CFR cfr = this.m_cfrList[i];
                if ((cfr != null) && (cfr.state == state))
                {
                    return cfr;
                }
            }
            return null;
        }

        public void FindSetState(COM_INTIMACY_STATE target1, COM_INTIMACY_STATE newState)
        {
            for (int i = 0; i < this.m_cfrList.Count; i++)
            {
                CFR cfr = this.m_cfrList[i];
                if ((cfr != null) && (cfr.state == target1))
                {
                    cfr.state = newState;
                }
            }
        }

        public void FindSetState(COM_INTIMACY_STATE target1, COM_INTIMACY_STATE target2, COM_INTIMACY_STATE newState)
        {
            for (int i = 0; i < this.m_cfrList.Count; i++)
            {
                CFR cfr = this.m_cfrList[i];
                if ((cfr != null) && ((cfr.state == target1) || (cfr.state == target2)))
                {
                    cfr.state = newState;
                }
            }
        }

        public int GetCandiRelationCount()
        {
            return 2;
        }

        public FRConfig GetCFGByIndex(int index)
        {
            for (int i = 0; i < this.frConfig_list.Count; i++)
            {
                FRConfig config = this.frConfig_list[i];
                if ((config != null) && (config.piority == index))
                {
                    return config;
                }
            }
            return null;
        }

        public CFR GetCfr(ulong ulluid, uint worldID)
        {
            for (int i = 0; i < this.m_cfrList.Count; i++)
            {
                CFR cfr = this.m_cfrList[i];
                if (((cfr != null) && (cfr.ulluid == ulluid)) && (cfr.worldID == worldID))
                {
                    return cfr;
                }
            }
            return null;
        }

        public int GetCount()
        {
            return this.m_cfrList.Count;
        }

        public COM_INTIMACY_STATE GetFirstChoiseRelation()
        {
            return COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY;
        }

        public COM_INTIMACY_STATE GetFirstChoiseState()
        {
            return this._state;
        }

        public ListView<CFR> GetList()
        {
            return this.m_cfrList;
        }

        public bool HasRedDot()
        {
            for (int i = 0; i < this.m_cfrList.Count; i++)
            {
                CFR cfr = this.m_cfrList[i];
                if ((cfr != null) && cfr.bRedDot)
                {
                    return true;
                }
            }
            return false;
        }

        public void LoadConfig()
        {
            this.frConfig_list.Clear();
            this.frConfig_list.Add(new FRConfig(0, COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY, Singleton<CTextManager>.instance.GetText("IntimRela_Type_Gay")));
            this.frConfig_list.Add(new FRConfig(1, COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER, Singleton<CTextManager>.instance.GetText("IntimRela_Type_Lover")));
            this.IntimRela_Tips_AlreadyHasGay = Singleton<CTextManager>.instance.GetText("IntimRela_Tips_AlreadyHasGay");
            this.IntimRela_Tips_AlreadyHasLover = Singleton<CTextManager>.instance.GetText("IntimRela_Tips_AlreadyHasLover");
            this.IntimRela_Type_Gay = Singleton<CTextManager>.instance.GetText("IntimRela_Type_Gay");
            this.IntimRela_Type_Lover = Singleton<CTextManager>.instance.GetText("IntimRela_Type_Lover");
            this.IntimRela_Tips_SendRequestGaySuccess = Singleton<CTextManager>.instance.GetText("IntimRela_Tips_SendRequestGaySuccess");
            this.IntimRela_Tips_SendRequestLoverSuccess = Singleton<CTextManager>.instance.GetText("IntimRela_Tips_SendRequestLoverSuccess");
            this.IntimRela_Tips_SendDelGaySuccess = Singleton<CTextManager>.instance.GetText("IntimRela_Tips_SendDelGaySuccess");
            this.IntimRela_Tips_SendDelLoverSuccess = Singleton<CTextManager>.instance.GetText("IntimRela_Tips_SendDelLoverSuccess");
            this.IntimRela_Tips_DenyYourRequestGay = Singleton<CTextManager>.instance.GetText("IntimRela_Tips_DenyYourRequestGay");
            this.IntimRela_Tips_DenyYourRequestLover = Singleton<CTextManager>.instance.GetText("IntimRela_Tips_DenyYourRequestLover");
            this.IntimRela_Tips_DenyYourDelGay = Singleton<CTextManager>.instance.GetText("IntimRela_Tips_DenyYourDelGay");
            this.IntimRela_Tips_DenyYourDelLover = Singleton<CTextManager>.instance.GetText("IntimRela_Tips_DenyYourDelLover");
            this.IntimRela_CD_CountDown = Singleton<CTextManager>.instance.GetText("IntimRela_CD_CountDown");
            this.IntimRela_Tips_ReceiveOtherReqRela = Singleton<CTextManager>.instance.GetText("IntimRela_Tips_ReceiveOtherReqRela");
            this.IntimRela_Tips_Wait4TargetRspReqRela = Singleton<CTextManager>.instance.GetText("IntimRela_Tips_Wait4TargetRspReqRela");
            this.IntimRela_Tips_ReceiveOtherDelRela = Singleton<CTextManager>.instance.GetText("IntimRela_Tips_ReceiveOtherDelRela");
            this.IntimRela_Tips_Wait4TargetRspDelRela = Singleton<CTextManager>.instance.GetText("IntimRela_Tips_Wait4TargetRspDelRela");
            this.IntimRela_Tips_SelectRelation = Singleton<CTextManager>.instance.GetText("IntimRela_Tips_SelectRelation");
            this.IntimRela_Tips_MidText = Singleton<CTextManager>.instance.GetText("IntimRela_Tips_MidText");
            this.IntimRela_Tips_RelaHasDel = Singleton<CTextManager>.instance.GetText("IntimRela_Tips_RelaHasDel");
            this.IntimRela_Tips_OK = Singleton<CTextManager>.instance.GetText("IntimRela_Tips_OK");
            this.IntimRela_Tips_Cancle = Singleton<CTextManager>.instance.GetText("IntimRela_Tips_Cancle");
            this.IntimRela_DoFristChoise = Singleton<CTextManager>.instance.GetText("IntimRela_DoFristChoise");
            this.IntimRela_AleadyFristChoise = Singleton<CTextManager>.instance.GetText("IntimRela_AleadyFristChoise");
            this.IntimRela_ReselectRelation = Singleton<CTextManager>.instance.GetText("IntimRela_ReselectRelation");
            this.IntimRela_ReDelRelation = Singleton<CTextManager>.instance.GetText("IntimRela_ReDelRelation");
            this.IntimRela_EmptyDataText = Singleton<CTextManager>.instance.GetText("IntimRela_EmptyDataText");
            this.IntimRela_TypeColor_Gay = Singleton<CTextManager>.instance.GetText("IntimRela_TypeColor_Gay");
            this.IntimRela_TypeColor_Lover = Singleton<CTextManager>.instance.GetText("IntimRela_TypeColor_Lover");
        }

        public void ProcessFriendList(COMDT_ACNT_UNIQ uniq, COMDT_INTIMACY_DATA data)
        {
            if ((uniq != null) && (data != null))
            {
                byte bIntimacyState = data.bIntimacyState;
                CFR cfr = this.GetCfr(uniq.ullUid, uniq.dwLogicWorldId);
                if ((cfr == null) || (cfr.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_NULL))
                {
                    if (CFR.GetCDDays(data.dwTerminateTime) != -1)
                    {
                        this.Add(uniq.ullUid, uniq.dwLogicWorldId, (COM_INTIMACY_STATE) bIntimacyState, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_NULL, data.dwTerminateTime, false);
                    }
                    else if ((data.bIntimacyState == 0) && (data.wIntimacyValue == Singleton<CFriendContoller>.instance.model.GetMaxIntimacyNum()))
                    {
                        this.Add(uniq.ullUid, uniq.dwLogicWorldId, COM_INTIMACY_STATE.COM_INTIMACY_STATE_VALUE_FULL, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_NULL, data.dwTerminateTime, false);
                    }
                    else if (bIntimacyState != 0)
                    {
                        this.Add(uniq.ullUid, uniq.dwLogicWorldId, (COM_INTIMACY_STATE) bIntimacyState, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_NULL, data.dwTerminateTime, false);
                    }
                }
            }
        }

        public void ProcessOtherRequest(CSDT_VERIFICATION_INFO data, bool bReceiveNtf = false)
        {
            this.Add(data.stFriendInfo.stUin.ullUid, data.stFriendInfo.stUin.dwLogicWorldId, (COM_INTIMACY_STATE) data.bIntimacyState, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_ADD, 0, bReceiveNtf);
        }

        public void Remove(ulong ulluid, uint worldID)
        {
            int index = -1;
            for (int i = 0; i < this.m_cfrList.Count; i++)
            {
                CFR cfr = this.m_cfrList[i];
                if (((cfr != null) && (cfr.ulluid == ulluid)) && (cfr.worldID == worldID))
                {
                    index = i;
                }
            }
            if (index != -1)
            {
                this.m_cfrList.RemoveAt(index);
                Singleton<EventRouter>.GetInstance().BroadCastEvent("FRDataChange");
            }
        }

        public void ResetChoiseRelaState(ulong ulluid, uint worldID)
        {
            CFR cfr = this.GetCfr(ulluid, worldID);
            if (cfr != null)
            {
                cfr.choiseRelation = -1;
                cfr.bInShowChoiseRelaList = false;
            }
        }

        public void SetFirstChoiseState(COM_INTIMACY_STATE state)
        {
            this._state = state;
            Singleton<EventRouter>.GetInstance().BroadCastEvent("FRDataChange");
        }

        public void SetFirstChoiseState(byte state)
        {
            this.SetFirstChoiseState((COM_INTIMACY_STATE) state);
        }

        public void Sort()
        {
        }

        public static CFriendRelationship FRData
        {
            get
            {
                return Singleton<CFriendContoller>.instance.model.FRData;
            }
        }

        public uint InitmacyLimitTime
        {
            get
            {
                if (this._initmacyLimitTime == 0)
                {
                    this._initmacyLimitTime = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_INTIMACY_LIMITTIME);
                }
                return this._initmacyLimitTime;
            }
        }

        public string IntimRela_AleadyFristChoise
        {
            [CompilerGenerated]
            get
            {
                return this.<IntimRela_AleadyFristChoise>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<IntimRela_AleadyFristChoise>k__BackingField = value;
            }
        }

        public string IntimRela_CD_CountDown
        {
            [CompilerGenerated]
            get
            {
                return this.<IntimRela_CD_CountDown>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<IntimRela_CD_CountDown>k__BackingField = value;
            }
        }

        public string IntimRela_DoFristChoise
        {
            [CompilerGenerated]
            get
            {
                return this.<IntimRela_DoFristChoise>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<IntimRela_DoFristChoise>k__BackingField = value;
            }
        }

        public string IntimRela_EmptyDataText
        {
            [CompilerGenerated]
            get
            {
                return this.<IntimRela_EmptyDataText>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<IntimRela_EmptyDataText>k__BackingField = value;
            }
        }

        public string IntimRela_ReDelRelation
        {
            [CompilerGenerated]
            get
            {
                return this.<IntimRela_ReDelRelation>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<IntimRela_ReDelRelation>k__BackingField = value;
            }
        }

        public string IntimRela_ReselectRelation
        {
            [CompilerGenerated]
            get
            {
                return this.<IntimRela_ReselectRelation>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<IntimRela_ReselectRelation>k__BackingField = value;
            }
        }

        public string IntimRela_Tips_AlreadyHasGay
        {
            [CompilerGenerated]
            get
            {
                return this.<IntimRela_Tips_AlreadyHasGay>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<IntimRela_Tips_AlreadyHasGay>k__BackingField = value;
            }
        }

        public string IntimRela_Tips_AlreadyHasLover
        {
            [CompilerGenerated]
            get
            {
                return this.<IntimRela_Tips_AlreadyHasLover>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<IntimRela_Tips_AlreadyHasLover>k__BackingField = value;
            }
        }

        public string IntimRela_Tips_Cancle
        {
            [CompilerGenerated]
            get
            {
                return this.<IntimRela_Tips_Cancle>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<IntimRela_Tips_Cancle>k__BackingField = value;
            }
        }

        public string IntimRela_Tips_DenyYourDelGay
        {
            [CompilerGenerated]
            get
            {
                return this.<IntimRela_Tips_DenyYourDelGay>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<IntimRela_Tips_DenyYourDelGay>k__BackingField = value;
            }
        }

        public string IntimRela_Tips_DenyYourDelLover
        {
            [CompilerGenerated]
            get
            {
                return this.<IntimRela_Tips_DenyYourDelLover>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<IntimRela_Tips_DenyYourDelLover>k__BackingField = value;
            }
        }

        public string IntimRela_Tips_DenyYourRequestGay
        {
            [CompilerGenerated]
            get
            {
                return this.<IntimRela_Tips_DenyYourRequestGay>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<IntimRela_Tips_DenyYourRequestGay>k__BackingField = value;
            }
        }

        public string IntimRela_Tips_DenyYourRequestLover
        {
            [CompilerGenerated]
            get
            {
                return this.<IntimRela_Tips_DenyYourRequestLover>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<IntimRela_Tips_DenyYourRequestLover>k__BackingField = value;
            }
        }

        public string IntimRela_Tips_MidText
        {
            [CompilerGenerated]
            get
            {
                return this.<IntimRela_Tips_MidText>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<IntimRela_Tips_MidText>k__BackingField = value;
            }
        }

        public string IntimRela_Tips_OK
        {
            [CompilerGenerated]
            get
            {
                return this.<IntimRela_Tips_OK>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<IntimRela_Tips_OK>k__BackingField = value;
            }
        }

        public string IntimRela_Tips_ReceiveOtherDelRela
        {
            [CompilerGenerated]
            get
            {
                return this.<IntimRela_Tips_ReceiveOtherDelRela>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<IntimRela_Tips_ReceiveOtherDelRela>k__BackingField = value;
            }
        }

        public string IntimRela_Tips_ReceiveOtherReqRela
        {
            [CompilerGenerated]
            get
            {
                return this.<IntimRela_Tips_ReceiveOtherReqRela>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<IntimRela_Tips_ReceiveOtherReqRela>k__BackingField = value;
            }
        }

        public string IntimRela_Tips_RelaHasDel
        {
            [CompilerGenerated]
            get
            {
                return this.<IntimRela_Tips_RelaHasDel>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<IntimRela_Tips_RelaHasDel>k__BackingField = value;
            }
        }

        public string IntimRela_Tips_SelectRelation
        {
            [CompilerGenerated]
            get
            {
                return this.<IntimRela_Tips_SelectRelation>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<IntimRela_Tips_SelectRelation>k__BackingField = value;
            }
        }

        public string IntimRela_Tips_SendDelGaySuccess
        {
            [CompilerGenerated]
            get
            {
                return this.<IntimRela_Tips_SendDelGaySuccess>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<IntimRela_Tips_SendDelGaySuccess>k__BackingField = value;
            }
        }

        public string IntimRela_Tips_SendDelLoverSuccess
        {
            [CompilerGenerated]
            get
            {
                return this.<IntimRela_Tips_SendDelLoverSuccess>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<IntimRela_Tips_SendDelLoverSuccess>k__BackingField = value;
            }
        }

        public string IntimRela_Tips_SendRequestGaySuccess
        {
            [CompilerGenerated]
            get
            {
                return this.<IntimRela_Tips_SendRequestGaySuccess>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<IntimRela_Tips_SendRequestGaySuccess>k__BackingField = value;
            }
        }

        public string IntimRela_Tips_SendRequestLoverSuccess
        {
            [CompilerGenerated]
            get
            {
                return this.<IntimRela_Tips_SendRequestLoverSuccess>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<IntimRela_Tips_SendRequestLoverSuccess>k__BackingField = value;
            }
        }

        public string IntimRela_Tips_Wait4TargetRspDelRela
        {
            [CompilerGenerated]
            get
            {
                return this.<IntimRela_Tips_Wait4TargetRspDelRela>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<IntimRela_Tips_Wait4TargetRspDelRela>k__BackingField = value;
            }
        }

        public string IntimRela_Tips_Wait4TargetRspReqRela
        {
            [CompilerGenerated]
            get
            {
                return this.<IntimRela_Tips_Wait4TargetRspReqRela>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<IntimRela_Tips_Wait4TargetRspReqRela>k__BackingField = value;
            }
        }

        public string IntimRela_Type_Gay
        {
            [CompilerGenerated]
            get
            {
                return this.<IntimRela_Type_Gay>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<IntimRela_Type_Gay>k__BackingField = value;
            }
        }

        public string IntimRela_Type_Lover
        {
            [CompilerGenerated]
            get
            {
                return this.<IntimRela_Type_Lover>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<IntimRela_Type_Lover>k__BackingField = value;
            }
        }

        public string IntimRela_TypeColor_Gay
        {
            [CompilerGenerated]
            get
            {
                return this.<IntimRela_TypeColor_Gay>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<IntimRela_TypeColor_Gay>k__BackingField = value;
            }
        }

        public string IntimRela_TypeColor_Lover
        {
            [CompilerGenerated]
            get
            {
                return this.<IntimRela_TypeColor_Lover>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<IntimRela_TypeColor_Lover>k__BackingField = value;
            }
        }

        public class FRConfig
        {
            public string cfgRelaStr;
            public int piority = -1;
            public COM_INTIMACY_STATE state;

            public FRConfig(int piority, COM_INTIMACY_STATE state, string cfgRelaStr)
            {
                this.piority = piority;
                this.state = state;
                this.cfgRelaStr = cfgRelaStr;
            }
        }
    }
}

