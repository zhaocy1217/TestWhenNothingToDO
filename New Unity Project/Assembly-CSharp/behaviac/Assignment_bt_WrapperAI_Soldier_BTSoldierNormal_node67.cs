namespace behaviac
{
    using System;

    internal class Assignment_bt_WrapperAI_Soldier_BTSoldierNormal_node67 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            ushort num = 0;
            pAgent.SetVariable<ushort>("p_chooseTargetCount", num, 0x3914f846);
            return status;
        }
    }
}

