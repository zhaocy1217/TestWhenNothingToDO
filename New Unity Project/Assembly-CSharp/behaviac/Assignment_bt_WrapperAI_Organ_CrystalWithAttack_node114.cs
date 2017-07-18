namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Organ_CrystalWithAttack_node114 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            uint bigDragon = ((ObjAgent) pAgent).GetBigDragon();
            pAgent.SetVariable<uint>("p_commandTarget", bigDragon, 0xb8b56d83);
            return status;
        }
    }
}

