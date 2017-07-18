namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Action_bt_WrapperAI_Organ_CrystalWithAttack_node135 : Action
    {
        private int method_p0 = 0x7530;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            uint variable = (uint) pAgent.GetVariable((uint) 0xb8b56d83);
            ((ObjAgent) pAgent).NotifyTeamSelectRandomRoute(this.method_p0, variable);
            return EBTStatus.BT_SUCCESS;
        }
    }
}

