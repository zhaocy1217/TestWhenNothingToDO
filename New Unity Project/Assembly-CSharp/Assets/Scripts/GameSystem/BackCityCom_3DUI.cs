namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;
    using UnityEngine;

    public class BackCityCom_3DUI
    {
        public static void HideBack2City(PoolObjHandle<ActorRoot> actorHandle)
        {
            MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
            if ((theMinimapSys != null) && (theMinimapSys.MMiniMapBackCityCom_3Dui != null))
            {
                theMinimapSys.MMiniMapBackCityCom_3Dui.HideBack2City_Imp(actorHandle);
            }
        }

        public void HideBack2City_Imp(PoolObjHandle<ActorRoot> actorHandle)
        {
            this.SetActorReturnCityVisible(actorHandle, false);
        }

        private void SetActorReturnCityVisible(PoolObjHandle<ActorRoot> actorHandle, bool bShow)
        {
            if (actorHandle != 0)
            {
                HudComponent3D hudControl = actorHandle.handle.HudControl;
                if (hudControl != null)
                {
                    if (hudControl.MapPointerSmall != null)
                    {
                        SetVisible(hudControl.MapPointerSmall, bShow);
                    }
                    if (hudControl.MapPointerBig != null)
                    {
                        SetVisible(hudControl.MapPointerBig, bShow);
                    }
                }
            }
        }

        public static void SetVisible(GameObject node, bool bShow)
        {
            Transform transform = node.get_transform().Find("ReturCity");
            if (transform != null)
            {
                transform.get_gameObject().CustomSetActive(bShow);
            }
        }

        public static void ShowBack2City(PoolObjHandle<ActorRoot> actorHandle)
        {
            MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
            if ((theMinimapSys != null) && (theMinimapSys.MMiniMapBackCityCom_3Dui != null))
            {
                theMinimapSys.MMiniMapBackCityCom_3Dui.ShowBack2City_Imp(actorHandle);
            }
        }

        public void ShowBack2City_Imp(PoolObjHandle<ActorRoot> actorHandle)
        {
            this.SetActorReturnCityVisible(actorHandle, true);
        }
    }
}

