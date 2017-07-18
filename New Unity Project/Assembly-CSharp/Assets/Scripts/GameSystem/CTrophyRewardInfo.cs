namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using System.Runtime.InteropServices;

    public class CTrophyRewardInfo
    {
        public ResTrophyLvl Cfg;
        public int Index;
        public uint MaxPoint;
        public uint MinPoint;
        public TrophyState State;

        public CTrophyRewardInfo(ResTrophyLvl resTrophy, TrophyState state = 3, int index = 0, uint minPoint = 0)
        {
            this.Cfg = resTrophy;
            this.State = state;
            this.Index = index;
            this.MinPoint = minPoint;
            this.MaxPoint = this.Cfg.dwTrophyScore;
        }

        public uint GetPointStep()
        {
            return (this.MaxPoint - this.MinPoint);
        }

        public string GetTrophyImagePath()
        {
            return string.Format("{0}{1}", CUIUtility.s_Sprite_System_BattleEquip_Dir, this.Cfg.dwImage);
        }

        public CUseable[] GetTrophyRewards()
        {
            ListView<CUseable> view = new ListView<CUseable>();
            for (int i = 0; i < this.Cfg.astReqReward.Length; i++)
            {
                if (this.Cfg.astReqReward[i].dwRewardNum > 0)
                {
                    view.Add(CUseableManager.CreateUsableByServerType((RES_REWARDS_TYPE) this.Cfg.astReqReward[i].bRewardType, (int) this.Cfg.astReqReward[i].dwRewardNum, this.Cfg.astReqReward[i].dwRewardID));
                }
            }
            CUseable[] useableArray = new CUseable[view.Count];
            for (int j = 0; j < view.Count; j++)
            {
                useableArray[j] = view[j];
            }
            return useableArray;
        }

        public bool HasGotAward()
        {
            return (this.State == TrophyState.GotRewards);
        }

        public bool IsFinish()
        {
            return ((this.State == TrophyState.Finished) || (this.State == TrophyState.GotRewards));
        }

        public bool IsRewardConfiged()
        {
            uint num = 0;
            for (int i = 0; i < 3; i++)
            {
                if (this.Cfg.astReqReward[i].dwRewardNum != 0)
                {
                    num++;
                }
            }
            return (num > 0);
        }
    }
}

