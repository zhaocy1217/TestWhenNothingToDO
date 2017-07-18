namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Organ_CrystalWithAttack_node130 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            uint variable = (uint) pAgent.GetVariable((uint) 0xb8b56d83);
            int num2 = ((ObjAgent) pAgent).GerPathIndexBuTowerId(variable);
            pAgent.SetVariable<int>("p_pathIndex", num2, 0xe20dcd70);
            return status;
        }
    }
}

