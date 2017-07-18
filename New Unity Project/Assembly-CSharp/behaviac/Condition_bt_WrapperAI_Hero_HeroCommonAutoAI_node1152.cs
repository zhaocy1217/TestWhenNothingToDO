﻿namespace behaviac
{
    using Assets.Scripts.GameLogic;

    internal class Condition_bt_WrapperAI_Hero_HeroCommonAutoAI_node1152 : Condition
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            ObjDeadMode actorDeadState = ((ObjAgent) pAgent).GetActorDeadState();
            ObjDeadMode mode2 = ObjDeadMode.DeadState_Normal;
            return ((actorDeadState != mode2) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

