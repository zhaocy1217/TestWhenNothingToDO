namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Organ_CrystalWithAttack_node120 : Assignment
    {
        private int opr_p0 = 0x4268;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            uint commandAttackHero = ((ObjAgent) pAgent).GetCommandAttackHero(this.opr_p0);
            pAgent.SetVariable<uint>("p_commandTarget", commandAttackHero, 0xb8b56d83);
            return status;
        }
    }
}

