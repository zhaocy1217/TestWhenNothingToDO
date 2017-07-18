namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.InteropServices;

    public class CAchieveItem2
    {
        public ResAchievement Cfg;
        public int DoneCnt;
        public uint DoneTime;
        public RES_ACHIEVE_DONE_TYPE DoneType;
        public uint ID;
        public CAchieveItem2 Next;
        public CAchieveItem2 Prev;
        public uint PrevID;
        public COM_ACHIEVEMENT_STATE State;

        public CAchieveItem2(ref ResAchievement achievement)
        {
            this.ID = achievement.dwID;
            this.DoneTime = 0;
            this.State = COM_ACHIEVEMENT_STATE.COM_ACHIEVEMENT_STATE_UNFIN;
            this.DoneType = (RES_ACHIEVE_DONE_TYPE) achievement.dwDoneType;
            this.Cfg = achievement;
            this.PrevID = achievement.dwPreAchievementID;
            this.DoneCnt = 0;
        }

        public string GetAchieveImagePath()
        {
            return string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Task_Dir, this.Cfg.dwImage);
        }

        public string GetAchievementBgIconPath()
        {
            return string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Task_Dir, this.Cfg.dwBgImage);
        }

        public int GetAchievementCnt()
        {
            int num = 1;
            if (this.Next != null)
            {
                return (num += this.Next.GetAchievementCnt());
            }
            return num;
        }

        public string GetAchievementDesc()
        {
            string szDesc = this.Cfg.szDesc;
            if (this.Cfg.dwDoneType == 0x29)
            {
                return string.Format(szDesc, CLadderView.GetRankName((byte) this.Cfg.dwDoneCondi, this.Cfg.dwDoneParm));
            }
            if (this.Cfg.dwDoneParm == 0)
            {
                if ((this.Cfg.bShowProcess > 0) && !this.IsFinish())
                {
                    return string.Format("{0}\n({1}/{2})", string.Format(szDesc, this.Cfg.dwDoneCondi), this.DoneCnt, this.Cfg.dwDoneCondi);
                }
                return string.Format(szDesc, this.Cfg.dwDoneCondi);
            }
            if ((this.Cfg.bShowProcess > 0) && !this.IsFinish())
            {
                return string.Format("{0}\n({1}/{2})", string.Format(szDesc, this.Cfg.dwDoneCondi, this.Cfg.dwDoneParm), this.DoneCnt, this.Cfg.dwDoneCondi);
            }
            return string.Format(szDesc, this.Cfg.dwDoneCondi, this.Cfg.dwDoneParm);
        }

        public string GetAchievementTips()
        {
            return string.Format(this.Cfg.szTips, Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().Name);
        }

        public string GetGotTimeText(bool needCheckMostRecentlyDone = false, bool needJudgeLeveUp = false)
        {
            CAchieveItem2 item = this;
            if (needCheckMostRecentlyDone)
            {
                item = this.TryToGetMostRecentlyDoneItem();
            }
            if (item == null)
            {
                return Singleton<CTextManager>.GetInstance().GetText("Achievement_Status_Not_Done");
            }
            if (!item.IsFinish())
            {
                return Singleton<CTextManager>.GetInstance().GetText("Achievement_Status_Not_Done");
            }
            if (item == this)
            {
                if (this.DoneTime == 0)
                {
                    return Singleton<CTextManager>.GetInstance().GetText("Achievement_Status_Done");
                }
                return string.Format("{0:yyyy.M.d} {1}", Utility.ToUtcTime2Local((long) this.DoneTime), Singleton<CTextManager>.GetInstance().GetText("Achievement_Status_Done"));
            }
            if (item.DoneTime == 0)
            {
                return Singleton<CTextManager>.GetInstance().GetText("Achievement_Status_Done");
            }
            if (needJudgeLeveUp)
            {
                return string.Format("{0:yyyy.M.d} {1}", Utility.ToUtcTime2Local((long) item.DoneTime), Singleton<CTextManager>.GetInstance().GetText("Achievement_Status_Level_Up"));
            }
            return string.Format("{0:yyyy.M.d} {1}", Utility.ToUtcTime2Local((long) item.DoneTime), Singleton<CTextManager>.GetInstance().GetText("Achievement_Status_Done"));
        }

        public CAchieveItem2 GetHead()
        {
            if (this.Prev != null)
            {
                return this.Prev.GetHead();
            }
            return this;
        }

        public CAchieveItem2 GetHeadAndSetFinishRecursively(uint doneTime)
        {
            this.State = COM_ACHIEVEMENT_STATE.COM_ACHIEVEMENT_STATE_FIN;
            this.DoneTime = doneTime;
            if (this.Prev != null)
            {
                return this.Prev.GetHeadAndSetFinishRecursively(doneTime);
            }
            return this;
        }

        public uint GetMostRecentlyModifyTime()
        {
            if ((this.Next != null) && this.Next.IsFinish())
            {
                return this.Next.GetMostRecentlyModifyTime();
            }
            return this.DoneTime;
        }

        public uint GetTotalDonePoints()
        {
            uint num = 0;
            if (this.IsFinish())
            {
                num += this.Cfg.dwPoint;
            }
            if ((this.Next != null) && this.Next.IsFinish())
            {
                return (num += this.Next.GetTotalDonePoints());
            }
            return num;
        }

        public bool IsFinish()
        {
            return ((this.State == COM_ACHIEVEMENT_STATE.COM_ACHIEVEMENT_STATE_REWARD) || (COM_ACHIEVEMENT_STATE.COM_ACHIEVEMENT_STATE_FIN == this.State));
        }

        public bool IsHideForegroundIcon()
        {
            return (this.Cfg.bHideForegroundImage > 0);
        }

        public CAchieveItem2 TryToGetMostRecentlyDoneItem()
        {
            if ((this.Next != null) && this.Next.IsFinish())
            {
                return this.Next.TryToGetMostRecentlyDoneItem();
            }
            if (this.IsFinish())
            {
                return this;
            }
            return null;
        }
    }
}

