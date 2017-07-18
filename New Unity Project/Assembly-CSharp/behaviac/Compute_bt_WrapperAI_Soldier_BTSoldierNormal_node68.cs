namespace behaviac
{
    using System;

    internal class Compute_bt_WrapperAI_Soldier_BTSoldierNormal_node68 : Compute
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            ushort variable = (ushort) pAgent.GetVariable((uint) 0x3914f846);
            ushort num2 = 1;
            ushort num3 = (ushort) (variable + num2);
            pAgent.SetVariable<ushort>("p_chooseTargetCount", num3, 0x3914f846);
            return status;
        }
    }
}

