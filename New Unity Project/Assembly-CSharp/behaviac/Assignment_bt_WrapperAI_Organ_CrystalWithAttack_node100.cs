namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Organ_CrystalWithAttack_node100 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int variable = (int) pAgent.GetVariable((uint) 0xd5f60189);
            int waitFrame = ((ObjAgent) pAgent).GetWaitFrame(variable);
            pAgent.SetVariable<int>("p_waitFrame", waitFrame, 0xd5f60189);
            return status;
        }
    }
}

