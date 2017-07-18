namespace behaviac
{
    using System;

    internal class Compute_bt_WrapperAI_Hero_HeroWarmNormalAI_node505 : Compute
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int variable = (int) pAgent.GetVariable((uint) 0x105720bd);
            int num2 = 5;
            int num3 = variable * num2;
            pAgent.SetVariable<int>("p_useSkillWeightActually", num3, 0xf25eb1eb);
            return status;
        }
    }
}

