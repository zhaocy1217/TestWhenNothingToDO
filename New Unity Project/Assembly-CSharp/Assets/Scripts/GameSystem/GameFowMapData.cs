namespace Assets.Scripts.GameSystem
{
    using CSProtocol;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class GameFowMapData
    {
        [CompilerGenerated]
        private FowMinimap <m_pMinimap>k__BackingField;
        private CampFowMapData[] m_campFowMapDataArr = new CampFowMapData[3];
        public FOWSurfCell[] m_fowCells;
        public int m_surfaceH;
        public int m_surfaceW;

        private int GetSurfaceCellIndex(int w, int h)
        {
            DebugHelper.Assert(this.IsInsideSurface(w, h));
            return ((h * this.m_surfaceW) + w);
        }

        private void InitPermanentLit(COM_PLAYERCAMP camp)
        {
        }

        public void InitSurface(int w, int h)
        {
            this.m_fowCells = null;
            this.m_surfaceW = w;
            this.m_surfaceH = h;
            for (int i = 0; i < 3; i++)
            {
                this.m_campFowMapDataArr[i] = new CampFowMapData();
                this.m_campFowMapDataArr[i].Init(this.m_surfaceW, this.m_surfaceH, (COM_PLAYERCAMP) i);
            }
            float inExploredAlpha = 0.4f;
            float inUnexploredAlpha = 0.2f;
            this.m_pMinimap = new FowMinimap();
            this.m_pMinimap.Init(w, h, inExploredAlpha, inUnexploredAlpha);
        }

        public bool IsExplored(int w, int h, COM_PLAYERCAMP camp)
        {
            int index = (int) camp;
            if ((index >= 0) && (index < this.m_campFowMapDataArr.Length))
            {
                CampFowMapData data = this.m_campFowMapDataArr[index];
                if (data != null)
                {
                    return data.IsExplored(this.GetSurfaceCellIndex(w, h));
                }
            }
            return false;
        }

        public bool IsInsideSurface(int sx, int sy)
        {
            return ((((sx >= 0) && (sy >= 0)) && (sx < this.m_surfaceW)) && (sy < this.m_surfaceH));
        }

        public bool IsVisible(int w, int h, COM_PLAYERCAMP camp)
        {
            int index = (int) camp;
            if ((index >= 0) && (index < this.m_campFowMapDataArr.Length))
            {
                CampFowMapData data = this.m_campFowMapDataArr[index];
                if (data != null)
                {
                    return data.IsVisible(this.GetSurfaceCellIndex(w, h));
                }
            }
            return false;
        }

        public void PrecomputeData(GameFowManager fowMgr)
        {
            this.PrecomputeDataInternal(fowMgr);
        }

        private void PrecomputeDataInternal(GameFowManager fowMgr)
        {
            if ((this.m_fowCells == null) && ((fowMgr != null) && fowMgr.GUsePrecomputedFOW))
            {
                if (Application.get_isPlaying())
                {
                    FOGameFowOfflineSerializer serializer = new FOGameFowOfflineSerializer(fowMgr);
                    if (serializer.TryLoad())
                    {
                        return;
                    }
                }
                int num = this.m_surfaceW * this.m_surfaceH;
                int gridUnits = 0;
                fowMgr.m_pFieldObj.UnrealToGridX(0x2ee0, out gridUnits);
                this.m_fowCells = new FOWSurfCell[num];
                for (int i = 0; i < this.m_surfaceH; i++)
                {
                    for (int k = 0; k < this.m_surfaceW; k++)
                    {
                        int index = (i * this.m_surfaceW) + k;
                        this.m_fowCells[index].rect.set_xMin((float) Mathf.Max(0, k - gridUnits));
                        this.m_fowCells[index].rect.set_xMax((float) Mathf.Min(this.m_surfaceW - 1, k + gridUnits));
                        this.m_fowCells[index].rect.set_yMin((float) Mathf.Max(0, i - gridUnits));
                        this.m_fowCells[index].rect.set_yMax((float) Mathf.Min(this.m_surfaceH - 1, i + gridUnits));
                        this.m_fowCells[index].Init();
                    }
                }
                List<VInt2> visibleSurfCellList = new List<VInt2>();
                VInt3 zero = VInt3.zero;
                for (int j = 0; j < this.m_surfaceH; j++)
                {
                    int num8 = j * this.m_surfaceW;
                    for (int m = 0; m < this.m_surfaceW; m++)
                    {
                        int num10 = num8 + m;
                        fowMgr.GridToWorldPos(m, j, out zero);
                        fowMgr.m_los.ExploreCellsInternal<FOWSurfCell>(ref this.m_fowCells[num10], new VInt2(m, j), zero, 0x2ee0, visibleSurfCellList, fowMgr, COM_PLAYERCAMP.COM_PLAYERCAMP_MID, false);
                        this.m_fowCells[num10].finished = true;
                    }
                }
            }
        }

        public void ResetExplored(bool bExplored, COM_PLAYERCAMP camp)
        {
            int index = (int) camp;
            if ((index >= 0) && (index < this.m_campFowMapDataArr.Length))
            {
                CampFowMapData data = this.m_campFowMapDataArr[index];
                if (data != null)
                {
                    data.ResetExplored(bExplored);
                }
            }
        }

        public void ResetVisible(bool bVisible, COM_PLAYERCAMP camp)
        {
            int index = (int) camp;
            if ((index >= 0) && (index < this.m_campFowMapDataArr.Length))
            {
                CampFowMapData data = this.m_campFowMapDataArr[index];
                if (data != null)
                {
                    data.ResetVisible(bVisible);
                }
            }
        }

        public void SetExplored(bool bExplored, int w, int h, COM_PLAYERCAMP camp)
        {
            int index = (int) camp;
            if ((index >= 0) && (index < this.m_campFowMapDataArr.Length))
            {
                CampFowMapData data = this.m_campFowMapDataArr[index];
                if (data != null)
                {
                    data.SetExplored(bExplored, this.GetSurfaceCellIndex(w, h));
                }
            }
        }

        public void SetVisible(bool bVisible, int w, int h, COM_PLAYERCAMP camp)
        {
            int index = (int) camp;
            if ((index >= 0) && (index < this.m_campFowMapDataArr.Length))
            {
                CampFowMapData data = this.m_campFowMapDataArr[index];
                if (data != null)
                {
                    data.SetVisible(bVisible, this.GetSurfaceCellIndex(w, h));
                }
            }
        }

        public void SyncPermanentToExplored(COM_PLAYERCAMP camp)
        {
            int index = (int) camp;
            if ((index >= 0) && (index < this.m_campFowMapDataArr.Length))
            {
                CampFowMapData data = this.m_campFowMapDataArr[index];
                if (data != null)
                {
                    data.SyncPermanentToExplored();
                }
            }
        }

        public void SyncPermanentToVisible(COM_PLAYERCAMP camp)
        {
            int index = (int) camp;
            if ((index >= 0) && (index < this.m_campFowMapDataArr.Length))
            {
                CampFowMapData data = this.m_campFowMapDataArr[index];
                if (data != null)
                {
                    data.SyncPermanentToVisible();
                }
            }
        }

        public void UninitSurface()
        {
            this.m_fowCells = null;
            if (this.m_pMinimap != null)
            {
                this.m_pMinimap.UnInit();
                this.m_pMinimap = null;
            }
            for (int i = 0; i < 3; i++)
            {
                if (this.m_campFowMapDataArr[i] != null)
                {
                    this.m_campFowMapDataArr[i].UnInit();
                    this.m_campFowMapDataArr[i] = null;
                }
            }
        }

        public FowMinimap m_pMinimap
        {
            [CompilerGenerated]
            get
            {
                return this.<m_pMinimap>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<m_pMinimap>k__BackingField = value;
            }
        }
    }
}

