namespace Assets.Scripts.GameSystem
{
    using CSProtocol;
    using System;

    public class CampFowMapData
    {
        public COM_PLAYERCAMP m_camp;
        public byte[] m_explored;
        public byte[] m_permanentLit;
        public byte[] m_visible;

        public void Init(int surfaceW, int surfaceH, COM_PLAYERCAMP camp)
        {
            int num = surfaceW * surfaceH;
            this.m_camp = camp;
            this.m_visible = new byte[num];
            this.ResetVisible(false);
            this.m_explored = new byte[num];
            this.ResetExplored(false);
            this.m_permanentLit = null;
        }

        public bool IsExplored(int index)
        {
            return ((((this.m_explored != null) && (index >= 0)) && (index < this.m_explored.Length)) && (this.m_explored[index] > 0));
        }

        public bool IsVisible(int index)
        {
            return ((((this.m_visible != null) && (index >= 0)) && (index < this.m_visible.Length)) && (this.m_visible[index] > 0));
        }

        public void ResetExplored(bool bExplored)
        {
            if (this.m_explored != null)
            {
                int length = this.m_explored.Length;
                for (int i = 0; i < length; i++)
                {
                    this.m_explored[i] = !bExplored ? ((byte) 0) : ((byte) 1);
                }
            }
        }

        public void ResetVisible(bool bVisible)
        {
            if (this.m_visible != null)
            {
                int length = this.m_visible.Length;
                for (int i = 0; i < length; i++)
                {
                    this.m_visible[i] = !bVisible ? ((byte) 0) : ((byte) 1);
                }
            }
        }

        public void SetExplored(bool bExplored, int index)
        {
            if (((this.m_explored != null) && (index >= 0)) && (index < this.m_explored.Length))
            {
                this.m_explored[index] = !bExplored ? ((byte) 0) : ((byte) 1);
            }
        }

        public void SetVisible(bool bVisible, int index)
        {
            if (((this.m_visible != null) && (index >= 0)) && (index < this.m_visible.Length))
            {
                this.m_visible[index] = !bVisible ? ((byte) 0) : ((byte) 1);
            }
        }

        public void SyncPermanentToExplored()
        {
            if (this.m_explored != null)
            {
                if (this.m_permanentLit != null)
                {
                    int length = this.m_explored.Length;
                    for (int i = 0; i < length; i++)
                    {
                        this.m_explored[i] = this.m_permanentLit[i];
                    }
                }
                else
                {
                    this.ResetExplored(false);
                }
            }
        }

        public void SyncPermanentToVisible()
        {
            if (this.m_visible != null)
            {
                if (this.m_permanentLit != null)
                {
                    int length = this.m_visible.Length;
                    for (int i = 0; i < length; i++)
                    {
                        this.m_visible[i] = this.m_permanentLit[i];
                    }
                }
                else
                {
                    this.ResetVisible(false);
                }
            }
        }

        public void UnInit()
        {
            this.m_visible = null;
            this.m_explored = null;
            this.m_camp = COM_PLAYERCAMP.COM_PLAYERCAMP_MID;
            this.m_permanentLit = null;
        }
    }
}

