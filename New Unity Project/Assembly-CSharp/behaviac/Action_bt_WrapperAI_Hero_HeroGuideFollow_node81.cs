﻿namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Action_bt_WrapperAI_Hero_HeroGuideFollow_node81 : Action
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            uint variable = (uint) pAgent.GetVariable((uint) 0x3d4ae8e1);
            ((ObjAgent) pAgent).LeaveActor(variable);
            return EBTStatus.BT_SUCCESS;
        }
    }
}

