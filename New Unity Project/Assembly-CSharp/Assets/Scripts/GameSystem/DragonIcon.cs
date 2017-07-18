namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.DataCenter;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class DragonIcon
    {
        public const string Dragon_born = "Dragon_born";
        public const string Dragon_dead = "Dragon_dead";
        private static string dragonBornEffect = "Prefab_Skill_Effects/tongyong_effects/Indicator/blin_01_c.prefab";
        private bool m_b5v5;
        private int m_cdTimer;
        private ListView<DragonNode> node_ary = new ListView<DragonNode>();

        public static void Check_Dragon_Born_Evt(ActorRoot actor, bool bThrow_Born_Evt)
        {
            if (actor != null)
            {
                switch (actor.ActorControl.GetActorSubSoliderType())
                {
                    case 8:
                    case 9:
                    case 7:
                    case 13:
                        if (bThrow_Born_Evt)
                        {
                            Singleton<EventRouter>.GetInstance().BroadCastEvent<ActorRoot>("Dragon_born", actor);
                        }
                        else
                        {
                            Singleton<EventRouter>.GetInstance().BroadCastEvent<ActorRoot>("Dragon_dead", actor);
                        }
                        break;
                }
            }
        }

        public void Clear()
        {
            Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this.m_cdTimer);
            for (int i = 0; i < this.node_ary.Count; i++)
            {
                DragonNode node = this.node_ary[i];
                if (node != null)
                {
                    node.Clear();
                }
            }
            this.node_ary.Clear();
            this.node_ary = null;
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<ActorRoot>("Dragon_born", new Action<ActorRoot>(this.onDragon_Born));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<ActorRoot>("Dragon_dead", new Action<ActorRoot>(this.onDragon_Dead));
            this.m_b5v5 = false;
        }

        private DragonNode getDragonNode(byte type = 0)
        {
            for (int i = 0; i < this.node_ary.Count; i++)
            {
                DragonNode node = this.node_ary[i];
                if ((node != null) && node.IsType(type))
                {
                    return node;
                }
            }
            return null;
        }

        private DragonNode getDragonNode(uint objid, byte type)
        {
            for (int i = 0; i < this.node_ary.Count; i++)
            {
                DragonNode node = this.node_ary[i];
                if (((node != null) && node.IsType(type)) && (node.objid == objid))
                {
                    return node;
                }
            }
            for (int j = 0; j < this.node_ary.Count; j++)
            {
                DragonNode node2 = this.node_ary[j];
                if (((node2 != null) && node2.IsType(type)) && (node2.objid == 0))
                {
                    node2.objid = objid;
                    return node2;
                }
            }
            return null;
        }

        public void Init(GameObject mmNode_ugui, GameObject bmNode_ugui, GameObject mmNode_3dui, GameObject bmNode_3dui, bool b5V5)
        {
            this.m_b5v5 = b5V5;
            Singleton<EventRouter>.GetInstance().AddEventHandler<ActorRoot>("Dragon_born", new Action<ActorRoot>(this.onDragon_Born));
            Singleton<EventRouter>.GetInstance().AddEventHandler<ActorRoot>("Dragon_dead", new Action<ActorRoot>(this.onDragon_Dead));
            for (int i = 0; i < mmNode_ugui.get_transform().get_childCount(); i++)
            {
                mmNode_ugui.get_transform().GetChild(i).get_gameObject().CustomSetActive(false);
            }
            for (int j = 0; j < bmNode_ugui.get_transform().get_childCount(); j++)
            {
                bmNode_ugui.get_transform().GetChild(j).get_gameObject().CustomSetActive(false);
            }
            for (int k = 0; k < mmNode_3dui.get_transform().get_childCount(); k++)
            {
                mmNode_3dui.get_transform().GetChild(k).get_gameObject().CustomSetActive(false);
            }
            for (int m = 0; m < bmNode_3dui.get_transform().get_childCount(); m++)
            {
                bmNode_3dui.get_transform().GetChild(m).get_gameObject().CustomSetActive(false);
            }
            this.node_ary.Add(new DragonNode(mmNode_ugui, bmNode_ugui, mmNode_3dui, bmNode_3dui, "d_3", 7, 0));
            this.node_ary.Add(new DragonNode(mmNode_ugui, bmNode_ugui, mmNode_3dui, bmNode_3dui, "d_5_big", 8, 0));
            this.node_ary.Add(new DragonNode(mmNode_ugui, bmNode_ugui, mmNode_3dui, bmNode_3dui, "d_5_small_1", 9, 13));
            this.node_ary.Add(new DragonNode(mmNode_ugui, bmNode_ugui, mmNode_3dui, bmNode_3dui, "d_5_small_2", 9, 13));
            SpawnGroup group = null;
            ListView<SpawnGroup> spawnGroups = Singleton<BattleLogic>.instance.mapLogic.GetSpawnGroups();
            if (spawnGroups != null)
            {
                for (int n = 0; n < spawnGroups.Count; n++)
                {
                    group = spawnGroups[n];
                    if ((group != null) && (group.NextGroups.Length == 0))
                    {
                        ActorMeta meta = group.TheActorsMeta[0];
                        ResMonsterCfgInfo dataCfgInfoByCurLevelDiff = MonsterDataHelper.GetDataCfgInfoByCurLevelDiff(meta.ConfigId);
                        if ((dataCfgInfoByCurLevelDiff != null) && ((((dataCfgInfoByCurLevelDiff.bSoldierType == 8) || (dataCfgInfoByCurLevelDiff.bSoldierType == 9)) || (dataCfgInfoByCurLevelDiff.bSoldierType == 7)) || (dataCfgInfoByCurLevelDiff.bSoldierType == 13)))
                        {
                            DragonNode node = this.getDragonNode(dataCfgInfoByCurLevelDiff.bSoldierType);
                            if (node != null)
                            {
                                node.spawnGroup = group;
                                node.SetData(group.get_gameObject().get_transform().get_position(), dataCfgInfoByCurLevelDiff.bSoldierType, 0, this.m_b5v5, true, true, true);
                                node.ShowDead(true);
                                MiniMapSysUT.RefreshMapPointerBig(node.bmDragonNode_3dui);
                            }
                        }
                    }
                }
            }
            if (Singleton<WatchController>.GetInstance().IsWatching)
            {
                this.m_cdTimer = Singleton<CTimerManager>.GetInstance().AddTimer(0x3e8, 0, new CTimer.OnTimeUpHandler(this.OnCDTimer));
            }
        }

        private void OnCDTimer(int seq)
        {
            if (this.node_ary != null)
            {
                for (int i = 0; i < this.node_ary.Count; i++)
                {
                    this.node_ary[i].ValidateCD();
                }
            }
        }

        private void onDragon_Born(ActorRoot actor)
        {
            DragonNode node = this.getDragonNode(actor.ObjID, actor.ActorControl.GetActorSubSoliderType());
            DebugHelper.Assert(node != null, "onDragon_Born mmDNode_ugui == null, check out...");
            if (node != null)
            {
                MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
                bool bRefreshCache = theMinimapSys.CurMapType() == MinimapSys.EMapType.Mini;
                node.SetData(actor.myTransform.get_position(), actor.ActorControl.GetActorSubSoliderType(), actor.ObjID, this.m_b5v5, true, bRefreshCache, true);
                node.ShowDead(actor.ActorControl.IsDeadState);
                switch (actor.ActorControl.GetActorSubSoliderType())
                {
                    case 8:
                    case 9:
                    case 13:
                    {
                        bool flag2 = (theMinimapSys != null) && (theMinimapSys.CurMapType() == MinimapSys.EMapType.Mini);
                        if (flag2)
                        {
                            Camera currentCamera = Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera();
                            if (currentCamera != null)
                            {
                                Vector3 sreenLoc = currentCamera.WorldToScreenPoint(!flag2 ? node.bmDragonNode_3dui.get_transform().get_position() : node.mmDragonNode_3dui.get_transform().get_position());
                                Singleton<CUIParticleSystem>.instance.AddParticle(dragonBornEffect, 3f, sreenLoc);
                            }
                        }
                        break;
                    }
                    case 8:
                        Singleton<CSoundManager>.GetInstance().PlayBattleSound2D("Play_DaLong_VO_Refresh");
                        return;

                    case 9:
                        Singleton<CSoundManager>.GetInstance().PlayBattleSound2D("Play_XiaoLong_VO_Refresh");
                        return;
                }
                Singleton<CSoundManager>.GetInstance().PlayBattleSound2D("Play_BaoJun_VO_Anger");
            }
        }

        private void onDragon_Dead(ActorRoot actor)
        {
            DragonNode node = this.getDragonNode(actor.ObjID, actor.ActorControl.GetActorSubSoliderType());
            if (node != null)
            {
                node.ShowDead(actor.ActorControl.IsDeadState);
                node.objid = 0;
                SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                if ((curLvelContext != null) && curLvelContext.IsFireHolePlayMode())
                {
                    node.Recycle();
                }
            }
        }

        public void RefreshDragNode(bool bUseCache, bool bRefreshMM = false)
        {
            ListView<SpawnGroup> spawnGroups = Singleton<BattleLogic>.instance.mapLogic.GetSpawnGroups();
            if (spawnGroups != null)
            {
                for (int i = 0; i < spawnGroups.Count; i++)
                {
                    SpawnGroup group = spawnGroups[i];
                    if ((group != null) && (group.NextGroups.Length == 0))
                    {
                        ActorMeta meta = group.TheActorsMeta[0];
                        ResMonsterCfgInfo dataCfgInfoByCurLevelDiff = MonsterDataHelper.GetDataCfgInfoByCurLevelDiff(meta.ConfigId);
                        if ((dataCfgInfoByCurLevelDiff != null) && ((((dataCfgInfoByCurLevelDiff.bSoldierType == 8) || (dataCfgInfoByCurLevelDiff.bSoldierType == 9)) || (dataCfgInfoByCurLevelDiff.bSoldierType == 7)) || (dataCfgInfoByCurLevelDiff.bSoldierType == 13)))
                        {
                            DragonNode node = this.getDragonNode(dataCfgInfoByCurLevelDiff.bSoldierType);
                            if (node != null)
                            {
                                node.spawnGroup = group;
                                node.SetData(group.get_gameObject().get_transform().get_position(), dataCfgInfoByCurLevelDiff.bSoldierType, node.objid, this.m_b5v5, bUseCache, false, bRefreshMM);
                                if (node.objid != 0)
                                {
                                    PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(node.objid);
                                    if (actor != 0)
                                    {
                                        node.ShowDead(actor.handle.ActorControl.IsDeadState);
                                    }
                                    else
                                    {
                                        node.ShowDead(true);
                                    }
                                }
                                else
                                {
                                    node.ShowDead(true);
                                }
                                MiniMapSysUT.RefreshMapPointerBig(node.bmDragonNode_3dui);
                            }
                        }
                    }
                }
            }
        }

        private class DragonNode
        {
            public GameObject bmDragonNode_3dui;
            public GameObject bmDragonNode_ugui;
            private Vector3 cachePos = Vector3.get_zero();
            public Text cdTxtInBig;
            public Text cdTxtInMini;
            public GameObject dragon_dead_icon_bigMap;
            public GameObject dragon_dead_icon_smallMap;
            public GameObject dragon_live_icon_bigMap;
            public GameObject dragon_live_icon_smallMap;
            public GameObject mmDragonNode_3dui;
            public GameObject mmDragonNode_ugui;
            public uint objid;
            public byte optType;
            public SpawnGroup spawnGroup = null;
            public byte type;

            public DragonNode(GameObject mmNode, GameObject bmNode, GameObject mmNode_3dui, GameObject bmNode_3dui, string path, byte type, byte optType = 0)
            {
                this._init(mmNode.get_transform().Find(path).get_gameObject(), bmNode.get_transform().Find(path).get_gameObject(), mmNode_3dui.get_transform().Find(path).get_gameObject(), bmNode_3dui.get_transform().Find(path).get_gameObject(), type, optType);
            }

            public void _init(GameObject mmDNode_ugui, GameObject bmDNode_ugui, GameObject mmDNode_3dui, GameObject bmDNode_3dui, byte type, byte optType)
            {
                this.mmDragonNode_ugui = mmDNode_ugui;
                this.bmDragonNode_ugui = bmDNode_ugui;
                this.type = type;
                this.optType = optType;
                this.mmDragonNode_3dui = mmDNode_3dui;
                this.dragon_live_icon_smallMap = this.mmDragonNode_3dui.get_transform().Find("live").get_gameObject();
                this.dragon_dead_icon_smallMap = this.mmDragonNode_3dui.get_transform().Find("dead").get_gameObject();
                this.cdTxtInMini = Utility.GetComponetInChild<Text>(mmDNode_ugui, "cdTxt");
                this.bmDragonNode_3dui = bmDNode_3dui;
                this.dragon_live_icon_bigMap = this.bmDragonNode_3dui.get_transform().Find("live").get_gameObject();
                this.dragon_dead_icon_bigMap = this.bmDragonNode_3dui.get_transform().Find("dead").get_gameObject();
                this.cdTxtInBig = Utility.GetComponetInChild<Text>(bmDNode_ugui, "cdTxt");
                if (type == 7)
                {
                    MiniMapSysUT.SetMapElement_EventParam(bmDNode_ugui, false, MinimapSys.ElementType.Dragon_3, 0, 0);
                }
                else if (type == 8)
                {
                    MiniMapSysUT.SetMapElement_EventParam(bmDNode_ugui, false, MinimapSys.ElementType.Dragon_5_big, 0, 0);
                }
                else if (type == 9)
                {
                    MiniMapSysUT.SetMapElement_EventParam(bmDNode_ugui, false, MinimapSys.ElementType.Dragon_5_small, 0, 0);
                }
            }

            public void Clear()
            {
                this.spawnGroup = null;
                this.mmDragonNode_ugui = (GameObject) (this.bmDragonNode_ugui = null);
                this.dragon_live_icon_smallMap = null;
                this.dragon_dead_icon_smallMap = null;
                this.mmDragonNode_3dui = null;
                this.cdTxtInMini = null;
                this.dragon_live_icon_bigMap = null;
                this.dragon_dead_icon_bigMap = null;
                this.bmDragonNode_3dui = null;
                this.cdTxtInBig = null;
                this.objid = 0;
                this.type = 0;
                this.optType = 0;
                this.cachePos = Vector3.get_zero();
            }

            public bool IsType(byte type)
            {
                return ((this.type == type) || (this.optType == type));
            }

            public void Recycle()
            {
                this.mmDragonNode_3dui.CustomSetActive(false);
                this.bmDragonNode_3dui.CustomSetActive(false);
                this.mmDragonNode_ugui.CustomSetActive(false);
                this.bmDragonNode_ugui.CustomSetActive(false);
            }

            public void SetData(Vector3 worldpos, int type, uint id, bool b5v5 = false, bool bUseCache = true, bool bRefreshCache = false, bool bRefreshMM = true)
            {
                if (b5v5)
                {
                    if (type == 8)
                    {
                        this.SetScale(false);
                    }
                    else if (type == 9)
                    {
                        this.SetScale(true);
                    }
                }
                if ((this.mmDragonNode_3dui != null) && bRefreshMM)
                {
                    if (this.mmDragonNode_ugui != null)
                    {
                        (this.mmDragonNode_ugui.get_transform() as RectTransform).set_anchoredPosition(new Vector2(worldpos.x * Singleton<CBattleSystem>.instance.world_UI_Factor_Small.x, worldpos.z * Singleton<CBattleSystem>.instance.world_UI_Factor_Small.y));
                    }
                    Vector3 vector = MiniMapSysUT.Set3DUIWorldPos_ByScreenPoint(ref worldpos, true);
                    this.mmDragonNode_3dui.get_transform().set_position(vector);
                }
                if (this.bmDragonNode_3dui != null)
                {
                    if (this.bmDragonNode_ugui != null)
                    {
                        (this.bmDragonNode_ugui.get_transform() as RectTransform).set_anchoredPosition(new Vector2(worldpos.x * Singleton<CBattleSystem>.instance.world_UI_Factor_Big.x, worldpos.z * Singleton<CBattleSystem>.instance.world_UI_Factor_Big.y));
                    }
                    if (bUseCache)
                    {
                        if ((this.cachePos == Vector3.get_zero()) || bRefreshCache)
                        {
                            this.cachePos = MiniMapSysUT.Set3DUIWorldPos_ByScreenPoint(ref worldpos, false);
                        }
                        this.bmDragonNode_3dui.get_transform().set_position(this.cachePos);
                    }
                    else
                    {
                        this.bmDragonNode_3dui.get_transform().set_position(MiniMapSysUT.Set3DUIWorldPos_ByScreenPoint(ref worldpos, false));
                    }
                }
                this.objid = id;
            }

            public void SetScale(bool bSmall = false)
            {
            }

            public void ShowDead(bool bDead)
            {
                this.mmDragonNode_3dui.CustomSetActive(true);
                this.bmDragonNode_3dui.CustomSetActive(true);
                this.mmDragonNode_ugui.CustomSetActive(true);
                this.bmDragonNode_ugui.CustomSetActive(true);
                this.dragon_live_icon_smallMap.CustomSetActive(!bDead);
                this.dragon_live_icon_bigMap.CustomSetActive(!bDead);
                this.dragon_dead_icon_smallMap.CustomSetActive(bDead);
                this.dragon_dead_icon_bigMap.CustomSetActive(bDead);
            }

            public void ValidateCD()
            {
                if (null != this.spawnGroup)
                {
                    if (null != this.cdTxtInMini)
                    {
                        if (this.spawnGroup.IsCountingDown())
                        {
                            this.cdTxtInMini.get_gameObject().CustomSetActive(true);
                            this.cdTxtInMini.set_text((this.spawnGroup.GetSpawnTimer() / 0x3e8).ToString());
                        }
                        else
                        {
                            this.cdTxtInMini.get_gameObject().CustomSetActive(false);
                        }
                    }
                    if (null != this.cdTxtInBig)
                    {
                        if (this.spawnGroup.IsCountingDown())
                        {
                            this.cdTxtInBig.get_gameObject().CustomSetActive(true);
                            this.cdTxtInBig.set_text((this.spawnGroup.GetSpawnTimer() / 0x3e8).ToString());
                        }
                        else
                        {
                            this.cdTxtInBig.get_gameObject().CustomSetActive(false);
                        }
                    }
                }
            }
        }
    }
}

