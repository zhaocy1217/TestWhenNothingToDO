﻿namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Action_bt_WrapperAI_Monster_BTMonsterInitiative_node25 : Action
    {
        private int method_p0 = 0x1388;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            ((ObjAgent) pAgent).NotifySelfCampSelfWillAttack(this.method_p0);
            return EBTStatus.BT_SUCCESS;
        }
    }
}

