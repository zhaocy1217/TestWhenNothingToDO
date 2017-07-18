namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic.DataCenter;
    using CSProtocol;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [ExecuteInEditMode]
    public class ActorConfig : MonoBehaviour, IPooledMonoBehaviour
    {
        private ObjWrapper ActorControl;
        private PlayerMovement ActorMovement;
        [NonSerialized, HideInInspector]
        private ActorRoot ActorObj;
        private PoolObjHandle<ActorRoot> ActorPtr = new PoolObjHandle<ActorRoot>();
        public ActorTypeDef ActorType;
        public int BattleOrder;
        public int[] BattleOrderDepend;
        [NonSerialized, HideInInspector]
        private bool bNeedLerp;
        [NonSerialized, HideInInspector]
        private bool bNeedReloadGizmos = true;
        public bool CanMovable = true;
        [NonSerialized, HideInInspector]
        public CActorInfo CharInfo;
        public COM_PLAYERCAMP CmpType;
        public int ConfigID;
        [NonSerialized, HideInInspector]
        private Material drawMat;
        [NonSerialized, HideInInspector]
        private Mesh drawMesh;
        [NonSerialized, HideInInspector]
        private Quaternion drawRot;
        [NonSerialized, HideInInspector]
        private Vector3 drawScale;
        [NonSerialized, HideInInspector]
        private uint FrameBlockIndex;
        [NonSerialized, HideInInspector]
        private int groundSpeed;
        public bool Invincible;
        [SerializeField, HideInInspector]
        public bool isStatic;
        [NonSerialized, HideInInspector]
        private double lastUpdateTime;
        [NonSerialized, HideInInspector]
        private float maxFrameMove;
        [NonSerialized, HideInInspector]
        public GameObject meshObject;
        [NonSerialized, HideInInspector]
        private Vector3 moveForward = Vector3.get_forward();
        [NonSerialized, HideInInspector]
        private Renderer myRenderer;
        private Transform myTransform;
        public int nPreMoveSeq = -1;
        [NonSerialized, HideInInspector]
        private VInt3 oldLocation;
        public int PathIndex;
        [NonSerialized, HideInInspector]
        private uint RepairFramesMin = 1;
        [NonSerialized, HideInInspector]
        private string szCharInfoPath;
        [NonSerialized, HideInInspector]
        public Quaternion tarRotation;
        public OrganPos theOrganPos = OrganPos.Base;

        private event CustomMoveLerpFunc CustomMoveLerp;

        private event CustomRotateLerpFunc CustomRotateLerp;

        public void ActorStart()
        {
            this.bNeedLerp = Singleton<FrameSynchr>.instance.bActive && (this.ActorObj.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Organ);
            this.ActorControl = (this.ActorObj.TheActorMeta.ActorType >= ActorTypeDef.Actor_Type_Bullet) ? null : this.ActorObj.ActorControl;
            this.ActorMovement = (PlayerMovement) this.ActorObj.MovementComponent;
        }

        public void AddCustomMoveLerp(CustomMoveLerpFunc func)
        {
            if (this.bNeedLerp)
            {
                this.CustomMoveLerp = (CustomMoveLerpFunc) Delegate.Combine(this.CustomMoveLerp, func);
            }
        }

        public void AddCustomRotateLerp(CustomRotateLerpFunc func)
        {
            if (this.bNeedLerp)
            {
                this.CustomRotateLerp = (CustomRotateLerpFunc) Delegate.Combine(this.CustomRotateLerp, func);
            }
        }

        public PoolObjHandle<ActorRoot> AttachActorRoot(GameObject rootObj, ref ActorMeta theActorMeta, CActorInfo actorInfo = new CActorInfo())
        {
            VInt num;
            DebugHelper.Assert(this.ActorObj == null);
            this.ActorObj = ClassObjPool<ActorRoot>.Get();
            this.ActorPtr = new PoolObjHandle<ActorRoot>(this.ActorObj);
            this.ActorObj.ObjLinker = this;
            this.ActorObj.myTransform = rootObj.get_transform();
            this.ActorObj.location = rootObj.get_transform().get_position();
            this.ActorObj.forward = rootObj.get_transform().get_forward();
            this.ActorObj.rotation = rootObj.get_transform().get_rotation();
            this.oldLocation = this.ActorObj.location;
            this.tarRotation = this.ActorObj.rotation;
            this.ActorObj.TheActorMeta = theActorMeta;
            if (theActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
            {
                this.ActorObj.TheActorMeta.EnCId = theActorMeta.ConfigId;
            }
            this.ActorObj.CharInfo = actorInfo;
            if (((this.ActorObj.TheActorMeta.ActorType < ActorTypeDef.Actor_Type_Bullet) && (this.ActorObj.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Organ)) && PathfindingUtility.GetGroundY(this.ActorObj, out num))
            {
                this.ActorObj.groundY = num;
                VInt3 location = this.ActorObj.location;
                location.y = num.i;
                this.ActorObj.location = location;
            }
            return this.ActorPtr;
        }

        private void Awake()
        {
            base.get_gameObject().set_layer(LayerMask.NameToLayer("Actor"));
            this.myTransform = base.get_gameObject().get_transform();
            if (this.isStatic)
            {
                MonoSingleton<GameLoader>.instance.AddStaticActor(this);
            }
        }

        public void CustumLateUpdate()
        {
            if ((this.myRenderer != null) && (this.ActorObj != null))
            {
                bool flag = this.myRenderer.get_isVisible();
                if (flag != this.ActorObj.InCamera)
                {
                    this.ActorObj.InCamera = flag;
                    if (this.ActorObj.InCamera)
                    {
                        if (this.ActorObj.isMovable)
                        {
                            this.oldLocation = this.ActorObj.location;
                            this.myTransform.set_position((Vector3) this.ActorObj.location);
                        }
                        if (this.ActorObj.isRotatable)
                        {
                            VInt3 forward = this.ActorObj.forward;
                            VFactor factor = VInt3.AngleInt(forward, VInt3.forward);
                            int num2 = (forward.x * VInt3.forward.z) - (VInt3.forward.x * forward.z);
                            if (num2 < 0)
                            {
                                factor = VFactor.twoPi - factor;
                            }
                            this.tarRotation = Quaternion.AngleAxis(factor.single * 57.29578f, Vector3.get_up());
                            this.myTransform.set_rotation(this.tarRotation);
                        }
                    }
                }
            }
        }

        public void DetachActorRoot()
        {
            if (this.ActorObj != null)
            {
                if (this.ActorObj.SMNode != null)
                {
                    this.ActorObj.SMNode.Detach();
                    this.ActorObj.SMNode.Release();
                    this.ActorObj.SMNode = null;
                }
                this.ActorObj.UninitActor();
                this.ActorObj.ObjLinker = null;
                this.ActorObj.Release();
                this.ActorPtr.Release();
                this.ActorObj = null;
                this.myRenderer = null;
                this.CustomMoveLerp = null;
                this.CustomRotateLerp = null;
                this.ActorControl = null;
                this.ActorMovement = null;
                if (this.meshObject != null)
                {
                    Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.meshObject);
                    this.meshObject = null;
                }
            }
        }

        public PoolObjHandle<ActorRoot> GetActorHandle()
        {
            return this.ActorPtr;
        }

        private Vector3 NormalMoveLerp(uint nDeltaTick)
        {
            float distance = this.ActorObj.MovementComponent.GetDistance(nDeltaTick);
            Vector3 moveForward = this.moveForward;
            Vector3 location = (Vector3) this.ActorObj.location;
            Vector3 vector3 = this.myTransform.get_position();
            Vector3 vector4 = vector3 + ((Vector3) (moveForward * distance));
            Vector3 vector5 = location;
            if (this.ActorObj.hasReachedNavEdge || this.ActorObj.hasCollidedWithAgents)
            {
                location.y = vector3.y;
                float num2 = (vector3 - location).get_magnitude();
                if (num2 < distance)
                {
                    num2 = distance;
                }
                vector5 = Vector3.Lerp(vector3, location, distance / num2);
                this.RepairFramesMin = 1;
                this.FrameBlockIndex = Singleton<FrameSynchr>.instance.CurFrameNum;
                return vector5;
            }
            location.y = vector4.y;
            Vector3 vector7 = vector4 - location;
            float num3 = vector7.get_magnitude();
            float num4 = this.maxFrameMove * Singleton<FrameSynchr>.instance.PreActFrames;
            if (num3 < (this.RepairFramesMin * this.maxFrameMove))
            {
                vector5 = vector4;
                this.RepairFramesMin = 1;
                this.FrameBlockIndex = Singleton<FrameSynchr>.instance.CurFrameNum;
                return vector5;
            }
            if (num3 < num4)
            {
                float num5 = Mathf.Clamp(num3 / num4, 0.05f, 0.3f);
                float num6 = Vector3.Dot(vector7, moveForward);
                Vector3 vector8 = location + ((Vector3) (moveForward * num4));
                Vector3 vector9 = (vector8 - vector3).get_normalized();
                if (num6 > (num3 * 0.707f))
                {
                    vector5 = vector3 + ((Vector3) ((vector9 * distance) * (1f - num5)));
                }
                else if (num6 < (num3 * -0.707f))
                {
                    vector5 = vector3 + ((Vector3) ((vector9 * distance) * (1f + num5)));
                }
                else
                {
                    vector5 = vector3 + ((Vector3) ((vector9 * distance) * (1f + num5)));
                }
                this.RepairFramesMin = 1;
                this.FrameBlockIndex = Singleton<FrameSynchr>.instance.CurFrameNum;
                return vector5;
            }
            if (Singleton<FrameSynchr>.instance.CurFrameNum == this.FrameBlockIndex)
            {
                return vector3;
            }
            this.RepairFramesMin = 1;
            return vector5;
        }

        private Quaternion ObjRotationLerp()
        {
            return Quaternion.RotateTowards(this.myTransform.get_rotation(), this.tarRotation, this.ActorObj.MovementComponent.rotateSpeed * Time.get_deltaTime());
        }

        public void OnActorMeshChanged(GameObject newMesh)
        {
            if (this.meshObject != null)
            {
                Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.meshObject);
            }
            this.meshObject = newMesh;
            this.myRenderer = base.get_gameObject().GetSkinnedMeshRendererInChildren();
            if (this.myRenderer == null)
            {
                this.myRenderer = base.get_gameObject().GetMeshRendererInChildren();
            }
        }

        public void OnCreate()
        {
            this.CanMovable = true;
            this.isStatic = false;
            this.CharInfo = null;
            this.ActorObj = null;
            this.ActorPtr.Release();
            this.myRenderer = null;
            this.bNeedLerp = false;
            this.GroundSpeed = 0;
            this.nPreMoveSeq = -1;
            this.RepairFramesMin = 1;
            this.FrameBlockIndex = 0;
            this.CustomMoveLerp = null;
            this.CustomRotateLerp = null;
            this.ActorControl = null;
            this.ActorMovement = null;
        }

        protected void OnDestroy()
        {
        }

        public void OnGet()
        {
            this.CanMovable = true;
            this.isStatic = false;
            this.CharInfo = null;
            this.ActorObj = null;
            this.ActorPtr.Release();
            this.myRenderer = null;
            this.bNeedLerp = false;
            this.GroundSpeed = 0;
            this.nPreMoveSeq = -1;
            this.RepairFramesMin = 1;
            this.FrameBlockIndex = 0;
            this.CustomMoveLerp = null;
            this.CustomRotateLerp = null;
            this.ActorControl = null;
            this.ActorMovement = null;
        }

        public void OnRecycle()
        {
            this.DetachActorRoot();
        }

        public void ReattachActor()
        {
            this.ActorPtr.Validate();
        }

        public void RmvCustomMoveLerp(CustomMoveLerpFunc func)
        {
            if (this.bNeedLerp)
            {
                this.CustomMoveLerp = (CustomMoveLerpFunc) Delegate.Remove(this.CustomMoveLerp, func);
                this.myTransform.set_position((Vector3) this.ActorObj.location);
                if (this.CustomMoveLerp != null)
                {
                    this.CustomMoveLerp(this.ActorObj, 0, true);
                }
                if (this.ActorObj.MovementComponent != null)
                {
                    this.ActorObj.MovementComponent.GravityModeLerp(0, true);
                }
            }
        }

        public void RmvCustomRotateLerp(CustomRotateLerpFunc func)
        {
            if (this.bNeedLerp)
            {
                this.CustomRotateLerp = (CustomRotateLerpFunc) Delegate.Remove(this.CustomRotateLerp, func);
            }
        }

        public void SetForward(VInt3 InDir, int nSeq)
        {
            if (this.bNeedLerp && this.ActorObj.InCamera)
            {
                bool flag = false;
                if (((this.nPreMoveSeq < 0) || (nSeq < 0)) || (nSeq == this.nPreMoveSeq))
                {
                    flag = true;
                }
                else if (nSeq > this.nPreMoveSeq)
                {
                    byte num = (byte) nSeq;
                    byte nPreMoveSeq = (byte) this.nPreMoveSeq;
                    num = (byte) (num - 0x80);
                    nPreMoveSeq = (byte) (nPreMoveSeq - 0x80);
                    flag = num < nPreMoveSeq;
                }
                if (flag)
                {
                    VInt3 moveForward;
                    this.moveForward = ((Vector3) InDir).get_normalized();
                    if ((this.ActorObj.ActorControl != null) && this.ActorObj.ActorControl.CanRotate)
                    {
                        moveForward = this.moveForward;
                    }
                    else
                    {
                        moveForward = this.ActorObj.forward;
                    }
                    VFactor factor = VInt3.AngleInt(moveForward, VInt3.forward);
                    int num4 = (moveForward.x * VInt3.forward.z) - (VInt3.forward.x * moveForward.z);
                    if (num4 < 0)
                    {
                        factor = VFactor.twoPi - factor;
                    }
                    this.tarRotation = Quaternion.AngleAxis(factor.single * 57.29578f, Vector3.get_up());
                }
            }
        }

        public void Start()
        {
        }

        private void Update()
        {
            if (((this.ActorObj != null) && this.ActorObj.Visible) && (Singleton<BattleLogic>.instance.isFighting && Singleton<FrameSynchr>.GetInstance().isRunning))
            {
                try
                {
                    bool flag = ((Singleton<FrameSynchr>.instance.FrameSpeed == 1) && this.bNeedLerp) && (this.ActorObj.InCamera || (this.ActorObj.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Bullet));
                    uint nDelta = (uint) (Time.get_deltaTime() * 1000f);
                    bool bReset = false;
                    if ((this.CustomMoveLerp != null) && flag)
                    {
                        this.CustomMoveLerp(this.ActorObj, nDelta, false);
                    }
                    else
                    {
                        VInt num2;
                        Vector3 vector;
                        if (((flag && (this.ActorControl != null)) && (this.ActorControl.CanMove && (this.ActorMovement != null))) && (this.ActorMovement.isMoving || (this.ActorMovement.nLerpStep > 0)))
                        {
                            vector = this.NormalMoveLerp(nDelta);
                            if (!this.ActorMovement.isLerpFlying && PathfindingUtility.GetGroundY((VInt3) vector, out num2))
                            {
                                vector.y = (float) num2;
                            }
                            this.myTransform.set_position(vector);
                            this.ActorMovement.nLerpStep--;
                        }
                        else if (this.oldLocation != this.ActorObj.location)
                        {
                            this.oldLocation = this.ActorObj.location;
                            Vector3 oldLocation = (Vector3) this.oldLocation;
                            Vector3 vector3 = this.myTransform.get_position();
                            oldLocation.y = vector3.y;
                            Vector3 vector4 = oldLocation - vector3;
                            float num3 = 0f;
                            if (((this.groundSpeed <= 0) || !flag) || ((num3 = vector4.get_magnitude()) > (this.maxFrameMove * Singleton<FrameSynchr>.instance.PreActFrames)))
                            {
                                this.myTransform.set_position((Vector3) this.ActorObj.location);
                                if (this.CustomMoveLerp != null)
                                {
                                    this.CustomMoveLerp(this.ActorObj, 0, true);
                                }
                                bReset = true;
                            }
                            else if (((num3 > 0.1f) && !ActorHelper.IsHostCtrlActor(ref this.ActorPtr)) && (this.ActorMovement != null))
                            {
                                float distance = this.ActorMovement.GetDistance(nDelta);
                                vector = Vector3.Lerp(vector3, oldLocation, distance / num3);
                                if (!this.ActorMovement.isLerpFlying && PathfindingUtility.GetGroundY((VInt3) vector, out num2))
                                {
                                    vector.y = (float) num2;
                                }
                                this.myTransform.set_position(vector);
                                this.oldLocation = vector;
                            }
                        }
                    }
                    if (flag && (this.ActorMovement != null))
                    {
                        this.ActorMovement.GravityModeLerp(nDelta, bReset);
                    }
                    if ((this.CustomRotateLerp != null) && flag)
                    {
                        this.CustomRotateLerp(this.ActorObj, nDelta);
                    }
                    else if ((flag && (this.ActorControl != null)) && this.ActorControl.CanRotate)
                    {
                        if (this.myTransform.get_rotation() != this.tarRotation)
                        {
                            this.myTransform.set_rotation(this.ObjRotationLerp());
                        }
                    }
                    else if (this.myTransform.get_rotation() != this.ActorObj.rotation)
                    {
                        this.myTransform.set_rotation(this.ActorObj.rotation);
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        public int GroundSpeed
        {
            get
            {
                return this.groundSpeed;
            }
            set
            {
                this.groundSpeed = value;
                this.maxFrameMove = ((this.groundSpeed * Singleton<FrameSynchr>.instance.FrameDelta) / ((ulong) 0x3e8L)) * 0.001f;
            }
        }
    }
}

