namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Action_bt_WrapperAI_Organ_CrystalWithAttack_node131 : Action
    {
        private int method_p1 = 0x11170;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            int variable = (int) pAgent.GetVariable((uint) 0xe20dcd70);
            uint centerObjId = (uint) pAgent.GetVariable((uint) 0xb8b56d83);
            ((ObjAgent) pAgent).NotifyTeamSelectRoute(variable, this.method_p1, centerObjId);
            return EBTStatus.BT_SUCCESS;
        }
    }
}

