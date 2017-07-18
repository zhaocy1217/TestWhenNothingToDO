namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Organ_CrystalWithAttack_node127 : Assignment
    {
        private int opr_p0 = 0x7530;
        private int opr_p1 = 0x4e20;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            uint commandAttackOrgan = ((ObjAgent) pAgent).GetCommandAttackOrgan(this.opr_p0, this.opr_p1);
            pAgent.SetVariable<uint>("p_commandTarget", commandAttackOrgan, 0xb8b56d83);
            return status;
        }
    }
}

