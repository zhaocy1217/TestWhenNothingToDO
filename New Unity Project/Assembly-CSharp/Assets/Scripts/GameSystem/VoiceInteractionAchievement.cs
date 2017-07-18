namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.GameLogic;
    using ResData;
    using System;

    [VoiceInteraction(4)]
    public class VoiceInteractionAchievement : VoiceInteraction
    {
        public override void Init(ResVoiceInteraction InInteractionCfg)
        {
            base.Init(InInteractionCfg);
            Singleton<EventRouter>.instance.AddEventHandler<KillDetailInfo>(EventID.AchievementRecorderEvent, new Action<KillDetailInfo>(this.OnAchievementEvent));
        }

        private void OnAchievementEvent(KillDetailInfo DetailInfo)
        {
            if (this.ForwardCheck() && (((DetailInfo.Killer != 0) && (DetailInfo.Killer.handle.TheActorMeta.ConfigId == base.groupID)) && (this.achievementType == DetailInfo.Type)))
            {
                this.TryTrigger(ref DetailInfo.Killer, ref DetailInfo.Victim, ref DetailInfo.Killer);
            }
        }

        public override void Unit()
        {
            Singleton<EventRouter>.instance.RemoveEventHandler<KillDetailInfo>(EventID.AchievementRecorderEvent, new Action<KillDetailInfo>(this.OnAchievementEvent));
            base.Unit();
        }

        private int achievementType
        {
            get
            {
                return (int) base.InteractionCfg.SpecialTriggerConditions[0];
            }
        }
    }
}

