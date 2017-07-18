namespace behaviac
{
    using Assets.Scripts.GameLogic;

    internal class Condition_bt_WrapperAI_Hero_HeroWarmNormalAI_node548 : Condition
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = ((ObjAgent) pAgent).IsLadder();
            EBTStatus status2 = EBTStatus.BT_SUCCESS;
            return ((status == status2) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

