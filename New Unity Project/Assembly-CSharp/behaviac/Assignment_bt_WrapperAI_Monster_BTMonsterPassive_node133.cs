namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Monster_BTMonsterPassive_node133 : Assignment
    {
        private uint opr_p1 = 6;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int variable = (int) pAgent.GetVariable((uint) 0x921d0d6a);
            uint nearestEnemyWithFilter = ((ObjAgent) pAgent).GetNearestEnemyWithFilter(variable, this.opr_p1);
            pAgent.SetVariable<uint>("p_targetID", nearestEnemyWithFilter, 0x4349179f);
            return status;
        }
    }
}

