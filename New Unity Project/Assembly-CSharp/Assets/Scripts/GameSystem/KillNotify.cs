namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class KillNotify
    {
        private CUIAnimatorScript animatorScript;
        private Image[] assistHeadFrames = new Image[4];
        private Image[] assistHeads = new Image[4];
        private GameObject assistList;
        public static string blue_assist_frame_icon = "UGUI/Sprite/Battle/LockEnemy/Battle_KillNotify_Blue_ring";
        public static string blue_cannon_icon = "UGUI/Sprite/Battle/kn_Blue_Paoche";
        public static string blue_soldier_icon = "UGUI/Sprite/Battle/kn_Blue_Soldier";
        public static string building_icon = "UGUI/Sprite/Battle/kn_Tower";
        public static string dragon_icon = "UGUI/Sprite/Battle/kn_dragon";
        public static int HideTime = 0xdac;
        private int hideTimer;
        private bool IsPlaying;
        private GameObject killerHead;
        private Image KillerImg;
        private List<KillInfo> KillInfoList = new List<KillInfo>();
        public static Dictionary<KillDetailInfoType, byte> knPriority = new Dictionary<KillDetailInfoType, byte>();
        private CUIFormScript m_formScript;
        public static int max_count = 5;
        public static string monster_icon = "UGUI/Sprite/Battle/kn_Monster";
        private GameObject node;
        private int play_delta_timer;
        public static string red_assist_frame_icon = "UGUI/Sprite/Battle/LockEnemy/Battle_KillNotify_Red_ring";
        public static string red_cannon_icon = "UGUI/Sprite/Battle/kn_Red_Paoche";
        public static string red_soldier_icon = "UGUI/Sprite/Battle/kn_Red_Soldier";
        public static string s_killNotifyFormPath = "UGUI/Form/Battle/Part/Form_Battle_Part_KillNotify.prefab";
        public static int s_play_deltaTime = 200;
        private FireHoleKillNotify sub_sys;
        private GameObject VictimHead;
        private Image VictimImg;
        public static string yeguai_icon = "UGUI/Sprite/Battle/kn_yeguai";

        private void _AddkillInfo(KillInfo killInfo)
        {
            if (this.IsDragonKilled(killInfo.MsgType))
            {
                this.KillInfoList.Insert(0, killInfo);
            }
            else if (killInfo.MsgType == KillDetailInfoType.Info_Type_AllDead)
            {
                this.KillInfoList.Add(killInfo);
                Debug.Log("---KN 添加团灭进队列");
            }
            else
            {
                if (this.KillInfoList.Count < max_count)
                {
                    this.KillInfoList.Add(killInfo);
                }
                else
                {
                    int bPriority = 0x2710;
                    int index = 0;
                    for (int j = 0; j < this.KillInfoList.Count; j++)
                    {
                        KillInfo info2 = this.KillInfoList[j];
                        if (info2.bPriority < bPriority)
                        {
                            KillInfo info3 = this.KillInfoList[j];
                            bPriority = info3.bPriority;
                            index = j;
                        }
                    }
                    if (((killInfo.bPriority >= bPriority) && (index >= 0)) && (index < this.KillInfoList.Count))
                    {
                        this.KillInfoList.RemoveAt(index);
                        this.KillInfoList.Add(killInfo);
                    }
                }
                int num4 = -1;
                for (int i = 0; i < this.KillInfoList.Count; i++)
                {
                    KillInfo info4 = this.KillInfoList[i];
                    if (info4.MsgType == KillDetailInfoType.Info_Type_AllDead)
                    {
                        num4 = i;
                    }
                }
                if ((num4 >= 0) && (num4 < this.KillInfoList.Count))
                {
                    KillInfo info = this.KillInfoList[num4];
                    this.KillInfoList[num4] = this.KillInfoList[this.KillInfoList.Count - 1];
                    this.KillInfoList[this.KillInfoList.Count - 1] = info;
                }
            }
        }

        public void AddKillInfo(KillDetailInfo info)
        {
            if (!MonoSingleton<Reconnection>.instance.isProcessingRelayRecover)
            {
                KillInfo killInfo = KillNotifyUT.Convert_DetailInfo_KillInfo(info);
                this.AddKillInfo(ref killInfo);
            }
        }

        public void AddKillInfo(ref KillInfo killInfo)
        {
            if (this.IsPlaying)
            {
                this._AddkillInfo(killInfo);
            }
            else
            {
                this.PlayKillNotify(ref killInfo);
            }
        }

        public void Clear()
        {
            Singleton<EventRouter>.instance.RemoveEventHandler<KillDetailInfo>(EventID.AchievementRecorderEvent, new Action<KillDetailInfo>(this.OnAchievementEvent));
            this.KillInfoList.Clear();
            this.animatorScript = null;
            this.killerHead = (GameObject) (this.VictimHead = null);
            this.KillerImg = (Image) (this.VictimImg = null);
            this.assistHeads = null;
            this.assistHeadFrames = null;
            Singleton<CTimerManager>.GetInstance().RemoveTimer(this.hideTimer);
            Singleton<CTimerManager>.GetInstance().RemoveTimer(this.play_delta_timer);
            this.IsPlaying = false;
            this.node = null;
            if (this.sub_sys != null)
            {
                this.sub_sys.Clear();
                this.sub_sys = null;
            }
            this.m_formScript = null;
            Singleton<CUIManager>.instance.CloseForm(s_killNotifyFormPath);
        }

        public void ClearKillNotifyList()
        {
            if (this.KillInfoList != null)
            {
                this.KillInfoList.Clear();
            }
        }

        public static CUIFormScript GetKillNotifyFormScript()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_killNotifyFormPath);
            if (form == null)
            {
                form = Singleton<CUIManager>.instance.OpenForm(s_killNotifyFormPath, true, true);
            }
            return form;
        }

        public static byte GetPriority(KillDetailInfoType type)
        {
            byte num;
            knPriority.TryGetValue(type, out num);
            return num;
        }

        public void Hide()
        {
            if (this.node != null)
            {
                this.m_formScript.Hide(enFormHideFlag.HideByCustom, true);
            }
            if (this.animatorScript != null)
            {
                this.animatorScript.SetAnimatorEnable(false);
            }
        }

        public void Init()
        {
            this.IsPlaying = false;
            Singleton<EventRouter>.instance.AddEventHandler<KillDetailInfo>(EventID.AchievementRecorderEvent, new Action<KillDetailInfo>(this.OnAchievementEvent));
            this.m_formScript = GetKillNotifyFormScript();
            this.node = Utility.FindChild(this.m_formScript.get_gameObject(), "KillNotify_New");
            this.animatorScript = Utility.GetComponetInChild<CUIAnimatorScript>(this.node, "KillNotify_Sub");
            this.KillerImg = Utility.GetComponetInChild<Image>(this.node, "KillNotify_Sub/KillerHead/KillerImg");
            this.VictimImg = Utility.GetComponetInChild<Image>(this.node, "KillNotify_Sub/VictimHead/VictimImg");
            this.killerHead = Utility.FindChild(this.node, "KillNotify_Sub/KillerHead");
            this.VictimHead = Utility.FindChild(this.node, "KillNotify_Sub/VictimHead");
            this.assistList = Utility.FindChild(this.node, "KillNotify_Sub/AssistHeadList");
            this.assistHeads[0] = Utility.GetComponetInChild<Image>(this.node, "KillNotify_Sub/AssistHeadList/Pl_Assist/Pl_Head/Head_1");
            this.assistHeads[1] = Utility.GetComponetInChild<Image>(this.node, "KillNotify_Sub/AssistHeadList/Pl_Assist/Pl_Head/Head_2");
            this.assistHeads[2] = Utility.GetComponetInChild<Image>(this.node, "KillNotify_Sub/AssistHeadList/Pl_Assist/Pl_Head/Head_3");
            this.assistHeads[3] = Utility.GetComponetInChild<Image>(this.node, "KillNotify_Sub/AssistHeadList/Pl_Assist/Pl_Head/Head_4");
            this.assistHeadFrames[0] = Utility.GetComponetInChild<Image>(this.node, "KillNotify_Sub/AssistHeadList/Pl_Assist/Pl_Head/Head_1/Frame");
            this.assistHeadFrames[1] = Utility.GetComponetInChild<Image>(this.node, "KillNotify_Sub/AssistHeadList/Pl_Assist/Pl_Head/Head_2/Frame");
            this.assistHeadFrames[2] = Utility.GetComponetInChild<Image>(this.node, "KillNotify_Sub/AssistHeadList/Pl_Assist/Pl_Head/Head_3/Frame");
            this.assistHeadFrames[3] = Utility.GetComponetInChild<Image>(this.node, "KillNotify_Sub/AssistHeadList/Pl_Assist/Pl_Head/Head_4/Frame");
            this.Hide();
            this.hideTimer = Singleton<CTimerManager>.GetInstance().AddTimer(HideTime, -1, new CTimer.OnTimeUpHandler(this.OnPlayEnd));
            Singleton<CTimerManager>.GetInstance().PauseTimer(this.hideTimer);
            this.play_delta_timer = Singleton<CTimerManager>.GetInstance().AddTimer(s_play_deltaTime, -1, new CTimer.OnTimeUpHandler(this.On_Play_DeltaEnd));
            Singleton<CTimerManager>.GetInstance().PauseTimer(this.play_delta_timer);
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if ((curLvelContext != null) && curLvelContext.IsFireHolePlayMode())
            {
                this.sub_sys = new FireHoleKillNotify();
            }
        }

        private bool IsDragonKilled(KillDetailInfoType type)
        {
            return (((type == KillDetailInfoType.Info_Type_Kill_3V3_Dragon) || (type == KillDetailInfoType.Info_Type_Kill_5V5_SmallDragon)) || (type == KillDetailInfoType.Info_Type_Kill_5V5_BigDragon));
        }

        public static void LoadConfig()
        {
            if (!int.TryParse(Singleton<CTextManager>.instance.GetText("KN_MaxCount"), out max_count))
            {
                DebugHelper.Assert(false, "---killnotify 教练你配的 KN_MaxCount 好像不是整数哦， check out");
            }
            if (!int.TryParse(Singleton<CTextManager>.instance.GetText("KN_HideTime"), out HideTime))
            {
                DebugHelper.Assert(false, "---killnotify 教练你配的 KN_HideTime 好像不是整数哦， check out");
            }
            if (!int.TryParse(Singleton<CTextManager>.instance.GetText("KN_Play_DeltaTime"), out s_play_deltaTime))
            {
                DebugHelper.Assert(false, "---killnotify 教练你配的 KN_Play_DeltaTime 好像不是整数哦， check out");
            }
            Array values = Enum.GetValues(typeof(KillDetailInfoType));
            for (int i = 0; i < values.Length; i++)
            {
                int num2 = (int) values.GetValue(i);
                if (num2 != 0)
                {
                    ResKNPriority dataByKey = GameDataMgr.killNotifyDatabin.GetDataByKey((long) num2);
                    DebugHelper.Assert(dataByKey != null, "播报配置找不到 配置项:" + ((KillDetailInfoType) num2) + ", 教练 检查下...");
                    if (dataByKey != null)
                    {
                        if (!knPriority.ContainsKey((KillDetailInfoType) num2))
                        {
                            knPriority.Add((KillDetailInfoType) num2, dataByKey.bPriority);
                        }
                        else
                        {
                            knPriority[(KillDetailInfoType) num2] = dataByKey.bPriority;
                        }
                    }
                }
            }
        }

        private void On_Play_DeltaEnd(int timerSequence)
        {
            UT.ResetTimer(this.play_delta_timer, true);
            if (this.KillInfoList.Count > 0)
            {
                KillInfo killInfo = this.KillInfoList[0];
                this.KillInfoList.RemoveAt(0);
                this.PlayKillNotify(ref killInfo);
            }
        }

        private void OnAchievementEvent(KillDetailInfo DetailInfo)
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
            if ((curLvelContext != null) && curLvelContext.IsMobaMode())
            {
                this.AddKillInfo(DetailInfo);
            }
        }

        private void OnPlayEnd(int timerSequence)
        {
            Singleton<CTimerManager>.GetInstance().PauseTimer(this.hideTimer);
            UT.ResetTimer(this.play_delta_timer, false);
            this.Hide();
            this.IsPlaying = this.KillInfoList.Count > 0;
        }

        public void PlayAnimator(string strAnimName)
        {
            if (!string.IsNullOrEmpty(strAnimName) && (this.animatorScript != null))
            {
                this.Show();
                UT.ResetTimer(this.hideTimer, false);
                this.animatorScript.PlayAnimator(strAnimName);
                this.IsPlaying = true;
            }
        }

        private void PlayKillNotify(ref KillInfo killInfo)
        {
            string killerImgSrc = killInfo.KillerImgSrc;
            string victimImgSrc = killInfo.VictimImgSrc;
            string[] assistImgSrc = killInfo.assistImgSrc;
            KillDetailInfoType msgType = killInfo.MsgType;
            bool bSrcAllies = killInfo.bSrcAllies;
            bool bSelfKillORKilled = killInfo.bPlayerSelf_KillOrKilled;
            ActorTypeDef actorType = killInfo.actorType;
            string spt = !bSrcAllies ? red_assist_frame_icon : blue_assist_frame_icon;
            if (msgType == KillDetailInfoType.Info_Type_AllDead)
            {
                Debug.Log("---KN 播团灭");
            }
            this.Show();
            UT.ResetTimer(this.hideTimer, false);
            string str4 = KillNotifyUT.GetSoundEvent(msgType, bSrcAllies, bSelfKillORKilled, actorType);
            if (!string.IsNullOrEmpty(str4))
            {
                Singleton<CSoundManager>.GetInstance().PlayBattleSound2D(str4);
            }
            string animation = KillNotifyUT.GetAnimation(msgType, bSrcAllies);
            if (!string.IsNullOrEmpty(animation) && (this.animatorScript != null))
            {
                this.animatorScript.PlayAnimator(animation);
            }
            KillNotifyUT.SetImageSprite(this.KillerImg, killerImgSrc);
            if (string.IsNullOrEmpty(killerImgSrc))
            {
                this.SetKillerShow(false);
            }
            else
            {
                this.SetKillerShow(true);
            }
            bool flag3 = (((((msgType == KillDetailInfoType.Info_Type_DestroyTower) || (msgType == KillDetailInfoType.Info_Type_DestroyBase)) || ((msgType == KillDetailInfoType.Info_Type_AllDead) || (msgType == KillDetailInfoType.Info_Type_RunningMan))) || (((msgType == KillDetailInfoType.Info_Type_Reconnect) || (msgType == KillDetailInfoType.Info_Type_Disconnect)) || ((msgType == KillDetailInfoType.Info_Type_Kill_3V3_Dragon) || (msgType == KillDetailInfoType.Info_Type_Game_Start_Wel)))) || (((msgType == KillDetailInfoType.Info_Type_Soldier_Activate_Countdown3) || (msgType == KillDetailInfoType.Info_Type_Soldier_Activate_Countdown5)) || ((msgType == KillDetailInfoType.Info_Type_Soldier_Activate) || (msgType == KillDetailInfoType.Info_Type_Kill_5V5_SmallDragon)))) || (msgType == KillDetailInfoType.Info_Type_Kill_5V5_BigDragon);
            this.SetVictimShow(!flag3);
            KillNotifyUT.SetImageSprite(this.VictimImg, victimImgSrc);
            int num = 0;
            if (assistImgSrc != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (!string.IsNullOrEmpty(assistImgSrc[i]))
                    {
                        if (this.assistHeads[i].get_gameObject() != null)
                        {
                            this.assistHeads[i].get_gameObject().CustomSetActive(true);
                        }
                        KillNotifyUT.SetImageSprite(this.assistHeads[i], assistImgSrc[i]);
                        KillNotifyUT.SetImageSprite(this.assistHeadFrames[i], spt);
                        num++;
                    }
                    else if (this.assistHeads[i].get_gameObject() != null)
                    {
                        this.assistHeads[i].get_gameObject().CustomSetActive(false);
                    }
                }
            }
            this.assistList.CustomSetActive(num > 0);
            this.IsPlaying = true;
        }

        public static void Preload(ref ActorPreloadTab preloadTab)
        {
            preloadTab.AddSprite(building_icon);
            preloadTab.AddSprite(monster_icon);
            preloadTab.AddSprite(dragon_icon);
            preloadTab.AddSprite(yeguai_icon);
            preloadTab.AddSprite(blue_cannon_icon);
            preloadTab.AddSprite(red_cannon_icon);
            preloadTab.AddSprite(blue_soldier_icon);
            preloadTab.AddSprite(red_soldier_icon);
        }

        private void SetKillerShow(bool bShow)
        {
            if (this.killerHead != null)
            {
                this.killerHead.get_gameObject().CustomSetActive(bShow);
            }
        }

        private void SetVictimShow(bool bShow)
        {
            if (this.VictimHead != null)
            {
                this.VictimHead.get_gameObject().CustomSetActive(bShow);
            }
        }

        public void Show()
        {
            if (this.node != null)
            {
                this.m_formScript.Appear(enFormHideFlag.HideByCustom, false);
            }
            if (this.animatorScript != null)
            {
                this.animatorScript.SetAnimatorEnable(true);
            }
        }
    }
}

