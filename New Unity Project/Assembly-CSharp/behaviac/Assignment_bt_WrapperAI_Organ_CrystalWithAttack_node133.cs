namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Organ_CrystalWithAttack_node133 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int variable = (int) pAgent.GetVariable((uint) 0x2e38d5e4);
            int waitFrame = ((ObjAgent) pAgent).GetWaitFrame(variable);
            pAgent.SetVariable<int>("p_attackTowerWaitFrame", waitFrame, 0x2e38d5e4);
            return status;
        }
    }
}

