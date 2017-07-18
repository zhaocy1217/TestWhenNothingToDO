namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Soldier_BTSoldierNormal_node61 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            uint variable = (uint) pAgent.GetVariable((uint) 0x4349179f);
            int srchR = (int) pAgent.GetVariable((uint) 0x921d0d6a);
            uint newTargetByPriority = ((ObjAgent) pAgent).GetNewTargetByPriority(variable, srchR);
            pAgent.SetVariable<uint>("p_tempTargetId", newTargetByPriority, 0x894ebed0);
            return status;
        }
    }
}

