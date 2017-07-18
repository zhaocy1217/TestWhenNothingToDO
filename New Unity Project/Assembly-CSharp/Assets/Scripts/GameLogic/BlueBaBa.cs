namespace Assets.Scripts.GameLogic
{
    using System;
    using UnityEngine;

    public class BlueBaBa : BasePet
    {
        public override void LateUpdate(int nDelta)
        {
            if (base.CheckUpdate())
            {
                Vector3 vector;
                Vector3 vector2;
                base.deltaTime += nDelta;
                if (base.deltaTime > 500)
                {
                    base.deltaTime -= 500;
                    vector = base.petTrans.get_position();
                    vector2 = base.parentTrans.get_localToWorldMatrix().MultiplyPoint(base.offset);
                    float num = Vector3.Distance(vector, vector2);
                    if (num > base.offsetDistance)
                    {
                        base.curState = PetState.Run;
                        base.PlayAnimation("Run", 0.05f);
                        base.moveDir = vector2 - vector;
                        this.moveDir.Normalize();
                        float num2 = this.actorPtr.handle.ValueComponent.actorMoveSpeed * 1E-06f;
                        if (num < (3f * base.offsetDistance))
                        {
                            base.moveSpeed = num2;
                        }
                        else if (num < (10f * base.offsetDistance))
                        {
                            base.moveSpeed = num2 * 1.5f;
                        }
                        else
                        {
                            base.petTrans.set_position(vector2);
                            base.petTrans.set_rotation(base.parentTrans.get_rotation());
                        }
                    }
                    else
                    {
                        base.curState = PetState.Idle;
                        base.PlayAnimation("Idle", 0.05f);
                    }
                }
                else if (base.curState == PetState.Run)
                {
                    base.petTrans.set_position(base.petTrans.get_position() + ((Vector3) ((base.moveSpeed * base.moveDir) * nDelta)));
                    base.petTrans.set_rotation(base.ObjRotationLerp(base.moveDir, nDelta));
                    vector = base.petTrans.get_position();
                    vector2 = base.parentTrans.get_localToWorldMatrix().MultiplyPoint(base.offset);
                    if (Vector3.Distance(vector, vector2) < base.offsetDistance)
                    {
                        base.curState = PetState.Idle;
                        base.PlayAnimation("Idle", 0.05f);
                    }
                }
            }
        }
    }
}

