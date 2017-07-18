namespace behaviac
{
    using System;

    internal class Compute_bt_WrapperAI_Soldier_BTSoldierPro_node121 : Compute
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            short variable = (short) pAgent.GetVariable((uint) 0x3914f846);
            short num2 = 1;
            short num3 = (short) (variable + num2);
            pAgent.SetVariable<short>("p_chooseTargetCount", num3, 0x3914f846);
            return status;
        }
    }
}

