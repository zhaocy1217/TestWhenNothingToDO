namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Condition_bt_WrapperAI_Hero_HeroCommonAutoAI_node1256 : Condition
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            uint variable = (uint) pAgent.GetVariable((uint) 0x98af4863);
            ActorTypeDef actorType = ((ObjAgent) pAgent).GetActorType(variable);
            ActorTypeDef def2 = ActorTypeDef.Actor_Type_Hero;
            return ((actorType != def2) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

