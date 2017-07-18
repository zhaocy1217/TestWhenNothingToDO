namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Common;
    using System;
    using UnityEngine;

    public class TriggerActionActivator : TriggerActionBase
    {
        public TriggerActionActivator(TriggerActionWrapper inWrapper, int inTriggerId) : base(inWrapper, inTriggerId)
        {
        }

        public override RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
        {
            GameObject[] refObjList = base.RefObjList;
            if ((refObjList != null) && (refObjList.Length > 0))
            {
                foreach (GameObject obj2 in refObjList)
                {
                    if (obj2 != null)
                    {
                        obj2.CustomSetActive(base.bEnable);
                        FuncRegion component = obj2.GetComponent<FuncRegion>();
                        if (component != null)
                        {
                            if (base.bEnable)
                            {
                                component.Startup();
                            }
                            else
                            {
                                component.Stop();
                            }
                        }
                    }
                }
            }
            if (base.bSrc && (src != 0))
            {
                src.handle.gameObject.CustomSetActive(base.bEnable);
                FuncRegion region2 = src.handle.gameObject.GetComponent<FuncRegion>();
                if (region2 != null)
                {
                    if (base.bEnable)
                    {
                        region2.Startup();
                    }
                    else
                    {
                        region2.Stop();
                    }
                }
            }
            if (base.bAtker && (atker != 0))
            {
                atker.handle.gameObject.CustomSetActive(base.bEnable);
                FuncRegion region3 = atker.handle.gameObject.GetComponent<FuncRegion>();
                if (region3 != null)
                {
                    if (base.bEnable)
                    {
                        region3.Startup();
                    }
                    else
                    {
                        region3.Stop();
                    }
                }
            }
            return null;
        }

        public override void TriggerLeave(PoolObjHandle<ActorRoot> src, ITrigger inTrigger)
        {
            if (base.bStopWhenLeaving)
            {
                GameObject[] refObjList = base.RefObjList;
                if ((refObjList != null) && (refObjList.Length > 0))
                {
                    foreach (GameObject obj2 in refObjList)
                    {
                        if (obj2 != null)
                        {
                            obj2.CustomSetActive(!base.bEnable);
                        }
                    }
                }
                if (base.bSrc && (src != 0))
                {
                    src.handle.gameObject.CustomSetActive(!base.bEnable);
                }
                if (base.bAtker && (inTrigger != null))
                {
                    inTrigger.GetTriggerObj().CustomSetActive(!base.bEnable);
                }
            }
        }
    }
}

