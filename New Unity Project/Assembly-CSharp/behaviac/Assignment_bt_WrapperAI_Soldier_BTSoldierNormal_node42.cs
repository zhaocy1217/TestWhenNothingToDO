﻿namespace behaviac
{
    using System;

    internal class Assignment_bt_WrapperAI_Soldier_BTSoldierNormal_node42 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            uint num = 0;
            pAgent.SetVariable<uint>("p_targetID", num, 0x4349179f);
            return status;
        }
    }
}

