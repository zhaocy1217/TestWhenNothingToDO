namespace Assets.Scripts.GameSystem
{
    using CSProtocol;
    using System;

    public class CMentorListOffset
    {
        public bool m_isEnd = true;
        public uint m_lastNum;
        public CS_STUDENTLIST_TYPE m_type;

        public bool needShowMore()
        {
            return !this.m_isEnd;
        }
    }
}

