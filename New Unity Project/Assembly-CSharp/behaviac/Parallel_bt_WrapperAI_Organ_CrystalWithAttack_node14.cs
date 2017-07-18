namespace behaviac
{
    using System;

    internal class Parallel_bt_WrapperAI_Organ_CrystalWithAttack_node14 : Parallel
    {
        public Parallel_bt_WrapperAI_Organ_CrystalWithAttack_node14()
        {
            base.m_failPolicy = FAILURE_POLICY.FAIL_ON_ALL;
            base.m_succeedPolicy = SUCCESS_POLICY.SUCCEED_ON_ALL;
            base.m_exitPolicy = EXIT_POLICY.EXIT_NONE;
            base.m_childFinishPolicy = CHILDFINISH_POLICY.CHILDFINISH_LOOP;
        }
    }
}

