﻿namespace behaviac
{
    using Assets.Scripts.GameLogic;

    internal class Action_bt_WrapperAI_Hero_HeroCommonAutoAI_node940 : Action
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            ((ObjAgent) pAgent).UseGoHomeSkill();
            return EBTStatus.BT_SUCCESS;
        }
    }
}

