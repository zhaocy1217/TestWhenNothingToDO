namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Condition_bt_WrapperAI_Hero_HeroWarmNormalAI_node484 : Condition
    {
        private int opl_p0 = 0x1f40;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            uint num = ((ObjAgent) pAgent).IsUnderEnemyBuilding(this.opl_p0);
            uint num2 = 0;
            return ((num <= num2) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

