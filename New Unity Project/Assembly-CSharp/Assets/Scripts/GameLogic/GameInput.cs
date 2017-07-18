namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class GameInput : Singleton<GameInput>
    {
        private bool bMessageBoxIsOpen;
        private bool bSmartUse;
        private int ConfirmDirSndFrame = -1;
        public static float DoubleTouchDeltaTime = 0.25f;
        public static int enemyExploreOptRadius = 0x7d0;
        private int FixtimeDirSndFrame = -1;
        private StateMachine inputMode = new StateMachine();
        private DateTime lastClickEscape = DateTime.Now;
        public static float minCurveTrackDistance = 60f;
        public static float minGuestureCircleRadian = 20f;
        private byte nDirMoveSeq;
        private int PreMoveDirection = 0x7fffffff;
        public static float UseDirectionSkillDistance = 80f;

        private VInt3 CalcDirectionByTouchPosition(Vector2 InFirst, Vector2 InSecond)
        {
            if (Camera.get_main() != null)
            {
                Vector3 vector = Camera.get_main().ScreenToWorldPoint(new Vector3(InFirst.x, InFirst.y, Camera.get_main().get_nearClipPlane()));
                Vector3 vector4 = Camera.get_main().ScreenToWorldPoint(new Vector3(InSecond.x, InSecond.y, Camera.get_main().get_nearClipPlane())) - vector;
                return new VInt3(Vector3.ProjectOnPlane(vector4.get_normalized(), new Vector3(0f, 1f, 0f)).get_normalized());
            }
            DebugHelper.Assert(false, "CalcDirectionByTouchPosition, Main camera is null");
            return VInt3.forward;
        }

        public void ChangeBattleMode(bool bBriefness)
        {
            this.inputMode.ChangeState("JoystickMode");
        }

        public void ChangeLobbyMode()
        {
            this.inputMode.ChangeState("LobbyInputMode");
        }

        public override void Init()
        {
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorClearMove, new RefAction<DefaultGameEventParam>(this.OnHostActorClearMove));
            this.inputMode.RegisterState<LobbyInputMode>(new LobbyInputMode(this), "LobbyInputMode");
            this.inputMode.RegisterState<JoystickMode>(new JoystickMode(this), "JoystickMode");
            this.inputMode.ChangeState("LobbyInputMode");
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Quit_Game, new CUIEventManager.OnUIEventHandler(this.OnQuitGame));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Quit_GameCancel, new CUIEventManager.OnUIEventHandler(this.OnQuitCameCancel));
            this.bSmartUse = false;
        }

        public bool IsSmartUse()
        {
            return this.bSmartUse;
        }

        private void OnHostActorClearMove(ref DefaultGameEventParam prm)
        {
            if (ActorHelper.IsHostCtrlActor(ref prm.src))
            {
                this.PreMoveDirection = -2147483648;
            }
        }

        public void OnHostActorRecvMove(int nDegree)
        {
            if (nDegree == this.PreMoveDirection)
            {
                this.ConfirmDirSndFrame = -1;
            }
        }

        private void OnQuitCameCancel(CUIEvent uiEvent)
        {
            this.bMessageBoxIsOpen = false;
        }

        private void OnQuitGame(CUIEvent uiEvent)
        {
            SGameApplication.Quit();
        }

        private void SendMoveDirection(int moveDegree)
        {
            byte num;
            this.PreMoveDirection = moveDegree;
            this.ConfirmDirSndFrame = 0;
            this.FixtimeDirSndFrame = 0;
            FrameCommand<MoveDirectionCommand> command = FrameCommandFactory.CreateCSSyncFrameCommand<MoveDirectionCommand>();
            command.cmdData.Degree = (short) moveDegree;
            if ((this.nDirMoveSeq <= 0xff) && (this.nDirMoveSeq >= 0))
            {
                Singleton<FrameSynchr>.GetInstance().m_MoveCMDSendTime[this.nDirMoveSeq] = (uint) (Time.get_realtimeSinceStartup() * 1000f);
            }
            this.nDirMoveSeq = (byte) ((num = this.nDirMoveSeq) + 1);
            command.cmdData.nSeq = num;
            command.Send();
        }

        public void SendMoveDirection(Vector2 start, Vector2 end)
        {
            this.FixtimeDirSndFrame++;
            VInt3 num = this.CalcDirectionByTouchPosition(start, end);
            if (num != VInt3.zero)
            {
                int moveDegree = (int) (((double) (IntMath.atan2(-num.z, num.x).single * 180f)) / 3.1416);
                DebugHelper.Assert((moveDegree < 0x7fff) && (moveDegree > -32768), "向量转换成2pi空间超过范围了");
                int num3 = moveDegree - this.PreMoveDirection;
                if (((num3 > 1) || (num3 < -1)) || (this.FixtimeDirSndFrame > 30))
                {
                    this.SendMoveDirection(moveDegree);
                }
            }
        }

        public void SendStopMove(bool force = false)
        {
            if ((this.PreMoveDirection != 0x7fffffff) || force)
            {
                this.PreMoveDirection = 0x7fffffff;
                this.ConfirmDirSndFrame = 0;
                this.FixtimeDirSndFrame = 0;
                FrameCommandFactory.CreateFrameCommand<StopMoveCommand>().Send();
            }
        }

        public void SetSmartUse(bool _bUse)
        {
            this.bSmartUse = _bUse;
        }

        public void StopInput()
        {
            ((GameInputMode) this.inputMode.tarState).StopInput();
        }

        public void UpdateEscape()
        {
            if (Input.GetKeyDown(0x1b))
            {
                TimeSpan span = (TimeSpan) (DateTime.Now - this.lastClickEscape);
                if (span.TotalMilliseconds < 1500.0)
                {
                    if (!this.bMessageBoxIsOpen)
                    {
                        Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Is_QuitGame"), enUIEventID.Quit_Game, enUIEventID.Quit_GameCancel, false);
                        this.bMessageBoxIsOpen = true;
                    }
                }
                else
                {
                    this.lastClickEscape = DateTime.Now;
                }
            }
        }

        public void UpdateFrame()
        {
            if (this.inputMode.tarState != null)
            {
                ((GameInputMode) this.inputMode.tarState).Update();
            }
            if (((this.ConfirmDirSndFrame >= 0) && ((++this.ConfirmDirSndFrame % 7) == 6)) && ((this.ConfirmDirSndFrame < 15) && (this.PreMoveDirection != -2147483648)))
            {
                int confirmDirSndFrame = this.ConfirmDirSndFrame;
                if (this.PreMoveDirection != 0x7fffffff)
                {
                    this.SendMoveDirection(this.PreMoveDirection);
                }
                else
                {
                    this.SendStopMove(true);
                }
                this.ConfirmDirSndFrame = confirmDirSndFrame;
            }
            this.UpdateEscape();
        }
    }
}

