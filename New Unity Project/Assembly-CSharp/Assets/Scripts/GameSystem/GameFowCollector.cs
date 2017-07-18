namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using CSProtocol;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class GameFowCollector
    {
        private List<BULLET_INFO>[] m_explorerBulletListInternal;
        public List<ACTOR_INFO> m_explorerPosList = new List<ACTOR_INFO>();
        private List<VirtualParticleAttachContext> VirtualParentParticles_ = new List<VirtualParticleAttachContext>();

        public void AddVirtualParentParticle(GameObject inParObj, PoolObjHandle<ActorRoot> inAttachActor, bool inUseShape)
        {
            if (inParObj != null)
            {
                this.VirtualParentParticles_.Add(new VirtualParticleAttachContext(inParObj, inAttachActor, inUseShape));
            }
        }

        public void ClearExplorerBulletList()
        {
            int length = this.m_explorerBulletList.Length;
            for (int i = 0; i < length; i++)
            {
                List<BULLET_INFO> list = this.m_explorerBulletList[i];
                int count = list.Count;
                for (int j = 0; j < count; j++)
                {
                    list[j].Release();
                }
                list.Clear();
            }
        }

        public void ClearExplorerPosList()
        {
            int count = this.m_explorerPosList.Count;
            for (int i = 0; i < count; i++)
            {
                this.m_explorerPosList[i].Release();
            }
            this.m_explorerPosList.Clear();
        }

        private void ClearVirtualParentParticles()
        {
            this.VirtualParentParticles_.Clear();
        }

        public void CollectExplorer(bool bForce)
        {
            GameObjMgr instance = Singleton<GameObjMgr>.instance;
            GameFowManager manager = Singleton<GameFowManager>.instance;
            uint num = Singleton<FrameSynchr>.instance.CurFrameNum % manager.InterpolateFrameInterval;
            uint num2 = Singleton<FrameSynchr>.instance.CurFrameNum % manager.InterpolateFrameIntervalBullet;
            uint num3 = Singleton<FrameSynchr>.instance.CurFrameNum % manager.InterpolateFrameIntervalHero;
            this.ClearExplorerPosList();
            int count = instance.GameActors.Count;
            for (int i = 0; i < count; i++)
            {
                PoolObjHandle<ActorRoot> handle = instance.GameActors[i];
                if (handle == 0)
                {
                    continue;
                }
                ActorRoot root = handle.handle;
                ActorTypeDef actorType = root.TheActorMeta.ActorType;
                if (actorType == ActorTypeDef.Actor_Type_Hero)
                {
                    if (((root.ObjID % manager.InterpolateFrameIntervalHero) == num3) || bForce)
                    {
                        goto Label_00E0;
                    }
                    continue;
                }
                if (((root.ObjID % manager.InterpolateFrameInterval) != num) && !bForce)
                {
                    continue;
                }
            Label_00E0:
                if ((actorType != ActorTypeDef.Actor_Type_Organ) && (!root.ActorControl.IsDeadState || root.TheStaticData.TheBaseAttribute.DeadControl))
                {
                    VInt3 num6 = new VInt3(root.location.x, root.location.z, 0);
                    if (root.HorizonMarker != null)
                    {
                        int[] exposedCamps = root.HorizonMarker.GetExposedCamps();
                        ACTOR_INFO item = ClassObjPool<ACTOR_INFO>.Get();
                        item.camps = exposedCamps;
                        item.location = root.HorizonMarker.GetExposedPos();
                        this.m_explorerPosList.Add(item);
                    }
                }
            }
            this.ClearExplorerBulletList();
            for (int j = 1; j < 3; j++)
            {
                List<PoolObjHandle<ActorRoot>> campBullet = Singleton<GameObjMgr>.instance.GetCampBullet((COM_PLAYERCAMP) j);
                int num8 = campBullet.Count;
                for (int k = 0; k < num8; k++)
                {
                    PoolObjHandle<ActorRoot> handle2 = campBullet[k];
                    if (handle2 != 0)
                    {
                        ActorRoot root2 = handle2.handle;
                        BulletWrapper actorControl = root2.ActorControl as BulletWrapper;
                        if ((0 < actorControl.SightRadius) && (((root2.ObjID % manager.InterpolateFrameIntervalBullet) == num2) || bForce))
                        {
                            VInt3 num10 = new VInt3(root2.location.x, root2.location.z, 0);
                            BULLET_INFO bullet_info = ClassObjPool<BULLET_INFO>.Get();
                            bullet_info.radius = actorControl.SightRange;
                            bullet_info.location = num10;
                            this.m_explorerBulletList[j - 1].Add(bullet_info);
                        }
                    }
                }
            }
        }

        public List<BULLET_INFO> GetExplorerBulletList(COM_PLAYERCAMP inCamp)
        {
            return this.m_explorerBulletList[HorizonMarkerByFow.TranslateCampToIndex(inCamp)];
        }

        public void InitSurface()
        {
        }

        private static bool IsPointInCircularSector2(float cx, float cy, float ux, float uy, float squaredR, float cosTheta, float px, float py)
        {
            if (squaredR <= 0f)
            {
                return false;
            }
            float num = px - cx;
            float num2 = py - cy;
            float num3 = (num * num) + (num2 * num2);
            if (num3 > squaredR)
            {
                return false;
            }
            float num4 = (num * ux) + (num2 * uy);
            if ((num4 >= 0f) && (cosTheta >= 0f))
            {
                return ((num4 * num4) > ((num3 * cosTheta) * cosTheta));
            }
            if ((num4 < 0f) && (cosTheta < 0f))
            {
                return ((num4 * num4) < ((num3 * cosTheta) * cosTheta));
            }
            return (num4 >= 0f);
        }

        public void RemoveVirtualParentParticle(GameObject inParObj)
        {
            int count = this.VirtualParentParticles_.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    VirtualParticleAttachContext context = this.VirtualParentParticles_[i];
                    if (context.VirtualParticle == inParObj)
                    {
                        this.VirtualParentParticles_.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public static bool SetObjVisibleByFow(GameObject obj, GameFowManager fowMgr, COM_PLAYERCAMP inHostCamp)
        {
            if (obj == null)
            {
                return false;
            }
            VInt3 worldLoc = obj.get_transform().get_position();
            worldLoc = new VInt3(worldLoc.x, worldLoc.z, 0);
            bool flag = fowMgr.IsSurfaceCellVisible(worldLoc, inHostCamp);
            if (flag)
            {
                obj.SetLayer("Actor", "Particles", true);
                return flag;
            }
            obj.SetLayer("Hide", true);
            return flag;
        }

        public static bool SetObjVisibleByFowAttached(GameObject obj, GameFowManager fowMgr, COM_PLAYERCAMP inHostCamp, PoolObjHandle<ActorRoot> inAttachActor)
        {
            if (obj == null)
            {
                return false;
            }
            bool flag = false;
            if (inAttachActor != 0)
            {
                VInt3 location = inAttachActor.handle.location;
                location = new VInt3(location.x, location.z, 0);
                flag = fowMgr.IsSurfaceCellVisible(location, inHostCamp);
                if (flag)
                {
                    obj.SetLayer("Actor", "Particles", true);
                    return flag;
                }
                obj.SetLayer("Hide", true);
                return flag;
            }
            return SetObjVisibleByFow(obj, fowMgr, inHostCamp);
        }

        public static bool SetObjWithColVisibleByFow(PoolObjHandle<ActorRoot> inActor, GameFowManager fowMgr, COM_PLAYERCAMP inHostCamp)
        {
            if (inActor == 0)
            {
                return false;
            }
            ActorRoot handle = inActor.handle;
            VCollisionShape shape = handle.shape;
            if (shape == null)
            {
                return SetObjVisibleByFow(handle.gameObject, fowMgr, inHostCamp);
            }
            VInt3 location = handle.location;
            location = new VInt3(location.x, location.z, 0);
            bool flag = fowMgr.IsSurfaceCellVisible(location, inHostCamp);
            if (flag)
            {
                handle.gameObject.SetLayer("Actor", "Particles", true);
                return flag;
            }
            flag = shape.AcceptFowVisibilityCheck(inHostCamp, fowMgr);
            if (flag)
            {
                handle.gameObject.SetLayer("Actor", "Particles", true);
                return flag;
            }
            handle.gameObject.SetLayer("Hide", true);
            return flag;
        }

        public static bool SetObjWithColVisibleByFowAttached(GameObject inObj, GameFowManager fowMgr, COM_PLAYERCAMP inHostCamp, PoolObjHandle<ActorRoot> inAttachActor)
        {
            if (inObj == null)
            {
                return false;
            }
            if ((inAttachActor == 0) || (inAttachActor.handle.shape == null))
            {
                return SetObjVisibleByFow(inObj, fowMgr, inHostCamp);
            }
            VCollisionShape shape = inAttachActor.handle.shape;
            VInt3 location = inAttachActor.handle.location;
            location = new VInt3(location.x, location.z, 0);
            bool flag = fowMgr.IsSurfaceCellVisible(location, inHostCamp);
            if (flag)
            {
                inObj.SetLayer("Actor", "Particles", true);
                return flag;
            }
            flag = shape.AcceptFowVisibilityCheck(inHostCamp, fowMgr);
            if (flag)
            {
                inObj.SetLayer("Actor", "Particles", true);
                return flag;
            }
            inObj.SetLayer("Hide", true);
            return flag;
        }

        public void UninitSurface()
        {
            this.ClearExplorerPosList();
            this.ClearExplorerBulletList();
            this.ClearVirtualParentParticles();
        }

        public void UpdateFowVisibility(bool bForce)
        {
            GameObjMgr instance = Singleton<GameObjMgr>.instance;
            GameFowManager fowMgr = Singleton<GameFowManager>.instance;
            COM_PLAYERCAMP playerCamp = Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp;
            uint num = Singleton<FrameSynchr>.instance.CurFrameNum % fowMgr.InterpolateFrameInterval;
            uint num2 = Singleton<FrameSynchr>.instance.CurFrameNum % fowMgr.InterpolateFrameIntervalBullet;
            uint num3 = Singleton<FrameSynchr>.instance.CurFrameNum % fowMgr.InterpolateFrameIntervalHero;
            List<PoolObjHandle<ActorRoot>> gameActors = instance.GameActors;
            int count = gameActors.Count;
            for (int i = 0; i < count; i++)
            {
                if (gameActors[i] == 0)
                {
                    continue;
                }
                PoolObjHandle<ActorRoot> handle2 = gameActors[i];
                ActorRoot root = handle2.handle;
                ActorTypeDef actorType = root.TheActorMeta.ActorType;
                if (actorType == ActorTypeDef.Actor_Type_Hero)
                {
                    if (((root.ObjID % fowMgr.InterpolateFrameIntervalHero) == num3) || bForce)
                    {
                        goto Label_00F6;
                    }
                    continue;
                }
                if (((root.ObjID % fowMgr.InterpolateFrameInterval) != num) && !bForce)
                {
                    continue;
                }
            Label_00F6:
                if ((actorType != ActorTypeDef.Actor_Type_Organ) && (!root.ActorControl.IsDeadState || root.TheStaticData.TheBaseAttribute.DeadControl))
                {
                    VInt3 worldLoc = new VInt3(root.location.x, root.location.z, 0);
                    switch (actorType)
                    {
                        case ActorTypeDef.Actor_Type_Hero:
                        case ActorTypeDef.Actor_Type_Monster:
                        {
                            bool bSet = fowMgr.QueryAttr(root.location) == FieldObj.EViewBlockType.Grass;
                            root.HorizonMarker.SetTranslucentMark(HorizonConfig.HideMark.Jungle, bSet, false);
                            if ((root.HudControl != null) && (root.HudControl.HasStatus(StatusHudType.InJungle) != bSet))
                            {
                                if (bSet)
                                {
                                    root.HudControl.ShowStatus(StatusHudType.InJungle);
                                }
                                else
                                {
                                    root.HudControl.HideStatus(StatusHudType.InJungle);
                                }
                            }
                            break;
                        }
                    }
                    for (int m = 1; m < 3; m++)
                    {
                        COM_PLAYERCAMP targetCamp = (COM_PLAYERCAMP) m;
                        if (targetCamp != root.TheActorMeta.ActorCamp)
                        {
                            root.HorizonMarker.SetHideMark(targetCamp, HorizonConfig.HideMark.Jungle, !fowMgr.IsSurfaceCellVisibleConsiderNeighbor(worldLoc, targetCamp));
                        }
                    }
                }
            }
            Dictionary<int, ShenFuObjects>.Enumerator enumerator = Singleton<ShenFuSystem>.instance._shenFuTriggerPool.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<int, ShenFuObjects> current = enumerator.Current;
                if ((((long) current.Key) % ((ulong) fowMgr.InterpolateFrameInterval)) == num)
                {
                    KeyValuePair<int, ShenFuObjects> pair2 = enumerator.Current;
                    SetObjVisibleByFow(pair2.Value.ShenFu, fowMgr, playerCamp);
                }
            }
            for (int j = 0; j < 3; j++)
            {
                if (j != playerCamp)
                {
                    List<PoolObjHandle<ActorRoot>> campBullet = instance.GetCampBullet((COM_PLAYERCAMP) j);
                    int num10 = campBullet.Count;
                    for (int n = 0; n < num10; n++)
                    {
                        PoolObjHandle<ActorRoot> inActor = campBullet[n];
                        if (inActor != 0)
                        {
                            ActorRoot handle = inActor.handle;
                            BulletWrapper actorControl = handle.ActorControl as BulletWrapper;
                            if (actorControl.m_bVisibleByFow && ((handle.ObjID % fowMgr.InterpolateFrameIntervalBullet) == num2))
                            {
                                if (actorControl.m_bVisibleByShape)
                                {
                                    actorControl.UpdateSubParObjVisibility(SetObjWithColVisibleByFow(inActor, fowMgr, playerCamp));
                                }
                                else
                                {
                                    actorControl.UpdateSubParObjVisibility(SetObjVisibleByFow(handle.gameObject, fowMgr, playerCamp));
                                }
                            }
                        }
                    }
                }
            }
            int num12 = this.VirtualParentParticles_.Count;
            for (int k = 0; k < num12; k++)
            {
                VirtualParticleAttachContext context = this.VirtualParentParticles_[k];
                VirtualParticleAttachContext context2 = this.VirtualParentParticles_[k];
                GameObject virtualParticle = context2.VirtualParticle;
                if (virtualParticle != null)
                {
                    int instanceID = virtualParticle.GetInstanceID();
                    if (instanceID < 0)
                    {
                        instanceID = -instanceID;
                    }
                    if ((((long) instanceID) % ((ulong) fowMgr.InterpolateFrameIntervalBullet)) == num)
                    {
                        if (context.bUseShape)
                        {
                            VirtualParticleAttachContext context3 = this.VirtualParentParticles_[k];
                            SetObjWithColVisibleByFowAttached(virtualParticle, fowMgr, playerCamp, context3.AttachActor);
                        }
                        else
                        {
                            VirtualParticleAttachContext context4 = this.VirtualParentParticles_[k];
                            SetObjVisibleByFowAttached(virtualParticle, fowMgr, playerCamp, context4.AttachActor);
                        }
                    }
                }
            }
        }

        public static bool VisitFowVisibilityCheck(VCollisionBox box, PoolObjHandle<ActorRoot> inActor, COM_PLAYERCAMP inHostCamp, GameFowManager fowMgr)
        {
            VInt2 zero = VInt2.zero;
            if (fowMgr.WorldPosToGrid(new VInt3(box.WorldPos.x, box.WorldPos.z, 0), out zero.x, out zero.y))
            {
                int num2;
                FieldObj pFieldObj = fowMgr.m_pFieldObj;
                int gridUnits = 0;
                pFieldObj.UnrealToGridX(box.WorldExtends.x, out num2);
                pFieldObj.UnrealToGridX(box.WorldExtends.z, out gridUnits);
                int num4 = zero.x - num2;
                num4 = Math.Max(0, num4);
                int num5 = zero.x + num2;
                num5 = Math.Min(num5, pFieldObj.NumX - 1);
                int num6 = zero.y - gridUnits;
                num6 = Math.Max(0, num6);
                int num7 = zero.y + gridUnits;
                num7 = Math.Min(num7, pFieldObj.NumY - 1);
                for (int i = num4; i <= num5; i++)
                {
                    for (int j = num6; j <= num7; j++)
                    {
                        if (Singleton<GameFowManager>.instance.IsVisible(i, j, inHostCamp))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool VisitFowVisibilityCheck(VCollisionCylinderSector cylinder, PoolObjHandle<ActorRoot> inActor, COM_PLAYERCAMP inHostCamp, GameFowManager fowMgr)
        {
            VInt2 zero = VInt2.zero;
            if (fowMgr.WorldPosToGrid(new VInt3(cylinder.WorldPos.x, cylinder.WorldPos.z, 0), out zero.x, out zero.y))
            {
                float radius = cylinder.Radius;
                radius *= 0.001f;
                radius *= radius;
                Vector3 worldPos = (Vector3) cylinder.WorldPos;
                float degree = cylinder.Degree;
                degree *= 0.5f;
                degree = Mathf.Cos(degree);
                Vector3 vector2 = (Vector3) cylinder.WorldPos;
                Vector3 forward = (Vector3) inActor.handle.forward;
                FieldObj pFieldObj = fowMgr.m_pFieldObj;
                int gridUnits = 0;
                pFieldObj.UnrealToGridX(cylinder.Radius, out gridUnits);
                int num5 = zero.x - gridUnits;
                num5 = Math.Max(0, num5);
                int num6 = zero.x + gridUnits;
                num6 = Math.Min(num6, pFieldObj.NumX - 1);
                int num7 = zero.y - gridUnits;
                num7 = Math.Max(0, num7);
                int num8 = zero.y + gridUnits;
                num8 = Math.Min(num8, pFieldObj.NumY - 1);
                for (int i = num5; i <= num6; i++)
                {
                    for (int j = num7; j <= num8; j++)
                    {
                        if (Singleton<GameFowManager>.instance.IsVisible(i, j, inHostCamp) && IsPointInCircularSector2(worldPos.x, worldPos.z, forward.x, forward.z, radius, degree, vector2.x, vector2.z))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool VisitFowVisibilityCheck(VCollisionSphere sphere, PoolObjHandle<ActorRoot> inActor, COM_PLAYERCAMP inHostCamp, GameFowManager fowMgr)
        {
            VInt2 zero = VInt2.zero;
            if (fowMgr.WorldPosToGrid(new VInt3(sphere.WorldPos.x, sphere.WorldPos.z, 0), out zero.x, out zero.y))
            {
                int gridUnits = 0;
                fowMgr.m_pFieldObj.UnrealToGridX(sphere.WorldRadius, out gridUnits);
                for (int i = -gridUnits; i <= gridUnits; i++)
                {
                    for (int j = -gridUnits; j <= gridUnits; j++)
                    {
                        VInt2 num5 = new VInt2(i, j);
                        VInt2 num6 = zero + num5;
                        if (fowMgr.IsInsideSurface(num6.x, num6.y) && ((num5.sqrMagnitude <= (gridUnits * gridUnits)) && Singleton<GameFowManager>.instance.IsVisible(num6.x, num6.y, inHostCamp)))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private List<BULLET_INFO>[] m_explorerBulletList
        {
            get
            {
                if (this.m_explorerBulletListInternal == null)
                {
                    this.m_explorerBulletListInternal = new List<BULLET_INFO>[2];
                    int length = this.m_explorerBulletListInternal.Length;
                    for (int i = 0; i < length; i++)
                    {
                        this.m_explorerBulletListInternal[i] = new List<BULLET_INFO>();
                    }
                }
                return this.m_explorerBulletListInternal;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct VirtualParticleAttachContext
        {
            public GameObject VirtualParticle;
            public PoolObjHandle<ActorRoot> AttachActor;
            public bool bUseShape;
            public VirtualParticleAttachContext(GameObject inParObj, PoolObjHandle<ActorRoot> inAttachActor, bool inUseShape)
            {
                this.VirtualParticle = inParObj;
                this.AttachActor = inAttachActor;
                this.bUseShape = inUseShape;
            }
        }
    }
}

