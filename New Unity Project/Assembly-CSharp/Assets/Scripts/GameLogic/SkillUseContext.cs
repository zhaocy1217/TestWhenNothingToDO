namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using ResData;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class SkillUseContext
    {
        [CompilerGenerated]
        private SkillRangeAppointType <AppointType>k__BackingField;
        [CompilerGenerated]
        private VInt3 <BulletPos>k__BackingField;
        [CompilerGenerated]
        private VInt3 <EffectDir>k__BackingField;
        [CompilerGenerated]
        private VInt3 <EffectPos>k__BackingField;
        [CompilerGenerated]
        private VInt3 <EndVector>k__BackingField;
        [CompilerGenerated]
        private SkillSlotType <SlotType>k__BackingField;
        [CompilerGenerated]
        private uint <TargetID>k__BackingField;
        [CompilerGenerated]
        private VInt3 <UseVector>k__BackingField;
        public bool bExposing;
        public bool bSpecialUse;
        public int EffectCount;
        public int EffectCountInSingleTrigger;
        public int GatherTime;
        public object Instigator;
        public int MarkCount;
        public PoolObjHandle<ActorRoot> Originator;
        public PoolObjHandle<ActorRoot> TargetActor;

        public SkillUseContext()
        {
            this.GatherTime = 1;
            this.SlotType = SkillSlotType.SLOT_SKILL_VALID;
        }

        public SkillUseContext(SkillSlotType InSlot)
        {
            this.GatherTime = 1;
            this.SlotType = InSlot;
            this.bSpecialUse = false;
            this.AppointType = SkillRangeAppointType.Target;
        }

        public SkillUseContext(SkillSlotType InSlot, PoolObjHandle<ActorRoot> InActorRoot)
        {
            this.GatherTime = 1;
            this.SlotType = InSlot;
            this.TargetActor = InActorRoot;
            this.bSpecialUse = false;
        }

        public SkillUseContext(SkillSlotType InSlot, uint ObjID)
        {
            this.GatherTime = 1;
            this.SlotType = InSlot;
            this.TargetID = ObjID;
            this.TargetActor = Singleton<GameObjMgr>.GetInstance().GetActor(this.TargetID);
            this.UseVector = (this.TargetActor == 0) ? VInt3.zero : this.TargetActor.handle.location;
            this.AppointType = SkillRangeAppointType.Target;
            this.bSpecialUse = false;
        }

        public SkillUseContext(SkillSlotType InSlot, VInt3 InVec)
        {
            this.GatherTime = 1;
            this.SlotType = InSlot;
            this.UseVector = InVec;
            this.AppointType = SkillRangeAppointType.Pos;
            this.bSpecialUse = false;
        }

        public SkillUseContext(SkillSlotType InSlot, VInt3 InVec, bool bSpecial)
        {
            this.GatherTime = 1;
            this.SlotType = InSlot;
            this.UseVector = InVec;
            this.AppointType = SkillRangeAppointType.Directional;
            this.bSpecialUse = bSpecial;
        }

        public SkillUseContext(SkillSlotType InSlot, VInt3 InBegin, VInt3 InEnd)
        {
            this.GatherTime = 1;
            this.SlotType = InSlot;
            this.UseVector = InBegin;
            this.EndVector = InEnd;
            this.bSpecialUse = false;
        }

        public bool CalcAttackerDir(out VInt3 dir, PoolObjHandle<ActorRoot> attacker)
        {
            if (attacker == 0)
            {
                dir = VInt3.forward;
                return false;
            }
            dir = attacker.handle.forward;
            switch (this.AppointType)
            {
                case SkillRangeAppointType.Auto:
                case SkillRangeAppointType.Target:
                    if (this.TargetActor == 0)
                    {
                        return false;
                    }
                    dir = this.TargetActor.handle.location;
                    dir -= attacker.handle.location;
                    dir.y = 0;
                    break;

                case SkillRangeAppointType.Pos:
                    dir = this.UseVector;
                    dir -= attacker.handle.location;
                    dir.y = 0;
                    break;

                case SkillRangeAppointType.Directional:
                    dir = this.UseVector;
                    dir.y = 0;
                    break;

                case SkillRangeAppointType.Track:
                {
                    VInt3 num = this.EndVector + this.UseVector;
                    dir = num.DivBy2() - attacker.handle.location;
                    dir.y = 0;
                    break;
                }
            }
            return true;
        }

        public void Copy(SkillUseContext rhs)
        {
            this.SlotType = rhs.SlotType;
            this.AppointType = rhs.AppointType;
            this.TargetID = rhs.TargetID;
            this.UseVector = rhs.UseVector;
            this.EndVector = rhs.EndVector;
            this.EffectPos = rhs.EffectPos;
            this.EffectDir = rhs.EffectDir;
            this.BulletPos = rhs.BulletPos;
            this.TargetActor = rhs.TargetActor;
            this.Originator = rhs.Originator;
            this.Instigator = rhs.Instigator;
            this.bSpecialUse = rhs.bSpecialUse;
            this.GatherTime = rhs.GatherTime;
            this.EffectCount = rhs.EffectCount;
            this.EffectCountInSingleTrigger = rhs.EffectCountInSingleTrigger;
            this.MarkCount = rhs.MarkCount;
            this.bExposing = rhs.bExposing;
        }

        public void Copy(ref SkillUseParam param)
        {
            this.SlotType = param.SlotType;
            this.AppointType = param.AppointType;
            this.TargetID = param.TargetID;
            this.UseVector = param.UseVector;
            this.bSpecialUse = param.bSpecialUse;
            this.TargetActor = param.TargetActor;
            this.Originator = param.Originator;
            this.Instigator = param.Instigator;
            this.bExposing = param.bExposing;
        }

        public override bool Equals(object obj)
        {
            return (((obj != null) && (base.GetType() == obj.GetType())) && this.IsEquals((SkillUseContext) obj));
        }

        private bool IsEquals(SkillUseContext rhs)
        {
            return ((((((this.SlotType == rhs.SlotType) && (this.AppointType == rhs.AppointType)) && ((this.TargetID == rhs.TargetID) && (this.UseVector == rhs.UseVector))) && (((this.EndVector == rhs.EndVector) && (this.EffectPos == rhs.EffectPos)) && ((this.BulletPos == rhs.BulletPos) && (this.EffectDir == rhs.EffectDir)))) && ((((this.TargetActor == rhs.TargetActor) && (this.Originator == rhs.Originator)) && ((this.Instigator == rhs.Instigator) && (this.bSpecialUse == rhs.bSpecialUse))) && (((this.GatherTime == rhs.GatherTime) && (this.EffectCount == rhs.EffectCount)) && ((this.EffectCountInSingleTrigger == rhs.EffectCountInSingleTrigger) && (this.MarkCount == rhs.MarkCount))))) && (this.bExposing == rhs.bExposing));
        }

        public void Reset()
        {
            this.SlotType = SkillSlotType.SLOT_SKILL_VALID;
            this.AppointType = SkillRangeAppointType.Auto;
            this.TargetID = 0;
            this.UseVector = VInt3.zero;
            this.EndVector = VInt3.zero;
            this.EffectPos = VInt3.zero;
            this.EffectDir = VInt3.zero;
            this.BulletPos = VInt3.zero;
            this.bSpecialUse = false;
            this.GatherTime = 1;
            this.EffectCount = 0;
            this.EffectCountInSingleTrigger = 0;
            this.MarkCount = 0;
            this.TargetActor.Release();
            this.Originator.Release();
            this.Instigator = null;
            this.bExposing = false;
        }

        public void SetOriginator(PoolObjHandle<ActorRoot> inOriginator)
        {
            this.Originator = inOriginator;
        }

        public SkillRangeAppointType AppointType
        {
            [CompilerGenerated]
            get
            {
                return this.<AppointType>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<AppointType>k__BackingField = value;
            }
        }

        public VInt3 BulletPos
        {
            [CompilerGenerated]
            get
            {
                return this.<BulletPos>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<BulletPos>k__BackingField = value;
            }
        }

        public VInt3 EffectDir
        {
            [CompilerGenerated]
            get
            {
                return this.<EffectDir>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<EffectDir>k__BackingField = value;
            }
        }

        public VInt3 EffectPos
        {
            [CompilerGenerated]
            get
            {
                return this.<EffectPos>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<EffectPos>k__BackingField = value;
            }
        }

        public VInt3 EndVector
        {
            [CompilerGenerated]
            get
            {
                return this.<EndVector>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<EndVector>k__BackingField = value;
            }
        }

        public SkillSlotType SlotType
        {
            [CompilerGenerated]
            get
            {
                return this.<SlotType>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<SlotType>k__BackingField = value;
            }
        }

        public uint TargetID
        {
            [CompilerGenerated]
            get
            {
                return this.<TargetID>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<TargetID>k__BackingField = value;
            }
        }

        public VInt3 UseVector
        {
            [CompilerGenerated]
            get
            {
                return this.<UseVector>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<UseVector>k__BackingField = value;
            }
        }
    }
}

