namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class SignalPanel
    {
        private const float c_latestSignalTipsMaxDuringTime = 5f;
        private Plane m_battleSceneGroundPlane;
        private CUIFormScript m_formScript;
        private float m_latestSignalTipsDuringTime;
        private GameObject m_miniMap;
        private Vector2 m_miniMapScreenPosition;
        private Dictionary<uint, CPlayerSignalCooldown> m_playerSignalCooldowns;
        private int m_selectedSignalID = -1;
        private CSignalButton[] m_signalButtons;
        private ListView<CSignal> m_signals;
        private CUIContainerScript m_signalSrcHeroNameContainer;
        private ListView<CSignalTipsElement> m_signalTipses;
        private CUIListScript m_signalTipsList;
        private CanvasGroup m_signalTipsListCanvasGroup;
        private bool m_useSignalButton;
        private static int[][] s_signalButtonInfos;
        public static string s_signalTipsFormPath;
        private const float speed = 0.15f;

        static SignalPanel()
        {
            int[][] numArrayArray1 = new int[4][];
            numArrayArray1[0] = new int[] { 1, 12 };
            numArrayArray1[1] = new int[] { 2, 13 };
            numArrayArray1[2] = new int[] { 3, 14 };
            numArrayArray1[3] = new int[] { 4, 15 };
            s_signalButtonInfos = numArrayArray1;
            s_signalTipsFormPath = "UGUI/Form/Battle/Part/Form_Battle_Part_SignalTips.prefab";
        }

        public void Add_SignalTip(CSignalTipsElement obj)
        {
            if (obj != null)
            {
                this.m_signalTipses.Add(obj);
            }
            this.RefreshSignalTipsList();
        }

        public void CancelSelectedSignalButton()
        {
            if (this.m_useSignalButton && (this.m_selectedSignalID >= 0))
            {
                CSignalButton singleButton = this.GetSingleButton(this.m_selectedSignalID);
                if (singleButton != null)
                {
                    singleButton.SetHighLight(false);
                }
                this.m_selectedSignalID = -1;
            }
        }

        public bool CheckSignalPositionValid(Vector2 sigScreenPos)
        {
            MinimapSys theMinimapSys = Singleton<CBattleSystem>.instance.TheMinimapSys;
            if (theMinimapSys == null)
            {
                return false;
            }
            return (((sigScreenPos.x >= (theMinimapSys.GetMMFianlScreenPos().x - (theMinimapSys.mmFinalScreenSize.x / 2f))) && (sigScreenPos.x <= (theMinimapSys.GetMMFianlScreenPos().x + (theMinimapSys.mmFinalScreenSize.x / 2f)))) && ((sigScreenPos.y >= (theMinimapSys.GetMMFianlScreenPos().y - (theMinimapSys.mmFinalScreenSize.y / 2f))) && (sigScreenPos.y <= (theMinimapSys.GetMMFianlScreenPos().y + (theMinimapSys.mmFinalScreenSize.y / 2f)))));
        }

        public void Clear()
        {
            if (this.m_useSignalButton)
            {
                Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_ClickMiniMap, new CUIEventManager.OnUIEventHandler(this.OnClickMiniMap));
                Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Click_Scene, new CUIEventManager.OnUIEventHandler(this.OnClickBattleScene));
                Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnSignalButtonClicked, new CUIEventManager.OnUIEventHandler(this.OnSignalButtonClicked));
                Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnSignalTipsListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnSignalListElementEnabled));
                this.m_signals = null;
                this.m_playerSignalCooldowns = null;
                this.m_signalTipses = null;
            }
            this.m_signalButtons = null;
            this.m_formScript = null;
            this.m_miniMap = null;
            this.m_signalSrcHeroNameContainer = null;
            this.m_signalTipsList = null;
            Singleton<CUIManager>.GetInstance().CloseForm(s_signalTipsFormPath);
        }

        private void ClearSignalTipses()
        {
            if ((this.m_signalTipses != null) && (this.m_signalTipses.Count > 0))
            {
                this.m_signalTipses.Clear();
                if (this.m_signalTipsList != null)
                {
                    this.m_signalTipsList.SetElementAmount(0);
                    this.m_signalTipsList.ResetContentPosition();
                }
            }
        }

        public void ExecCommand(PoolObjHandle<ActorRoot> followActor, uint senderPlayerID, uint heroID, int signalID, byte bAlice = 0, byte elementType = 0, uint targetObjID = 0, uint targetHeroID = 0)
        {
            if (this.m_useSignalButton && (this.m_formScript != null))
            {
                Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                Player player = Singleton<GamePlayerCenter>.GetInstance().GetPlayer(senderPlayerID);
                if (((hostPlayer != null) && (player != null)) && (hostPlayer.PlayerCamp == player.PlayerCamp))
                {
                    bool isHostPlayer = hostPlayer == player;
                    ResSignalInfo dataByKey = GameDataMgr.signalDatabin.GetDataByKey((long) signalID);
                    if (dataByKey == null)
                    {
                        DebugHelper.Assert(dataByKey != null, "ExecCommand signalInfo is null, check out...");
                    }
                    else
                    {
                        uint cDTime = this.GetCDTime(dataByKey);
                        ulong logicFrameTick = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
                        CPlayerSignalCooldown cooldown = null;
                        this.m_playerSignalCooldowns.TryGetValue(senderPlayerID, out cooldown);
                        if (cooldown != null)
                        {
                            if (((uint) (logicFrameTick - cooldown.m_lastSignalExecuteTimestamp)) < cooldown.m_cooldownTime)
                            {
                                return;
                            }
                            cooldown.m_lastSignalExecuteTimestamp = logicFrameTick;
                            cooldown.m_cooldownTime = cDTime;
                        }
                        else
                        {
                            cooldown = new CPlayerSignalCooldown(logicFrameTick, cDTime);
                            this.m_playerSignalCooldowns.Add(senderPlayerID, cooldown);
                        }
                        if (isHostPlayer && (this.m_signalButtons != null))
                        {
                            for (int i = 0; i < this.m_signalButtons.Length; i++)
                            {
                                if (this.m_signalButtons[i] != null)
                                {
                                    this.m_signalButtons[i].StartCooldown(cDTime);
                                }
                            }
                        }
                        bool bSmall = Singleton<CBattleSystem>.instance.TheMinimapSys.CurMapType() == MinimapSys.EMapType.Mini;
                        this.PlaySignalTipsSound(elementType, bAlice, targetHeroID);
                        bool bUseCfgSound = elementType == 0;
                        if ((followActor != 0) && followActor.handle.Visible)
                        {
                            CSignal item = new CSignal(followActor, signalID, true, bSmall, bUseCfgSound);
                            item.Initialize(this.m_formScript, dataByKey);
                            this.m_signals.Add(item);
                        }
                        CSignalTips tips = new CSignalTips(signalID, heroID, isHostPlayer, bAlice, elementType, targetHeroID);
                        this.Add_SignalTip(tips);
                    }
                }
            }
        }

        public void ExecCommand(uint senderPlayerID, uint heroID, int signalID, int worldPositionX, int worldPositionY, int worldPositionZ, byte bAlice = 0, byte elementType = 0, uint targetHeroID = 0)
        {
            if (this.m_useSignalButton && (this.m_formScript != null))
            {
                Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                Player player = Singleton<GamePlayerCenter>.GetInstance().GetPlayer(senderPlayerID);
                if (((hostPlayer != null) && (player != null)) && (hostPlayer.PlayerCamp == player.PlayerCamp))
                {
                    bool isHostPlayer = hostPlayer == player;
                    ResSignalInfo dataByKey = GameDataMgr.signalDatabin.GetDataByKey((long) signalID);
                    if (dataByKey == null)
                    {
                        DebugHelper.Assert(dataByKey != null, "ExecCommand signalInfo is null, check out...");
                    }
                    else
                    {
                        uint cDTime = this.GetCDTime(dataByKey);
                        ulong logicFrameTick = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
                        CPlayerSignalCooldown cooldown = null;
                        this.m_playerSignalCooldowns.TryGetValue(senderPlayerID, out cooldown);
                        if (cooldown != null)
                        {
                            if (((uint) (logicFrameTick - cooldown.m_lastSignalExecuteTimestamp)) < cooldown.m_cooldownTime)
                            {
                                return;
                            }
                            cooldown.m_lastSignalExecuteTimestamp = logicFrameTick;
                            cooldown.m_cooldownTime = cDTime;
                        }
                        else
                        {
                            cooldown = new CPlayerSignalCooldown(logicFrameTick, cDTime);
                            this.m_playerSignalCooldowns.Add(senderPlayerID, cooldown);
                        }
                        if (isHostPlayer && (this.m_signalButtons != null))
                        {
                            for (int i = 0; i < this.m_signalButtons.Length; i++)
                            {
                                if (this.m_signalButtons[i] != null)
                                {
                                    this.m_signalButtons[i].StartCooldown(cDTime);
                                }
                            }
                        }
                        bool bSmall = Singleton<CBattleSystem>.instance.TheMinimapSys.CurMapType() == MinimapSys.EMapType.Mini;
                        this.PlaySignalTipsSound(elementType, bAlice, targetHeroID);
                        bool bUseCfgSound = elementType == 0;
                        senderPlayerID = (elementType == 0) ? senderPlayerID : 0;
                        CSignal item = new CSignal(senderPlayerID, signalID, worldPositionX, worldPositionY, worldPositionZ, true, bSmall, bUseCfgSound);
                        item.Initialize(this.m_formScript, dataByKey);
                        this.m_signals.Add(item);
                        CSignalTips tips = new CSignalTips(signalID, heroID, isHostPlayer, bAlice, elementType, targetHeroID);
                        this.Add_SignalTip(tips);
                    }
                }
            }
        }

        public void ExecCommand_SignalBtn_Position(uint senderPlayerID, byte signalID, ref VInt3 worldPos)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if (((hostPlayer != null) && (hostPlayer.Captain != 0)) && (hostPlayer.Captain.handle != null))
            {
                int configId = Singleton<GamePlayerCenter>.instance.GetPlayer(senderPlayerID).Captain.handle.TheActorMeta.ConfigId;
                this.ExecCommand(senderPlayerID, (uint) configId, signalID, worldPos.x, worldPos.y, worldPos.z, 0, 0, 0);
            }
        }

        public void ExecCommand_SignalMiniMap_Position(uint senderPlayerID, byte signalID, ref VInt3 worldPos, byte elementType)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if (((hostPlayer != null) && (hostPlayer.Captain != 0)) && (hostPlayer.Captain.handle != null))
            {
                Player player = Singleton<GamePlayerCenter>.instance.GetPlayer(senderPlayerID);
                bool flag = hostPlayer.PlayerCamp == player.PlayerCamp;
                int configId = player.Captain.handle.TheActorMeta.ConfigId;
                this.ExecCommand(senderPlayerID, (uint) configId, signalID, worldPos.x, worldPos.y, worldPos.z, !flag ? ((byte) 0) : ((byte) 1), elementType, 0);
            }
        }

        public void ExecCommand_SignalMiniMap_Target(uint senderPlayerID, byte signalID, byte type, uint targetObjID)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if (((hostPlayer != null) && (hostPlayer.Captain != 0)) && (hostPlayer.Captain.handle != null))
            {
                PoolObjHandle<ActorRoot> actor = this.GetActor(type, targetObjID);
                if (((actor != 0) && (actor.handle.ObjID != 0)) && actor.handle.HorizonMarker.IsVisibleFor(hostPlayer.PlayerCamp))
                {
                    Player player = Singleton<GamePlayerCenter>.instance.GetPlayer(senderPlayerID);
                    bool flag = hostPlayer.PlayerCamp == actor.handle.TheActorMeta.ActorCamp;
                    int configId = actor.handle.TheActorMeta.ConfigId;
                    int num2 = player.Captain.handle.TheActorMeta.ConfigId;
                    this.ExecCommand(actor, senderPlayerID, (uint) num2, signalID, !flag ? ((byte) 0) : ((byte) 1), type, 0, (uint) configId);
                }
            }
        }

        private PoolObjHandle<ActorRoot> GetActor(byte type, uint targetObjID)
        {
            List<PoolObjHandle<ActorRoot>> gameActors = null;
            if (((type == 6) || (type == 4)) || (type == 5))
            {
                gameActors = Singleton<GameObjMgr>.instance.GameActors;
            }
            else if (type == 3)
            {
                gameActors = Singleton<GameObjMgr>.instance.HeroActors;
            }
            else if (type == 1)
            {
                gameActors = Singleton<GameObjMgr>.instance.TowerActors;
            }
            else if (type == 2)
            {
                gameActors = Singleton<GameObjMgr>.instance.OrganActors;
            }
            for (int i = 0; i < gameActors.Count; i++)
            {
                PoolObjHandle<ActorRoot> handle = gameActors[i];
                if ((handle != 0) && (handle.handle.ObjID == targetObjID))
                {
                    return handle;
                }
            }
            return new PoolObjHandle<ActorRoot>();
        }

        private uint GetCDTime(ResSignalInfo signalInfo)
        {
            uint num = (uint) (signalInfo.bCooldownTime * 0x3e8);
            if (CBattleGuideManager.Is5v5GuideLevel(Singleton<BattleLogic>.GetInstance().GetCurLvelContext().m_mapID))
            {
                num = 0x7d0;
            }
            return num;
        }

        public static CUIListScript GetSignalTipsListScript()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_signalTipsFormPath);
            if (form == null)
            {
                form = Singleton<CUIManager>.GetInstance().OpenForm(s_signalTipsFormPath, true, true);
            }
            return Utility.GetComponetInChild<CUIListScript>(form.get_gameObject(), "List_SignalTips");
        }

        public CSignalButton GetSingleButton(int signalID)
        {
            if (this.m_signalButtons != null)
            {
                for (int i = 0; i < this.m_signalButtons.Length; i++)
                {
                    if ((this.m_signalButtons[i] != null) && (this.m_signalButtons[i].m_signalID == signalID))
                    {
                        return this.m_signalButtons[i];
                    }
                }
            }
            return null;
        }

        public void Init(CUIFormScript formScript, GameObject minimapGameObject, GameObject signalSrcHeroNameContainer, bool useSignalButton)
        {
            if (formScript != null)
            {
                this.m_formScript = formScript;
                this.m_miniMap = minimapGameObject;
                if (this.m_miniMap != null)
                {
                    this.m_miniMapScreenPosition = CUIUtility.WorldToScreenPoint(formScript.GetCamera(), this.m_miniMap.get_transform().get_position());
                }
                this.m_signalSrcHeroNameContainer = (signalSrcHeroNameContainer != null) ? signalSrcHeroNameContainer.GetComponent<CUIContainerScript>() : null;
                this.m_signalTipsList = GetSignalTipsListScript();
                if (this.m_signalTipsList != null)
                {
                    this.m_signalTipsListCanvasGroup = this.m_signalTipsList.get_gameObject().GetComponent<CanvasGroup>();
                    if (this.m_signalTipsListCanvasGroup == null)
                    {
                        this.m_signalTipsListCanvasGroup = this.m_signalTipsList.get_gameObject().AddComponent<CanvasGroup>();
                    }
                    this.m_signalTipsListCanvasGroup.set_alpha(0f);
                    this.m_signalTipsListCanvasGroup.set_blocksRaycasts(false);
                }
                this.m_useSignalButton = useSignalButton;
                this.m_signalButtons = new CSignalButton[s_signalButtonInfos.Length];
                for (int i = 0; i < this.m_signalButtons.Length; i++)
                {
                    this.m_signalButtons[i] = new CSignalButton(this.m_formScript.GetWidget(s_signalButtonInfos[i][1]), s_signalButtonInfos[i][0]);
                    this.m_signalButtons[i].Initialize(this.m_formScript);
                    if (!useSignalButton)
                    {
                        this.m_signalButtons[i].Disable();
                    }
                }
                if (this.m_useSignalButton)
                {
                    Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_ClickMiniMap, new CUIEventManager.OnUIEventHandler(this.OnClickMiniMap));
                    Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Click_Scene, new CUIEventManager.OnUIEventHandler(this.OnClickBattleScene));
                    Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnSignalButtonClicked, new CUIEventManager.OnUIEventHandler(this.OnSignalButtonClicked));
                    Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnSignalTipsListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnSignalListElementEnabled));
                    this.m_battleSceneGroundPlane = new Plane(new Vector3(0f, 1f, 0f), 0.15f);
                    this.m_signals = new ListView<CSignal>();
                    this.m_playerSignalCooldowns = new Dictionary<uint, CPlayerSignalCooldown>();
                    this.m_signalTipses = new ListView<CSignalTipsElement>();
                }
            }
        }

        public bool IsUseSingalButton()
        {
            if (this.m_miniMap == null)
            {
                return false;
            }
            return (this.m_useSignalButton && (this.m_selectedSignalID >= 0));
        }

        private void OnClickBattleScene(CUIEvent uievent)
        {
            if (!this.m_useSignalButton || (this.m_selectedSignalID < 0))
            {
                Singleton<CBattleSystem>.instance.TheMinimapSys.Switch(MinimapSys.EMapType.Mini);
                Singleton<InBattleMsgMgr>.instance.HideView();
            }
            else
            {
                float num;
                Ray ray = Camera.get_main().ScreenPointToRay(uievent.m_pointerEventData.get_position());
                if (this.m_battleSceneGroundPlane.Raycast(ray, ref num))
                {
                    VInt3 point = ray.GetPoint(num);
                    this.SendCommand_SignalBtn_Position((byte) this.m_selectedSignalID, point);
                }
            }
        }

        public void OnClickMiniMap(CUIEvent uiEvent)
        {
            if ((this.m_useSignalButton && (this.m_selectedSignalID >= 0)) && (this.m_miniMap != null))
            {
                VInt num;
                Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                ActorRoot root = (hostPlayer != null) ? hostPlayer.Captain.handle : null;
                this.m_miniMapScreenPosition = CUIUtility.WorldToScreenPoint(uiEvent.m_srcFormScript.GetCamera(), this.m_miniMap.get_transform().get_position());
                Vector3 vector = Vector3.get_zero();
                vector.x = (uiEvent.m_pointerEventData.get_position().x - this.m_miniMapScreenPosition.x) * Singleton<CBattleSystem>.GetInstance().UI_world_Factor_Small.x;
                vector.y = (root == null) ? 0.15f : ((Vector3) root.location).y;
                vector.z = (uiEvent.m_pointerEventData.get_position().y - this.m_miniMapScreenPosition.y) * Singleton<CBattleSystem>.GetInstance().UI_world_Factor_Small.y;
                PathfindingUtility.GetGroundY((VInt3) vector, out num);
                vector.y = num.scalar;
                this.SendCommand_SignalMiniMap_Position((byte) this.m_selectedSignalID, (VInt3) vector, MinimapSys.ElementType.None);
            }
        }

        private void OnSignalButtonClicked(CUIEvent uiEvent)
        {
            if (!Singleton<CBattleGuideManager>.instance.bPauseGame && this.m_useSignalButton)
            {
                int tag = uiEvent.m_eventParams.tag;
                switch (tag)
                {
                    case 2:
                        CPlayerBehaviorStat.Plus(CPlayerBehaviorStat.BehaviorType.Battle_Signal_2);
                        break;

                    case 3:
                        CPlayerBehaviorStat.Plus(CPlayerBehaviorStat.BehaviorType.Battle_Signal_3);
                        break;

                    case 4:
                        CPlayerBehaviorStat.Plus(CPlayerBehaviorStat.BehaviorType.Battle_Signal_4);
                        break;
                }
                CSignalButton singleButton = this.GetSingleButton(tag);
                if ((singleButton != null) && !singleButton.IsInCooldown())
                {
                    if (singleButton.m_signalInfo.bSignalType == 0)
                    {
                        if (this.m_selectedSignalID != tag)
                        {
                            if (this.m_selectedSignalID >= 0)
                            {
                                CSignalButton button2 = this.GetSingleButton(this.m_selectedSignalID);
                                if (button2 != null)
                                {
                                    button2.SetHighLight(false);
                                }
                            }
                            this.m_selectedSignalID = tag;
                            singleButton.SetHighLight(true);
                        }
                    }
                    else
                    {
                        this.SendCommand_SignalBtn_Position((byte) tag, VInt3.zero);
                    }
                }
            }
        }

        private void OnSignalListElementEnabled(CUIEvent uiEvent)
        {
            CUIListElementScript srcWidgetScript = uiEvent.m_srcWidgetScript;
            int index = srcWidgetScript.m_index;
            if ((index >= 0) || (index < this.m_signalTipses.Count))
            {
                CSignalTipShower component = srcWidgetScript.GetComponent<CSignalTipShower>();
                if (component != null)
                {
                    component.Set(this.m_signalTipses[index], uiEvent.m_srcFormScript);
                }
            }
        }

        private void PlaySignalTipsSound(byte elementType, byte bAlice, uint targetHeroID)
        {
            string str = string.Empty;
            switch (elementType)
            {
                case 1:
                    str = (bAlice != 1) ? "Play_notice_map_1" : "Play_notice_map_2";
                    break;

                case 2:
                    str = (bAlice != 1) ? "Play_sys_bobao_jihe_6" : "Play_sys_bobao_jihe_7";
                    break;

                case 3:
                {
                    ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(targetHeroID);
                    if ((dataByKey != null) && !string.IsNullOrEmpty(dataByKey.szHeroSound))
                    {
                        Singleton<CSoundManager>.GetInstance().PlayBattleSound2D(dataByKey.szHeroSound);
                    }
                    str = (bAlice != 1) ? "Play_Call_Attack" : "Play_Call_Guard";
                    break;
                }
                case 4:
                    str = "Play_sys_bobao_jihe_3";
                    break;

                case 5:
                    str = "Play_sys_bobao_jihe_4";
                    break;

                case 6:
                    str = "Play_sys_bobao_jihe_4";
                    break;
            }
            if (!string.IsNullOrEmpty(str))
            {
                Singleton<CSoundManager>.GetInstance().PlayBattleSound2D(str);
            }
        }

        private void RefreshSignalTipsList()
        {
            this.m_latestSignalTipsDuringTime = 5f;
            if (this.m_signalTipsList != null)
            {
                this.m_signalTipsList.SetElementAmount(this.m_signalTipses.Count);
                this.m_signalTipsList.MoveElementInScrollArea(this.m_signalTipses.Count - 1, false);
            }
        }

        public void SendCommand_SignalBtn_Position(byte signalID, VInt3 worldPos)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if (((hostPlayer != null) && (hostPlayer.Captain != 0)) && (hostPlayer.Captain.handle != null))
            {
                FrameCommand<SignalBtnPosition> command = FrameCommandFactory.CreateFrameCommand<SignalBtnPosition>();
                command.cmdData.m_signalID = signalID;
                command.cmdData.m_worldPos = worldPos;
                command.Send();
            }
        }

        public void SendCommand_SignalMiniMap_Position(byte signalID, VInt3 worldPos, MinimapSys.ElementType elementType)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if (((hostPlayer != null) && (hostPlayer.Captain != 0)) && (hostPlayer.Captain.handle != null))
            {
                FrameCommand<SignalMiniMapPosition> command = FrameCommandFactory.CreateFrameCommand<SignalMiniMapPosition>();
                command.cmdData.m_signalID = signalID;
                command.cmdData.m_worldPos = worldPos;
                command.cmdData.m_elementType = (byte) elementType;
                command.Send();
            }
        }

        public void SendCommand_SignalMiniMap_Target(byte signalID, byte type, uint targetObjID)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if (((hostPlayer != null) && (hostPlayer.Captain != 0)) && (hostPlayer.Captain.handle != null))
            {
                FrameCommand<SignalMiniMapTarget> command = FrameCommandFactory.CreateFrameCommand<SignalMiniMapTarget>();
                command.cmdData.m_signalID = signalID;
                command.cmdData.m_type = type;
                command.cmdData.m_targetObjID = targetObjID;
                command.Send();
            }
        }

        public void Update()
        {
            if (this.m_useSignalButton)
            {
                if (this.m_signalButtons != null)
                {
                    for (int i = 0; i < this.m_signalButtons.Length; i++)
                    {
                        if (this.m_signalButtons[i] != null)
                        {
                            this.m_signalButtons[i].UpdateCooldown();
                        }
                    }
                }
                if (this.m_signals != null)
                {
                    int index = 0;
                    while (index < this.m_signals.Count)
                    {
                        if (this.m_signals[index].IsNeedDisposed())
                        {
                            this.m_signals[index].Dispose();
                            this.m_signals.RemoveAt(index);
                        }
                        else
                        {
                            this.m_signals[index].Update(this.m_formScript, Time.get_deltaTime());
                            index++;
                        }
                    }
                }
                this.UpdateSignalTipses();
            }
        }

        private void UpdateSignalTipses()
        {
            if (this.m_signalTipsListCanvasGroup != null)
            {
                if (this.m_latestSignalTipsDuringTime > 0f)
                {
                    if (this.m_signalTipsListCanvasGroup.get_alpha() < 1f)
                    {
                        this.m_signalTipsListCanvasGroup.set_alpha(this.m_signalTipsListCanvasGroup.get_alpha() + 0.15f);
                        if (this.m_signalTipsListCanvasGroup.get_alpha() > 1f)
                        {
                            this.m_signalTipsListCanvasGroup.set_alpha(1f);
                        }
                    }
                }
                else if (this.m_signalTipsListCanvasGroup.get_alpha() > 0f)
                {
                    this.m_signalTipsListCanvasGroup.set_alpha(this.m_signalTipsListCanvasGroup.get_alpha() - 0.15f);
                    if (this.m_signalTipsListCanvasGroup.get_alpha() < 0f)
                    {
                        this.m_signalTipsListCanvasGroup.set_alpha(0f);
                    }
                }
                else
                {
                    this.ClearSignalTipses();
                }
            }
            if (((this.m_latestSignalTipsDuringTime > 0f) && (this.m_signalTipsList != null)) && this.m_signalTipsList.IsElementInScrollArea(this.m_signalTipsList.GetElementAmount() - 1))
            {
                this.m_latestSignalTipsDuringTime -= Time.get_deltaTime();
                if (this.m_latestSignalTipsDuringTime < 0f)
                {
                    this.m_latestSignalTipsDuringTime = 0f;
                }
            }
        }

        private class CPlayerSignalCooldown
        {
            public uint m_cooldownTime;
            public ulong m_lastSignalExecuteTimestamp;

            public CPlayerSignalCooldown(ulong lastSignalExecuteTimestamp, uint cooldownTime)
            {
                this.m_lastSignalExecuteTimestamp = lastSignalExecuteTimestamp;
                this.m_cooldownTime = cooldownTime;
            }
        }
    }
}

