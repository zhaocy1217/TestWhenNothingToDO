namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.DataCenter;
    using Assets.Scripts.UI;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine.UI;

    public class KillNotifyUT
    {
        public static KillInfo Convert_DetailInfo_KillInfo(KillDetailInfo detail)
        {
            KillDetailInfoType type = KillDetailInfoType.Info_Type_None;
            PoolObjHandle<ActorRoot> killer = detail.Killer;
            PoolObjHandle<ActorRoot> victim = detail.Victim;
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            if (killer != 0)
            {
                flag = killer.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ;
                flag2 = killer.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster;
                flag3 = killer.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero;
            }
            KillInfo info = new KillInfo();
            info.KillerImgSrc = info.VictimImgSrc = string.Empty;
            info.MsgType = detail.Type;
            info.bPlayerSelf_KillOrKilled = detail.bPlayerSelf_KillOrKilled;
            info.actorType = (killer == 0) ? ActorTypeDef.Invalid : killer.handle.TheActorMeta.ActorType;
            info.bSrcAllies = detail.bSelfCamp;
            info.assistImgSrc = new string[4];
            if (flag2)
            {
                info.KillerImgSrc = KillNotify.monster_icon;
            }
            if (flag)
            {
                info.KillerImgSrc = KillNotify.building_icon;
            }
            if (flag3)
            {
                info.KillerImgSrc = GetHero_Icon(detail.Killer, false);
            }
            if (killer != 0)
            {
                if (killer.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
                {
                    info.KillerImgSrc = KillNotify.building_icon;
                }
                if (killer.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
                {
                    MonsterWrapper wrapper = killer.handle.AsMonster();
                    if (killer.handle.TheActorMeta.ConfigId == Singleton<BattleLogic>.instance.DragonId)
                    {
                        info.KillerImgSrc = KillNotify.dragon_icon;
                    }
                    else if ((wrapper.cfgInfo != null) && (wrapper.cfgInfo.bMonsterType == 2))
                    {
                        info.KillerImgSrc = KillNotify.yeguai_icon;
                    }
                    else
                    {
                        info.KillerImgSrc = KillNotify.monster_icon;
                    }
                }
                if (killer.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
                {
                    info.KillerImgSrc = GetHero_Icon(killer, false);
                }
            }
            if (victim != 0)
            {
                if (victim.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
                {
                    info.VictimImgSrc = GetHero_Icon(victim, false);
                }
                if (victim.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
                {
                    info.VictimImgSrc = KillNotify.building_icon;
                }
                if ((victim.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster) && (victim.handle.TheActorMeta.ConfigId == Singleton<BattleLogic>.instance.DragonId))
                {
                    info.VictimImgSrc = KillNotify.dragon_icon;
                }
            }
            if ((detail.assistList != null) && (info.assistImgSrc != null))
            {
                for (int i = 0; i < detail.assistList.Count; i++)
                {
                    uint num2 = detail.assistList[i];
                    for (int j = 0; j < Singleton<GameObjMgr>.instance.HeroActors.Count; j++)
                    {
                        PoolObjHandle<ActorRoot> handle3 = Singleton<GameObjMgr>.instance.HeroActors[j];
                        if (num2 == handle3.handle.ObjID)
                        {
                            info.assistImgSrc[i] = GetHero_Icon(Singleton<GameObjMgr>.instance.HeroActors[j], false);
                        }
                    }
                }
            }
            if (detail.Type == KillDetailInfoType.Info_Type_Soldier_Boosted)
            {
                info.KillerImgSrc = !detail.bSelfCamp ? KillNotify.red_soldier_icon : KillNotify.blue_soldier_icon;
                return info;
            }
            if ((detail.Type == KillDetailInfoType.Info_Type_Reconnect) || (detail.Type == KillDetailInfoType.Info_Type_RunningMan))
            {
                info.VictimImgSrc = string.Empty;
                return info;
            }
            if (detail.HeroMultiKillType != type)
            {
                info.MsgType = detail.HeroMultiKillType;
                return info;
            }
            if (detail.Type != KillDetailInfoType.Info_Type_StopMultiKill)
            {
                if (detail.Type == KillDetailInfoType.Info_Type_First_Kill)
                {
                    return info;
                }
                if (detail.Type == KillDetailInfoType.Info_Type_DestroyTower)
                {
                    return info;
                }
                if (((detail.Type == KillDetailInfoType.Info_Type_Kill_3V3_Dragon) || (detail.Type == KillDetailInfoType.Info_Type_Kill_5V5_SmallDragon)) || (detail.Type == KillDetailInfoType.Info_Type_Kill_5V5_BigDragon))
                {
                    if (flag2)
                    {
                        info.KillerImgSrc = KillNotify.monster_icon;
                    }
                    if (flag)
                    {
                        info.KillerImgSrc = KillNotify.building_icon;
                    }
                    if (flag3)
                    {
                        info.KillerImgSrc = GetHero_Icon(detail.Killer, false);
                    }
                    info.bSrcAllies = detail.bSelfCamp;
                    return info;
                }
                if (detail.bAllDead)
                {
                    info.MsgType = KillDetailInfoType.Info_Type_AllDead;
                    return info;
                }
                if (detail.HeroContiKillType != type)
                {
                    info.MsgType = detail.HeroContiKillType;
                    return info;
                }
                if (detail.Type == KillDetailInfoType.Info_Type_Kill)
                {
                    return info;
                }
            }
            return info;
        }

        public static List<string> GetAllAnimations()
        {
            List<string> list = new List<string>();
            Array values = Enum.GetValues(typeof(KillDetailInfoType));
            for (int i = 0; i < values.Length; i++)
            {
                KillDetailInfoType type = (KillDetailInfoType) ((int) values.GetValue(i));
                string animation = GetAnimation(type, true);
                if (!string.IsNullOrEmpty(animation))
                {
                    list.Add(animation);
                }
                animation = GetAnimation(type, false);
                if (!string.IsNullOrEmpty(animation))
                {
                    list.Add(animation);
                }
            }
            return list;
        }

        public static string GetAnimation(KillDetailInfoType Type, bool bSrc)
        {
            string str = !bSrc ? "_B" : "_A";
            string str2 = string.Empty;
            bool flag = false;
            KillDetailInfoType type = Type;
            switch (type)
            {
                case KillDetailInfoType.Info_Type_First_Kill:
                    str2 = "FirstBlood";
                    break;

                case KillDetailInfoType.Info_Type_Kill:
                    str2 = "NormalKill";
                    break;

                case KillDetailInfoType.Info_Type_DoubleKill:
                    str2 = "DoubleKill";
                    break;

                case KillDetailInfoType.Info_Type_TripleKill:
                    str2 = "TrebleKill";
                    break;

                case KillDetailInfoType.Info_Type_QuataryKill:
                    str2 = "QuataryKill";
                    break;

                case KillDetailInfoType.Info_Type_PentaKill:
                    str2 = "PentaKill";
                    break;

                case KillDetailInfoType.Info_Type_MonsterKill:
                    str2 = "DaShaTeSha";
                    break;

                case KillDetailInfoType.Info_Type_DominateBattle:
                    str2 = "ShaRenRuMa";
                    break;

                case KillDetailInfoType.Info_Type_Legendary:
                    str2 = "WuRenNenDang";
                    break;

                case KillDetailInfoType.Info_Type_TotalAnnihilat:
                    str2 = "HengSaoQianJun";
                    break;

                case KillDetailInfoType.Info_Type_Odyssey:
                    str2 = "TianXiaWuShuang";
                    break;

                case KillDetailInfoType.Info_Type_DestroyTower:
                    str2 = "BreakTower";
                    break;

                default:
                    switch (type)
                    {
                        case KillDetailInfoType.Info_Type_Cannon_Spawned:
                            str2 = "GongChengCheJiaRu";
                            break;

                        case KillDetailInfoType.Info_Type_Soldier_Boosted:
                            str2 = "XiaoBingZengQiang";
                            break;

                        case KillDetailInfoType.Info_Type_Game_Start_Wel:
                            return "Welcome";

                        case KillDetailInfoType.Info_Type_Soldier_Activate_Countdown3:
                            return "ThreeSecond";

                        case KillDetailInfoType.Info_Type_Soldier_Activate_Countdown5:
                            return "FiveSecond";

                        case KillDetailInfoType.Info_Type_Soldier_Activate:
                            return "Battle";

                        default:
                            switch (type)
                            {
                                case KillDetailInfoType.Info_Type_Kill_3V3_Dragon:
                                    str2 = "DragonKill";
                                    break;

                                case KillDetailInfoType.Info_Type_Kill_5V5_SmallDragon:
                                    str2 = "KillJuLong";
                                    break;

                                case KillDetailInfoType.Info_Type_Kill_5V5_BigDragon:
                                    str2 = "KillZhuZai";
                                    break;

                                case KillDetailInfoType.Info_Type_RunningMan:
                                    str2 = "ExitGame";
                                    break;

                                case KillDetailInfoType.Info_Type_Reconnect:
                                    str2 = "ChongLian";
                                    break;

                                case KillDetailInfoType.Info_Type_Disconnect:
                                    str2 = "ExitGame";
                                    break;

                                case KillDetailInfoType.Info_Type_FireHole_first:
                                    str2 = "YouShi";
                                    break;

                                case KillDetailInfoType.Info_Type_FireHole_second:
                                    str2 = "JiJiangShengLi";
                                    break;

                                case KillDetailInfoType.Info_Type_FireHole_third:
                                    str2 = "ShengLi";
                                    break;

                                case KillDetailInfoType.Info_Type_AllDead:
                                    str2 = "Ace";
                                    break;

                                case KillDetailInfoType.Info_Type_StopMultiKill:
                                    str2 = "EndKill";
                                    break;
                            }
                            flag = true;
                            break;
                    }
                    break;
            }
            if (flag)
            {
                return string.Empty;
            }
            return (str2 + str);
        }

        public static string GetHero_Icon(PoolObjHandle<ActorRoot> actor, bool bSmall = false)
        {
            if (actor == 0)
            {
                return string.Empty;
            }
            return GetHero_Icon(ref actor.handle.TheActorMeta, bSmall);
        }

        public static string GetHero_Icon(ActorRoot actor, bool bSmall)
        {
            string str = string.Empty;
            if (actor != null)
            {
                str = GetHero_Icon(ref actor.TheActorMeta, bSmall);
            }
            return str;
        }

        public static string GetHero_Icon(ref ActorMeta actorMeta, bool bSmall = false)
        {
            string str = string.Empty;
            IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.ServerDataProvider);
            ActorServerData actorData = new ActorServerData();
            if ((actorDataProvider != null) && actorDataProvider.GetActorServerData(ref actorMeta, ref actorData))
            {
                str = GetHero_Icon((uint) actorData.TheActorMeta.ConfigId, 0, bSmall);
            }
            return str;
        }

        public static string GetHero_Icon(uint ConfigID, uint SkinID = 0, bool bSmall = false)
        {
            string heroSkinPic = CSkinInfo.GetHeroSkinPic(ConfigID, SkinID);
            return string.Format("{0}{1}", !bSmall ? CUIUtility.s_Sprite_Dynamic_BustCircle_Dir : CUIUtility.s_Sprite_Dynamic_BustCircleSmall_Dir, heroSkinPic);
        }

        public static string GetSoundEvent(KillDetailInfoType Type, bool bSrcAllies, bool bSelfKillORKilled, ActorTypeDef actorType)
        {
            string str = !bSrcAllies ? "Enemy_" : "Self_";
            if (Type != KillDetailInfoType.Info_Type_Soldier_Boosted)
            {
                if ((actorType == ActorTypeDef.Actor_Type_Monster) || (actorType == ActorTypeDef.Actor_Type_Organ))
                {
                    if (Type == KillDetailInfoType.Info_Type_DestroyTower)
                    {
                        return (str + "TowerDie");
                    }
                    return "Executed";
                }
                switch (Type)
                {
                    case KillDetailInfoType.Info_Type_First_Kill:
                        return "First_Blood";

                    case KillDetailInfoType.Info_Type_Kill:
                        if (!bSrcAllies)
                        {
                            if (bSelfKillORKilled)
                            {
                                return "Self_OneDie";
                            }
                            return "Self_TeamDie";
                        }
                        if (!bSelfKillORKilled)
                        {
                            return "Self_OneKill";
                        }
                        return "Self_YouKill";

                    case KillDetailInfoType.Info_Type_DoubleKill:
                        return (str + "DoubleKill");

                    case KillDetailInfoType.Info_Type_TripleKill:
                        return (str + "TripleKill");

                    case KillDetailInfoType.Info_Type_QuataryKill:
                        return (str + "QuadraKill");

                    case KillDetailInfoType.Info_Type_PentaKill:
                        return (str + "PentaKill");

                    case KillDetailInfoType.Info_Type_MonsterKill:
                        return (str + "KillingSpree1");

                    case KillDetailInfoType.Info_Type_DominateBattle:
                        return (str + "KillingSpree2");

                    case KillDetailInfoType.Info_Type_Legendary:
                        return (str + "KillingSpree3");

                    case KillDetailInfoType.Info_Type_TotalAnnihilat:
                        return (str + "KillingSpree4");

                    case KillDetailInfoType.Info_Type_Odyssey:
                        return (str + "KillingSpree5");

                    case KillDetailInfoType.Info_Type_DestroyTower:
                        return (str + "TowerDie");

                    case KillDetailInfoType.Info_Type_Game_Start_Wel:
                        return "Play_5V5_sys_1_01";

                    case KillDetailInfoType.Info_Type_Soldier_Activate_Countdown3:
                        return "Play_5V5_sys_2";

                    case KillDetailInfoType.Info_Type_Soldier_Activate_Countdown5:
                        return "Play_5V5_sys_3";

                    case KillDetailInfoType.Info_Type_Soldier_Activate:
                        return "Play_5V5_war_1";

                    case KillDetailInfoType.Info_Type_Kill_3V3_Dragon:
                        return (str + "BaoJunSkill");

                    case KillDetailInfoType.Info_Type_Kill_5V5_SmallDragon:
                        return (str + "BaoJunSkill");

                    case KillDetailInfoType.Info_Type_Kill_5V5_BigDragon:
                        return (str + "BaoJunSkill");

                    case KillDetailInfoType.Info_Type_AllDead:
                        return "Common_Ace";

                    case KillDetailInfoType.Info_Type_StopMultiKill:
                        return "ShutDown";
                }
            }
            return string.Empty;
        }

        public static void SetImageSprite(Image img, string spt)
        {
            if ((img != null) && !string.IsNullOrEmpty(spt))
            {
                img.SetSprite(spt, Singleton<CBattleSystem>.GetInstance().FormScript, true, false, false, false);
            }
        }
    }
}

