using Assets.Scripts.GameSystem;
using CSProtocol;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class FowLos
{
    public const int INDEX_NONE = -1;
    public const int inThicknessMax = 3;
    private _SetCellVisible m_setCellVisible = new _SetCellVisible();
    public const int Point_Attach_Threshold = 10;
    public const int Point_Attach_Threshold_Bigger = 100;
    private const int VIEWSIGHT_BOUND_DEFLATE_LENGTH = 40;

    public void ExploreCells(VInt3 location, int inSightRange, GameFowManager pFowMgr, COM_PLAYERCAMP camp, EViewExploringMode ViewExploringMode, bool bDrawDebugLines)
    {
        if ((pFowMgr != null) && (pFowMgr.m_pFieldObj != null))
        {
            if ((pFowMgr.m_pFieldObj.ViewBlockArrayImpl == null) && Application.get_isPlaying())
            {
                pFowMgr.m_pFieldObj.CreateBlockWalls();
            }
            VInt3 inWorldPos = location;
            VInt2 zero = VInt2.zero;
            if (pFowMgr.WorldPosToGrid(inWorldPos, out zero.x, out zero.y))
            {
                pFowMgr.ReviseWorldPosToCenter(inWorldPos, out inWorldPos);
                this.ExploreCellsInternal<_SetCellVisible>(ref this.m_setCellVisible, zero, inWorldPos, inSightRange, pFowMgr, camp, ViewExploringMode, bDrawDebugLines);
            }
        }
    }

    public bool ExploreCellsFast(VInt3 location, int surfSightRange, GameFowManager pFowMgr, FieldObj inFieldObj, COM_PLAYERCAMP camp, bool bStaticExplore, bool bDistOnly)
    {
        if ((pFowMgr == null) || (inFieldObj == null))
        {
            return false;
        }
        VInt2 zero = VInt2.zero;
        if (!pFowMgr.WorldPosToGrid(location, out zero.x, out zero.y))
        {
            return false;
        }
        FieldObj.SViewBlockAttr outAttr = new FieldObj.SViewBlockAttr();
        if (inFieldObj.QueryAttr(zero, out outAttr) && (outAttr.BlockType == 2))
        {
            VInt2 result = VInt2.zero;
            if (inFieldObj.FindNearestGrid(zero, location, FieldObj.EViewBlockType.Brick, 3, null, out result))
            {
                zero = result;
            }
        }
        if (bStaticExplore)
        {
            TraverseStaticSurCell(surfSightRange, zero.x, zero.y, inFieldObj.NumX, inFieldObj.NumY, camp, camp == pFowMgr.m_hostPlayerCamp);
        }
        else
        {
            TraverseSurCell(surfSightRange, zero.x, zero.y, inFieldObj.NumX, inFieldObj.NumY, camp, camp == pFowMgr.m_hostPlayerCamp, bDistOnly);
        }
        return true;
    }

    public void ExploreCellsInternal<TSetCellVisible>(ref TSetCellVisible setCellVisible, VInt2 newSurfPos, VInt3 unitLoc, int surfSightRange, GameFowManager pFowMgr, COM_PLAYERCAMP camp, EViewExploringMode ViewExploringMode, bool bDrawDebugLines) where TSetCellVisible: ISetCellVisible
    {
        if (((pFowMgr != null) && (pFowMgr.m_pFieldObj != null)) && (ViewExploringMode != EViewExploringMode.EViewExploringMode_ShadowCast))
        {
            if (ViewExploringMode == EViewExploringMode.EViewExploringMode_DistOnly)
            {
                for (int i = -surfSightRange - 1; i <= (surfSightRange + 1); i++)
                {
                    for (int j = -surfSightRange - 1; j <= (surfSightRange + 1); j++)
                    {
                        VInt2 num3 = new VInt2(i, j);
                        VInt2 inPos = newSurfPos + num3;
                        if (pFowMgr.IsInsideSurface(inPos.x, inPos.y) && (num3.sqrMagnitude < (surfSightRange * surfSightRange)))
                        {
                            setCellVisible.SetVisible(inPos, camp, true);
                        }
                    }
                }
            }
            else if ((ViewExploringMode != EViewExploringMode.EViewExploringMode_WatchTower) && (ViewExploringMode == EViewExploringMode.EViewExploringMode_RayCast))
            {
                int sightSqr = surfSightRange * surfSightRange;
                int x = newSurfPos.x - surfSightRange;
                int num7 = newSurfPos.x + surfSightRange;
                int y = newSurfPos.y - surfSightRange;
                int num9 = newSurfPos.y + surfSightRange;
                x = Mathf.Clamp(x, 0, pFowMgr.m_pFieldObj.NumX - 1);
                num7 = Mathf.Clamp(num7, 0, pFowMgr.m_pFieldObj.NumX - 1);
                y = Mathf.Clamp(y, 0, pFowMgr.m_pFieldObj.NumY - 1);
                num9 = Mathf.Clamp(num9, 0, pFowMgr.m_pFieldObj.NumY - 1);
                byte viewBlockId = pFowMgr.m_pFieldObj.LevelGrid.GetGridCell(newSurfPos).m_viewBlockId;
                FieldObj.SViewBlockAttr outAttr = new FieldObj.SViewBlockAttr();
                pFowMgr.m_pFieldObj.QueryAttr(newSurfPos, out outAttr);
                CRaycastQuadrant quadrant = new CRaycastQuadrant();
                quadrant.min = new VInt2(x, y);
                quadrant.max = new VInt2(num7, num9);
                if (pFowMgr.m_pFieldObj.ViewBlockArrayImpl != null)
                {
                    List<SBlockWalls>.Enumerator enumerator = pFowMgr.m_pFieldObj.ViewBlockArrayImpl.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        SBlockWalls current = enumerator.Current;
                        int areaId = current.m_areaId;
                        DebugHelper.Assert(areaId != 0);
                        if (viewBlockId == areaId)
                        {
                            if (outAttr.BlockType == 1)
                            {
                                continue;
                            }
                            if (outAttr.BlockType == 2)
                            {
                            }
                        }
                        if (this.ValidateViewBlock(current, quadrant.min.x, quadrant.max.x, quadrant.min.y, quadrant.max.y))
                        {
                            quadrant.viewBlockArrayFinal.Add(current);
                            Dictionary<byte, List<SGridWall>>.Enumerator enumerator2 = current.m_wallsHorizontal.GetEnumerator();
                            while (enumerator2.MoveNext())
                            {
                                KeyValuePair<byte, List<SGridWall>> pair = enumerator2.Current;
                                BaseAlgorithm.AddUniqueItem<byte>(quadrant.wallsHorizontal, pair.Key);
                            }
                            enumerator2 = current.m_wallsVertical.GetEnumerator();
                            while (enumerator2.MoveNext())
                            {
                                KeyValuePair<byte, List<SGridWall>> pair2 = enumerator2.Current;
                                BaseAlgorithm.AddUniqueItem<byte>(quadrant.wallsVertical, pair2.Key);
                            }
                        }
                    }
                    this.RaycastCheck<TSetCellVisible>(pFowMgr, quadrant, newSurfPos, unitLoc, sightSqr, camp, bDrawDebugLines, ref setCellVisible);
                }
                quadrant.Clear();
                quadrant = null;
            }
        }
    }

    public void Init()
    {
        this.m_setCellVisible.Init();
    }

    [DllImport("SGameFowProject", CallingConvention=CallingConvention.Cdecl)]
    public static extern void PreCopyBitmap(int surfPosX, int surfPosY, int surfMaxX, int surfMaxY, COM_PLAYERCAMP camp);
    private void RaycastCheck<TSetCellVisible>(GameFowManager pFowMgr, CRaycastQuadrant quadrant, VInt2 newSurfPos, VInt3 unitLoc, int sightSqr, COM_PLAYERCAMP camp, bool bDrawDebugLines, ref TSetCellVisible setCellVisible) where TSetCellVisible: ISetCellVisible
    {
        VInt2 aa = new VInt2(unitLoc.x, unitLoc.y);
        int x = quadrant.min.x;
        int num3 = quadrant.max.x;
        int y = quadrant.min.y;
        int num5 = quadrant.max.y;
        VInt2 zero = VInt2.zero;
        int halfSizeX = pFowMgr.m_halfSizeX;
        int halfSizeY = pFowMgr.m_halfSizeY;
        for (int i = x; i <= num3; i++)
        {
            for (int j = y; j <= num5; j++)
            {
                VInt2 inPos = new VInt2(i, j);
                if ((i == newSurfPos.x) && (j == newSurfPos.y))
                {
                    setCellVisible.SetVisible(inPos, camp, true);
                }
                else
                {
                    VInt2 num12 = inPos - newSurfPos;
                    if (num12.sqrMagnitude < sightSqr)
                    {
                        VInt3 outWorldPos = VInt3.zero;
                        pFowMgr.m_pFieldObj.LevelGrid.GridToWorldPos(i, j, out outWorldPos);
                        VInt2 bb = new VInt2(outWorldPos.x, outWorldPos.y);
                        VInt2 num15 = bb;
                        num15.x -= halfSizeX;
                        num15.y -= halfSizeY;
                        VInt2 num16 = bb;
                        num16.x += halfSizeX;
                        num16.y -= halfSizeY;
                        VInt2 num17 = bb;
                        num17.x += halfSizeX;
                        num17.y += halfSizeY;
                        VInt2 num18 = bb;
                        num18.x -= halfSizeX;
                        num18.y += halfSizeY;
                        SGridWall wall = new SGridWall();
                        bool flag = false;
                        bool flag2 = false;
                        bool flag3 = false;
                        bool flag4 = i < newSurfPos.x;
                        bool flag5 = j < newSurfPos.y;
                        int num19 = !flag4 ? (newSurfPos.x + 1) : newSurfPos.x;
                        int num20 = !flag5 ? (newSurfPos.y + 1) : newSurfPos.y;
                        for (int k = 0; !flag && (!flag2 || !flag3); k++)
                        {
                            if (!flag2)
                            {
                                int num22 = num19 + (!flag4 ? k : -k);
                                if (flag4 && (num22 < (i + 1)))
                                {
                                    flag2 = true;
                                }
                                else if (!flag4 && (num22 > i))
                                {
                                    flag2 = true;
                                }
                                if (!flag2 && quadrant.wallsVertical.Contains((byte) num22))
                                {
                                    List<SBlockWalls>.Enumerator enumerator = quadrant.viewBlockArrayFinal.GetEnumerator();
                                    while (enumerator.MoveNext() && !flag)
                                    {
                                        SBlockWalls current = enumerator.Current;
                                        List<SGridWall> list = null;
                                        if (current.m_wallsVertical.TryGetValue((byte) num22, out list))
                                        {
                                            int count = list.Count;
                                            for (int m = 0; (m < count) && !flag; m++)
                                            {
                                                SGridWall wall2 = list[m];
                                                if (bDrawDebugLines)
                                                {
                                                    if (SegmentIntersect(aa, bb, wall2.m_start, wall2.m_end, ref zero))
                                                    {
                                                        flag = true;
                                                        wall = wall2;
                                                    }
                                                    else if ((SegmentIntersect(aa, num15, wall2.m_start, wall2.m_end, ref zero) || SegmentIntersect(aa, num16, wall2.m_start, wall2.m_end, ref zero)) || (SegmentIntersect(aa, num17, wall2.m_start, wall2.m_end, ref zero) || SegmentIntersect(aa, num18, wall2.m_start, wall2.m_end, ref zero)))
                                                    {
                                                        flag = true;
                                                        wall = wall2;
                                                    }
                                                }
                                                else if (SegmentIntersect(aa, bb, wall2.m_start, wall2.m_end))
                                                {
                                                    flag = true;
                                                    wall = wall2;
                                                }
                                                else if ((SegmentIntersect(aa, num15, wall2.m_start, wall2.m_end, ref zero) || SegmentIntersect(aa, num16, wall2.m_start, wall2.m_end, ref zero)) || (SegmentIntersect(aa, num17, wall2.m_start, wall2.m_end, ref zero) || SegmentIntersect(aa, num18, wall2.m_start, wall2.m_end, ref zero)))
                                                {
                                                    flag = true;
                                                    wall = wall2;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (!flag3 && !flag)
                            {
                                int num25 = num20 + (!flag5 ? k : -k);
                                if (flag5 && (num25 < (j + 1)))
                                {
                                    flag3 = true;
                                }
                                else if (!flag5 && (num25 > j))
                                {
                                    flag3 = true;
                                }
                                if (!flag3 && quadrant.wallsHorizontal.Contains((byte) num25))
                                {
                                    List<SBlockWalls>.Enumerator enumerator2 = quadrant.viewBlockArrayFinal.GetEnumerator();
                                    while (enumerator2.MoveNext() && !flag)
                                    {
                                        SBlockWalls walls2 = enumerator2.Current;
                                        List<SGridWall> list2 = null;
                                        if (walls2.m_wallsHorizontal.TryGetValue((byte) num25, out list2))
                                        {
                                            int num26 = list2.Count;
                                            for (int n = 0; (n < num26) && !flag; n++)
                                            {
                                                SGridWall wall3 = list2[n];
                                                if (bDrawDebugLines)
                                                {
                                                    if (SegmentIntersect(aa, bb, wall3.m_start, wall3.m_end, ref zero))
                                                    {
                                                        flag = true;
                                                        wall = wall3;
                                                    }
                                                    else if ((SegmentIntersect(aa, num15, wall3.m_start, wall3.m_end, ref zero) || SegmentIntersect(aa, num16, wall3.m_start, wall3.m_end, ref zero)) || (SegmentIntersect(aa, num17, wall3.m_start, wall3.m_end, ref zero) || SegmentIntersect(aa, num18, wall3.m_start, wall3.m_end, ref zero)))
                                                    {
                                                        flag = true;
                                                        wall = wall3;
                                                    }
                                                }
                                                else if (SegmentIntersect(aa, bb, wall3.m_start, wall3.m_end))
                                                {
                                                    flag = true;
                                                }
                                                else if ((SegmentIntersect(aa, num15, wall3.m_start, wall3.m_end, ref zero) || SegmentIntersect(aa, num16, wall3.m_start, wall3.m_end, ref zero)) || (SegmentIntersect(aa, num17, wall3.m_start, wall3.m_end, ref zero) || SegmentIntersect(aa, num18, wall3.m_start, wall3.m_end, ref zero)))
                                                {
                                                    flag = true;
                                                    wall = wall3;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (!flag)
                        {
                            setCellVisible.SetVisible(inPos, camp, true);
                        }
                    }
                }
            }
        }
    }

    public static bool SegmentIntersect(VInt2 aa, VInt2 bb, VInt2 cc, VInt2 dd)
    {
        Vector2 vector = (Vector2) aa;
        Vector2 vector2 = (Vector2) bb;
        Vector2 vector3 = (Vector2) cc;
        Vector2 vector4 = (Vector2) dd;
        float num = ((vector.x - vector3.x) * (vector2.y - vector3.y)) - ((vector.y - vector3.y) * (vector2.x - vector3.x));
        float num2 = ((vector.x - vector4.x) * (vector2.y - vector4.y)) - ((vector.y - vector4.y) * (vector2.x - vector4.x));
        if ((num * num2) >= 0f)
        {
            return false;
        }
        float num3 = ((vector3.x - vector.x) * (vector4.y - vector.y)) - ((vector3.y - vector.y) * (vector4.x - vector.x));
        float num4 = (num3 + num) - num2;
        if ((num3 * num4) >= 0f)
        {
            return false;
        }
        return true;
    }

    public static bool SegmentIntersect(VInt2 aa, VInt2 bb, VInt2 cc, VInt2 dd, ref VInt2 intersectPoint)
    {
        Vector2 vector = (Vector2) aa;
        Vector2 vector2 = (Vector2) bb;
        Vector2 vector3 = (Vector2) cc;
        Vector2 vector4 = (Vector2) dd;
        float num = ((vector.x - vector3.x) * (vector2.y - vector3.y)) - ((vector.y - vector3.y) * (vector2.x - vector3.x));
        float num2 = ((vector.x - vector4.x) * (vector2.y - vector4.y)) - ((vector.y - vector4.y) * (vector2.x - vector4.x));
        if ((num * num2) >= 0f)
        {
            return false;
        }
        float num3 = ((vector3.x - vector.x) * (vector4.y - vector.y)) - ((vector3.y - vector.y) * (vector4.x - vector.x));
        float num4 = (num3 + num) - num2;
        if ((num3 * num4) >= 0f)
        {
            return false;
        }
        float num5 = num2 - num;
        num5 = (num5 >= 0f) ? Mathf.Max(num5, 0.0001f) : Mathf.Min(num5, -0.0001f);
        float num6 = num3 / num5;
        float num7 = num6 * (vector2.x - vector.x);
        float num8 = num6 * (vector2.y - vector.y);
        Vector2 vector5 = new Vector2(vector.x + num7, vector.y + num8);
        intersectPoint = vector5;
        return true;
    }

    [DllImport("SGameFowProject", CallingConvention=CallingConvention.Cdecl)]
    public static extern void TraverseStaticSurCell(int sightRange, int surfPosX, int surfPosY, int surfMaxX, int surfMaxY, COM_PLAYERCAMP camp, bool bHostCamp);
    [DllImport("SGameFowProject", CallingConvention=CallingConvention.Cdecl)]
    private static extern void TraverseSurCell(int sightRange, int surfPosX, int surfPosY, int surfMaxX, int surfMaxY, COM_PLAYERCAMP camp, bool bHostCamp, bool bDistOnly);
    public void Uninit()
    {
    }

    private bool ValidateViewBlock(FBoxSphereBounds inBounds, VInt2 segStart, int inViewSight)
    {
        bool flag = true;
        int num = inBounds.Origin.x - inBounds.BoxExtent.x;
        int num2 = inBounds.Origin.x + inBounds.BoxExtent.x;
        int num3 = inBounds.Origin.y - inBounds.BoxExtent.y;
        int num4 = inBounds.Origin.y + inBounds.BoxExtent.y;
        return ((((num <= (segStart.x + inViewSight)) && (num2 >= (segStart.x - inViewSight))) && ((num3 <= (segStart.y + inViewSight)) && (num4 >= (segStart.y - inViewSight)))) && flag);
    }

    private bool ValidateViewBlock(SBlockWalls inBlockWalls, byte xMin, byte xMax, byte yMin, byte yMax)
    {
        return (((inBlockWalls.m_xMin <= xMax) && (inBlockWalls.m_xMax >= xMin)) && ((inBlockWalls.m_yMin <= yMax) && (inBlockWalls.m_yMax >= yMin)));
    }

    private bool ValidateViewBlock(SBlockWalls inBlockWalls, int xMin, int xMax, int yMin, int yMax)
    {
        return (((inBlockWalls.m_xMin <= xMax) && (inBlockWalls.m_xMax >= xMin)) && ((inBlockWalls.m_yMin <= yMax) && (inBlockWalls.m_yMax >= yMin)));
    }

    private class ComparerSortedSlope : IComparer<int>
    {
        public int Compare(int A, int B)
        {
            return (A - B);
        }
    }

    private class ComparerVertex : IComparer<FowLos.SPolylineVertex>
    {
        private VInt2 m_scanlineStart = VInt2.zero;

        public ComparerVertex(VInt2 inSegStart)
        {
            this.m_scanlineStart = inSegStart;
        }

        public int Compare(FowLos.SPolylineVertex A, FowLos.SPolylineVertex B)
        {
            VInt2 num = A.m_point - this.m_scanlineStart;
            VInt2 num2 = B.m_point - this.m_scanlineStart;
            if (num.sqrMagnitude > num2.sqrMagnitude)
            {
                return 1;
            }
            return -1;
        }
    }

    private class CRaycastQuadrant
    {
        public VInt2 max = VInt2.zero;
        public VInt2 min = VInt2.zero;
        public List<FowLos.SBlockWalls> viewBlockArrayFinal = new List<FowLos.SBlockWalls>();
        public List<byte> wallsHorizontal = new List<byte>();
        public List<byte> wallsVertical = new List<byte>();

        public void Clear()
        {
            this.viewBlockArrayFinal.Clear();
            this.viewBlockArrayFinal = null;
            this.wallsHorizontal.Clear();
            this.wallsHorizontal = null;
            this.wallsVertical.Clear();
            this.wallsVertical = null;
        }
    }

    public enum EBlockRegionType
    {
        EBlockRegionType_Box,
        EBlockRegionType_Sphere,
        EBlockRegionType_Polygon,
        EBlockRegionType_Count
    }

    public enum EViewExploringMode
    {
        EViewExploringMode_ShadowCast,
        EViewExploringMode_RayCast,
        EViewExploringMode_WatchTower,
        EViewExploringMode_DistOnly,
        EViewExploringMode_Count
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FBoxSphereBounds
    {
        public VInt3 Origin;
        public VInt3 BoxExtent;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SBlockContext
    {
        public FowLos.FBoxSphereBounds m_bounds;
        public byte m_xMin;
        public byte m_xMax;
        public byte m_yMin;
        public byte m_yMax;
        public int m_areaId;
        public List<int> m_blockerGridIndexList;
        public List<FowLos.SPolylineSegment> m_polylineSegList;
        public List<FowLos.SPolylineVertex> m_polylineVertices;
        public SBlockContext(FowLos.SBlockContext rhs)
        {
            this.m_bounds = rhs.m_bounds;
            this.m_xMin = rhs.m_xMin;
            this.m_xMax = rhs.m_xMax;
            this.m_yMin = rhs.m_yMin;
            this.m_yMax = rhs.m_yMax;
            this.m_areaId = rhs.m_areaId;
            this.m_blockerGridIndexList = new List<int>(rhs.m_blockerGridIndexList);
            this.m_polylineSegList = new List<FowLos.SPolylineSegment>(rhs.m_polylineSegList);
            this.m_polylineVertices = new List<FowLos.SPolylineVertex>(rhs.m_polylineVertices);
        }

        public void ParamlessConstruct()
        {
            this.m_bounds = new FowLos.FBoxSphereBounds();
            this.m_areaId = -1;
            this.m_blockerGridIndexList = new List<int>();
            this.m_polylineSegList = new List<FowLos.SPolylineSegment>();
            this.m_polylineVertices = new List<FowLos.SPolylineVertex>();
        }

        public bool ContainsPoint(VInt2 point, out int index)
        {
            int count = this.m_polylineVertices.Count;
            for (int i = 0; i < count; i++)
            {
                FowLos.SPolylineVertex vertex = this.m_polylineVertices[i];
                if (vertex.IsNear(point))
                {
                    index = i;
                    return true;
                }
            }
            index = -1;
            return false;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SBlockWalls
    {
        public byte m_xMin;
        public byte m_xMax;
        public byte m_yMin;
        public byte m_yMax;
        public byte m_areaId;
        public Dictionary<byte, List<FowLos.SGridWall>> m_wallsHorizontal;
        public Dictionary<byte, List<FowLos.SGridWall>> m_wallsVertical;
        public void ParamlessConstruct()
        {
            this.m_areaId = 0;
            this.m_wallsHorizontal = new Dictionary<byte, List<FowLos.SGridWall>>();
            this.m_wallsVertical = new Dictionary<byte, List<FowLos.SGridWall>>();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SGridWall
    {
        public bool m_horizontal;
        public VInt2 m_start;
        public VInt2 m_end;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SPolylineSegment
    {
        public int m_startPtIndex;
        public int m_endPtIndex;
        public int m_belongBlockId;
        public VInt2 m_start;
        public VInt2 m_end;
        public int m_index;
        public SPolylineSegment(FowLos.SPolylineSegment rhs)
        {
            this.m_startPtIndex = rhs.m_startPtIndex;
            this.m_endPtIndex = rhs.m_endPtIndex;
            this.m_belongBlockId = rhs.m_belongBlockId;
            this.m_start = rhs.m_start;
            this.m_end = rhs.m_end;
            this.m_index = rhs.m_index;
        }

        public SPolylineSegment(VInt2 start, VInt2 end)
        {
            this.m_startPtIndex = -1;
            this.m_endPtIndex = -1;
            this.m_belongBlockId = -1;
            this.m_start = start;
            this.m_end = end;
            this.m_index = -1;
        }

        public void ParamlessConstruct()
        {
            this.m_startPtIndex = -1;
            this.m_endPtIndex = -1;
            this.m_belongBlockId = -1;
            this.m_start = VInt2.zero;
            this.m_end = VInt2.zero;
            this.m_index = -1;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SPolylineVertex
    {
        public VInt2 m_point;
        public int m_belongBlockId;
        public List<int> m_belongSegNoList;
        public bool m_bNative;
        public SPolylineVertex(FowLos.SPolylineVertex rhs)
        {
            this.m_point = rhs.m_point;
            this.m_belongBlockId = rhs.m_belongBlockId;
            this.m_belongSegNoList = new List<int>(rhs.m_belongSegNoList);
            this.m_bNative = rhs.m_bNative;
        }

        public SPolylineVertex(VInt2 point)
        {
            this.m_point = point;
            this.m_belongBlockId = -1;
            this.m_belongSegNoList = new List<int>();
            this.m_bNative = true;
        }

        public void ParamlessConstruct()
        {
            this.m_point = VInt2.zero;
            this.m_belongBlockId = -1;
            this.m_belongSegNoList = new List<int>();
            this.m_bNative = true;
        }

        public bool Equals(FowLos.SPolylineVertex rhs)
        {
            return (((BaseAlgorithm.IsNearlyZero(this.m_point - rhs.m_point, 10) && (this.m_belongBlockId == rhs.m_belongBlockId)) && (this.m_belongSegNoList == rhs.m_belongSegNoList)) && (this.m_bNative == rhs.m_bNative));
        }

        public bool NotEqual(FowLos.SPolylineVertex rhs)
        {
            return !this.Equals(rhs);
        }

        public bool IsNear(VInt2 point)
        {
            return BaseAlgorithm.IsNearlyZero(this.m_point - point, 10);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SSegUID
    {
        public int m_blockId;
        public int m_segIndex;
        public SSegUID(int blockId, int segIndex)
        {
            this.m_blockId = blockId;
            this.m_segIndex = segIndex;
        }
    }
}

