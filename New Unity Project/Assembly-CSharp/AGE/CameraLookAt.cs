namespace AGE
{
    using Assets.Scripts.Common;
    using System;
    using UnityEngine;

    [EventCategory("Movement")]
    public class CameraLookAt : TickEvent
    {
        [ObjectTemplate(new Type[] {  })]
        public int cameraId;
        public Vector3 localOffset = Vector3.get_zero();
        public bool overrideUpDir = true;
        public float rowAngleByZ;
        [ObjectTemplate(new Type[] {  })]
        public int targetId = -1;
        public Vector3 upDir = Vector3.get_up();
        public EUpDirType UpDirType;
        public Vector3 worldOffset = Vector3.get_zero();

        public override BaseEvent Clone()
        {
            CameraLookAt at = ClassObjPool<CameraLookAt>.Get();
            at.CopyData(this);
            return at;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            CameraLookAt at = src as CameraLookAt;
            this.worldOffset = at.worldOffset;
            this.localOffset = at.localOffset;
            this.overrideUpDir = at.overrideUpDir;
            this.upDir = at.upDir;
            this.UpDirType = at.UpDirType;
            this.rowAngleByZ = at.rowAngleByZ;
            this.cameraId = at.cameraId;
            this.targetId = at.targetId;
        }

        private Quaternion GetLookRotation(Action _action)
        {
            GameObject gameObject = _action.GetGameObject(this.cameraId);
            if (gameObject == null)
            {
                return Quaternion.get_identity();
            }
            GameObject obj3 = _action.GetGameObject(this.targetId);
            Vector3 vector = new Vector3(0f, 0f, 0f);
            if (obj3 == null)
            {
                vector = this.localOffset + this.worldOffset;
            }
            else
            {
                vector = (obj3.get_transform().get_position() + obj3.get_transform().TransformDirection(this.localOffset)) + this.worldOffset;
            }
            Vector3 vector2 = vector - gameObject.get_transform().get_position();
            if (this.UpDirType == EUpDirType.NoOverrideUp)
            {
                return Quaternion.LookRotation(vector2, gameObject.get_transform().get_up());
            }
            Quaternion quaternion = Quaternion.AngleAxis(this.rowAngleByZ, Vector3.get_forward());
            return (Quaternion.LookRotation(vector2, Vector3.get_up()) * quaternion);
        }

        public override void OnUse()
        {
            base.OnUse();
            this.worldOffset = Vector3.get_zero();
            this.localOffset = Vector3.get_zero();
            this.overrideUpDir = true;
            this.upDir = Vector3.get_up();
            this.UpDirType = EUpDirType.NoOverrideUp;
            this.rowAngleByZ = 0f;
            this.cameraId = 0;
            this.targetId = -1;
        }

        public override void Process(Action _action, Track _track)
        {
            if (_action.GetGameObject(this.cameraId) != null)
            {
                _action.GetGameObject(this.cameraId).get_transform().set_rotation(this.GetLookRotation(_action));
            }
        }

        public override void ProcessBlend(Action _action, Track _track, TickEvent _prevEvent, float _blendWeight)
        {
            if ((_action.GetGameObject(this.cameraId) != null) && (_prevEvent != null))
            {
                _action.GetGameObject(this.cameraId).get_transform().set_rotation(Quaternion.Slerp((_prevEvent as CameraLookAt).GetLookRotation(_action), this.GetLookRotation(_action), _blendWeight));
            }
        }

        public override bool SupportEditMode()
        {
            return true;
        }

        public enum EUpDirType
        {
            NoOverrideUp,
            RowAngleByZ
        }
    }
}

