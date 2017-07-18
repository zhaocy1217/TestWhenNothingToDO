namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Common;
    using System;
    using UnityEngine;

    public class TriggerActionSpawn : TriggerActionBase
    {
        public TriggerActionSpawn(TriggerActionWrapper inWrapper, int inTriggerId) : base(inWrapper, inTriggerId)
        {
        }

        public override RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
        {
            foreach (GameObject obj2 in base.RefObjList)
            {
                if (obj2 != null)
                {
                    SpawnGroup component = obj2.GetComponent<SpawnGroup>();
                    if (component != null)
                    {
                        component.TriggerStartUp();
                    }
                }
            }
            return null;
        }
    }
}

