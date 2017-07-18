﻿namespace Assets.Scripts.GameLogic
{
    using System;

    public enum GameEventDef
    {
        Event_GameEnd,
        Event_FightOver,
        Event_FightPrepare,
        Event_FightStart,
        Event_FightPrepareFin,
        Event_BeginFightOver,
        Event_ActorDead,
        Event_PostActorDead,
        Event_ActorDestroy,
        Event_ActorRevive,
        Event_ActorCrit,
        Event_ActorImmune,
        Event_ActorImmuneDeadHurt,
        Event_ActorHurtAbsorb,
        Event_ActorInit,
        Event_ActorMoveCity,
        Event_ActorPreFight,
        Event_ActorStartFight,
        Event_SoldierWaveNext,
        Event_SoldierWaveNextRepeat,
        Event_ActorDamage,
        Event_ActorBeAttack,
        Event_ActorClearMove,
        Event_CaptainSwitch,
        Event_ActorEnterCombat,
        Event_ActorExitCombat,
        Event_ActorLearnTalent,
        Event_LobbyRelogining,
        Event_MultiRecoverFin,
        Event_MonsterGroupDead,
        Event_DoubleKill,
        Event_TripleKill,
        Event_Odyssey,
        Event_AutoAISwitch,
        Event_CameraHeightChange,
        Event_TalentLevelChange,
        Event_ActorBeChosenAsTarget,
        Event_AddExpValue,
        Event_HitTrigger,
        Event_SpawnGroupStartCount,
        Event_SpawnGroupSpawn,
        Event_SpawnGroupDead,
        Event_TailsmanSpawn,
        Event_TailsmanUsed,
        Event_CampScoreUpdated,
        Event_QuataryKill,
        Event_PentaKill,
        Event_OdysseyBeStopped,
        Event_SettleComplete,
        Event_PreDialogStarted,
        Event_Hemophagia,
        Event_ActorInGrass,
        Event_Max
    }
}

