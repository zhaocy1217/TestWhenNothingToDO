namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Condition_bt_WrapperAI_Organ_CrystalWithAttack_node112 : Condition
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            int selfCampAliveHerpNum = ((ObjAgent) pAgent).GetSelfCampAliveHerpNum();
            int num2 = 4;
            return ((selfCampAliveHerpNum < num2) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

