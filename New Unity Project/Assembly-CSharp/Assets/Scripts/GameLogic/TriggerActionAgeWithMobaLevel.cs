namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.GameSystem;
    using System;
    using UnityEngine;

    public class TriggerActionAgeWithMobaLevel : TriggerActionAge
    {
        public TriggerActionAgeWithMobaLevel(TriggerActionWrapper inWrapper, int inTriggerId) : base(inWrapper, inTriggerId)
        {
        }

        protected override ListView<Action> PlayAgeActionShared(AreaEventTrigger.EActTiming inTiming, AreaEventTrigger.STimingAction[] inTimingActs, ActionStopDelegate inCallback, ListView<Action> outDuraActs, GameObject inSrc, GameObject inAtker)
        {
            ListView<Action> view = new ListView<Action>();
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                int iMobaLevel = masterRoleInfo.acntMobaInfo.iMobaLevel;
                if (inTimingActs.Length <= iMobaLevel)
                {
                    return view;
                }
                AreaEventTrigger.STimingAction action = inTimingActs[iMobaLevel];
                ActionStopDelegate delegate2 = null;
                if (inTiming == AreaEventTrigger.EActTiming.EnterDura)
                {
                    delegate2 = inCallback;
                }
                Action item = TriggerActionAge.PlayAgeActionShared(action.ActionName, action.HelperName, inSrc, inAtker, action.HelperIndex, inCallback);
                if (item != null)
                {
                    view.Add(item);
                    if (delegate2 != null)
                    {
                        outDuraActs.Add(item);
                    }
                }
            }
            return view;
        }
    }
}

