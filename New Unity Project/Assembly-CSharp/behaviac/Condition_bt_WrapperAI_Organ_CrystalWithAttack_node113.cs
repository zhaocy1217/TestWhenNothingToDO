namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Condition_bt_WrapperAI_Organ_CrystalWithAttack_node113 : Condition
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            int enemyCampAliveHerpNum = ((ObjAgent) pAgent).GetEnemyCampAliveHerpNum();
            int num2 = 4;
            return ((enemyCampAliveHerpNum > num2) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

