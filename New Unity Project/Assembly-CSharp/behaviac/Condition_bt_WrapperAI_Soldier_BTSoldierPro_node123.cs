namespace behaviac
{
    using System;

    internal class Condition_bt_WrapperAI_Soldier_BTSoldierPro_node123 : Condition
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            short variable = (short) pAgent.GetVariable((uint) 0x3914f846);
            short num2 = 5;
            return ((variable <= num2) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

