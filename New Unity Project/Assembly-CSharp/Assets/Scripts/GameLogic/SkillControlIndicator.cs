namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameSystem;
    using ResData;
    using System;
    using UnityEngine;

    public class SkillControlIndicator
    {
        private bool bControlMove = false;
        private bool bMoveFlag;
        private bool bRotateFlag;
        private bool bSelectEffectPrefab;
        private bool bSkillBtnDrag;
        private bool bUseAdvanceSelect;
        private float deltaAngle;
        private Vector3 deltaDirection = Vector3.get_zero();
        private Vector3 deltaPosition = Vector3.get_zero();
        private int effectHideFrameNum = 0;
        private GameObject effectPrefabBlock;
        private GameObject effectPrefabGrass;
        private GameObject effectPrefabNormal;
        private bool effectSelectActive;
        private GameObject effectWarnPrefab;
        private GameObject fixedPrefab;
        private GameObject fixedWarnPrefab;
        private GameObject guidePrefab;
        private bool guideSelectActive;
        private GameObject guideWarnPrefab;
        public Vector3 highLitColor = new Vector3(1f, 1f, 1f);
        private int hlcId;
        private EEffectPrefabSelect m_prefabSelecter;
        private Vector3 movePosition = Vector3.get_zero();
        private float moveSpeed = 0.03f;
        private const string PrefabBlockPostfix = "_y";
        private const string PrefabGrassPostfix = "_G";
        public const float PrefabHeight = 0.3f;
        private int pressTime = 0;
        private Vector3 rootRosition = Vector3.get_zero();
        private float rotateAngle;
        private Vector3 rotateDirection = Vector3.get_zero();
        private float rotateSpeed = 0.5f;
        private SkillSlot skillSlot;
        private ActorRoot targetActor;
        private Vector3 useOffsetPosition = Vector3.get_zero();
        private Vector3 useSkillDirection = Vector3.get_zero();
        private Vector3 useSkillPosition = Vector3.get_zero();

        public SkillControlIndicator(SkillSlot _skillSlot)
        {
            this.skillSlot = _skillSlot;
            this.bUseAdvanceSelect = true;
            this.targetActor = null;
        }

        public void CreateIndicatePrefab(Skill _skillObj)
        {
            if (((this.skillSlot.Actor != 0) && ActorHelper.IsHostActor(ref this.skillSlot.Actor)) && ((_skillObj != null) && (_skillObj.cfgData != null)))
            {
                this.effectHideFrameNum = 0;
                ActorRoot handle = this.skillSlot.Actor.handle;
                Quaternion rot = handle.myTransform.get_rotation();
                Vector3 pos = handle.myTransform.get_position();
                pos.y += 0.3f;
                GameObject obj2 = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(_skillObj.GuidePrefabName, true, SceneObjType.ActionRes, pos, rot);
                if (obj2 != null)
                {
                    this.guidePrefab = obj2;
                    this.guidePrefab.get_transform().SetParent(handle.myTransform);
                    this.HidePrefab(this.guidePrefab);
                    this.SetPrefabTag(this.guidePrefab);
                    this.guideSelectActive = false;
                }
                obj2 = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(_skillObj.GuideWarnPrefabName, true, SceneObjType.ActionRes, pos, rot);
                if (obj2 != null)
                {
                    this.guideWarnPrefab = obj2;
                    this.guideWarnPrefab.get_transform().SetParent(handle.myTransform);
                    this.HidePrefab(this.guideWarnPrefab);
                    this.SetPrefabTag(this.guideWarnPrefab);
                }
                obj2 = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(_skillObj.EffectPrefabName, true, SceneObjType.ActionRes, pos, rot);
                if (obj2 != null)
                {
                    this.effectPrefabNormal = obj2;
                    this.HidePrefab(this.effectPrefabNormal);
                    this.SetPrefabTag(this.effectPrefabNormal);
                    this.effectSelectActive = false;
                    MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.effectPrefabNormal, SceneObjType.ActionRes);
                }
                this.bSelectEffectPrefab = _skillObj.cfgData.bSelectEffectPrefab > 0;
                if (this.bSelectEffectPrefab)
                {
                    obj2 = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(_skillObj.EffectPrefabName + "_y", true, SceneObjType.ActionRes, pos, rot);
                    if (obj2 != null)
                    {
                        this.effectPrefabBlock = obj2;
                        this.HidePrefab(this.effectPrefabBlock);
                        this.SetPrefabTag(this.effectPrefabBlock);
                        MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.effectPrefabBlock, SceneObjType.ActionRes);
                    }
                    obj2 = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(_skillObj.EffectPrefabName + "_G", true, SceneObjType.ActionRes, pos, rot);
                    if (obj2 != null)
                    {
                        this.effectPrefabGrass = obj2;
                        this.HidePrefab(this.effectPrefabGrass);
                        this.SetPrefabTag(this.effectPrefabGrass);
                        MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.effectPrefabGrass, SceneObjType.ActionRes);
                    }
                }
                obj2 = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(_skillObj.EffectWarnPrefabName, true, SceneObjType.ActionRes, pos, rot);
                if (obj2 != null)
                {
                    this.effectWarnPrefab = obj2;
                    this.HidePrefab(this.effectWarnPrefab);
                    this.SetPrefabTag(this.effectWarnPrefab);
                    MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.effectWarnPrefab, SceneObjType.ActionRes);
                }
                obj2 = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(_skillObj.FixedPrefabName, true, SceneObjType.ActionRes, pos, rot);
                if (obj2 != null)
                {
                    this.fixedPrefab = obj2;
                    this.HidePrefab(this.fixedPrefab);
                    this.SetPrefabTag(this.fixedPrefab);
                    this.fixedPrefab.get_transform().SetParent(handle.myTransform);
                }
                obj2 = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(_skillObj.FixedWarnPrefabName, true, SceneObjType.ActionRes, pos, rot);
                if (obj2 != null)
                {
                    this.fixedWarnPrefab = obj2;
                    this.HidePrefab(this.fixedWarnPrefab);
                    this.SetPrefabTag(this.fixedWarnPrefab);
                    this.fixedWarnPrefab.get_transform().SetParent(handle.myTransform);
                }
                int iGuideDistance = (int) _skillObj.cfgData.iGuideDistance;
                this.SetPrefabScaler(this.guidePrefab, iGuideDistance);
                this.SetPrefabScaler(this.guideWarnPrefab, iGuideDistance);
                if ((_skillObj.cfgData.bRangeAppointType == 3) || (_skillObj.cfgData.bRangeAppointType == 1))
                {
                    this.SetPrefabScaler(this.effectPrefabNormal, iGuideDistance);
                    this.SetPrefabScaler(this.effectPrefabBlock, iGuideDistance);
                    this.SetPrefabScaler(this.effectPrefabGrass, iGuideDistance);
                    this.SetPrefabScaler(this.effectWarnPrefab, iGuideDistance);
                }
                int iFixedDistance = (int) _skillObj.cfgData.iFixedDistance;
                this.SetPrefabScaler(this.fixedPrefab, iFixedDistance);
                this.SetPrefabScaler(this.fixedWarnPrefab, iFixedDistance);
            }
        }

        private void ForceSetGuildPrefabShow(bool bShow)
        {
            if (this.guidePrefab != null)
            {
                if (bShow)
                {
                    this.ShowPrefab(this.guidePrefab);
                }
                else
                {
                    this.HidePrefab(this.guidePrefab);
                }
                this.guideSelectActive = bShow;
            }
            if ((this.effectPrefab != null) && !Singleton<GameInput>.GetInstance().IsSmartUse())
            {
                if (bShow)
                {
                    this.ShowPrefabEffect();
                }
                else
                {
                    this.HidePrefabEffect();
                }
                this.effectSelectActive = bShow;
            }
        }

        public bool GetSkillBtnDrag()
        {
            return this.bSkillBtnDrag;
        }

        public bool GetUseAdvanceMode()
        {
            return this.bUseAdvanceSelect;
        }

        public Vector3 GetUseSkillDirection()
        {
            return this.useSkillDirection;
        }

        public Vector3 GetUseSkillPosition()
        {
            return this.useSkillPosition;
        }

        public ActorRoot GetUseSkillTargetDefaultAttackMode()
        {
            if (!this.bUseAdvanceSelect)
            {
                SkillSelectControl instance = Singleton<SkillSelectControl>.GetInstance();
                Skill skill = (this.skillSlot.NextSkillObj == null) ? this.skillSlot.SkillObj : this.skillSlot.NextSkillObj;
                if (skill.cfgData.bWheelType != 1)
                {
                    this.targetActor = instance.SelectTarget((SkillTargetRule) skill.cfgData.bSkillTargetRule, this.skillSlot);
                }
            }
            return this.targetActor;
        }

        public ActorRoot GetUseSkillTargetLockAttackMode()
        {
            if (this.bUseAdvanceSelect)
            {
                return this.targetActor;
            }
            return null;
        }

        private void HidePrefab(GameObject _prefab)
        {
            if (_prefab != null)
            {
                _prefab.SetLayer("Hide", false);
            }
        }

        private void HidePrefabEffect()
        {
            this.PrefabSelecter = EEffectPrefabSelect.Normal;
            this.HidePrefab(this.effectPrefab);
        }

        public void InitControlIndicator()
        {
            if (ActorHelper.IsHostActor(ref this.skillSlot.Actor))
            {
                float spd = 0f;
                float angularSpd = 0f;
                GameSettings.GetLunPanSensitivity(out spd, out angularSpd);
                this.SetIndicatorSpeed(spd, angularSpd);
            }
        }

        public bool IsAllowUseSkill()
        {
            Skill skill = (this.skillSlot.NextSkillObj == null) ? this.skillSlot.SkillObj : this.skillSlot.NextSkillObj;
            if (((skill.cfgData.bRangeAppointType == 2) && !this.bControlMove) && (this.pressTime <= 0x3e8))
            {
                return false;
            }
            return true;
        }

        private bool IsPrefabVisible(GameObject _prefab)
        {
            if (_prefab != null)
            {
                int num = LayerMask.NameToLayer("Hide");
                if (_prefab.get_layer() != num)
                {
                    return true;
                }
            }
            return false;
        }

        public void LateUpdate(int nDelta)
        {
            if (((this.skillSlot != null) && (this.skillSlot.SkillObj != null)) && (this.skillSlot.SkillObj.cfgData != null))
            {
                if ((this.effectHideFrameNum > 0) && (Time.get_frameCount() > this.effectHideFrameNum))
                {
                    this.ForceSetGuildPrefabShow(false);
                    this.effectHideFrameNum = 0;
                }
                this.pressTime += nDelta;
                if (this.effectPrefab != null)
                {
                    if (this.bMoveFlag)
                    {
                        Vector3 vector = (Vector3) ((this.deltaDirection * this.moveSpeed) * nDelta);
                        this.deltaPosition += vector;
                        if (this.deltaPosition.get_sqrMagnitude() >= this.movePosition.get_sqrMagnitude())
                        {
                            this.bMoveFlag = false;
                            this.useSkillPosition = this.skillSlot.Actor.handle.myTransform.get_position() + this.useOffsetPosition;
                            this.effectPrefab.get_transform().set_position(this.useSkillPosition);
                        }
                        else
                        {
                            this.useSkillPosition = this.effectPrefab.get_transform().get_position() + vector;
                            this.useSkillPosition += this.skillSlot.Actor.handle.myTransform.get_position() - this.rootRosition;
                            this.effectPrefab.get_transform().set_position(this.useSkillPosition);
                            this.rootRosition = this.skillSlot.Actor.handle.myTransform.get_position();
                        }
                    }
                    else
                    {
                        this.useSkillPosition += this.skillSlot.Actor.handle.myTransform.get_position() - this.rootRosition;
                        this.effectPrefab.get_transform().set_position(this.useSkillPosition);
                        this.rootRosition = this.skillSlot.Actor.handle.myTransform.get_position();
                    }
                    if (this.bRotateFlag)
                    {
                        float num = this.rotateSpeed * nDelta;
                        this.deltaAngle += num;
                        if (this.deltaAngle >= this.rotateAngle)
                        {
                            this.bRotateFlag = false;
                            this.useSkillDirection = this.rotateDirection;
                            this.effectPrefab.get_transform().set_forward(this.useSkillDirection);
                        }
                        else
                        {
                            Vector3 vector2 = this.effectPrefab.get_transform().get_forward();
                            if (Vector3.Cross(this.useSkillDirection, this.rotateDirection).y < 0f)
                            {
                                vector2 = Quaternion.Euler(0f, -num, 0f) * vector2;
                            }
                            else
                            {
                                vector2 = Quaternion.Euler(0f, num, 0f) * vector2;
                            }
                            this.useSkillDirection = vector2;
                            this.effectPrefab.get_transform().set_forward(this.useSkillDirection);
                        }
                    }
                    VInt groundY = 0;
                    if (PathfindingUtility.GetGroundY((VInt3) this.effectPrefab.get_transform().get_position(), out groundY))
                    {
                        Vector3 vector4 = this.effectPrefab.get_transform().get_position();
                        vector4.y = ((float) groundY) + 0.3f;
                        this.effectPrefab.get_transform().set_position(vector4);
                    }
                }
                if ((this.effectWarnPrefab != null) && (this.effectPrefab != null))
                {
                    this.effectWarnPrefab.get_transform().set_position(this.effectPrefab.get_transform().get_position());
                    this.effectWarnPrefab.get_transform().set_forward(this.effectPrefab.get_transform().get_forward());
                }
                this.SetUseSkillTarget();
                if ((this.bSelectEffectPrefab && this.IsPrefabVisible(this.effectPrefab)) && FogOfWar.enable)
                {
                    Vector3 inActorLoc = this.effectPrefab.get_transform().get_position();
                    if (!PathfindingUtility.IsValidTarget(this.skillSlot.Actor.handle, (VInt3) inActorLoc))
                    {
                        this.PrefabSelecter = EEffectPrefabSelect.Block;
                    }
                    else if (Singleton<GameFowManager>.instance.QueryAttr(inActorLoc) == FieldObj.EViewBlockType.Grass)
                    {
                        this.PrefabSelecter = EEffectPrefabSelect.Grass;
                    }
                    else
                    {
                        this.PrefabSelecter = EEffectPrefabSelect.Normal;
                    }
                }
            }
        }

        private void PlayCommonAttackTargetEffect(ActorRoot actorRoot)
        {
            if (actorRoot != null)
            {
                Singleton<SkillIndicateSystem>.GetInstance().PlayCommonAttackTargetEffect(actorRoot);
                if (actorRoot.MatHurtEffect != null)
                {
                    this.hlcId = actorRoot.MatHurtEffect.PlayHighLitEffect(this.highLitColor);
                }
            }
        }

        public void SelectSkillTarget(Vector2 axis, bool isSkillCursorInCancelArea)
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
            Skill skill = (this.skillSlot.NextSkillObj == null) ? this.skillSlot.SkillObj : this.skillSlot.NextSkillObj;
            if ((curLvelContext != null) && curLvelContext.m_isCameraFlip)
            {
                axis = -axis;
            }
            Vector3 vector = Vector3.get_zero();
            Vector3 vector2 = Vector3.get_zero();
            if ((this.effectPrefab != null) && (skill != null))
            {
                if (skill.cfgData.bRangeAppointType == 1)
                {
                    vector2.x = skill.cfgData.iGuideDistance * axis.x;
                    vector2.z = skill.cfgData.iGuideDistance * axis.y;
                    if (vector2.get_magnitude() <= (((double) skill.cfgData.iGuideDistance) * 0.5))
                    {
                        this.bUseAdvanceSelect = false;
                    }
                    else
                    {
                        this.bUseAdvanceSelect = true;
                    }
                    vector.x = axis.x;
                    vector.z = axis.y;
                    this.useOffsetPosition = Vector3.get_zero();
                    this.bRotateFlag = true;
                    this.rotateAngle = Vector3.Angle(this.useSkillDirection, vector);
                    this.deltaAngle = 0f;
                    this.rotateDirection = vector;
                    this.rootRosition = this.skillSlot.Actor.handle.myTransform.get_position();
                    this.useSkillPosition = this.skillSlot.Actor.handle.myTransform.get_position();
                }
                else if (skill.cfgData.bRangeAppointType == 2)
                {
                    vector2.x = (skill.cfgData.iGuideDistance / 1000f) * axis.x;
                    vector2.z = (skill.cfgData.iGuideDistance / 1000f) * axis.y;
                    this.useOffsetPosition = vector2;
                    this.movePosition = (this.skillSlot.Actor.handle.myTransform.get_position() + vector2) - this.effectPrefab.get_transform().get_position();
                    this.movePosition.y = 0f;
                    this.deltaDirection = this.movePosition;
                    this.deltaDirection.Normalize();
                    this.deltaPosition = Vector3.get_zero();
                    this.bMoveFlag = true;
                    this.rootRosition = this.skillSlot.Actor.handle.myTransform.get_position();
                    this.bControlMove = true;
                    if (skill.cfgData.bIndicatorType == 1)
                    {
                        vector.x = axis.x;
                        vector.z = axis.y;
                        this.bRotateFlag = true;
                        this.rotateAngle = Vector3.Angle(this.useSkillDirection, vector);
                        this.deltaAngle = 0f;
                        this.rotateDirection = vector;
                    }
                }
                else if (skill.cfgData.bRangeAppointType == 3)
                {
                    if ((axis == Vector2.get_zero()) && !isSkillCursorInCancelArea)
                    {
                        return;
                    }
                    vector.x = axis.x;
                    vector.z = axis.y;
                    this.useOffsetPosition = Vector3.get_zero();
                    this.bRotateFlag = true;
                    this.rotateAngle = Vector3.Angle(this.useSkillDirection, vector);
                    this.deltaAngle = 0f;
                    this.rotateDirection = vector;
                    this.rootRosition = this.skillSlot.Actor.handle.myTransform.get_position();
                    this.useSkillPosition = this.skillSlot.Actor.handle.myTransform.get_position();
                }
            }
            if (isSkillCursorInCancelArea)
            {
                if ((this.effectWarnPrefab != null) && (this.effectPrefab != null))
                {
                    this.effectWarnPrefab.get_transform().set_position(this.effectPrefab.get_transform().get_position());
                    this.effectWarnPrefab.get_transform().set_forward(this.effectPrefab.get_transform().get_forward());
                }
                this.SetGuildWarnPrefabShow(true);
                this.SetGuildPrefabShow(false);
                this.SetFixedPrefabShow(false);
            }
            else
            {
                this.SetGuildWarnPrefabShow(false);
                this.SetFixedPrefabShow(true);
                if (!this.bUseAdvanceSelect)
                {
                    this.SetGuildPrefabShow(false);
                    this.SetEffectPrefabShow(false);
                }
                else
                {
                    this.SetGuildPrefabShow(true);
                    this.SetEffectPrefabShow(true);
                }
            }
        }

        public void SetEffectPrefabShow(bool bShow)
        {
            if ((this.effectPrefab != null) && !Singleton<GameInput>.GetInstance().IsSmartUse())
            {
                if (bShow)
                {
                    this.ShowPrefabEffect();
                }
                else
                {
                    this.HidePrefabEffect();
                }
                this.effectSelectActive = bShow;
            }
        }

        public void SetEffectWarnPrefabShow(bool bShow)
        {
            if ((this.effectWarnPrefab != null) && !Singleton<GameInput>.GetInstance().IsSmartUse())
            {
                if (bShow)
                {
                    this.ShowPrefab(this.effectWarnPrefab);
                }
                else
                {
                    this.HidePrefab(this.effectWarnPrefab);
                }
            }
        }

        public void SetFixedPrefabShow(bool bShow)
        {
            if (this.fixedPrefab != null)
            {
                if (bShow)
                {
                    this.ShowPrefab(this.fixedPrefab);
                }
                else
                {
                    this.HidePrefab(this.fixedPrefab);
                }
            }
        }

        public void SetFixedWarnPrefabShow(bool bShow)
        {
            if (this.fixedWarnPrefab != null)
            {
                if (bShow)
                {
                    this.ShowPrefab(this.fixedWarnPrefab);
                }
                else
                {
                    this.HidePrefab(this.fixedWarnPrefab);
                }
            }
        }

        public void SetGuildPrefabShow(bool bShow)
        {
            if (bShow)
            {
                this.effectHideFrameNum = 0;
                this.ForceSetGuildPrefabShow(bShow);
            }
            else
            {
                this.effectHideFrameNum = Time.get_frameCount();
            }
        }

        public void SetGuildWarnPrefabShow(bool bShow)
        {
            if (this.guideWarnPrefab != null)
            {
                if (bShow)
                {
                    this.ShowPrefab(this.guideWarnPrefab);
                }
                else
                {
                    this.HidePrefab(this.guideWarnPrefab);
                }
            }
            if ((this.effectWarnPrefab != null) && !Singleton<GameInput>.GetInstance().IsSmartUse())
            {
                if (bShow)
                {
                    this.ShowPrefab(this.effectWarnPrefab);
                }
                else
                {
                    this.HidePrefab(this.effectWarnPrefab);
                }
            }
            if ((this.fixedWarnPrefab != null) && !Singleton<GameInput>.GetInstance().IsSmartUse())
            {
                if (bShow)
                {
                    this.ShowPrefab(this.fixedWarnPrefab);
                }
                else
                {
                    this.HidePrefab(this.fixedWarnPrefab);
                }
            }
        }

        public void SetIndicatorSpeed(float _moveSpeed, float _rotateSpeed)
        {
            this.moveSpeed = _moveSpeed;
            this.rotateSpeed = _rotateSpeed;
        }

        public void SetPrefabScaler(GameObject _obj, int _distance)
        {
            if ((null != _obj) && (_obj != null))
            {
                ParticleScaler[] componentsInChildren = _obj.GetComponentsInChildren<ParticleScaler>(true);
                for (int i = 0; i < componentsInChildren.Length; i++)
                {
                    componentsInChildren[i].particleScale = ((float) _distance) / 10000f;
                    componentsInChildren[i].CheckAndApplyScale();
                }
            }
        }

        private void SetPrefabTag(GameObject _prefab)
        {
            if (_prefab != null)
            {
                _prefab.set_tag("SCI");
            }
        }

        public void SetSkillBtnDrag(bool bDrag)
        {
            this.bSkillBtnDrag = bDrag;
        }

        public void SetSkillUseDefaultPosition()
        {
            this.pressTime = 0;
            this.bControlMove = false;
            this.useOffsetPosition = Vector3.get_zero();
            this.useSkillPosition = this.skillSlot.Actor.handle.myTransform.get_position();
            this.useSkillDirection = this.skillSlot.Actor.handle.myTransform.get_forward();
            if (this.effectPrefab != null)
            {
                this.effectPrefab.get_transform().set_position(this.skillSlot.Actor.handle.myTransform.get_position());
                this.effectPrefab.get_transform().Translate(0f, 0.3f, 0f);
                this.effectPrefab.get_transform().set_forward(this.skillSlot.Actor.handle.myTransform.get_forward());
            }
        }

        public void SetSkillUsePosition(ActorRoot target)
        {
            Vector3 vector = Vector3.get_zero();
            Vector3 vector2 = Vector3.get_zero();
            Skill skill = (this.skillSlot.NextSkillObj == null) ? this.skillSlot.SkillObj : this.skillSlot.NextSkillObj;
            if (skill != null)
            {
                if (skill.cfgData.bRangeAppointType == 2)
                {
                    this.bControlMove = true;
                    this.useSkillPosition = target.myTransform.get_position();
                    vector = target.myTransform.get_position() - this.skillSlot.Actor.handle.myTransform.get_position();
                    this.useOffsetPosition = vector;
                    vector2 = vector;
                    vector2.y = 0f;
                    vector2.Normalize();
                    this.useSkillDirection = vector2;
                    if (this.effectPrefab != null)
                    {
                        this.effectPrefab.get_transform().set_forward(vector2);
                        this.effectPrefab.get_transform().set_position(target.myTransform.get_position());
                        this.effectPrefab.get_transform().Translate(0f, 0.3f, 0f);
                    }
                }
                else if (skill.cfgData.bRangeAppointType == 3)
                {
                    this.useSkillPosition = this.skillSlot.Actor.handle.myTransform.get_position();
                    vector = target.myTransform.get_position() - this.skillSlot.Actor.handle.myTransform.get_position();
                    vector2 = vector;
                    vector2.y = 0f;
                    vector2.Normalize();
                    this.useSkillDirection = vector2;
                    if (this.effectPrefab != null)
                    {
                        this.effectPrefab.get_transform().set_forward(vector2);
                        this.effectPrefab.get_transform().set_position(this.skillSlot.Actor.handle.myTransform.get_position());
                        this.effectPrefab.get_transform().Translate(0f, 0.3f, 0f);
                    }
                }
            }
        }

        public void SetUseAdvanceMode(bool b)
        {
            this.bUseAdvanceSelect = b;
        }

        public void SetUseSkillTarget()
        {
            ActorRoot actorRoot = null;
            Skill skill = (this.skillSlot.NextSkillObj == null) ? this.skillSlot.SkillObj : this.skillSlot.NextSkillObj;
            if (((skill != null) && (skill.cfgData != null)) && (skill.cfgData.bWheelType != 1))
            {
                uint dwSkillTargetFilter = skill.cfgData.dwSkillTargetFilter;
                if (skill.cfgData.bRangeAppointType == 1)
                {
                    if ((!this.bUseAdvanceSelect && (this.guidePrefab != null)) && this.guideSelectActive)
                    {
                        actorRoot = Singleton<SkillSelectControl>.GetInstance().SelectTarget((SkillTargetRule) skill.cfgData.bSkillTargetRule, this.skillSlot);
                    }
                    else if ((this.bUseAdvanceSelect && (this.effectPrefab != null)) && this.effectSelectActive)
                    {
                        int srchR = 0;
                        if (this.skillSlot.SlotType != SkillSlotType.SLOT_SKILL_0)
                        {
                            srchR = (int) skill.cfgData.iMaxAttackDistance;
                        }
                        else
                        {
                            srchR = skill.GetMaxSearchDistance(this.skillSlot.GetSkillLevel());
                        }
                        uint filter = 2;
                        filter |= dwSkillTargetFilter;
                        actorRoot = Singleton<SectorTargetSearcher>.GetInstance().GetEnemyTarget((ActorRoot) this.skillSlot.Actor, srchR, this.useSkillDirection, 50f, filter);
                        if (actorRoot == null)
                        {
                            uint num4 = 1;
                            num4 |= dwSkillTargetFilter;
                            actorRoot = Singleton<SectorTargetSearcher>.GetInstance().GetEnemyTarget((ActorRoot) this.skillSlot.Actor, srchR, this.useSkillDirection, 50f, num4);
                        }
                    }
                    if (actorRoot != this.targetActor)
                    {
                        this.StopCommonAttackTargetEffect(this.targetActor);
                        if (((this.skillSlot.Actor != 0) && (actorRoot != null)) && (actorRoot.ObjID != this.skillSlot.Actor.handle.ObjID))
                        {
                            this.PlayCommonAttackTargetEffect(actorRoot);
                        }
                        this.targetActor = actorRoot;
                    }
                }
            }
        }

        public void SetUseSkillTarget(ActorRoot actorRoot)
        {
            if (actorRoot != this.targetActor)
            {
                if (this.targetActor != null)
                {
                    this.StopCommonAttackTargetEffect(this.targetActor);
                }
                if (actorRoot != null)
                {
                    this.PlayCommonAttackTargetEffect(actorRoot);
                }
                this.targetActor = actorRoot;
            }
        }

        private void ShowPrefab(GameObject _prefab)
        {
            if (_prefab != null)
            {
                _prefab.SetLayer("Actor", "Particles", false);
            }
        }

        private void ShowPrefabEffect()
        {
            this.ShowPrefab(this.effectPrefab);
        }

        private void StopCommonAttackTargetEffect(ActorRoot actorRoot)
        {
            Singleton<SkillIndicateSystem>.GetInstance().StopCommonAttackTargetEffect();
            if ((actorRoot != null) && (actorRoot.MatHurtEffect != null))
            {
                actorRoot.MatHurtEffect.StopHighLitEffect(this.hlcId);
            }
        }

        public void UnInitIndicatePrefab(bool bDestroy)
        {
            if ((this.skillSlot.Actor != 0) && ActorHelper.IsHostActor(ref this.skillSlot.Actor))
            {
                if (this.guidePrefab != null)
                {
                    this.HidePrefab(this.guidePrefab);
                    this.guideSelectActive = false;
                    if (bDestroy)
                    {
                        Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.guidePrefab);
                        this.guidePrefab = null;
                    }
                }
                if (this.effectPrefabNormal != null)
                {
                    this.HidePrefab(this.effectPrefabNormal);
                    this.effectSelectActive = false;
                    if (bDestroy)
                    {
                        Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.effectPrefabNormal);
                        this.effectPrefabNormal = null;
                    }
                }
                if (this.effectPrefabBlock != null)
                {
                    this.HidePrefab(this.effectPrefabBlock);
                    if (bDestroy)
                    {
                        Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.effectPrefabBlock);
                        this.effectPrefabBlock = null;
                    }
                }
                if (this.effectPrefabGrass != null)
                {
                    this.HidePrefab(this.effectPrefabGrass);
                    if (bDestroy)
                    {
                        Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.effectPrefabGrass);
                        this.effectPrefabGrass = null;
                    }
                }
                if (this.guideWarnPrefab != null)
                {
                    this.HidePrefab(this.guideWarnPrefab);
                    if (bDestroy)
                    {
                        Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.guideWarnPrefab);
                        this.guideWarnPrefab = null;
                    }
                }
                if (this.effectWarnPrefab != null)
                {
                    this.HidePrefab(this.effectWarnPrefab);
                    if (bDestroy)
                    {
                        Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.effectWarnPrefab);
                        this.effectWarnPrefab = null;
                    }
                }
                if (this.fixedPrefab != null)
                {
                    this.HidePrefab(this.fixedPrefab);
                    if (bDestroy)
                    {
                        Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.fixedPrefab);
                        this.fixedPrefab = null;
                    }
                }
                if (this.fixedWarnPrefab != null)
                {
                    this.HidePrefab(this.fixedWarnPrefab);
                    if (bDestroy)
                    {
                        Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.fixedWarnPrefab);
                        this.fixedWarnPrefab = null;
                    }
                }
            }
        }

        public void UpdatePrefabScaler(Skill _skillObj)
        {
            if (_skillObj != null)
            {
                int iFixedDistance = (int) _skillObj.cfgData.iFixedDistance;
                int iGuideDistance = (int) _skillObj.cfgData.iGuideDistance;
                this.SetPrefabScaler(this.guidePrefab, iGuideDistance);
                this.SetPrefabScaler(this.guideWarnPrefab, iGuideDistance);
                if ((_skillObj.cfgData.bRangeAppointType == 3) || (_skillObj.cfgData.bRangeAppointType == 1))
                {
                    this.SetPrefabScaler(this.effectPrefabNormal, iGuideDistance);
                    this.SetPrefabScaler(this.effectPrefabBlock, iGuideDistance);
                    this.SetPrefabScaler(this.effectPrefabGrass, iGuideDistance);
                    this.SetPrefabScaler(this.effectWarnPrefab, iGuideDistance);
                }
                this.SetPrefabScaler(this.fixedPrefab, iFixedDistance);
                this.SetPrefabScaler(this.fixedWarnPrefab, iFixedDistance);
            }
        }

        public GameObject effectPrefab
        {
            get
            {
                if (!this.bSelectEffectPrefab)
                {
                    return this.effectPrefabNormal;
                }
                GameObject effectPrefabNormal = this.effectPrefabNormal;
                if ((this.PrefabSelecter == EEffectPrefabSelect.Block) && (this.effectPrefabBlock != null))
                {
                    return this.effectPrefabBlock;
                }
                if ((this.PrefabSelecter == EEffectPrefabSelect.Grass) && (this.effectPrefabGrass != null))
                {
                    effectPrefabNormal = this.effectPrefabGrass;
                }
                return effectPrefabNormal;
            }
        }

        private EEffectPrefabSelect PrefabSelecter
        {
            get
            {
                return this.m_prefabSelecter;
            }
            set
            {
                if (this.m_prefabSelecter != value)
                {
                    GameObject effectPrefab = this.effectPrefab;
                    bool flag = this.IsPrefabVisible(this.effectPrefab);
                    this.m_prefabSelecter = value;
                    if (flag)
                    {
                        this.HidePrefab(effectPrefab);
                        this.ShowPrefab(this.effectPrefab);
                        this.effectPrefab.get_transform().set_position(effectPrefab.get_transform().get_position());
                    }
                }
            }
        }

        private enum EEffectPrefabSelect
        {
            Normal,
            Block,
            Grass
        }
    }
}

