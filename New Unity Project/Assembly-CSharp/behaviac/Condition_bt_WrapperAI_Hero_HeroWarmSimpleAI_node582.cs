﻿namespace behaviac
{
    using System;

    internal class Condition_bt_WrapperAI_Hero_HeroWarmSimpleAI_node582 : Condition
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            int variable = (int) pAgent.GetVariable((uint) 0xb8ff8feb);
            int num2 = 0x19;
            return ((variable <= num2) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

