namespace behaviac
{
    using System;

    internal class Condition_bt_WrapperAI_Hero_HeroWarmNormalAI_node544 : Condition
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            int variable = (int) pAgent.GetVariable((uint) 0x343447);
            int num2 = 3;
            return ((variable >= num2) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

