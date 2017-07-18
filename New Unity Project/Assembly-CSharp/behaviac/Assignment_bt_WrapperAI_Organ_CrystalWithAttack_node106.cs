namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using CSProtocol;

    internal class Assignment_bt_WrapperAI_Organ_CrystalWithAttack_node106 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            COM_PLAYERCAMP myCamp = ((ObjAgent) pAgent).GetMyCamp();
            pAgent.SetVariable<COM_PLAYERCAMP>("p_camp", myCamp, 0xa87853c3);
            return status;
        }
    }
}

