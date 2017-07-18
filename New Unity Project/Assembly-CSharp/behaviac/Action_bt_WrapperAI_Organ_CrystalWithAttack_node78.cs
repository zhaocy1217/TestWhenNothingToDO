namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Action_bt_WrapperAI_Organ_CrystalWithAttack_node78 : Action
    {
        private int method_p1 = 0xea60;
        private bool method_p2 = true;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            uint variable = (uint) pAgent.GetVariable((uint) 0x12f3ddee);
            ((ObjAgent) pAgent).NotifyTeamAttackEnemy(variable, this.method_p1, this.method_p2);
            return EBTStatus.BT_SUCCESS;
        }
    }
}

