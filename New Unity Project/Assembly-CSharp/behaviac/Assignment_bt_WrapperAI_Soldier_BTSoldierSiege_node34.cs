﻿namespace behaviac
{
    using System;

    internal class Assignment_bt_WrapperAI_Soldier_BTSoldierSiege_node34 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            uint variable = (uint) pAgent.GetVariable((uint) 0x894ebed0);
            pAgent.SetVariable<uint>("p_targetID", variable, 0x4349179f);
            return status;
        }
    }
}

