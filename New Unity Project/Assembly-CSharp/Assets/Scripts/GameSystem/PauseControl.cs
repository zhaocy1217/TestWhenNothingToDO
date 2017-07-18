namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class PauseControl
    {
        private int _passedSecond;
        private Text _passedTimeTxt;
        private Text _pauseCampTxt;
        private GameObject _pauseNode;
        private int _pauseTimer;
        private GameObject _remainNode;
        private Text _remainTimesTxt;
        private Button _resumeButton;
        private GameObject _resumeNode;
        private Text _resumeTimerTxt;
        private GameObject _root;
        private int _waitResumeSecond;
        [CompilerGenerated]
        private byte <CurPauseTimes>k__BackingField;
        [CompilerGenerated]
        private byte <MaxAllowTimes>k__BackingField;
        public const byte NoLimitTimes = 0xff;

        public PauseControl(CUIFormScript rootForm)
        {
            GameObject obj2 = Utility.FindChild(rootForm.get_gameObject(), "PauseResume");
            obj2.CustomSetActive(false);
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            this.MaxAllowTimes = !curLvelContext.IsGameTypePvpRoom() ? ((byte) 0) : (!Singleton<WatchController>.GetInstance().IsLiveCast ? curLvelContext.m_pauseTimes : ((byte) 0xff));
            if (this.MaxAllowTimes == 0)
            {
                this._root = null;
            }
            else
            {
                this.CurPauseTimes = (Singleton<LobbyLogic>.GetInstance().reconnGameInfo == null) ? ((byte) 0) : Singleton<LobbyLogic>.GetInstance().reconnGameInfo.bPauseNum;
                this._root = obj2;
                if (this._root != null)
                {
                    this._pauseNode = Utility.FindChild(this._root, "PauseNode");
                    this._resumeNode = Utility.FindChild(this._root, "ResumeNode");
                    this._passedTimeTxt = Utility.GetComponetInChild<Text>(this._pauseNode, "PassedTime");
                    this._pauseCampTxt = Utility.GetComponetInChild<Text>(this._pauseNode, "PauseCamp");
                    this._resumeButton = Utility.GetComponetInChild<Button>(this._pauseNode, "ResumeButton");
                    this._resumeTimerTxt = Utility.GetComponetInChild<Text>(this._pauseNode, "ResumeButton/Text");
                    this._remainNode = Utility.FindChild(this._pauseNode, "RemainTimes");
                    this._remainTimesTxt = Utility.GetComponetInChild<Text>(this._remainNode, "Times");
                    this._root.CustomSetActive(false);
                }
                this._pauseTimer = 0;
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_ResumeMultiGame, new CUIEventManager.OnUIEventHandler(this.OnResumeMultiGame));
            }
        }

        public void Clear()
        {
            Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this._pauseTimer);
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_ResumeMultiGame, new CUIEventManager.OnUIEventHandler(this.OnResumeMultiGame));
            this._root = null;
        }

        public void ExecPauseCommand(byte pauseCommand, byte pauseByCamp, byte[] campPauseTimes)
        {
            bool bActive = pauseCommand == 1;
            if (Singleton<WatchController>.GetInstance().IsWatching)
            {
                Singleton<WatchController>.GetInstance().IsRunning = !bActive;
            }
            else
            {
                Singleton<FrameSynchr>.GetInstance().SetSynchrRunning(!bActive);
                Singleton<FrameSynchr>.GetInstance().ResetStartTime();
            }
            if (this._root != null)
            {
                this._root.CustomSetActive(bActive);
                if (bActive)
                {
                    Singleton<CBattleSystem>.GetInstance().ClosePopupForms();
                    this._pauseNode.CustomSetActive(true);
                    this._resumeNode.CustomSetActive(false);
                    COM_PLAYERCAMP com_playercamp = (COM_PLAYERCAMP) pauseByCamp;
                    COM_PLAYERCAMP com_playercamp2 = !Singleton<WatchController>.GetInstance().IsWatching ? Singleton<GamePlayerCenter>.GetInstance().hostPlayerCamp : COM_PLAYERCAMP.COM_PLAYERCAMP_MID;
                    this._pauseCampTxt.set_text(Singleton<CTextManager>.GetInstance().GetText(string.Format("PauseTips_{0}Look{1}", (int) com_playercamp2, (int) com_playercamp)));
                    this._passedSecond = 0;
                    if (com_playercamp == com_playercamp2)
                    {
                        this._waitResumeSecond = 0;
                    }
                    else
                    {
                        ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xf2);
                        this._waitResumeSecond = (dataByKey == null) ? ((int) 60) : ((int) dataByKey.dwConfValue);
                    }
                    this.ValidateTimerState();
                    if (com_playercamp2 != COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
                    {
                        this._remainNode.CustomSetActive(true);
                        int num = (Singleton<BattleLogic>.GetInstance().GetCurLvelContext().m_pvpPlayerNum / 2) * this.MaxAllowTimes;
                        this._remainTimesTxt.set_text((num - campPauseTimes[(int) com_playercamp2]).ToString());
                    }
                    else
                    {
                        this._remainNode.CustomSetActive(false);
                    }
                    if (this._pauseTimer == 0)
                    {
                        this._pauseTimer = Singleton<CTimerManager>.GetInstance().AddTimer(0x3e8, -1, new CTimer.OnTimeUpHandler(this.OnPauseTimer), false);
                    }
                }
                else
                {
                    Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this._pauseTimer);
                }
            }
        }

        [MessageHandler(0x147c)]
        public static void OnPauseResponse(CSPkg msg)
        {
            Singleton<CBattleSystem>.GetInstance().pauseControl.ResponsePause(msg.stPkgData.stPauseRsp);
        }

        private void OnPauseTimer(int timeSeq)
        {
            this._passedSecond++;
            if (this._waitResumeSecond > 0)
            {
                this._waitResumeSecond--;
            }
            this.ValidateTimerState();
        }

        private void OnResumeMultiGame(CUIEvent uiEvt)
        {
            this.RequestPause(false);
        }

        public void RequestPause(bool isPause)
        {
            if (isPause)
            {
                if (this.CanPause)
                {
                    CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x147b);
                    msg.stPkgData.stPauseReq.bType = 1;
                    Singleton<NetworkModule>.GetInstance().SendGameMsg(ref msg, 0);
                }
            }
            else
            {
                CSPkg pkg2 = NetworkModule.CreateDefaultCSPKG(0x147b);
                pkg2.stPkgData.stPauseReq.bType = 2;
                Singleton<NetworkModule>.GetInstance().SendGameMsg(ref pkg2, 0);
            }
        }

        private void ResponsePause(SCPKG_PAUSE_RSP pkg)
        {
            if (pkg.bResult == 0)
            {
                if (pkg.bType == 2)
                {
                    Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this._pauseTimer);
                    if (this._root != null)
                    {
                        this._pauseNode.CustomSetActive(false);
                        this._resumeNode.CustomSetActive(true);
                    }
                }
                else
                {
                    this.CurPauseTimes = (byte) (this.CurPauseTimes + 1);
                }
            }
        }

        private void ValidateTimerState()
        {
            this._passedTimeTxt.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("PauseTimeFormate"), this._passedSecond / 60, this._passedSecond % 60));
            if (this._waitResumeSecond >= 0)
            {
                string text = Singleton<CTextManager>.GetInstance().GetText("ResumeGame");
                if (this._waitResumeSecond > 0)
                {
                    this._resumeTimerTxt.set_text(string.Concat(new object[] { text, "(", this._waitResumeSecond, ")" }));
                }
                else
                {
                    this._resumeTimerTxt.set_text(text);
                }
                bool isEnable = this._waitResumeSecond == 0;
                CUICommonSystem.SetButtonEnable(this._resumeButton, isEnable, isEnable, true);
                this._resumeButton.get_gameObject().CustomSetActive(true);
            }
            else
            {
                this._resumeButton.get_gameObject().CustomSetActive(false);
            }
        }

        public bool CanPause
        {
            get
            {
                return ((this.CurPauseTimes < this.MaxAllowTimes) || (this.MaxAllowTimes == 0xff));
            }
        }

        public byte CurPauseTimes
        {
            [CompilerGenerated]
            get
            {
                return this.<CurPauseTimes>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<CurPauseTimes>k__BackingField = value;
            }
        }

        public bool EnablePause
        {
            get
            {
                return (this.MaxAllowTimes > 0);
            }
        }

        public byte MaxAllowTimes
        {
            [CompilerGenerated]
            get
            {
                return this.<MaxAllowTimes>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<MaxAllowTimes>k__BackingField = value;
            }
        }
    }
}

