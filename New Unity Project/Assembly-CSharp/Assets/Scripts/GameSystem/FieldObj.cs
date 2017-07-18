namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.GameLogic;
    using CSProtocol;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class FieldObj : MonoBehaviour
    {
        [CompilerGenerated]
        private int <FieldX>k__BackingField;
        [CompilerGenerated]
        private int <FieldY>k__BackingField;
        [CompilerGenerated]
        private List<FowLos.SBlockWalls> <ViewBlockArrayImpl>k__BackingField;
        [CompilerGenerated]
        private int <xMax>k__BackingField;
        [CompilerGenerated]
        private int <xMin>k__BackingField;
        [CompilerGenerated]
        private int <yMax>k__BackingField;
        [CompilerGenerated]
        private int <yMin>k__BackingField;
        [NonSerialized]
        public int CellScale = 1;
        [SerializeField]
        public byte[] fowOfflineData = new byte[0];
        private const int INT_MAX = 0x7fffffff;
        private const int INT_MIN = -2147483648;
        [SerializeField]
        public FOLevelGrid LevelGrid;
        [NonSerialized]
        public FOWSurfCell[] m_fowCells;
        [SerializeField]
        public SViewBlockAttrIndexed[] SerializedBlockAttrList = new SViewBlockAttrIndexed[0];
        public Dictionary<byte, SViewBlockAttr> ViewBlockAttrMap = new Dictionary<byte, SViewBlockAttr>();

        public bool AddViewBlockAttr(byte inViewBlockId, byte inBlockType, byte inLightType)
        {
            if (!this.ViewBlockAttrMap.ContainsKey(inViewBlockId) && (inViewBlockId > 0))
            {
                SViewBlockAttr attr = new SViewBlockAttr();
                attr.BlockType = inBlockType;
                attr.LightType = inLightType;
                this.ViewBlockAttrMap.Add(inViewBlockId, attr);
                return true;
            }
            return false;
        }

        public void Clear()
        {
            if (this.ViewBlockArrayImpl != null)
            {
                this.ViewBlockArrayImpl.Clear();
                this.ViewBlockArrayImpl = null;
            }
            int num = 0;
            this.FieldY = num;
            this.FieldX = num;
            num = 0;
            this.yMax = num;
            num = num;
            this.yMin = num;
            num = num;
            this.xMax = num;
            this.xMin = num;
            this.LevelGrid.Clear();
        }

        public void ClearViewBlockAttrMap()
        {
            this.ViewBlockAttrMap.Clear();
            this.SerializedBlockAttrList = new SViewBlockAttrIndexed[0];
        }

        private void CollectNeighbourGrids(List<VInt2> inNeighbourGrids, List<VInt2> inSearchedGrids, VInt2 inCenterCell, int inThickness)
        {
            if (inNeighbourGrids != null)
            {
                int x = inCenterCell.x;
                int y = inCenterCell.y;
                int num3 = Mathf.Max(x - inThickness, 0);
                int num4 = Mathf.Min(x + inThickness, this.NumX - 1);
                int num5 = Mathf.Max(y - inThickness, 0);
                int num6 = Mathf.Min(y + inThickness, this.NumY - 1);
                for (int i = num3; i <= num4; i++)
                {
                    for (int j = num5; j <= num6; j++)
                    {
                        VInt2 item = new VInt2(i, j);
                        if ((inSearchedGrids == null) || !inSearchedGrids.Contains(item))
                        {
                            inNeighbourGrids.Add(item);
                        }
                    }
                }
            }
        }

        private Dictionary<byte, FowLos.SBlockContext> CreateBlockMap()
        {
            Dictionary<byte, FowLos.SBlockContext> dictionary = new Dictionary<byte, FowLos.SBlockContext>();
            FOGridInfo gridInfo = this.LevelGrid.GridInfo;
            int cellNumX = gridInfo.CellNumX;
            int cellNumY = gridInfo.CellNumY;
            int num3 = cellNumX * cellNumY;
            Dictionary<byte, List<int>> dictionary2 = new Dictionary<byte, List<int>>();
            for (int i = 0; i < num3; i++)
            {
                FOGridCell cell = this.LevelGrid.GridCells[i];
                byte viewBlockId = cell.m_viewBlockId;
                if (this.IsAreaViewBlocking(viewBlockId))
                {
                    List<int> list = null;
                    if (dictionary2.TryGetValue(viewBlockId, out list))
                    {
                        list.Add(i);
                    }
                    else
                    {
                        list = new List<int>();
                        list.Add(i);
                        dictionary2.Add(viewBlockId, list);
                    }
                }
            }
            Dictionary<byte, List<int>>.Enumerator enumerator = dictionary2.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<byte, List<int>> current = enumerator.Current;
                List<int> list2 = current.Value;
                KeyValuePair<byte, List<int>> pair2 = enumerator.Current;
                byte key = pair2.Key;
                List<int> list3 = new List<int>();
                int num7 = 0x7fffffff;
                int num8 = 0x7fffffff;
                int num9 = -2147483647;
                int num10 = -2147483647;
                int count = list2.Count;
                for (int j = 0; j < count; j++)
                {
                    int index = list2[j];
                    FOGridCell cell2 = this.LevelGrid.GridCells[index];
                    int cellX = cell2.CellX;
                    int cellY = cell2.CellY;
                    if (num7 > cellX)
                    {
                        num7 = cellX;
                    }
                    if (num9 < cellX)
                    {
                        num9 = cellX;
                    }
                    if (num8 > cellY)
                    {
                        num8 = cellY;
                    }
                    if (num10 < cellY)
                    {
                        num10 = cellY;
                    }
                    List<int> list4 = new List<int>();
                    bool flag = false;
                    if (cellX > 0)
                    {
                        list4.Add(index - 1);
                    }
                    else
                    {
                        list3.Add(index);
                        flag = true;
                    }
                    if (!flag)
                    {
                        if (cellX < (cellNumX - 1))
                        {
                            list4.Add(index + 1);
                        }
                        else
                        {
                            list3.Add(index);
                            flag = true;
                        }
                        if (!flag)
                        {
                            if (cellY > 0)
                            {
                                list4.Add(index - cellNumX);
                            }
                            else
                            {
                                list3.Add(index);
                                flag = true;
                            }
                            if (!flag)
                            {
                                if (cellY < (cellNumY - 1))
                                {
                                    list4.Add(index + cellNumX);
                                }
                                else
                                {
                                    list3.Add(index);
                                    flag = true;
                                }
                                if (!flag)
                                {
                                    int num16 = list4.Count;
                                    for (int num17 = 0; num17 < num16; num17++)
                                    {
                                        int item = list4[num17];
                                        if (!list2.Contains(item))
                                        {
                                            list3.Add(index);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                FowLos.SBlockContext blockContext = new FowLos.SBlockContext();
                blockContext.ParamlessConstruct();
                blockContext.m_areaId = key;
                int num19 = gridInfo.GridPos.x + (gridInfo.CellSizeX * (num9 + 1));
                int num20 = gridInfo.GridPos.y + (gridInfo.CellSizeY * (num10 + 1));
                int num21 = gridInfo.GridPos.x + (gridInfo.CellSizeX * num7);
                int num22 = gridInfo.GridPos.y + (gridInfo.CellSizeY * num8);
                blockContext.m_bounds.Origin.x = (num19 + num21) / 2;
                blockContext.m_bounds.Origin.y = (num20 + num22) / 2;
                blockContext.m_bounds.Origin.z = 0;
                blockContext.m_bounds.BoxExtent.x = (num19 - num21) / 2;
                blockContext.m_bounds.BoxExtent.y = (num20 - num22) / 2;
                blockContext.m_bounds.BoxExtent.z = 0x3e8;
                DebugHelper.Assert(num7 <= 0xff);
                DebugHelper.Assert(num9 <= 0xff);
                DebugHelper.Assert(num8 <= 0xff);
                DebugHelper.Assert(num10 <= 0xff);
                blockContext.m_xMin = (byte) num7;
                blockContext.m_xMax = (byte) num9;
                blockContext.m_yMin = (byte) num8;
                blockContext.m_yMax = (byte) num10;
                int num23 = list3.Count;
                List<FowLos.SPolylineSegment> rawPolylineSegList = new List<FowLos.SPolylineSegment>();
                for (int k = num8; k <= num10; k++)
                {
                    Dictionary<int, int> dictionary3 = new Dictionary<int, int>();
                    for (int num25 = 0; num25 < num23; num25++)
                    {
                        int num26 = list3[num25];
                        FOGridCell cell3 = this.LevelGrid.GridCells[num26];
                        int num27 = cell3.CellX;
                        if (cell3.CellY == k)
                        {
                            dictionary3.Add(num27, num26);
                        }
                    }
                    List<FowLos.SPolylineSegment> list6 = new List<FowLos.SPolylineSegment>();
                    List<FowLos.SPolylineSegment> list7 = new List<FowLos.SPolylineSegment>();
                    int num29 = -1;
                    int num30 = -1;
                    Dictionary<int, int>.Enumerator enumerator2 = dictionary3.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        KeyValuePair<int, int> pair3 = enumerator2.Current;
                        int num31 = pair3.Value;
                        FOGridCell cell4 = this.LevelGrid.GridCells[num31];
                        int num32 = cell4.CellX;
                        int num33 = cell4.CellY;
                        VInt2 start = new VInt2(gridInfo.GridPos.x + (gridInfo.CellSizeX * num32), gridInfo.GridPos.y + (gridInfo.CellSizeY * num33));
                        VInt2 end = new VInt2(gridInfo.GridPos.x + (gridInfo.CellSizeX * (num32 + 1)), gridInfo.GridPos.y + (gridInfo.CellSizeY * (num33 + 1)));
                        VInt2 num36 = new VInt2(start.x, end.y);
                        VInt2 num37 = new VInt2(end.x, start.y);
                        if ((num33 == 0) || (key != this.LevelGrid.GridCells[num31 - cellNumX].m_viewBlockId))
                        {
                            if ((num29 != -1) && (num32 == (num29 + 1)))
                            {
                                if (list7.Count > 0)
                                {
                                    FowLos.SPolylineSegment segment = new FowLos.SPolylineSegment(list7[list7.Count - 1]);
                                    segment.m_end = num37;
                                    list7[list7.Count - 1] = segment;
                                }
                                else
                                {
                                    list7.Add(new FowLos.SPolylineSegment(start, num37));
                                }
                            }
                            else
                            {
                                list7.Add(new FowLos.SPolylineSegment(start, num37));
                            }
                            num29 = num32;
                        }
                        if ((num33 == (cellNumY - 1)) || (key != this.LevelGrid.GridCells[num31 + cellNumX].m_viewBlockId))
                        {
                            if ((num30 != -1) && (num32 == (num30 + 1)))
                            {
                                if (list6.Count > 0)
                                {
                                    FowLos.SPolylineSegment segment2 = new FowLos.SPolylineSegment(list6[list6.Count - 1]);
                                    segment2.m_end = end;
                                    list6[list6.Count - 1] = segment2;
                                }
                                else
                                {
                                    list6.Add(new FowLos.SPolylineSegment(num36, end));
                                }
                            }
                            else
                            {
                                list6.Add(new FowLos.SPolylineSegment(num36, end));
                            }
                            num30 = num32;
                        }
                    }
                    MergeBlockSegList(ref blockContext, rawPolylineSegList, key);
                    MergeBlockSegList(ref blockContext, list7, key);
                    rawPolylineSegList = list6;
                }
                MergeBlockSegList(ref blockContext, rawPolylineSegList, key);
                rawPolylineSegList.Clear();
                DebugHelper.Assert(blockContext.m_polylineSegList.Count >= 2);
                for (int m = num7; m <= num9; m++)
                {
                    Dictionary<int, int> dictionary4 = new Dictionary<int, int>();
                    for (int num39 = 0; num39 < num23; num39++)
                    {
                        int num40 = list3[num39];
                        FOGridCell cell5 = this.LevelGrid.GridCells[num40];
                        int num41 = cell5.CellX;
                        int num42 = cell5.CellY;
                        if (num41 == m)
                        {
                            dictionary4.Add(num42, num40);
                        }
                    }
                    List<FowLos.SPolylineSegment> list8 = new List<FowLos.SPolylineSegment>();
                    List<FowLos.SPolylineSegment> list9 = new List<FowLos.SPolylineSegment>();
                    int num43 = -1;
                    int num44 = -1;
                    Dictionary<int, int>.Enumerator enumerator3 = dictionary4.GetEnumerator();
                    while (enumerator3.MoveNext())
                    {
                        KeyValuePair<int, int> pair4 = enumerator3.Current;
                        int num45 = pair4.Value;
                        FOGridCell cell6 = this.LevelGrid.GridCells[num45];
                        int num46 = cell6.CellX;
                        int num47 = cell6.CellY;
                        VInt2 num48 = new VInt2(gridInfo.GridPos.x + (gridInfo.CellSizeX * num46), gridInfo.GridPos.y + (gridInfo.CellSizeY * num47));
                        VInt2 num49 = new VInt2(gridInfo.GridPos.x + (gridInfo.CellSizeX * (num46 + 1)), gridInfo.GridPos.y + (gridInfo.CellSizeY * (num47 + 1)));
                        VInt2 num50 = new VInt2(num48.x, num49.y);
                        VInt2 num51 = new VInt2(num49.x, num48.y);
                        if ((num46 == 0) || (key != this.LevelGrid.GridCells[num45 - 1].m_viewBlockId))
                        {
                            if ((num43 != -1) && (num47 == (num43 + 1)))
                            {
                                if (list9.Count > 0)
                                {
                                    FowLos.SPolylineSegment segment3 = new FowLos.SPolylineSegment(list9[list9.Count - 1]);
                                    segment3.m_end = num50;
                                    list9[list9.Count - 1] = segment3;
                                }
                                else
                                {
                                    list9.Add(new FowLos.SPolylineSegment(num48, num50));
                                }
                            }
                            else
                            {
                                list9.Add(new FowLos.SPolylineSegment(num48, num50));
                            }
                            num43 = num47;
                        }
                        if ((num46 == (cellNumX - 1)) || (key != this.LevelGrid.GridCells[num45 + 1].m_viewBlockId))
                        {
                            if ((num44 != -1) && (num47 == (num44 + 1)))
                            {
                                if (list8.Count > 0)
                                {
                                    FowLos.SPolylineSegment segment4 = new FowLos.SPolylineSegment(list8[list8.Count - 1]);
                                    segment4.m_end = num49;
                                    list8[list8.Count - 1] = segment4;
                                }
                                else
                                {
                                    list8.Add(new FowLos.SPolylineSegment(num51, num49));
                                }
                            }
                            else
                            {
                                list8.Add(new FowLos.SPolylineSegment(num51, num49));
                            }
                            num44 = num47;
                        }
                    }
                    MergeBlockSegList(ref blockContext, rawPolylineSegList, key);
                    MergeBlockSegList(ref blockContext, list9, key);
                    rawPolylineSegList = list8;
                }
                MergeBlockSegList(ref blockContext, rawPolylineSegList, key);
                rawPolylineSegList.Clear();
                DebugHelper.Assert(blockContext.m_polylineSegList.Count >= 4);
                int num52 = blockContext.m_polylineVertices.Count;
                for (int n = 0; n < num52; n++)
                {
                    FowLos.SPolylineVertex vertex = blockContext.m_polylineVertices[n];
                    DebugHelper.Assert(vertex.m_bNative);
                    DebugHelper.Assert(vertex.m_belongBlockId != -1);
                    DebugHelper.Assert(vertex.m_belongSegNoList.Count <= 4);
                    DebugHelper.Assert(vertex.m_belongSegNoList.Count >= 2);
                }
                int num54 = blockContext.m_polylineSegList.Count;
                for (int num55 = 0; num55 < num54; num55++)
                {
                    FowLos.SPolylineSegment segment5 = blockContext.m_polylineSegList[num55];
                    DebugHelper.Assert(segment5.m_index == num55);
                    DebugHelper.Assert(segment5.m_belongBlockId != -1);
                    DebugHelper.Assert(segment5.m_startPtIndex != -1);
                    DebugHelper.Assert(segment5.m_endPtIndex != -1);
                }
                blockContext.m_blockerGridIndexList = list2;
                if (dictionary.ContainsKey(key))
                {
                    dictionary[key] = blockContext;
                }
                else
                {
                    dictionary.Add(key, blockContext);
                }
            }
            return dictionary;
        }

        public void CreateBlockWalls()
        {
            this.ViewBlockArrayImpl = this.CreateBlockWallsInternal();
        }

        private List<FowLos.SBlockWalls> CreateBlockWallsInternal()
        {
            List<FowLos.SBlockWalls> list = new List<FowLos.SBlockWalls>();
            Dictionary<byte, FowLos.SBlockContext>.Enumerator enumerator = this.CreateBlockMap().GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<byte, FowLos.SBlockContext> current = enumerator.Current;
                FowLos.SBlockContext context = current.Value;
                KeyValuePair<byte, FowLos.SBlockContext> pair2 = enumerator.Current;
                byte key = pair2.Key;
                FowLos.SBlockWalls item = new FowLos.SBlockWalls();
                item.ParamlessConstruct();
                item.m_areaId = key;
                item.m_xMin = context.m_xMin;
                item.m_xMax = context.m_xMax;
                item.m_yMin = context.m_yMin;
                item.m_yMax = context.m_yMax;
                KeyValuePair<byte, FowLos.SBlockContext> pair3 = enumerator.Current;
                List<FowLos.SPolylineSegment> polylineSegList = pair3.Value.m_polylineSegList;
                int count = polylineSegList.Count;
                for (int i = 0; i < count; i++)
                {
                    FowLos.SGridWall wall = new FowLos.SGridWall();
                    FowLos.SPolylineSegment segment = polylineSegList[i];
                    wall.m_start = segment.m_start;
                    FowLos.SPolylineSegment segment2 = polylineSegList[i];
                    wall.m_end = segment2.m_end;
                    DebugHelper.Assert(wall.m_start != wall.m_end);
                    if (wall.m_start.x == wall.m_end.x)
                    {
                        wall.m_horizontal = false;
                        byte num4 = (byte) (((wall.m_start.x + (this.PaneX / 2)) - this.LevelGrid.GridInfo.GridPos.x) / this.PaneX);
                        DebugHelper.Assert(num4 >= 0);
                        DebugHelper.Assert(num4 <= this.NumX);
                        if (wall.m_start.y > wall.m_end.y)
                        {
                            wall.m_start.y += 10;
                            wall.m_end.y -= 10;
                        }
                        else
                        {
                            DebugHelper.Assert(wall.m_start.y < wall.m_end.y);
                            wall.m_start.y -= 10;
                            wall.m_end.y += 10;
                        }
                        List<FowLos.SGridWall> list3 = null;
                        if (item.m_wallsVertical.TryGetValue(num4, out list3))
                        {
                            item.m_wallsVertical[num4].Add(wall);
                        }
                        else
                        {
                            list3 = new List<FowLos.SGridWall>();
                            list3.Add(wall);
                            item.m_wallsVertical.Add(num4, list3);
                        }
                    }
                    else
                    {
                        DebugHelper.Assert(wall.m_start.y == wall.m_end.y);
                        wall.m_horizontal = true;
                        byte num5 = (byte) (((wall.m_start.y + (this.PaneY / 2)) - this.LevelGrid.GridInfo.GridPos.y) / this.PaneY);
                        DebugHelper.Assert(num5 >= 0);
                        DebugHelper.Assert(num5 <= this.NumY);
                        if (wall.m_start.x > wall.m_end.x)
                        {
                            wall.m_start.x += 10;
                            wall.m_end.x -= 10;
                        }
                        else
                        {
                            DebugHelper.Assert(wall.m_start.x < wall.m_end.x);
                            wall.m_start.x -= 10;
                            wall.m_end.x += 10;
                        }
                        List<FowLos.SGridWall> list4 = null;
                        if (item.m_wallsHorizontal.TryGetValue(num5, out list4))
                        {
                            item.m_wallsHorizontal[num5].Add(wall);
                        }
                        else
                        {
                            list4 = new List<FowLos.SGridWall>();
                            list4.Add(wall);
                            item.m_wallsHorizontal.Add(num5, list4);
                        }
                    }
                }
                list.Add(item);
            }
            return list;
        }

        public void DeserializeMapIfNotYet()
        {
            if (((this.SerializedBlockAttrList != null) && (this.SerializedBlockAttrList.Length > 0)) && (this.ViewBlockAttrMap.Count == 0))
            {
                this.DeserializeViewBlockAttrMap();
            }
        }

        public void DeserializeViewBlockAttrMap()
        {
            this.ViewBlockAttrMap.Clear();
            if ((this.SerializedBlockAttrList != null) && (this.SerializedBlockAttrList.Length > 0))
            {
                foreach (SViewBlockAttrIndexed indexed in this.SerializedBlockAttrList)
                {
                    this.AddViewBlockAttr(indexed.ViewBlockId, indexed.Attr.BlockType, indexed.Attr.LightType);
                }
                if (Application.get_isPlaying())
                {
                    this.SerializedBlockAttrList = null;
                }
            }
        }

        public bool FindNearestGrid(VInt2 inCenterCell, VInt3 inCenterPosWorld, EViewBlockType inBlockType, int inThicknessMax, ActorRoot ar, out VInt2 result)
        {
            result = VInt2.zero;
            SViewBlockAttr outAttr = new SViewBlockAttr();
            bool flag = false;
            int inThickness = 1;
            List<VInt2> inSearchedGrids = new List<VInt2>();
            while (!flag && (inThickness <= inThicknessMax))
            {
                List<VInt2> inNeighbourGrids = new List<VInt2>();
                this.CollectNeighbourGrids(inNeighbourGrids, inSearchedGrids, inCenterCell, inThickness);
                inThickness++;
                inSearchedGrids.AddRange(inNeighbourGrids);
                if (inNeighbourGrids.Count > 0)
                {
                    for (int i = inNeighbourGrids.Count - 1; i >= 0; i--)
                    {
                        if (this.QueryAttr(inNeighbourGrids[i], out outAttr) && (outAttr.BlockType == inBlockType))
                        {
                            inNeighbourGrids.RemoveAt(i);
                        }
                    }
                }
                if (inNeighbourGrids.Count > 0)
                {
                    long num3 = 0x7fffffffL;
                    for (int j = 0; j < inNeighbourGrids.Count; j++)
                    {
                        VInt2 num5 = inNeighbourGrids[j];
                        VInt3 zero = VInt3.zero;
                        this.LevelGrid.GridToWorldPos(num5.x, num5.y, out zero);
                        VInt3 num9 = inCenterPosWorld - zero;
                        long sqrMagnitudeLong = num9.sqrMagnitudeLong;
                        if (sqrMagnitudeLong < num3)
                        {
                            if (ar == null)
                            {
                                flag = true;
                                num3 = sqrMagnitudeLong;
                                result = inNeighbourGrids[j];
                            }
                            else
                            {
                                VInt3 target = new VInt3(zero.x, 0, zero.y);
                                if (PathfindingUtility.IsValidTarget(ar, target))
                                {
                                    flag = true;
                                    num3 = sqrMagnitudeLong;
                                    result = inNeighbourGrids[j];
                                }
                            }
                        }
                    }
                }
            }
            return flag;
        }

        public bool FindNearestNotBrickFromWorldLocNonFow(ref VInt3 newPos, ActorRoot ar)
        {
            VInt3 inWorldPos = new VInt3(newPos.x, newPos.z, 0);
            VInt2 zero = VInt2.zero;
            if (this.LevelGrid.WorldPosToGrid(inWorldPos, out zero.x, out zero.y))
            {
                bool flag = false;
                SViewBlockAttr outAttr = new SViewBlockAttr();
                if (this.QueryAttr(zero, out outAttr) && (outAttr.BlockType == 2))
                {
                    flag = true;
                }
                else if (!PathfindingUtility.IsValidTarget(ar, newPos))
                {
                    flag = true;
                }
                if (!flag)
                {
                    return true;
                }
                VInt2 result = VInt2.zero;
                if (this.FindNearestGrid(zero, inWorldPos, EViewBlockType.Brick, 5, ar, out result))
                {
                    zero = result;
                    VInt3 outWorldPos = VInt3.zero;
                    this.LevelGrid.GridToWorldPos(zero.x, zero.y, out outWorldPos);
                    newPos = new VInt3(outWorldPos.x, newPos.y, outWorldPos.y);
                    return true;
                }
            }
            return false;
        }

        public int GetSurfaceCellX()
        {
            return this.PaneX;
        }

        public int GetSurfaceCellY()
        {
            return this.PaneY;
        }

        public void GridToUnrealX(int gridUnits, out int unrealUnits)
        {
            unrealUnits = gridUnits * this.PaneX;
        }

        public void GridToUnrealY(int gridUnits, out int unrealUnits)
        {
            unrealUnits = gridUnits * this.PaneY;
        }

        public void InitField()
        {
            if (this.bSynced)
            {
                DebugHelper.Assert(this.NumX > 0);
                DebugHelper.Assert(this.NumY > 0);
                DebugHelper.Assert(this.PaneX > 0);
                DebugHelper.Assert(this.PaneY > 0);
            }
            this.FieldX = this.NumX * this.PaneX;
            this.FieldY = this.NumY * this.PaneY;
            this.xMin = this.LevelGrid.GridInfo.GridPos.x;
            this.xMax = this.LevelGrid.GridInfo.GridPos.x + this.FieldX;
            this.yMin = this.LevelGrid.GridInfo.GridPos.y;
            this.yMax = this.LevelGrid.GridInfo.GridPos.y + this.FieldY;
            this.DeserializeViewBlockAttrMap();
        }

        public bool IsAreaPermanentLit(int x, int y, COM_PLAYERCAMP inCamp)
        {
            VInt2 inCell = new VInt2(x, y);
            SViewBlockAttr outAttr = new SViewBlockAttr();
            if (this.QueryAttr(inCell, out outAttr))
            {
                if (outAttr.LightType == 3)
                {
                    return true;
                }
                if ((outAttr.LightType == 1) && (inCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1))
                {
                    return true;
                }
                if ((outAttr.LightType == 2) && (inCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsAreaViewBlocking(byte areaId)
        {
            if (areaId == 0)
            {
                return false;
            }
            SViewBlockAttr attr = this.ViewBlockAttrMap[areaId];
            return (attr.BlockType > 0);
        }

        public bool IsCellViewBlocking(int surfCellX, int surfCellY, int unitAreadId)
        {
            int index = (surfCellY * this.LevelGrid.GridInfo.CellNumX) + surfCellX;
            FOGridCell cell = this.LevelGrid.GridCells[index];
            if (cell.m_viewBlockId == 0)
            {
                return false;
            }
            if ((unitAreadId != 0) && (unitAreadId == cell.m_viewBlockId))
            {
                return false;
            }
            return this.IsAreaViewBlocking(cell.m_viewBlockId);
        }

        private static void MergeBlockSegList(ref FowLos.SBlockContext blockContext, List<FowLos.SPolylineSegment> rawPolylineSegList, int inAreaId)
        {
            int count = rawPolylineSegList.Count;
            for (int i = 0; i < count; i++)
            {
                int item = blockContext.m_polylineSegList.Count;
                FowLos.SPolylineSegment segment = new FowLos.SPolylineSegment();
                segment.ParamlessConstruct();
                segment.m_belongBlockId = inAreaId;
                segment.m_index = item;
                FowLos.SPolylineSegment segment2 = rawPolylineSegList[i];
                VInt2 start = segment2.m_start;
                FowLos.SPolylineSegment segment3 = rawPolylineSegList[i];
                VInt2 end = segment3.m_end;
                int index = -1;
                if (blockContext.ContainsPoint(start, out index))
                {
                    segment.m_startPtIndex = index;
                    FowLos.SPolylineVertex vertex3 = blockContext.m_polylineVertices[index];
                    segment.m_start = vertex3.m_point;
                    FowLos.SPolylineVertex vertex4 = blockContext.m_polylineVertices[index];
                    DebugHelper.Assert(vertex4.m_belongSegNoList.Count > 0);
                    FowLos.SPolylineVertex vertex5 = blockContext.m_polylineVertices[index];
                    vertex5.m_belongSegNoList.Add(item);
                    FowLos.SPolylineVertex vertex6 = blockContext.m_polylineVertices[index];
                    DebugHelper.Assert(vertex6.m_belongSegNoList.Count <= 4);
                }
                else
                {
                    int num7 = blockContext.m_polylineVertices.Count;
                    FowLos.SPolylineVertex vertex = new FowLos.SPolylineVertex();
                    vertex.ParamlessConstruct();
                    vertex.m_point = start;
                    vertex.m_belongBlockId = inAreaId;
                    vertex.m_belongSegNoList.Add(item);
                    vertex.m_bNative = true;
                    segment.m_startPtIndex = num7;
                    segment.m_start = vertex.m_point;
                    blockContext.m_polylineVertices.Add(vertex);
                }
                if (blockContext.ContainsPoint(end, out index))
                {
                    segment.m_endPtIndex = index;
                    FowLos.SPolylineVertex vertex7 = blockContext.m_polylineVertices[index];
                    segment.m_end = vertex7.m_point;
                    FowLos.SPolylineVertex vertex8 = blockContext.m_polylineVertices[index];
                    DebugHelper.Assert(vertex8.m_belongSegNoList.Count > 0);
                    FowLos.SPolylineVertex vertex9 = blockContext.m_polylineVertices[index];
                    vertex9.m_belongSegNoList.Add(item);
                    FowLos.SPolylineVertex vertex10 = blockContext.m_polylineVertices[index];
                    DebugHelper.Assert(vertex10.m_belongSegNoList.Count <= 4);
                }
                else
                {
                    int num8 = blockContext.m_polylineVertices.Count;
                    FowLos.SPolylineVertex vertex2 = new FowLos.SPolylineVertex();
                    vertex2.ParamlessConstruct();
                    vertex2.m_point = end;
                    vertex2.m_belongBlockId = inAreaId;
                    vertex2.m_belongSegNoList.Add(item);
                    vertex2.m_bNative = true;
                    segment.m_endPtIndex = num8;
                    segment.m_end = vertex2.m_point;
                    blockContext.m_polylineVertices.Add(vertex2);
                }
                blockContext.m_polylineSegList.Add(segment);
            }
        }

        public bool QueryAttr(VInt2 inCell, out SViewBlockAttr outAttr)
        {
            return this.LevelGrid.GetGridCell(inCell).QueryAttr(this, out outAttr);
        }

        public bool QueryAttr(int x, int y, out SViewBlockAttr outAttr)
        {
            return this.LevelGrid.GetGridCell(x, y).QueryAttr(this, out outAttr);
        }

        public byte QueryViewblockId(VInt2 inCell)
        {
            return this.LevelGrid.GetGridCell(inCell.x, inCell.y).m_viewBlockId;
        }

        public void SerializeViewBlockAttrMap()
        {
            if (this.ViewBlockAttrMap.Count > 0)
            {
                this.SerializedBlockAttrList = new SViewBlockAttrIndexed[this.ViewBlockAttrMap.Count];
                Dictionary<byte, SViewBlockAttr>.Enumerator enumerator = this.ViewBlockAttrMap.GetEnumerator();
                int num = 0;
                while (enumerator.MoveNext())
                {
                    SViewBlockAttrIndexed indexed = new SViewBlockAttrIndexed();
                    KeyValuePair<byte, SViewBlockAttr> current = enumerator.Current;
                    indexed.ViewBlockId = current.Key;
                    KeyValuePair<byte, SViewBlockAttr> pair2 = enumerator.Current;
                    indexed.Attr = pair2.Value;
                    this.SerializedBlockAttrList[num++] = indexed;
                }
            }
            else
            {
                this.SerializedBlockAttrList = new SViewBlockAttrIndexed[0];
            }
        }

        public void SyncField(VInt2 inPos, int inFieldX, int inFieldY, int inCellSize)
        {
            int num = inFieldX / inCellSize;
            int num2 = inFieldY / inCellSize;
            DebugHelper.Assert(num <= 0xff);
            DebugHelper.Assert(num2 <= 0xff);
            this.LevelGrid.GridInfo.CellNumX = num;
            this.LevelGrid.GridInfo.CellNumY = num2;
            this.LevelGrid.GridInfo.CellSizeX = inFieldX / num;
            this.LevelGrid.GridInfo.CellSizeY = inFieldY / num2;
            this.LevelGrid.GridInfo.GridPos = inPos;
            this.LevelGrid.GridCells = new FOGridCell[this.LevelGrid.GridInfo.CellNumX * this.LevelGrid.GridInfo.CellNumY];
            int num3 = this.NumX * this.NumY;
            for (int i = 0; i < num3; i++)
            {
                this.LevelGrid.GridCells[i].CellX = (byte) (i % this.LevelGrid.GridInfo.CellNumX);
                this.LevelGrid.GridCells[i].CellY = (byte) (i / this.LevelGrid.GridInfo.CellNumX);
            }
            this.FieldX = this.NumX * this.PaneX;
            this.FieldY = this.NumY * this.PaneY;
            this.xMin = this.LevelGrid.GridInfo.GridPos.x;
            this.xMax = this.LevelGrid.GridInfo.GridPos.x + this.FieldX;
            this.yMin = this.LevelGrid.GridInfo.GridPos.y;
            this.yMax = this.LevelGrid.GridInfo.GridPos.y + this.FieldY;
        }

        public void UninitField()
        {
            if (this.ViewBlockArrayImpl != null)
            {
                this.ViewBlockArrayImpl.Clear();
                this.ViewBlockArrayImpl = null;
            }
            this.m_fowCells = null;
            this.ViewBlockAttrMap.Clear();
        }

        public void UnrealToGridX(int unrealUnits, out int gridUnits)
        {
            DebugHelper.Assert(this.PaneX > 0);
            gridUnits = unrealUnits / this.PaneX;
        }

        public void UnrealToGridY(int unrealUnits, out int gridUnits)
        {
            DebugHelper.Assert(this.PaneY > 0);
            gridUnits = unrealUnits / this.PaneY;
        }

        public bool bSynced
        {
            get
            {
                return ((this.LevelGrid.GridCells != null) && (this.LevelGrid.GridCells.Length > 0));
            }
        }

        public int FieldX
        {
            [CompilerGenerated]
            get
            {
                return this.<FieldX>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<FieldX>k__BackingField = value;
            }
        }

        public int FieldY
        {
            [CompilerGenerated]
            get
            {
                return this.<FieldY>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<FieldY>k__BackingField = value;
            }
        }

        public int NumX
        {
            get
            {
                return this.LevelGrid.GridInfo.CellNumX;
            }
        }

        public int NumY
        {
            get
            {
                return this.LevelGrid.GridInfo.CellNumY;
            }
        }

        public int PaneX
        {
            get
            {
                return this.LevelGrid.GridInfo.CellSizeX;
            }
        }

        public int PaneY
        {
            get
            {
                return this.LevelGrid.GridInfo.CellSizeY;
            }
        }

        public List<FowLos.SBlockWalls> ViewBlockArrayImpl
        {
            [CompilerGenerated]
            get
            {
                return this.<ViewBlockArrayImpl>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<ViewBlockArrayImpl>k__BackingField = value;
            }
        }

        public int xMax
        {
            [CompilerGenerated]
            get
            {
                return this.<xMax>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<xMax>k__BackingField = value;
            }
        }

        public int xMin
        {
            [CompilerGenerated]
            get
            {
                return this.<xMin>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<xMin>k__BackingField = value;
            }
        }

        public int yMax
        {
            [CompilerGenerated]
            get
            {
                return this.<yMax>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<yMax>k__BackingField = value;
            }
        }

        public int yMin
        {
            [CompilerGenerated]
            get
            {
                return this.<yMin>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<yMin>k__BackingField = value;
            }
        }

        public enum EViewBlockType : byte
        {
            Brick = 2,
            Grass = 1,
            None = 0
        }

        public enum EViewLightType : byte
        {
            None = 0,
            PermanentCamp1 = 1,
            PermanentCamp2 = 2,
            PermanentForAll = 3
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct FOGridCell
        {
            [SerializeField]
            public byte CellX;
            [SerializeField]
            public byte CellY;
            [SerializeField]
            public byte m_viewBlockId;
            public bool QueryAttr(FieldObj inFieldObj, out FieldObj.SViewBlockAttr outAttr)
            {
                return inFieldObj.ViewBlockAttrMap.TryGetValue(this.m_viewBlockId, out outAttr);
            }
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct FOGridInfo
        {
            [SerializeField]
            public int CellNumX;
            [SerializeField]
            public int CellNumY;
            [SerializeField]
            public int CellSizeX;
            [SerializeField]
            public int CellSizeY;
            [SerializeField]
            public VInt2 GridPos;
            public void Clear()
            {
                this.CellNumX = this.CellNumY = 0;
                this.CellSizeX = this.CellSizeY = 0;
                this.GridPos = VInt2.zero;
            }
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct FOLevelGrid
        {
            [SerializeField]
            public FieldObj.FOGridInfo GridInfo;
            [SerializeField]
            public FieldObj.FOGridCell[] GridCells;
            public void Clear()
            {
                this.GridCells = null;
                this.GridInfo.Clear();
            }

            public void SetViewBlockId(int index, byte inViewBlockId)
            {
                this.GridCells[index].m_viewBlockId = inViewBlockId;
            }

            public FieldObj.FOGridCell GetGridCell(VInt2 inCell)
            {
                return this.GetGridCell(inCell.x, inCell.y);
            }

            public FieldObj.FOGridCell GetGridCell(int X, int Y)
            {
                return this.GridCells[this.GetGridCellIndex(X, Y)];
            }

            public int GetGridCellIndex(int X, int Y)
            {
                return ((Y * this.GridInfo.CellNumX) + X);
            }

            public int GetGridCellX(int index)
            {
                return (index % this.GridInfo.CellNumX);
            }

            public int GetGridCellY(int index)
            {
                return (index / this.GridInfo.CellNumX);
            }

            public bool WorldPosToGrid(VInt3 inWorldPos, out int outCellX, out int outCellY)
            {
                outCellX = Mathf.Clamp((inWorldPos.x - this.GridInfo.GridPos.x) / this.GridInfo.CellSizeX, 0, this.GridInfo.CellNumX - 1);
                outCellY = Mathf.Clamp((inWorldPos.y - this.GridInfo.GridPos.y) / this.GridInfo.CellSizeY, 0, this.GridInfo.CellNumY - 1);
                return true;
            }

            public void GridToWorldPos(int inCellX, int inCellY, out VInt3 outWorldPos)
            {
                inCellX = Mathf.Clamp(inCellX, 0, this.GridInfo.CellNumX - 1);
                inCellY = Mathf.Clamp(inCellY, 0, this.GridInfo.CellNumY - 1);
                outWorldPos.x = this.GridInfo.GridPos.x + ((int) (this.GridInfo.CellSizeX * (inCellX + 0.5f)));
                outWorldPos.y = this.GridInfo.GridPos.y + ((int) (this.GridInfo.CellSizeY * (inCellY + 0.5f)));
                outWorldPos.z = 0;
            }

            public void ReviseWorldPosToCenter(VInt3 inWorldPos, out VInt3 outWorldPos)
            {
                int num;
                outWorldPos = VInt3.zero;
                int outCellY = 0;
                if (this.WorldPosToGrid(inWorldPos, out num, out outCellY))
                {
                    this.GridToWorldPos(num, outCellY, out outWorldPos);
                }
            }
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct SViewBlockAttr
        {
            public byte BlockType;
            public byte LightType;
            public SViewBlockAttr(byte inBlockType, byte inLightType)
            {
                this.BlockType = inBlockType;
                this.LightType = inLightType;
            }
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct SViewBlockAttrIndexed
        {
            [SerializeField]
            public byte ViewBlockId;
            [SerializeField]
            public FieldObj.SViewBlockAttr Attr;
        }
    }
}

