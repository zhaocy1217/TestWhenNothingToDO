namespace Assets.Scripts.GameSystem
{
    using System;
    using UnityEngine;

    public class FowMinimap
    {
        private float exploredAlpha = 0.4f;
        private Pixmap2D fowPixmap1 = null;
        private Pixmap2D fowTexExploredPixMap = null;
        private int scaledH = 0;
        private int scaledW = 0;
        private float unexploredAlpha = 0.2f;

        public Pixmap2D getFowTexture1()
        {
            return this.fowPixmap1;
        }

        public void incFowTextureAlphaSurface(VInt2 sPos, float alpha)
        {
            DebugHelper.Assert((sPos.x < this.fowPixmap1.getW()) && (sPos.y < this.fowPixmap1.getH()));
            if (this.fowPixmap1.getPixelf(sPos.x, sPos.y) < alpha)
            {
                this.fowPixmap1.setPixel(sPos.x, sPos.y, alpha);
            }
        }

        public void incFowTextureAlphaSurfaceExplored(VInt2 sPos, float alpha)
        {
            DebugHelper.Assert(sPos.x >= 0);
            DebugHelper.Assert(sPos.y >= 0);
            DebugHelper.Assert(sPos.x < this.fowTexExploredPixMap.getW());
            DebugHelper.Assert(sPos.y < this.fowTexExploredPixMap.getH());
            if (this.fowTexExploredPixMap.getPixelf(sPos.x, sPos.y) < alpha)
            {
                this.fowTexExploredPixMap.setPixel(sPos.x, sPos.y, alpha);
            }
        }

        public void Init(int w, int h, float inExploredAlpha, float inUnexploredAlpha)
        {
            this.exploredAlpha = inExploredAlpha;
            this.exploredAlpha = Mathf.Clamp(this.exploredAlpha, 0f, 1f);
            this.unexploredAlpha = inUnexploredAlpha;
            this.unexploredAlpha = Mathf.Clamp(this.unexploredAlpha, 0f, 1f);
            this.scaledW = w;
            this.scaledH = h;
            float[] numArray = new float[] { 0f };
            this.fowPixmap1 = new Pixmap2D(this.scaledW, this.scaledH, 1);
            this.fowPixmap1.setPixels(numArray);
            this.fowTexExploredPixMap = new Pixmap2D(this.scaledW, this.scaledH, 1);
            this.fowTexExploredPixMap.setPixels(numArray);
            DebugHelper.Assert(this.scaledW == this.fowPixmap1.getW());
            DebugHelper.Assert(this.scaledH == this.fowPixmap1.getH());
            this.InitFowTex(true);
        }

        private void InitFowTex(bool fogOfWar)
        {
            int num = this.fowPixmap1.getW();
            int num2 = this.fowPixmap1.getH();
            if (!fogOfWar)
            {
                for (int i = 0; i < num; i++)
                {
                    for (int j = 0; j < num2; j++)
                    {
                        this.fowPixmap1.setPixel(i, j, (float) 1f);
                        this.fowTexExploredPixMap.setPixel(i, j, (float) 1f);
                    }
                }
            }
            else
            {
                for (int k = 0; k < num; k++)
                {
                    for (int m = 0; m < num2; m++)
                    {
                        this.fowPixmap1.setPixel(k, m, this.unexploredAlpha);
                        this.fowTexExploredPixMap.setPixel(k, m, this.unexploredAlpha);
                    }
                }
            }
        }

        public float QueryExploredAlpha()
        {
            return this.exploredAlpha;
        }

        public float QueryUnexploredAlpha()
        {
            return this.unexploredAlpha;
        }

        public void SyncExploredToLatest()
        {
            this.fowPixmap1.copy(this.fowTexExploredPixMap);
        }

        public void UnInit()
        {
            if (this.fowPixmap1 != null)
            {
                this.fowPixmap1.UnInit();
            }
            this.fowPixmap1 = null;
            if (this.fowTexExploredPixMap != null)
            {
                this.fowTexExploredPixMap.UnInit();
            }
            this.fowTexExploredPixMap = null;
            this.scaledW = 0;
            this.scaledH = 0;
            this.exploredAlpha = 0.4f;
            this.unexploredAlpha = 0.2f;
        }

        private enum ExplorationState
        {
            esNotExplored,
            esExplored,
            esVisible
        }
    }
}

