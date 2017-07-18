namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Common;
    using System;

    public class TriggerActionNewbieForm : TriggerActionBase
    {
        public TriggerActionNewbieForm(TriggerActionWrapper inWrapper, int inTriggerId) : base(inWrapper, inTriggerId)
        {
        }

        public override RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
        {
            if ((base.EnterUniqueId > 0) && (base.EnterUniqueId < 0x22))
            {
                Singleton<CBattleGuideManager>.GetInstance().OpenFormShared((CBattleGuideManager.EBattleGuideFormType) base.EnterUniqueId, 0, true);
            }
            return null;
        }
    }
}

