namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct stEquipTree
    {
        public ushort m_rootEquipID;
        public uint m_2ndEquipCount;
        public ushort[] m_2ndEquipIDs;
        public uint[] m_3rdEquipCounts;
        public ushort[][] m_3rdEquipIDs;
        public stEquipTree(int _2ndEquipMaxCount, int _3rdEquipMaxCountPer2ndEquip, int backEquipMaxCount)
        {
            this.m_rootEquipID = 0;
            this.m_2ndEquipCount = 0;
            this.m_2ndEquipIDs = new ushort[_2ndEquipMaxCount];
            this.m_3rdEquipCounts = new uint[_2ndEquipMaxCount];
            this.m_3rdEquipIDs = new ushort[_2ndEquipMaxCount][];
            for (int i = 0; i < _2ndEquipMaxCount; i++)
            {
                this.m_3rdEquipCounts[i] = 0;
                this.m_3rdEquipIDs[i] = new ushort[_3rdEquipMaxCountPer2ndEquip];
            }
        }

        public void Clear()
        {
            this.m_rootEquipID = 0;
            this.m_2ndEquipCount = 0;
            for (int i = 0; i < this.m_2ndEquipIDs.Length; i++)
            {
                this.m_2ndEquipIDs[i] = 0;
            }
            for (int j = 0; j < this.m_3rdEquipCounts.Length; j++)
            {
                this.m_3rdEquipCounts[j] = 0;
            }
            for (int k = 0; k < this.m_3rdEquipIDs.Length; k++)
            {
                for (int m = 0; m < this.m_3rdEquipIDs[k].Length; m++)
                {
                    this.m_3rdEquipIDs[k][m] = 0;
                }
            }
        }

        public void Create(ushort rootEquipID, Dictionary<ushort, CEquipInfo> equipInfoDictionary)
        {
            this.Clear();
            if (rootEquipID != 0)
            {
                this.m_rootEquipID = rootEquipID;
                CEquipInfo info = null;
                if (equipInfoDictionary.TryGetValue(rootEquipID, out info))
                {
                    ushort key = 0;
                    CEquipInfo info2 = null;
                    for (int i = 0; i < info.m_resEquipInBattle.PreEquipID.Length; i++)
                    {
                        key = info.m_resEquipInBattle.PreEquipID[i];
                        if (((key > 0) && equipInfoDictionary.TryGetValue(key, out info2)) && (info2.m_resEquipInBattle.bInvalid == 0))
                        {
                            this.m_2ndEquipIDs[this.m_2ndEquipCount] = key;
                            ushort num3 = 0;
                            CEquipInfo info3 = null;
                            for (int j = 0; j < info2.m_resEquipInBattle.PreEquipID.Length; j++)
                            {
                                num3 = info2.m_resEquipInBattle.PreEquipID[j];
                                if (((num3 > 0) && equipInfoDictionary.TryGetValue(num3, out info3)) && (info3.m_resEquipInBattle.bInvalid == 0))
                                {
                                    this.m_3rdEquipIDs[this.m_2ndEquipCount][this.m_3rdEquipCounts[this.m_2ndEquipCount]] = num3;
                                    this.m_3rdEquipCounts[this.m_2ndEquipCount]++;
                                }
                            }
                            this.m_2ndEquipCount++;
                        }
                    }
                }
            }
        }
    }
}

