using Assets.Scripts.GameSystem;
using System;
using System.IO;
using UnityEngine;

[ObjectTypeSerializer(typeof(FieldObj.FOLevelGrid))]
public class FOLevelGridSerializer : ICustomizedObjectSerializer
{
    private static string NODE_NAME_FOLevelGrid = "FOLevelGrid";

    public bool IsObjectTheSame(object o, object oPrefab)
    {
        return false;
    }

    public void ObjectDeserialize(ref object o, BinaryNode node)
    {
        BinaryNode child = node.GetChild(0);
        if (child == null)
        {
            Debug.LogError("Deserialize FieldObj.FOLevelGrid Failed, child binary node is null");
        }
        else
        {
            MemoryStream input = new MemoryStream(child.GetValue());
            BinaryReader reader = new BinaryReader(input);
            FieldObj.FOLevelGrid grid = new FieldObj.FOLevelGrid();
            grid.GridInfo = new FieldObj.FOGridInfo();
            grid.GridInfo.CellNumX = UnityBasetypeSerializer.BytesToInt(reader.ReadBytes(4));
            grid.GridInfo.CellNumY = UnityBasetypeSerializer.BytesToInt(reader.ReadBytes(4));
            grid.GridInfo.CellSizeX = UnityBasetypeSerializer.BytesToInt(reader.ReadBytes(4));
            grid.GridInfo.CellSizeY = UnityBasetypeSerializer.BytesToInt(reader.ReadBytes(4));
            grid.GridInfo.GridPos = new VInt2();
            grid.GridInfo.GridPos.x = UnityBasetypeSerializer.BytesToInt(reader.ReadBytes(4));
            grid.GridInfo.GridPos.y = UnityBasetypeSerializer.BytesToInt(reader.ReadBytes(4));
            int num = UnityBasetypeSerializer.BytesToInt(reader.ReadBytes(4));
            grid.GridCells = new FieldObj.FOGridCell[num];
            for (int i = 0; i < num; i++)
            {
                grid.GridCells[i] = new FieldObj.FOGridCell();
                grid.GridCells[i].CellX = reader.ReadByte();
                grid.GridCells[i].CellY = reader.ReadByte();
                grid.GridCells[i].m_viewBlockId = reader.ReadByte();
            }
            o = grid;
        }
    }
}

