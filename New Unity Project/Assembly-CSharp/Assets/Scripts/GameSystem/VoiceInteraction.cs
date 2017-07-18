namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic.GameKernal;
    using ResData;
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public abstract class VoiceInteraction
    {
        protected bool bBeginTrigger;
        protected ResVoiceInteraction InteractionCfg;
        protected float LastGroupTriggerTime;
        protected float LastTriggerTime;
        protected static Random Rand = new Random();
        protected PoolObjHandle<ActorRoot> SoundSourceActor;
        protected int TriggerCount;

        public event OnInteractionTriggerDelegate OnBeginTriggered;

        protected VoiceInteraction()
        {
        }

        protected virtual bool BeginTrigger(ref PoolObjHandle<ActorRoot> InSoundSource)
        {
            this.bBeginTrigger = true;
            this.SoundSourceActor = InSoundSource;
            if (this.OnBeginTriggered != null)
            {
                this.OnBeginTriggered(this);
            }
            return true;
        }

        protected virtual bool CheckReceiveDistance(ref PoolObjHandle<ActorRoot> InSource, ref PoolObjHandle<ActorRoot> InRelevance)
        {
            Vector3 location = (Vector3) InSource.handle.location;
            Vector3 vector2 = (Vector3) InRelevance.handle.location;
            float num = Vector3.Distance(location, vector2) * 1000f;
            return (num <= this.InteractionCfg.dwReceiveRadius);
        }

        protected virtual bool CheckTriggerDistance(ref PoolObjHandle<ActorRoot> InSource, ref PoolObjHandle<ActorRoot> InRelevance)
        {
            Vector3 location = (Vector3) InSource.handle.location;
            Vector3 vector2 = (Vector3) InRelevance.handle.location;
            float num = Vector3.Distance(location, vector2) * 1000f;
            return (num <= this.InteractionCfg.dwTriggerRadius);
        }

        public void DoTrigger()
        {
            this.LastTriggerTime = Time.get_realtimeSinceStartup();
            this.TriggerCount++;
            if (this.InteractionCfg != null)
            {
                Singleton<CSoundManager>.instance.PlayBattleSound(this.InteractionCfg.szVoiceEvent, this.SoundSourceActor, null);
            }
        }

        public void FinishTrigger(float InGroupTriggerTime)
        {
            this.LastGroupTriggerTime = InGroupTriggerTime;
            this.bBeginTrigger = false;
            this.SoundSourceActor = new PoolObjHandle<ActorRoot>();
        }

        protected virtual bool ForwardCheck()
        {
            if (this.InteractionCfg == null)
            {
                return false;
            }
            if ((this.InteractionCfg.bTriggerCount != 0) && (this.TriggerCount >= this.InteractionCfg.bTriggerCount))
            {
                return false;
            }
            float num = Time.get_realtimeSinceStartup();
            if (((num - this.LastTriggerTime) < this.InteractionCfg.dwTriggerInterval) || ((num - this.LastGroupTriggerTime) < this.InteractionCfg.dwGroupTriggerInterval))
            {
                return false;
            }
            if (Rand.Next(0, 0x2710) > this.InteractionCfg.dwTriggerProbility)
            {
                return false;
            }
            return true;
        }

        public virtual void Init(ResVoiceInteraction InInteractionCfg)
        {
            this.InteractionCfg = InInteractionCfg;
        }

        public bool IsSpecialTriggerID(int InCheckID)
        {
            if (this.InteractionCfg != null)
            {
                for (int i = 0; i < this.InteractionCfg.SpecialTriggerConditions.Length; i++)
                {
                    if (this.InteractionCfg.SpecialTriggerConditions[i] == 0)
                    {
                        return false;
                    }
                    if (this.InteractionCfg.SpecialTriggerConditions[i] == InCheckID)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected virtual bool TryTrigger(ref PoolObjHandle<ActorRoot> InSource, ref PoolObjHandle<ActorRoot> InRelevance, ref PoolObjHandle<ActorRoot> InSoundSource)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
            if (((hostPlayer != null) && (hostPlayer.Captain != 0)) && (InSource != 0))
            {
                bool flag = false;
                if ((this.InteractionCfg.bReceiveType == 1) && InSource.handle.IsEnemyCamp(hostPlayer.Captain.handle))
                {
                    flag = true;
                }
                else if (this.InteractionCfg.bReceiveType == 100)
                {
                    flag = true;
                }
                else if ((this.InteractionCfg.bReceiveType == 0) && InSource.handle.IsSelfCamp(hostPlayer.Captain.handle))
                {
                    flag = true;
                }
                if (!flag)
                {
                    return false;
                }
                if (!this.isSpecialReceiver)
                {
                    return this.BeginTrigger(ref InSoundSource);
                }
                int configId = hostPlayer.Captain.handle.TheActorMeta.ConfigId;
                for (int i = 0; i < this.InteractionCfg.SpecialReceiveConditions.Length; i++)
                {
                    if (this.InteractionCfg.SpecialReceiveConditions[i] == 0)
                    {
                        break;
                    }
                    if (this.InteractionCfg.SpecialReceiveConditions[i] == configId)
                    {
                        return this.BeginTrigger(ref InSoundSource);
                    }
                }
            }
            return false;
        }

        public virtual void Unit()
        {
            this.InteractionCfg = null;
            this.OnBeginTriggered = null;
            this.SoundSourceActor = new PoolObjHandle<ActorRoot>();
            this.bBeginTrigger = false;
            this.TriggerCount = 0;
        }

        protected bool ValidateTriggerActor(ref PoolObjHandle<ActorRoot> InTestActor)
        {
            if (InTestActor == 0)
            {
                return false;
            }
            if (this.isSpecialTrigger)
            {
                return this.IsSpecialTriggerID(InTestActor.handle.TheActorMeta.ConfigId);
            }
            return true;
        }

        public int groupID
        {
            get
            {
                return ((this.InteractionCfg == null) ? ((int) 0) : ((int) this.InteractionCfg.dwGroupID));
            }
        }

        public bool isBeginTrigger
        {
            get
            {
                return this.bBeginTrigger;
            }
            set
            {
                this.bBeginTrigger = value;
            }
        }

        public bool isSpecialReceiver
        {
            get
            {
                return ((this.InteractionCfg != null) && (this.InteractionCfg.bSpecialReceive != 0));
            }
        }

        public bool isSpecialTrigger
        {
            get
            {
                return ((this.InteractionCfg != null) && (this.InteractionCfg.bSpecialTrigger != 0));
            }
        }

        public int priority
        {
            get
            {
                return ((this.InteractionCfg == null) ? -1 : this.InteractionCfg.bPriorityInGroup);
            }
        }
    }
}

