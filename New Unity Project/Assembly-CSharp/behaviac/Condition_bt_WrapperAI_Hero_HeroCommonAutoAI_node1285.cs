namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Condition_bt_WrapperAI_Hero_HeroCommonAutoAI_node1285 : Condition
    {
        private int opl_p0 = 0x36b0;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            int enemyHeroCountInRange = ((ObjAgent) pAgent).GetEnemyHeroCountInRange(this.opl_p0);
            int num2 = 0;
            return ((enemyHeroCountInRange != num2) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

