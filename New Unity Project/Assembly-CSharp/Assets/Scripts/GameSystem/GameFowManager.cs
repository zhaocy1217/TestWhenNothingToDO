namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class GameFowManager : Singleton<GameFowManager>
    {
        [CompilerGenerated]
        private GameFowCollector <m_collector>k__BackingField;
        [CompilerGenerated]
        private COM_PLAYERCAMP <m_hostPlayerCamp>k__BackingField;
        [CompilerGenerated]
        private FowLos <m_los>k__BackingField;
        [CompilerGenerated]
        private FieldObj <m_pFieldObj>k__BackingField;
        private byte[] ActorInfoDataExtra_;
        private IntPtr ActorInfoDataExtraPtr_ = IntPtr.Zero;
        private byte[] BulletInfoDataExtra_;
        private IntPtr BulletInfoDataExtraPtr_ = IntPtr.Zero;
        public const int DEBUG_DRAW_DELTA_Z = 0xaf0;
        public const byte FOW_ALPHA_MAX = 0xff;
        public const byte FOW_ALPHA_UNEXPLORED = 0;
        private const int HalfSizeShrink = 100;
        private byte[] m_commitBitMap;
        private uint m_gpuInterpFrameInterval = 8;
        private float m_gpuInterpReciprocal = 1f;
        public int m_halfSizeX;
        public int m_halfSizeY;
        private IntPtr m_tempCpyActorDataPtr = IntPtr.Zero;
        private IntPtr m_tempCpyBulletDataPtr = IntPtr.Zero;
        private int[] ms_Actor_Info_Data;
        private const int ms_actorInfoCnt = 80;
        private int[] ms_Bullet_Info_Data;
        private const int ms_bulletInfoCnt = 20;

        private void ComputeFow(COM_PLAYERCAMP camp)
        {
            DebugHelper.Assert(camp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID);
            if (camp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
            {
                this.CopyBulletInfoData(this.m_collector.GetExplorerBulletList(camp));
                ComputeFowByCamp(camp);
            }
        }

        [DllImport("SGameFowProject", CallingConvention=CallingConvention.Cdecl)]
        public static extern void ComputeFowByCamp(COM_PLAYERCAMP camp);
        [DllImport("SGameFowProject", CallingConvention=CallingConvention.Cdecl)]
        public static extern void ComputeFowByCampWithAutoDec(COM_PLAYERCAMP camp, bool bHostCamp);
        private void ComputeFowWithAutoDec(COM_PLAYERCAMP inCamp, bool bHostCamp)
        {
            if (inCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
            {
                this.CopyBulletInfoData(this.m_collector.GetExplorerBulletList(inCamp));
                ComputeFowByCampWithAutoDec(inCamp, bHostCamp);
            }
        }

        private void CopyActorInfoData(List<ACTOR_INFO> actorInfoList)
        {
            int dataSize = ACTOR_INFO.GetDataSize();
            int length = dataSize * 80;
            int dataSizeBytes = ACTOR_INFO.GetDataSizeBytes();
            int cb = dataSizeBytes * 80;
            if (this.ms_Actor_Info_Data == null)
            {
                this.ms_Actor_Info_Data = new int[length];
            }
            if (this.ActorInfoDataExtra_ == null)
            {
                this.ActorInfoDataExtra_ = new byte[cb];
            }
            int count = actorInfoList.Count;
            if (count > 80)
            {
                count = 80;
            }
            for (int i = 0; i < count; i++)
            {
                int index = i * dataSize;
                int num8 = HorizonMarkerByFow.TranslateCampToIndex(COM_PLAYERCAMP.COM_PLAYERCAMP_1);
                this.ms_Actor_Info_Data[index] = actorInfoList[i].location[num8].x;
                this.ms_Actor_Info_Data[index + 1] = actorInfoList[i].location[num8].y;
                this.ms_Actor_Info_Data[index + 4] = actorInfoList[i].camps[num8];
                int num9 = HorizonMarkerByFow.TranslateCampToIndex(COM_PLAYERCAMP.COM_PLAYERCAMP_2);
                this.ms_Actor_Info_Data[index + 2] = actorInfoList[i].location[num9].x;
                this.ms_Actor_Info_Data[index + 3] = actorInfoList[i].location[num9].y;
                this.ms_Actor_Info_Data[index + 5] = actorInfoList[i].camps[num9];
                int num10 = i * dataSizeBytes;
                this.ActorInfoDataExtra_[num10] = !actorInfoList[i].bDistOnly ? ((byte) 0) : ((byte) 1);
            }
            if (IntPtr.Zero == this.m_tempCpyActorDataPtr)
            {
                int num11 = length * 4;
                this.m_tempCpyActorDataPtr = Marshal.AllocHGlobal(num11);
            }
            if (IntPtr.Zero == this.ActorInfoDataExtraPtr_)
            {
                this.ActorInfoDataExtraPtr_ = Marshal.AllocHGlobal(cb);
            }
            if (count > 0)
            {
                Marshal.Copy(this.ms_Actor_Info_Data, 0, this.m_tempCpyActorDataPtr, length);
                Marshal.Copy(this.ActorInfoDataExtra_, 0, this.ActorInfoDataExtraPtr_, cb);
            }
            MemCopyActorInfoData(count, dataSize, this.m_tempCpyActorDataPtr, dataSizeBytes, this.ActorInfoDataExtraPtr_);
        }

        private void CopyBulletInfoData(List<BULLET_INFO> bulletInfoList)
        {
            int dataSize = BULLET_INFO.GetDataSize();
            int length = dataSize * 20;
            int dataSizeBytes = BULLET_INFO.GetDataSizeBytes();
            int cb = dataSizeBytes * 20;
            if (this.ms_Bullet_Info_Data == null)
            {
                this.ms_Bullet_Info_Data = new int[length];
            }
            if (this.BulletInfoDataExtra_ == null)
            {
                this.BulletInfoDataExtra_ = new byte[cb];
            }
            int count = bulletInfoList.Count;
            if (count > 20)
            {
                count = 20;
            }
            for (int i = 0; i < count; i++)
            {
                int index = i * dataSize;
                this.ms_Bullet_Info_Data[index] = bulletInfoList[i].location.x;
                this.ms_Bullet_Info_Data[index + 1] = bulletInfoList[i].location.y;
                this.ms_Bullet_Info_Data[index + 2] = bulletInfoList[i].radius;
                int num8 = i * dataSizeBytes;
                this.BulletInfoDataExtra_[num8] = !bulletInfoList[i].bDistOnly ? ((byte) 0) : ((byte) 1);
            }
            if (IntPtr.Zero == this.m_tempCpyBulletDataPtr)
            {
                int num9 = length * 4;
                this.m_tempCpyBulletDataPtr = Marshal.AllocHGlobal(num9);
            }
            if (IntPtr.Zero == this.BulletInfoDataExtraPtr_)
            {
                this.BulletInfoDataExtraPtr_ = Marshal.AllocHGlobal(cb);
            }
            if (count > 0)
            {
                Marshal.Copy(this.ms_Bullet_Info_Data, 0, this.m_tempCpyBulletDataPtr, length);
                Marshal.Copy(this.BulletInfoDataExtra_, 0, this.BulletInfoDataExtraPtr_, cb);
            }
            MemCopyBulletInfoData(count, dataSize, this.m_tempCpyBulletDataPtr, dataSizeBytes, this.BulletInfoDataExtraPtr_);
        }

        public void ForceUpdate(bool bSubmit)
        {
            this.m_collector.CollectExplorer(true);
            this.CopyActorInfoData(this.m_collector.m_explorerPosList);
            for (int i = 0; i < 3; i++)
            {
                if (i != 0)
                {
                    this.ComputeFow((COM_PLAYERCAMP) i);
                }
            }
            FogOfWar.CopyBitmap();
            if (bSubmit)
            {
                FogOfWar.CommitToMaterials();
            }
            if (bSubmit)
            {
                this.m_collector.UpdateFowVisibility(true);
            }
        }

        private void ForceUpdate(COM_PLAYERCAMP camp, bool bSubmit)
        {
            this.m_collector.CollectExplorer(true);
            this.CopyActorInfoData(this.m_collector.m_explorerPosList);
            this.ComputeFow(camp);
            FogOfWar.CopyBitmap();
            if (bSubmit)
            {
                FogOfWar.CommitToMaterials();
            }
            if (bSubmit)
            {
                this.m_collector.UpdateFowVisibility(true);
            }
        }

        [DllImport("SGameFowProject", CallingConvention=CallingConvention.Cdecl)]
        private static extern IntPtr GetCommitBitmapData();
        public byte[] GetCommitPixels()
        {
            Marshal.Copy(GetCommitBitmapData(), this.m_commitBitMap, 0, this.m_commitBitMap.Length);
            return this.m_commitBitMap;
        }

        public void GridToWorldPos(int inCellX, int inCellY, out VInt3 outWorldPos)
        {
            this.m_pFieldObj.LevelGrid.GridToWorldPos(inCellX, inCellY, out outWorldPos);
        }

        public override void Init()
        {
            base.Init();
            Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
            Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
        }

        [DllImport("SGameFowProject", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitGameFowMapData(int w, int h, int inUnexploredAlpha, int maxFowAlpha, uint inInterpolateFrameInterval, int inFakeSightRange, COM_PLAYERCAMP hostCamp);
        [DllImport("SGameFowProject", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitLevelGrid(int cellCnt, int iCellNumX, int iCellNumY, int iCellSizeX, int iCellSizeY, int iGridPosX, int iGridPosY);
        [DllImport("SGameFowProject", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitLevelGridCell(int index, int inBlockType, int inLightType);
        public void InitSurface(bool bDoInit, FieldObj inFieldObj, int inFakeSightRange)
        {
            this.UninitSurface();
            GC.Collect();
            this.m_pFieldObj = inFieldObj;
            this.m_halfSizeX = (this.m_pFieldObj.PaneX / 2) - 100;
            this.m_halfSizeY = (this.m_pFieldObj.PaneY / 2) - 100;
            DebugHelper.Assert(this.m_halfSizeX > 0);
            DebugHelper.Assert(this.m_halfSizeY > 0);
            if (Application.get_isPlaying())
            {
                Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
                DebugHelper.Assert(hostPlayer != null, "InitSurface hostPlayer is null");
                if (hostPlayer != null)
                {
                    this.m_hostPlayerCamp = hostPlayer.PlayerCamp;
                }
                this.SetCallBack();
                InitGameFowMapData(this.m_pFieldObj.NumX, this.m_pFieldObj.NumY, 0, 0xff, this.InterpolateFrameInterval, inFakeSightRange, this.m_hostPlayerCamp);
                this.m_commitBitMap = new byte[this.m_pFieldObj.NumX * this.m_pFieldObj.NumY];
                this.m_collector = new GameFowCollector();
                this.m_collector.InitSurface();
            }
            this.m_los = new FowLos();
            this.m_los.Init();
            this.m_gpuInterpFrameInterval = (uint) MonoSingleton<GlobalConfig>.instance.GPUInterpolateFrameInterval;
            this.m_gpuInterpReciprocal = 1f / ((float) this.m_gpuInterpFrameInterval);
        }

        [DllImport("SGameFowProject", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitSurfCell(int index, int xMin, int xMax, int yMin, int yMax, bool bValid);
        [DllImport("SGameFowProject", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitSurfCellsArray(int cellCnt);
        public static bool IsAreaPermanentLit(int x, int y, COM_PLAYERCAMP camp)
        {
            return Singleton<GameFowManager>.instance.m_pFieldObj.IsAreaPermanentLit(x, y, camp);
        }

        public static bool IsHostCamp(COM_PLAYERCAMP camp)
        {
            return (camp == Singleton<GameFowManager>.instance.m_hostPlayerCamp);
        }

        public bool IsInsideSurface(int sx, int sy)
        {
            return ((((sx >= 0) && (sy >= 0)) && (sx < this.m_pFieldObj.NumX)) && (sy < this.m_pFieldObj.NumY));
        }

        public bool IsSurfaceCellVisible(VInt3 worldLoc, COM_PLAYERCAMP camp)
        {
            int num;
            int outCellY = 0;
            if (!this.WorldPosToGrid(worldLoc, out num, out outCellY))
            {
                return false;
            }
            return this.IsVisible(num, outCellY, camp);
        }

        public bool IsSurfaceCellVisibleConsiderNeighbor(VInt3 worldLoc, COM_PLAYERCAMP camp)
        {
            VInt2 zero = VInt2.zero;
            if (!this.WorldPosToGrid(worldLoc, out zero.x, out zero.y))
            {
                return false;
            }
            FieldObj.SViewBlockAttr outAttr = new FieldObj.SViewBlockAttr();
            if (this.m_pFieldObj.QueryAttr(zero, out outAttr) && (outAttr.BlockType == 2))
            {
                VInt2 result = VInt2.zero;
                if (this.m_pFieldObj.FindNearestGrid(zero, worldLoc, FieldObj.EViewBlockType.Brick, 3, null, out result))
                {
                    zero = result;
                }
            }
            return this.IsVisible(zero.x, zero.y, camp);
        }

        public bool IsVisible(int x, int y, COM_PLAYERCAMP inCamp)
        {
            return IsVisibleDll(x, y, inCamp);
        }

        [DllImport("SGameFowProject", CallingConvention=CallingConvention.Cdecl)]
        private static extern bool IsVisibleDll(int w, int h, COM_PLAYERCAMP camp);
        public bool LoadPrecomputeData()
        {
            bool flag = false;
            if (Application.get_isPlaying() && (this.m_pFieldObj != null))
            {
                this.m_pFieldObj.m_fowCells = null;
                FOGameFowOfflineSerializer serializer = new FOGameFowOfflineSerializer();
                if (serializer.TryLoad(this.m_pFieldObj))
                {
                    flag = true;
                }
                this.m_pFieldObj.fowOfflineData = null;
            }
            return flag;
        }

        [DllImport("SGameFowProject", CallingConvention=CallingConvention.Cdecl)]
        public static extern void MemCopyActorInfoData(int actorCnt, int dataSize, IntPtr actorInfoData, int dataSizeExtra, IntPtr actorInfoDataExtra);
        [DllImport("SGameFowProject", CallingConvention=CallingConvention.Cdecl)]
        public static extern void MemCopyBulletInfoData(int bulletCnt, int dataSize, IntPtr bulletInfoData, int dataSizeExtra, IntPtr bulletInfoDataExtra);
        private void OnActorDead(ref GameDeadEventParam prm)
        {
            if (FogOfWar.enable)
            {
                PoolObjHandle<ActorRoot> src = prm.src;
                if ((src != 0) && (src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ))
                {
                    this.ResetBaseMapData(src.handle.TheActorMeta.ActorCamp, false);
                }
            }
        }

        public void OnStartFight()
        {
            if (FogOfWar.enable)
            {
                for (int i = 0; i < 3; i++)
                {
                    this.ResetBaseMapData((COM_PLAYERCAMP) i, true);
                }
            }
        }

        [DllImport("SGameFowProject", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PostInitGameFowMapData(IntPtr inVisDataPtr1, IntPtr inVisDataPtr2);
        public FieldObj.EViewBlockType QueryAttr(Vector3 inActorLoc)
        {
            return this.QueryAttr((VInt3) inActorLoc);
        }

        public FieldObj.EViewBlockType QueryAttr(VInt3 inActorLoc)
        {
            inActorLoc = new VInt3(inActorLoc.x, inActorLoc.z, 0);
            VInt2 zero = VInt2.zero;
            if (this.WorldPosToGrid(inActorLoc, out zero.x, out zero.y))
            {
                FieldObj.SViewBlockAttr outAttr = new FieldObj.SViewBlockAttr();
                if (this.m_pFieldObj.QueryAttr(zero, out outAttr))
                {
                    return (FieldObj.EViewBlockType) outAttr.BlockType;
                }
            }
            return FieldObj.EViewBlockType.None;
        }

        public void ResetBaseMapData(COM_PLAYERCAMP camp, bool bSubmit)
        {
            if (camp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
            {
                SyncPermanentToBaseData(camp, camp == this.m_hostPlayerCamp);
                this.UpdateBaseMapData(camp);
                SyncBaseDataToVisible(camp, camp == this.m_hostPlayerCamp);
                this.ForceUpdate(camp, bSubmit);
            }
        }

        public void ResetBaseMapDataAsync(COM_PLAYERCAMP camp)
        {
            if (camp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
            {
                SyncPermanentToBaseData(camp, camp == this.m_hostPlayerCamp);
                this.UpdateBaseMapData(camp);
                SyncBaseDataToVisible(camp, camp == this.m_hostPlayerCamp);
            }
        }

        public void ReviseWorldPosToCenter(VInt3 inWorldPos, out VInt3 outWorldPos)
        {
            this.m_pFieldObj.LevelGrid.ReviseWorldPosToCenter(inWorldPos, out outWorldPos);
        }

        [DllImport("SGameFowProject", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetBaseDataVisible(bool bVisible, int w, int h, COM_PLAYERCAMP camp, bool bHostCamp);
        public void SetCallBack()
        {
            SetIsAreaPermanentLitCallBack(new IsAreaPermanentLitCallBack(GameFowManager.IsAreaPermanentLit));
        }

        [DllImport("SGameFowProject", CallingConvention=CallingConvention.Cdecl)]
        private static extern void SetIsAreaPermanentLitCallBack([MarshalAs(UnmanagedType.FunctionPtr)] IsAreaPermanentLitCallBack callback);
        [DllImport("SGameFowProject", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetSurfCellData(int index, IntPtr data);
        [DllImport("SGameFowProject", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetSurfCellVisible(int index, int x, int y, COM_PLAYERCAMP camp, bool bVisible);
        [DllImport("SGameFowProject", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetVisible(bool bVisible, int w, int h, COM_PLAYERCAMP camp, bool bHostCamp);
        [DllImport("SGameFowProject", CallingConvention=CallingConvention.Cdecl)]
        private static extern void SyncBaseDataToVisible(COM_PLAYERCAMP camp, bool bHostCamp);
        [DllImport("SGameFowProject", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SyncPermanentToBaseData(COM_PLAYERCAMP camp, bool bHostCamp);
        public override void UnInit()
        {
            base.UnInit();
            Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
        }

        private void UninitCollectorDataInfo()
        {
            this.ms_Actor_Info_Data = null;
            this.ActorInfoDataExtra_ = null;
            this.ms_Bullet_Info_Data = null;
            this.BulletInfoDataExtra_ = null;
            if (this.m_tempCpyActorDataPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(this.m_tempCpyActorDataPtr);
                this.m_tempCpyActorDataPtr = IntPtr.Zero;
            }
            if (this.ActorInfoDataExtraPtr_ != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(this.ActorInfoDataExtraPtr_);
                this.ActorInfoDataExtraPtr_ = IntPtr.Zero;
            }
            if (this.m_tempCpyBulletDataPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(this.m_tempCpyBulletDataPtr);
                this.m_tempCpyBulletDataPtr = IntPtr.Zero;
            }
            if (this.BulletInfoDataExtraPtr_ != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(this.BulletInfoDataExtraPtr_);
                this.BulletInfoDataExtraPtr_ = IntPtr.Zero;
            }
        }

        [DllImport("SGameFowProject", CallingConvention=CallingConvention.Cdecl)]
        public static extern void UninitCollectorInfoDLL();
        [DllImport("SGameFowProject", CallingConvention=CallingConvention.Cdecl)]
        public static extern void UninitGameFowMapData();
        public void UninitSurface()
        {
            if (this.m_pFieldObj != null)
            {
                this.m_pFieldObj.UninitField();
                this.m_pFieldObj = null;
            }
            if (this.m_los != null)
            {
                this.m_los.Uninit();
                this.m_los = null;
            }
            this.UninitCollectorDataInfo();
            UninitCollectorInfoDLL();
            UninitSurfCellsArray();
            UninitGameFowMapData();
            this.m_commitBitMap = null;
            if (this.m_collector != null)
            {
                this.m_collector.UninitSurface();
                this.m_collector = null;
            }
        }

        [DllImport("SGameFowProject", CallingConvention=CallingConvention.Cdecl)]
        public static extern void UninitSurfCellsArray();
        private void UpdateBaseMapData(COM_PLAYERCAMP camp)
        {
            List<PoolObjHandle<ActorRoot>> gameActors = Singleton<GameObjMgr>.GetInstance().GameActors;
            int count = gameActors.Count;
            for (int i = 0; i < count; i++)
            {
                PoolObjHandle<ActorRoot> handle = gameActors[i];
                if ((((handle != 0) && (handle.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)) && (camp == handle.handle.TheActorMeta.ActorCamp)) && !handle.handle.ActorControl.IsDeadState)
                {
                    switch (handle.handle.ActorControl.GetActorSubType())
                    {
                        case 4:
                        case 1:
                        case 2:
                            this.m_los.ExploreCellsFast(new VInt3(handle.handle.location.x, handle.handle.location.z, 0), handle.handle.HorizonMarker.SightRange, this, this.m_pFieldObj, camp, true, false);
                            break;
                    }
                }
            }
        }

        public void UpdateComputing()
        {
            this.CopyActorInfoData(this.m_collector.m_explorerPosList);
            this.m_collector.ClearExplorerPosList();
            for (int i = 1; i < 3; i++)
            {
                COM_PLAYERCAMP inCamp = (COM_PLAYERCAMP) i;
                this.ComputeFowWithAutoDec(inCamp, inCamp == Singleton<GameFowManager>.instance.m_hostPlayerCamp);
            }
            this.m_collector.ClearExplorerBulletList();
        }

        public bool WorldPosToGrid(VInt3 inWorldPos, out int outCellX, out int outCellY)
        {
            return this.m_pFieldObj.LevelGrid.WorldPosToGrid(inWorldPos, out outCellX, out outCellY);
        }

        public uint GPUInterpolateFrameInterval
        {
            get
            {
                return this.m_gpuInterpFrameInterval;
            }
        }

        public float GPUInterpolateReciprocal
        {
            get
            {
                return this.m_gpuInterpReciprocal;
            }
        }

        public uint InterpolateFrameInterval
        {
            get
            {
                return 8;
            }
        }

        public uint InterpolateFrameIntervalBullet
        {
            get
            {
                return 4;
            }
        }

        public uint InterpolateFrameIntervalHero
        {
            get
            {
                return 2;
            }
        }

        public GameFowCollector m_collector
        {
            [CompilerGenerated]
            get
            {
                return this.<m_collector>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<m_collector>k__BackingField = value;
            }
        }

        public COM_PLAYERCAMP m_hostPlayerCamp
        {
            [CompilerGenerated]
            get
            {
                return this.<m_hostPlayerCamp>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<m_hostPlayerCamp>k__BackingField = value;
            }
        }

        public FowLos m_los
        {
            [CompilerGenerated]
            get
            {
                return this.<m_los>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<m_los>k__BackingField = value;
            }
        }

        public FieldObj m_pFieldObj
        {
            [CompilerGenerated]
            get
            {
                return this.<m_pFieldObj>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<m_pFieldObj>k__BackingField = value;
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate bool IsAreaPermanentLitCallBack(int x, int y, COM_PLAYERCAMP camp);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate bool IsHostCampCallBack(COM_PLAYERCAMP camp);
    }
}

