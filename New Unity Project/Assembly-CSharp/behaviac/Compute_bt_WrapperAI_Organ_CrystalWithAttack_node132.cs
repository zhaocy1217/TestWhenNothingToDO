namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Compute_bt_WrapperAI_Organ_CrystalWithAttack_node132 : Compute
    {
        private uint opr1_p0 = 300;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int randomInt = ((BTBaseAgent) pAgent).GetRandomInt(this.opr1_p0);
            int num2 = 400;
            int num3 = randomInt + num2;
            pAgent.SetVariable<int>("p_attackTowerWaitFrame", num3, 0x2e38d5e4);
            return status;
        }
    }
}

