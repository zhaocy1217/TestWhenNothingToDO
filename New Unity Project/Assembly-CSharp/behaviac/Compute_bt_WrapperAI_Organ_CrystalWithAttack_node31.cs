namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Compute_bt_WrapperAI_Organ_CrystalWithAttack_node31 : Compute
    {
        private uint opr1_p0 = 15;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int randomInt = ((BTBaseAgent) pAgent).GetRandomInt(this.opr1_p0);
            int num2 = 15;
            int num3 = randomInt + num2;
            pAgent.SetVariable<int>("p_waitFrame", num3, 0xd5f60189);
            return status;
        }
    }
}

