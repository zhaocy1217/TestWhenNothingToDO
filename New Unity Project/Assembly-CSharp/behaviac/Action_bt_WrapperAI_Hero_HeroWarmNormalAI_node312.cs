namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Action_bt_WrapperAI_Hero_HeroWarmNormalAI_node312 : Action
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            uint variable = (uint) pAgent.GetVariable((uint) 0x4349179f);
            int range = (int) pAgent.GetVariable((uint) 0x73e592c4);
            ((ObjAgent) pAgent).RealMoveInMoveAttack(variable, range);
            return EBTStatus.BT_SUCCESS;
        }
    }
}

