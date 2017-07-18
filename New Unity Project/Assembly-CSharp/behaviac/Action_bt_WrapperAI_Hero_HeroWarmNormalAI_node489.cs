namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Action_bt_WrapperAI_Hero_HeroWarmNormalAI_node489 : Action
    {
        private int method_p1 = 0x2328;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            uint variable = (uint) pAgent.GetVariable((uint) 0x4349179f);
            return ((ObjAgent) pAgent).IsDangerUnderEnemyBuilding(variable, this.method_p1);
        }
    }
}

