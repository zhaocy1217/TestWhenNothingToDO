namespace behaviac
{
    using System;

    internal class Condition_bt_WrapperAI_Organ_CrystalWithAttack_node77 : Condition
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            uint variable = (uint) pAgent.GetVariable((uint) 0x12f3ddee);
            uint num2 = 0;
            return ((variable <= num2) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

