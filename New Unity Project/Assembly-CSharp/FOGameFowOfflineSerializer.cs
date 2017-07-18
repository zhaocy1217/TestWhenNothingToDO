using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

internal class FOGameFowOfflineSerializer
{
    private uint CrcCode = 0;
    public const uint CURRENT_VERIFICATIONCODE = 0xcdcdcdcd;
    public const uint CURRENT_VERSION = 0x10;
    private const int ReadWriteChecker = 0x3ade68b1;
    private uint VerificationCode = 0xcdcdcdcd;
    private uint Version = 0x10;

    public static int[] ByteArrToIntArr(byte[] bytArr)
    {
        int length = bytArr.Length;
        List<int> list = new List<int>();
        for (int i = 0; i < length; i += 4)
        {
            int item = 0;
            int num4 = bytArr[i];
            int num5 = bytArr[i + 1];
            num5 = num5 << 8;
            int num6 = bytArr[i + 2];
            num6 = num6 << 0x10;
            int num7 = bytArr[i + 3];
            num7 = num7 << 0x18;
            item |= num4;
            item |= num5;
            item |= num6;
            item |= num7;
            list.Add(item);
        }
        int[] numArray = list.ToArray();
        DebugHelper.Assert(numArray.Length == (length / 4));
        return numArray;
    }

    public static uint[] ByteArrToUIntArr(byte[] bytArr)
    {
        int length = bytArr.Length;
        List<uint> list = new List<uint>();
        for (int i = 0; i < length; i += 4)
        {
            uint item = 0;
            uint num4 = bytArr[i];
            uint num5 = bytArr[i + 1];
            num5 = num5 << 8;
            uint num6 = bytArr[i + 2];
            num6 = num6 << 0x10;
            uint num7 = bytArr[i + 3];
            num7 = num7 << 0x18;
            item |= num4;
            item |= num5;
            item |= num6;
            item |= num7;
            list.Add(item);
        }
        uint[] numArray = list.ToArray();
        DebugHelper.Assert(numArray.Length == (length / 4));
        return numArray;
    }

    public static byte[] IntArrToByteArr(int[] intArr)
    {
        int cb = 4 * intArr.Length;
        byte[] destination = new byte[cb];
        IntPtr ptr = Marshal.AllocHGlobal(cb);
        Marshal.Copy(intArr, 0, ptr, intArr.Length);
        Marshal.Copy(ptr, destination, 0, destination.Length);
        Marshal.FreeHGlobal(ptr);
        return destination;
    }

    public bool SaveTo(FieldObj inFieldObj)
    {
        DebugHelper.Assert(inFieldObj != null);
        DebugHelper.Assert(inFieldObj.m_fowCells != null);
        MemoryStream output = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(output);
        writer.Write(this.VerificationCode);
        writer.Write(this.Version);
        writer.Write(this.CrcCode);
        int num = inFieldObj.NumX * inFieldObj.NumY;
        writer.Write(num);
        for (int i = 0; i < num; i++)
        {
            FOWSurfCell cell = inFieldObj.m_fowCells[i];
            if (cell.bValid)
            {
                int dataSize = cell.GetDataSize();
                writer.Write(dataSize);
                DebugHelper.Assert(cell.data != null);
                int[] intArr = new int[cell.data.Length];
                for (int j = 0; j < intArr.Length; j++)
                {
                    intArr[j] = (int) cell.data[j];
                }
                byte[] buffer = IntArrToByteArr(intArr);
                writer.Write(buffer, 0, buffer.Length);
            }
            else
            {
                int num5 = 0;
                writer.Write(num5);
            }
        }
        writer.Write(0x3ade68b1);
        writer.Flush();
        output.Flush();
        inFieldObj.fowOfflineData = output.ToArray();
        writer.Close();
        output.Close();
        return true;
    }

    public static void TestByteIntArrTrans()
    {
        int[] intArr = new int[] { 0xd05, 0x4a2cb71 };
        byte[] bytArr = IntArrToByteArr(intArr);
        DebugHelper.Assert(bytArr.Length == 8);
        int[] numArray2 = ByteArrToIntArr(bytArr);
        DebugHelper.Assert(numArray2.Length == intArr.Length);
        DebugHelper.Assert(numArray2[0] == intArr[0]);
        DebugHelper.Assert(numArray2[1] == intArr[1]);
        uint[] numArray3 = new uint[] { 0xc355, 0x98967f };
        intArr[0] = (int) numArray3[0];
        intArr[1] = (int) numArray3[1];
        byte[] buffer2 = IntArrToByteArr(intArr);
        DebugHelper.Assert(buffer2.Length == 8);
        uint[] numArray4 = ByteArrToUIntArr(buffer2);
        DebugHelper.Assert(numArray4.Length == numArray3.Length);
        DebugHelper.Assert(numArray4[0] == numArray3[0]);
        DebugHelper.Assert(numArray4[1] == numArray3[1]);
    }

    public bool TryLoad(FieldObj inFieldObj)
    {
        DebugHelper.Assert(inFieldObj != null);
        if ((inFieldObj.fowOfflineData == null) || (inFieldObj.fowOfflineData.Length == 0))
        {
            return false;
        }
        MemoryStream input = new MemoryStream(inFieldObj.fowOfflineData);
        BinaryReader reader = new BinaryReader(input);
        uint num = reader.ReadUInt32();
        uint num2 = reader.ReadUInt32();
        uint num3 = reader.ReadUInt32();
        if ((num != this.VerificationCode) || (this.Version != num2))
        {
            return false;
        }
        int cellCnt = inFieldObj.NumX * inFieldObj.NumY;
        int num5 = reader.ReadInt32();
        if (cellCnt != num5)
        {
            return false;
        }
        GameFowManager.InitSurfCellsArray(cellCnt);
        GameFowManager.InitLevelGrid(cellCnt, inFieldObj.LevelGrid.GridInfo.CellNumX, inFieldObj.LevelGrid.GridInfo.CellNumY, inFieldObj.LevelGrid.GridInfo.CellSizeX, inFieldObj.LevelGrid.GridInfo.CellSizeY, inFieldObj.LevelGrid.GridInfo.GridPos.x, inFieldObj.LevelGrid.GridInfo.GridPos.y);
        int gridUnits = 0;
        inFieldObj.UnrealToGridX(Horizon.QueryGlobalSight(), out gridUnits);
        for (int i = 0; i < cellCnt; i++)
        {
            int gridCellX = inFieldObj.LevelGrid.GetGridCellX(i);
            int gridCellY = inFieldObj.LevelGrid.GetGridCellY(i);
            int xMin = Mathf.Max(0, gridCellX - gridUnits);
            int xMax = Mathf.Min(inFieldObj.NumX - 1, gridCellX + gridUnits);
            int yMin = Mathf.Max(0, gridCellY - gridUnits);
            int yMax = Mathf.Min(inFieldObj.NumY - 1, gridCellY + gridUnits);
            int count = reader.ReadInt32();
            if (count > 0)
            {
                byte[] buffer = new byte[count];
                reader.Read(buffer, 0, count);
                DebugHelper.Assert(ByteArrToUIntArr(buffer).Length == (count / 4));
                GameFowManager.InitSurfCell(i, xMin, xMax, yMin, yMax, true);
                IntPtr destination = Marshal.AllocHGlobal(count);
                Marshal.Copy(buffer, 0, destination, count);
                GameFowManager.SetSurfCellData(i, destination);
                Marshal.FreeHGlobal(destination);
            }
            else
            {
                GameFowManager.InitSurfCell(i, xMin, xMax, yMin, yMax, false);
            }
            FieldObj.SViewBlockAttr outAttr = new FieldObj.SViewBlockAttr();
            inFieldObj.QueryAttr(gridCellX, gridCellY, out outAttr);
            GameFowManager.InitLevelGridCell(i, outAttr.BlockType, outAttr.LightType);
        }
        if (reader.ReadInt32() != 0x3ade68b1)
        {
        }
        reader.Close();
        input.Close();
        return true;
    }
}

