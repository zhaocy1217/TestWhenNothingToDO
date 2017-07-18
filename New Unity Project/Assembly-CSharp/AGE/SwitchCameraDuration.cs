namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;
    using UnityEngine;

    [EventCategory("MMGame/System")]
    public class SwitchCameraDuration : DurationCondition
    {
        [ObjectTemplate(new Type[] {  })]
        public int cameraId = -1;
        [ObjectTemplate(new Type[] {  })]
        public int cameraId2 = -1;
        private Camera curCamera;
        public bool cutBackOnExit;
        private Vector3 destPos = Vector3.get_zero();
        private Quaternion destRot = Quaternion.get_identity();
        private bool isMoba_camera;
        private Camera oldCamera;
        public int slerpTick = 0xbb8;
        private Vector3 startPos = Vector3.get_zero();
        private Quaternion startRot = Quaternion.get_identity();
        private bool switchFinished;

        public override bool Check(Action _action, Track _track)
        {
            return this.switchFinished;
        }

        public override BaseEvent Clone()
        {
            SwitchCameraDuration duration = ClassObjPool<SwitchCameraDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SwitchCameraDuration duration = src as SwitchCameraDuration;
            this.cameraId = duration.cameraId;
            this.cameraId2 = duration.cameraId2;
            this.slerpTick = duration.slerpTick;
            this.cutBackOnExit = duration.cutBackOnExit;
            this.startPos = duration.startPos;
            this.startRot = duration.startRot;
            this.destPos = duration.destPos;
            this.destRot = duration.destRot;
            this.curCamera = duration.curCamera;
            this.oldCamera = duration.oldCamera;
            this.switchFinished = duration.switchFinished;
        }

        public override void Enter(Action _action, Track _track)
        {
            this.switchFinished = false;
            GameObject destObj = this.GetDestObj(_action);
            if (destObj != null)
            {
                if (((destObj.get_transform().get_parent() != null) && (destObj.get_transform().get_parent().get_parent() != null)) && (destObj.get_transform().get_parent().get_parent().GetComponent<Moba_Camera>() != null))
                {
                    this.isMoba_camera = true;
                }
                else
                {
                    this.isMoba_camera = false;
                }
                this.curCamera = destObj.GetComponent<Camera>();
                DebugHelper.Assert(this.curCamera != null, "switch camera but dest camera not exist");
                if (this.curCamera != null)
                {
                    string[] textArray1 = new string[] { "Hide" };
                    this.curCamera.set_cullingMask(this.curCamera.get_cullingMask() & ~LayerMask.GetMask(textArray1));
                    string[] textArray2 = new string[] { "UIRaw" };
                    this.curCamera.set_cullingMask(this.curCamera.get_cullingMask() & ~LayerMask.GetMask(textArray2));
                    string[] textArray3 = new string[] { "UI_Background" };
                    this.curCamera.set_cullingMask(this.curCamera.get_cullingMask() & ~LayerMask.GetMask(textArray3));
                    string[] textArray4 = new string[] { "UI_Foreground" };
                    this.curCamera.set_cullingMask(this.curCamera.get_cullingMask() & ~LayerMask.GetMask(textArray4));
                    string[] textArray5 = new string[] { "UI_BottomBG" };
                    this.curCamera.set_cullingMask(this.curCamera.get_cullingMask() & ~LayerMask.GetMask(textArray5));
                    string[] textArray6 = new string[] { "3DUI" };
                    this.curCamera.set_cullingMask(this.curCamera.get_cullingMask() & ~LayerMask.GetMask(textArray6));
                    this.destPos = this.curCamera.get_transform().get_position();
                    this.destRot = this.curCamera.get_transform().get_rotation();
                    this.oldCamera = Camera.get_main();
                    if (this.oldCamera != null)
                    {
                        this.startPos = this.oldCamera.get_transform().get_position();
                        this.startRot = this.oldCamera.get_transform().get_rotation();
                        this.curCamera.get_transform().set_position(this.startPos);
                        this.curCamera.get_transform().set_rotation(this.startRot);
                        destObj.SetActive(true);
                        SwitchCamera(this.oldCamera, this.curCamera);
                    }
                }
            }
            base.Enter(_action, _track);
        }

        private GameObject GetDestObj(Action _action)
        {
            GameObject gameObject = _action.GetGameObject(this.cameraId);
            if (this.cameraId2 != -1)
            {
                SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
                if ((curLvelContext != null) && curLvelContext.m_isCameraFlip)
                {
                    gameObject = _action.GetGameObject(this.cameraId2);
                }
            }
            return gameObject;
        }

        public override void Leave(Action _action, Track _track)
        {
            if ((this.curCamera != null) && !this.switchFinished)
            {
                if (this.isMoba_camera)
                {
                    this.curCamera.get_transform().set_position(this.curCamera.get_transform().get_parent().get_position());
                    this.curCamera.get_transform().set_rotation(this.curCamera.get_transform().get_parent().get_rotation());
                }
                else
                {
                    this.curCamera.get_transform().set_position(this.destPos);
                    this.curCamera.get_transform().set_rotation(this.destRot);
                }
            }
            this.switchFinished = true;
            base.Leave(_action, _track);
        }

        public override void OnUse()
        {
            base.OnUse();
            this.cameraId = -1;
            this.cameraId2 = -1;
            this.slerpTick = 0xbb8;
            this.cutBackOnExit = false;
            this.startPos = Vector3.get_zero();
            this.startRot = Quaternion.get_identity();
            this.destPos = Vector3.get_zero();
            this.destRot = Quaternion.get_identity();
            this.curCamera = null;
            this.oldCamera = null;
        }

        public override void Process(Action _action, Track _track, int _localTime)
        {
            if (!this.switchFinished && (this.curCamera != null))
            {
                if (_localTime >= this.slerpTick)
                {
                    if (this.isMoba_camera)
                    {
                        this.curCamera.get_transform().set_position(this.curCamera.get_transform().get_parent().get_position());
                        this.curCamera.get_transform().set_rotation(this.curCamera.get_transform().get_parent().get_rotation());
                    }
                    else
                    {
                        this.curCamera.get_transform().set_position(this.destPos);
                        this.curCamera.get_transform().set_rotation(this.destRot);
                    }
                    this.switchFinished = true;
                }
                else if (this.isMoba_camera)
                {
                    this.curCamera.get_transform().set_position(Vector3.Lerp(this.startPos, this.curCamera.get_transform().get_parent().get_position(), ((float) _localTime) / ((float) this.slerpTick)));
                    this.curCamera.get_transform().set_rotation(Quaternion.Slerp(this.startRot, this.curCamera.get_transform().get_parent().get_rotation(), ((float) _localTime) / ((float) this.slerpTick)));
                }
                else
                {
                    this.curCamera.get_transform().set_position(Vector3.Lerp(this.startPos, this.destPos, ((float) _localTime) / ((float) this.slerpTick)));
                    this.curCamera.get_transform().set_rotation(Quaternion.Slerp(this.startRot, this.destRot, ((float) _localTime) / ((float) this.slerpTick)));
                }
                base.Process(_action, _track, _localTime);
            }
        }

        public override bool SupportEditMode()
        {
            return true;
        }

        private static void SwitchCamera(Camera camera1, Camera camera2)
        {
            if (camera1 != null)
            {
                camera1.set_enabled(false);
            }
            if (camera2 != null)
            {
                camera2.set_tag("MainCamera");
                camera2.set_enabled(true);
            }
        }
    }
}

