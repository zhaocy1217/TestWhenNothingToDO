namespace behaviac
{
    using System;

    internal class Assignment_bt_WrapperAI_Soldier_BTSoldierPro_node126 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            short num = 0;
            pAgent.SetVariable<short>("p_chooseTargetCount", num, 0x3914f846);
            return status;
        }
    }
}

