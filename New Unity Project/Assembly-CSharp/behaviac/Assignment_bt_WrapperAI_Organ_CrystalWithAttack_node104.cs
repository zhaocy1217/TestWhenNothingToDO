namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Organ_CrystalWithAttack_node104 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            uint smallDragon = ((ObjAgent) pAgent).GetSmallDragon();
            pAgent.SetVariable<uint>("p_commandTarget", smallDragon, 0xb8b56d83);
            return status;
        }
    }
}

