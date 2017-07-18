namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using CSProtocol;
    using System;

    internal class Condition_bt_WrapperAI_Organ_CrystalWithAttack_node107 : Condition
    {
        private int opl_p1 = 0xafc8;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            uint variable = (uint) pAgent.GetVariable((uint) 0xb8b56d83);
            COM_PLAYERCAMP camp = (COM_PLAYERCAMP) ((int) pAgent.GetVariable((uint) 0xa87853c3));
            int num2 = ((ObjAgent) pAgent).GetHeroNumInRange(variable, this.opl_p1, camp);
            int num3 = 2;
            return ((num2 < num3) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

