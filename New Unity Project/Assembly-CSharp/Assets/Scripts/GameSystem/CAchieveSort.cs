namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Collections.Generic;

    public class CAchieveSort : IComparer<CAchieveItem2>
    {
        public int Compare(CAchieveItem2 l, CAchieveItem2 r)
        {
            uint mostRecentlyModifyTime = l.GetMostRecentlyModifyTime();
            uint num2 = r.GetMostRecentlyModifyTime();
            return ((mostRecentlyModifyTime > num2) ? -1 : 1);
        }
    }
}

