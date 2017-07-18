namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class MiniMapTrack_3DUI
    {
        private ListView<InnerData> m_innerDatas = new ListLinqView<InnerData>();

        public void Clear()
        {
            for (int i = 0; i < this.m_innerDatas.Count; i++)
            {
                InnerData data = this.m_innerDatas[i];
                if (data != null)
                {
                    data.Recyle();
                }
            }
        }

        private InnerData GetCachedNoUsedInnerData()
        {
            for (int i = 0; i < this.m_innerDatas.Count; i++)
            {
                InnerData data = this.m_innerDatas[i];
                if ((data != null) && (((data.objID == 0) && (data.small_track == null)) && (data.big_track == null)))
                {
                    return data;
                }
            }
            return null;
        }

        private InnerData GetObjIDUsedInnerData(uint objID)
        {
            for (int i = 0; i < this.m_innerDatas.Count; i++)
            {
                InnerData data = this.m_innerDatas[i];
                if ((data != null) && (data.objID == objID))
                {
                    return data;
                }
            }
            return null;
        }

        public static void Prepare(PoolObjHandle<ActorRoot> actorHandle, string iconPath)
        {
            MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
            if ((theMinimapSys != null) && (theMinimapSys.MMiniMapTrack_3Dui != null))
            {
                theMinimapSys.MMiniMapTrack_3Dui.Prepare_Imp(actorHandle, iconPath);
            }
        }

        public InnerData Prepare_Imp(PoolObjHandle<ActorRoot> actorHandle, string iconPath)
        {
            InnerData cachedNoUsedInnerData = this.GetCachedNoUsedInnerData();
            if (cachedNoUsedInnerData == null)
            {
                cachedNoUsedInnerData = new InnerData();
                this.m_innerDatas.Add(cachedNoUsedInnerData);
            }
            cachedNoUsedInnerData.SetData(actorHandle.handle.ObjID, iconPath, actorHandle.handle.IsHostCamp());
            return cachedNoUsedInnerData;
        }

        public static void Recyle(uint objid)
        {
            MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
            if ((theMinimapSys != null) && (theMinimapSys.MMiniMapTrack_3Dui != null))
            {
                theMinimapSys.MMiniMapTrack_3Dui.Recyle_Imp(objid);
            }
        }

        public void Recyle_Imp(uint objid)
        {
            InnerData objIDUsedInnerData = this.GetObjIDUsedInnerData(objid);
            if (objIDUsedInnerData != null)
            {
                objIDUsedInnerData.Recyle();
            }
        }

        public static void SetTrackPosition(PoolObjHandle<ActorRoot> actorHandle, string iconPath)
        {
            MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
            if (theMinimapSys != null)
            {
                if (Singleton<WatchController>.GetInstance().IsWatching)
                {
                    if (theMinimapSys.MMiniMapTrack_3Dui != null)
                    {
                        theMinimapSys.MMiniMapTrack_3Dui.SetTrackPosition_Imp(actorHandle, iconPath);
                    }
                }
                else if (actorHandle.handle.IsHostCamp() && (theMinimapSys.MMiniMapTrack_3Dui != null))
                {
                    theMinimapSys.MMiniMapTrack_3Dui.SetTrackPosition_Imp(actorHandle, iconPath);
                }
            }
        }

        public void SetTrackPosition_Imp(PoolObjHandle<ActorRoot> actorHandle, string iconPath)
        {
            if (actorHandle != 0)
            {
                InnerData objIDUsedInnerData = this.GetObjIDUsedInnerData(actorHandle.handle.ObjID);
                if (objIDUsedInnerData == null)
                {
                    objIDUsedInnerData = this.Prepare_Imp(actorHandle, iconPath);
                }
                if (objIDUsedInnerData != null)
                {
                    objIDUsedInnerData.UpdateTransform(actorHandle);
                }
                else
                {
                    DebugHelper.Assert(false, "--- SetTrackPosition_Imp InnerData is null, check it....");
                }
            }
        }

        public class InnerData
        {
            [CompilerGenerated]
            private float <big_track_height>k__BackingField;
            [CompilerGenerated]
            private float <big_track_width>k__BackingField;
            [CompilerGenerated]
            private float <small_track_height>k__BackingField;
            [CompilerGenerated]
            private float <small_track_width>k__BackingField;
            public GameObject big_track = null;
            private bool bUpdate = true;
            private float exten = 20f;
            public VInt3 forward = VInt3.zero;
            public uint objID = 0;
            public GameObject small_track = null;

            private void _updateNodePosition(GameObject node, PoolObjHandle<ActorRoot> actorRoot, bool bSmallMap)
            {
                if ((actorRoot != 0) && (node != null))
                {
                    MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
                    if (theMinimapSys != null)
                    {
                        float num;
                        float num2;
                        Vector3 location = (Vector3) actorRoot.handle.location;
                        Vector2 vector2 = !bSmallMap ? theMinimapSys.GetBMFianlScreenPos() : theMinimapSys.GetMMFianlScreenPos();
                        Vector2 vector3 = !bSmallMap ? theMinimapSys.bmFinalScreenSize : theMinimapSys.mmFinalScreenSize;
                        node.get_transform().set_position(MiniMapSysUT.Set3DUIWorldPos_ByScreenPoint(ref location, bSmallMap, out num, out num2));
                        if (!this.IsRectInside(num, num2, this.small_track_width, this.small_track_height, vector2.x, vector2.y, vector3.x, vector3.y))
                        {
                            this.SetUpdateAble(false);
                        }
                    }
                }
            }

            public void _updateRotation(PoolObjHandle<ActorRoot> actorRoot)
            {
                if ((actorRoot != 0) && (this.forward != actorRoot.handle.forward))
                {
                    float num = (Mathf.Atan2((float) actorRoot.handle.forward.z, (float) actorRoot.handle.forward.x) * 57.29578f) - 90f;
                    if (Singleton<BattleLogic>.instance.GetCurLvelContext().m_isCameraFlip)
                    {
                        num -= 180f;
                    }
                    Quaternion quaternion = Quaternion.AngleAxis(num, Vector3.get_forward());
                    if ((this.small_track != null) && (this.small_track.get_transform() != null))
                    {
                        this.small_track.get_transform().set_rotation(quaternion);
                    }
                    if ((this.big_track != null) && (this.big_track.get_transform() != null))
                    {
                        this.big_track.get_transform().set_rotation(quaternion);
                    }
                    this.forward = actorRoot.handle.forward;
                }
            }

            private GameObject getContainerElement(bool bSmallMap, string iconPath, bool bSameCamp)
            {
                MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
                if (theMinimapSys == null)
                {
                    return null;
                }
                GameObject obj2 = !bSmallMap ? theMinimapSys.bmpcTrack : theMinimapSys.mmpcTrack;
                if (obj2 == null)
                {
                    return null;
                }
                GameObject mapTrackObj = MiniMapSysUT.GetMapTrackObj(iconPath, bSmallMap);
                if (mapTrackObj != null)
                {
                    Sprite3D component = mapTrackObj.GetComponent<Sprite3D>();
                    if (component != null)
                    {
                        if (bSmallMap)
                        {
                            this.small_track_width = (component.textureWidth * mapTrackObj.get_transform().get_localScale().x) * Sprite3D.Ratio();
                            this.small_track_height = (component.textureHeight * mapTrackObj.get_transform().get_localScale().y) * Sprite3D.Ratio();
                        }
                        else
                        {
                            this.big_track_width = (component.textureWidth * mapTrackObj.get_transform().get_localScale().x) * Sprite3D.Ratio();
                            this.big_track_height = (component.textureHeight * mapTrackObj.get_transform().get_localScale().y) * Sprite3D.Ratio();
                        }
                    }
                }
                mapTrackObj.CustomSetActive(true);
                return mapTrackObj;
            }

            private bool IsRectInside(float x, float y, float innerWidth, float innerHeight, float outerX, float outerY, float outerWidth, float outerHeight)
            {
                if (x < ((outerX - (outerWidth * 0.5f)) - (this.exten * Sprite3D.Ratio())))
                {
                    return false;
                }
                if (x > (outerX + (outerWidth * 0.5f)))
                {
                    return false;
                }
                if (y > (outerY + (outerHeight * 0.5f)))
                {
                    return false;
                }
                if (y < ((outerY - (outerHeight * 0.5f)) - (this.exten * Sprite3D.Ratio())))
                {
                    return false;
                }
                return true;
            }

            public void Recyle()
            {
                this.bUpdate = true;
                this.forward = VInt3.zero;
                this.objID = 0;
                MiniMapSysUT.RecycleMapGameObject(this.small_track);
                MiniMapSysUT.RecycleMapGameObject(this.big_track);
                this.small_track = null;
                this.big_track = null;
            }

            public void SetData(uint objID, string iconPath, bool bSameCamp)
            {
                this.forward = VInt3.zero;
                this.objID = objID;
                iconPath = string.Format("{0}_{1}", iconPath, !bSameCamp ? "red" : "blue");
                iconPath = string.Format("{0}.prefab", iconPath);
                this.small_track = this.getContainerElement(true, iconPath, bSameCamp);
                this.big_track = this.getContainerElement(false, iconPath, bSameCamp);
                this.SetUpdateAble(true);
                DebugHelper.Assert(this.small_track != null, string.Concat(new object[] { "---MiniMapTrackEX small_track is null, iconID:", iconPath, ",bSameCamp:", bSameCamp }));
                DebugHelper.Assert(this.big_track != null, string.Concat(new object[] { "---MiniMapTrackEX big_track is null, iconID:", iconPath, ",bSameCamp:", bSameCamp }));
            }

            public void SetUpdateAble(bool bShow)
            {
                this.bUpdate = bShow;
                if (this.small_track != null)
                {
                    this.small_track.CustomSetActive(bShow);
                }
                if (this.big_track != null)
                {
                    this.big_track.CustomSetActive(bShow);
                }
            }

            public void UpdateTransform(PoolObjHandle<ActorRoot> actorRoot)
            {
                if (this.bUpdate)
                {
                    this._updateNodePosition(this.small_track, actorRoot, true);
                    this._updateNodePosition(this.big_track, actorRoot, false);
                    this._updateRotation(actorRoot);
                }
            }

            public float big_track_height
            {
                [CompilerGenerated]
                get
                {
                    return this.<big_track_height>k__BackingField;
                }
                [CompilerGenerated]
                set
                {
                    this.<big_track_height>k__BackingField = value;
                }
            }

            public float big_track_width
            {
                [CompilerGenerated]
                get
                {
                    return this.<big_track_width>k__BackingField;
                }
                [CompilerGenerated]
                set
                {
                    this.<big_track_width>k__BackingField = value;
                }
            }

            public float small_track_height
            {
                [CompilerGenerated]
                get
                {
                    return this.<small_track_height>k__BackingField;
                }
                [CompilerGenerated]
                set
                {
                    this.<small_track_height>k__BackingField = value;
                }
            }

            public float small_track_width
            {
                [CompilerGenerated]
                get
                {
                    return this.<small_track_width>k__BackingField;
                }
                [CompilerGenerated]
                set
                {
                    this.<small_track_width>k__BackingField = value;
                }
            }
        }
    }
}

