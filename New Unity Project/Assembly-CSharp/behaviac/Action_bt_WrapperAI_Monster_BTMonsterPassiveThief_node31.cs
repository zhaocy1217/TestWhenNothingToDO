﻿namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Action_bt_WrapperAI_Monster_BTMonsterPassiveThief_node31 : Action
    {
        private string method_p0 = "Dead";
        private float method_p1 = 0.15f;
        private int method_p2 = 0;
        private bool method_p3 = false;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            ((ObjAgent) pAgent).PlayAnimation(this.method_p0, this.method_p1, this.method_p2, this.method_p3);
            return EBTStatus.BT_SUCCESS;
        }
    }
}

