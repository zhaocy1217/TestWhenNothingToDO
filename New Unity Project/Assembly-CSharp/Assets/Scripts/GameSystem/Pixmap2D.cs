namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Runtime.InteropServices;

    public class Pixmap2D
    {
        private int components;
        private int h;
        private byte[] pixels;
        private int w;

        public Pixmap2D(int components)
        {
            this.h = -1;
            this.w = -1;
            this.components = -1;
            this.init(components);
        }

        public Pixmap2D(int w, int h, int components)
        {
            this.h = -1;
            this.w = -1;
            this.components = -1;
            this.init(w, h, components);
        }

        public void copy(Pixmap2D sourcePixmap)
        {
            DebugHelper.Assert(this.components == sourcePixmap.getComponents());
            if ((this.w == sourcePixmap.getW()) && (this.h == sourcePixmap.getH()))
            {
                for (int i = 0; i < this.pixels.Length; i++)
                {
                    this.pixels[i] = sourcePixmap.pixels[i];
                }
            }
        }

        private bool doDimensionsAgree(Pixmap2D pixmap)
        {
            return ((pixmap.getW() == this.w) && (pixmap.getH() == this.h));
        }

        public void getComponent(int x, int y, int component, out byte value)
        {
            value = this.pixels[(((this.w * y) + x) * this.components) + component];
        }

        public void getComponent(int x, int y, int component, out float value)
        {
            value = ((float) this.pixels[(((this.w * y) + x) * this.components) + component]) / 255f;
        }

        public float getComponentf(int x, int y, int component)
        {
            float num;
            this.getComponent(x, y, component, out num);
            return num;
        }

        public int getComponents()
        {
            return this.components;
        }

        public int getH()
        {
            return this.h;
        }

        public void getPixel(int x, int y, byte[] value)
        {
            for (int i = 0; i < this.components; i++)
            {
                value[i] = this.pixels[(((this.w * y) + x) * this.components) + i];
            }
        }

        public void getPixel(int x, int y, float[] value)
        {
            for (int i = 0; i < this.components; i++)
            {
                value[i] = ((float) this.pixels[(((this.w * y) + x) * this.components) + i]) / 255f;
            }
        }

        public float getPixelf(int x, int y)
        {
            return (((float) this.pixels[((this.w * y) + x) * this.components]) / 255f);
        }

        public byte[] getPixels()
        {
            return this.pixels;
        }

        public int getW()
        {
            return this.w;
        }

        public void init(int inComp)
        {
            this.w = -1;
            this.h = -1;
            this.components = inComp;
            this.pixels = null;
        }

        public void init(int w, int h, int inComp)
        {
            this.w = w;
            this.h = h;
            this.components = inComp;
            this.pixels = new byte[(h * w) * this.components];
        }

        private void load(string path)
        {
        }

        private void loadBmp(string path)
        {
        }

        private void loadTga(string path)
        {
        }

        private void save(string path)
        {
        }

        private void saveBmp(string path)
        {
        }

        private void saveTga(string path)
        {
        }

        public void setComponent(int x, int y, int component, byte value)
        {
            this.pixels[(((this.w * y) + x) * this.components) + component] = value;
        }

        public void setComponent(int x, int y, int component, float value)
        {
            this.pixels[(((this.w * y) + x) * this.components) + component] = (byte) (value * 255f);
        }

        public void setComponents(int component, byte value)
        {
            DebugHelper.Assert(component < this.components);
            for (int i = 0; i < this.w; i++)
            {
                for (int j = 0; j < this.h; j++)
                {
                    this.setComponent(i, j, component, value);
                }
            }
        }

        public void setComponents(int component, float value)
        {
            DebugHelper.Assert(component < this.components);
            for (int i = 0; i < this.w; i++)
            {
                for (int j = 0; j < this.h; j++)
                {
                    this.setComponent(i, j, component, value);
                }
            }
        }

        public void setPixel(int x, int y, byte p)
        {
            this.pixels[((this.w * y) + x) * this.components] = p;
        }

        public void setPixel(int x, int y, float p)
        {
            this.pixels[((this.w * y) + x) * this.components] = (byte) (p * 255f);
        }

        public void setPixel(int x, int y, byte[] value)
        {
            for (int i = 0; i < this.components; i++)
            {
                this.pixels[(((this.w * y) + x) * this.components) + i] = value[i];
            }
        }

        public void setPixel(int x, int y, float[] value)
        {
            for (int i = 0; i < this.components; i++)
            {
                this.pixels[(((this.w * y) + x) * this.components) + i] = (byte) (value[i] * 255f);
            }
        }

        public void setPixels(byte[] value)
        {
            for (int i = 0; i < this.w; i++)
            {
                for (int j = 0; j < this.h; j++)
                {
                    this.setPixel(i, j, value);
                }
            }
        }

        public void setPixels(float[] value)
        {
            for (int i = 0; i < this.w; i++)
            {
                for (int j = 0; j < this.h; j++)
                {
                    this.setPixel(i, j, value);
                }
            }
        }

        public void UnInit()
        {
            this.h = -1;
            this.w = -1;
            this.components = -1;
            this.pixels = null;
        }
    }
}

