namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Action_bt_WrapperAI_Hero_HeroCommonAutoAI_node485 : Action
    {
        private int method_p0 = 0x2ee0;
        private int method_p1 = 0x2134;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            return ((ObjAgent) pAgent).IsAroundTeamThanStrongThanEnemise(this.method_p0, this.method_p1);
        }
    }
}

