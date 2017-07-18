namespace behaviac
{
    using System;

    internal class Condition_bt_WrapperAI_Soldier_BTSoldierSiege_node37 : Condition
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            ushort variable = (ushort) pAgent.GetVariable((uint) 0x3914f846);
            ushort num2 = 5;
            return ((variable <= num2) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

