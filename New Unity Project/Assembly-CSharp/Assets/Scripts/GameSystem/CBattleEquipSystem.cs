namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class CBattleEquipSystem
    {
        public const int c_animationTime = 0x7d0;
        public const int c_equipLevelMaxCount = 3;
        public const int c_fadeTime = 0x2710;
        public const int c_maxEquipCntPerLevel = 12;
        public const uint c_quicklyBuyEquipCount = 2;
        private int[] m_arrEquipActiveSkillCd = new int[6];
        private CUIListScript m_backEquipListScript;
        private ListView<Transform> m_bagEquipItemList = new ListView<Transform>();
        private CUIFormScript m_battleFormScript;
        private bool m_bPlayAnimation;
        private enEquipUsage m_curEquipUsage;
        private ListView<CEquipBuyPrice> m_equipBuyPricePool = new ListView<CEquipBuyPrice>(5);
        private uint m_equipChangedFlags;
        private CUIFormScript m_equipFormScript;
        private Dictionary<ushort, CEquipInfo> m_equipInfoDictionary = Singleton<CEquipSystem>.GetInstance().GetEquipInfoDictionary();
        private ListView<Transform> m_equipItemList = new ListView<Transform>();
        private List<ushort>[][] m_equipList = Singleton<CEquipSystem>.GetInstance().GetEquipList();
        private CEquipRelationPath m_equipRelationPath = new CEquipRelationPath();
        private stEquipTree m_equipTree = new stEquipTree(3, 2, 20);
        private ListView<CExistEquipInfoSet> m_existEquipInfoSetPool = new ListView<CExistEquipInfoSet>(5);
        private PoolObjHandle<ActorRoot> m_hostCtrlHero;
        private bool m_hostCtrlHeroPermitedToBuyEquip;
        private Dictionary<ushort, CEquipBuyPrice> m_hostPlayerCachedEquipBuyPrice = new Dictionary<ushort, CEquipBuyPrice>();
        private ushort[] m_hostPlayerQuicklyBuyEquipIDs = new ushort[2];
        private bool m_isEnabled;
        private bool m_isEquipTreeOpened;
        private bool m_isInEquipLimitedLevel;
        private GameObject[] m_objEquipActiveTimerTxt = new GameObject[6];
        private stSelectedEquipItem[] m_selectedEquipItems = new stSelectedEquipItem[3];
        private enSelectedEquipOrigin m_selectedEquipOrigin = enSelectedEquipOrigin.None;
        private ushort[] m_tempQuicklyBuyEquipIDs = new ushort[2];
        private ListView<CEquipInfo> m_tempRelatedRecommondEquips = new ListView<CEquipInfo>();
        private int m_tickAnimationTime;
        private int m_tickFadeTime;
        private float m_uiEquipItemContentDefaultHeight;
        private float m_uiEquipItemHeight;
        public static string s_equipFormPath = "UGUI/Form/Battle/Form_Battle_Equip.prefab";
        private static int s_equipUsageAmount = Enum.GetValues(typeof(enEquipUsage)).Length;

        public CBattleEquipSystem()
        {
            Dictionary<ushort, CEquipInfo>.Enumerator enumerator = this.m_equipInfoDictionary.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<ushort, CEquipInfo> current = enumerator.Current;
                KeyValuePair<ushort, CEquipInfo> pair2 = enumerator.Current;
                this.m_hostPlayerCachedEquipBuyPrice.Add(current.Key, new CEquipBuyPrice((uint) pair2.Value.m_resEquipInBattle.dwBuyPrice));
            }
        }

        private void AddRecommendPreEquip(ushort equipId, bool bRootRecommend)
        {
            ResEquipInBattle dataByKey = GameDataMgr.m_equipInBattleDatabin.GetDataByKey((uint) equipId);
            if (((dataByKey != null) && (dataByKey.bInvalid <= 0)) && (this.m_equipList[0][dataByKey.bLevel - 1].Count < 12))
            {
                if (bRootRecommend || !this.m_equipList[0][dataByKey.bLevel - 1].Contains(equipId))
                {
                    this.m_equipList[0][dataByKey.bLevel - 1].Add(equipId);
                }
                for (int i = 0; i < dataByKey.PreEquipID.Length; i++)
                {
                    this.AddRecommendPreEquip(dataByKey.PreEquipID[i], false);
                }
            }
        }

        private void BuyEquip(ushort equipID, ref PoolObjHandle<ActorRoot> actor)
        {
            if ((this.m_isEnabled && (actor != 0)) && actor.handle.EquipComponent.IsPermitedToBuyEquip(this.m_isInEquipLimitedLevel))
            {
                CEquipBuyPrice freeEquipBuyPrice = this.GetFreeEquipBuyPrice();
                if (!this.m_isEnabled || !this.IsEquipCanBought(equipID, ref actor, ref freeEquipBuyPrice))
                {
                    freeEquipBuyPrice.m_used = false;
                }
                else
                {
                    actor.handle.ValueComponent.ChangeGoldCoinInBattle((int) (freeEquipBuyPrice.m_buyPrice * -1), false, false, new Vector3(), false, new PoolObjHandle<ActorRoot>());
                    for (int i = 0; i < freeEquipBuyPrice.m_swappedPreEquipInfoCount; i++)
                    {
                        if ((freeEquipBuyPrice.m_swappedPreEquipInfos[i].m_equipID > 0) && (freeEquipBuyPrice.m_swappedPreEquipInfos[i].m_swappedAmount > 0))
                        {
                            while (freeEquipBuyPrice.m_swappedPreEquipInfos[i].m_swappedAmount > 0)
                            {
                                actor.handle.EquipComponent.RemoveEquip(freeEquipBuyPrice.m_swappedPreEquipInfos[i].m_equipID);
                                freeEquipBuyPrice.m_swappedPreEquipInfos[i].m_swappedAmount--;
                            }
                        }
                    }
                    ushort[] requiredEquipIDs = this.GetRequiredEquipIDs(equipID);
                    if ((requiredEquipIDs != null) && (requiredEquipIDs.Length > 0))
                    {
                        for (int j = 0; j < requiredEquipIDs.Length; j++)
                        {
                            if (actor.handle.EquipComponent.HasEquip(requiredEquipIDs[j], 1))
                            {
                                actor.handle.EquipComponent.RemoveEquip(requiredEquipIDs[j]);
                                break;
                            }
                        }
                    }
                    actor.handle.EquipComponent.AddEquip(equipID);
                    actor.handle.EquipComponent.UpdateEquipEffect();
                    freeEquipBuyPrice.m_used = false;
                }
            }
        }

        private void BuyHorizonEquip(ushort equipID, ref PoolObjHandle<ActorRoot> actor)
        {
            if ((this.m_isEnabled && (actor != 0)) && actor.handle.EquipComponent.IsPermitedToBuyEquip(this.m_isInEquipLimitedLevel))
            {
                CEquipInfo equipInfo = this.GetEquipInfo(equipID);
                if (equipInfo == null)
                {
                    DebugHelper.Assert(equipInfo != null, "BuyHorizonEquip GetEquipInfo is null equipId = " + equipID);
                }
                else
                {
                    SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                    if ((curLvelContext != null) && curLvelContext.m_bEnableOrnamentSlot)
                    {
                        SkillComponent skillControl = actor.handle.SkillControl;
                        if ((actor.handle.BuffHolderComp != null) && (skillControl != null))
                        {
                            SkillSlot slot = skillControl.SkillSlotArray[7];
                            if ((slot == null) || (slot.SkillObj == null))
                            {
                                skillControl.InitSkillSlot(7, (int) equipInfo.m_resEquipInBattle.dwActiveSkillID, 0);
                                if (actor.handle.SkillControl.TryGetSkillSlot(SkillSlotType.SLOT_SKILL_7, out slot))
                                {
                                    slot.SetSkillLevel(1);
                                    if ((actor == this.m_hostCtrlHero) && (Singleton<CBattleSystem>.GetInstance().FightForm != null))
                                    {
                                        Singleton<CBattleSystem>.GetInstance().FightForm.ResetSkillButtonManager(actor, true, SkillSlotType.SLOT_SKILL_7);
                                    }
                                }
                            }
                            else
                            {
                                BuffChangeSkillRule changeSkillRule = actor.handle.BuffHolderComp.changeSkillRule;
                                if (changeSkillRule != null)
                                {
                                    changeSkillRule.ChangeSkillSlot(SkillSlotType.SLOT_SKILL_7, (int) equipInfo.m_resEquipInBattle.dwActiveSkillID, slot.SkillObj.SkillID);
                                }
                            }
                            actor.handle.EquipComponent.m_horizonEquipId = equipID;
                        }
                        if ((this.m_hostCtrlHero != 0) && (actor == this.m_hostCtrlHero))
                        {
                            this.SetEquipChangeFlag(enEquipChangeFlag.EquipInBattleChanged);
                        }
                    }
                }
            }
        }

        public void Clear()
        {
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<uint, stEquipInfo[], bool, int>("HeroEquipInBattleChange", new Action<uint, stEquipInfo[], bool, int>(this, (IntPtr) this.OnHeroEquipInBattleChanged));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<uint>("HeroRecommendEquipInit", new Action<uint>(this.OnHeroRecommendEquipInit));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleEquip_ShowOrHideInBattle, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipShowOrHideInBattleBtnClicked));
            this.m_battleFormScript = null;
            this.m_hostCtrlHero.Release();
            this.m_isEnabled = false;
            this.m_hostCtrlHeroPermitedToBuyEquip = false;
            this.m_curEquipUsage = enEquipUsage.Recommend;
            this.ClearEquipList(enEquipUsage.Recommend);
            this.ClearHostPlayerEquipInfo();
            this.m_tickFadeTime = 0;
            this.m_tickAnimationTime = 0;
            this.m_bPlayAnimation = false;
            for (int i = 0; i < 6; i++)
            {
                this.m_objEquipActiveTimerTxt[i] = null;
                this.m_arrEquipActiveSkillCd[i] = 0;
            }
            Singleton<CUIManager>.GetInstance().CloseForm(s_equipFormPath);
        }

        private void ClearEquipChangeFlag()
        {
            this.m_equipChangedFlags = 0;
        }

        private void ClearEquipList(enEquipUsage equipUsage)
        {
            for (int i = 0; i < 3; i++)
            {
                this.m_equipList[(int) equipUsage][i].Clear();
            }
        }

        private void ClearHostPlayerEquipInfo()
        {
            for (int i = 0; i < this.m_hostPlayerQuicklyBuyEquipIDs.Length; i++)
            {
                this.m_hostPlayerQuicklyBuyEquipIDs[i] = 0;
            }
            this.m_hostPlayerCachedEquipBuyPrice.Clear();
        }

        private void ClearSelectedEquipItem()
        {
            for (int i = 0; i < 3; i++)
            {
                this.ClearSelectedEquipItem((enSelectedEquipOrigin) i);
            }
            this.m_selectedEquipOrigin = enSelectedEquipOrigin.None;
            if (this.m_equipRelationPath != null)
            {
                this.m_equipRelationPath.Reset();
            }
            this.EnableOpenEquipTreeButton(false);
        }

        private void ClearSelectedEquipItem(enSelectedEquipOrigin equipSelectedOrigin)
        {
            Transform equipItemTransform = this.m_selectedEquipItems[(int) equipSelectedOrigin].m_equipItemTransform;
            bool bIsActiveSKillEquip = false;
            if ((this.m_selectedEquipItems[(int) equipSelectedOrigin].m_equipInfo != null) && this.IsActiveEquipButNotHorizon(this.m_selectedEquipItems[(int) equipSelectedOrigin].m_equipInfo.m_equipID))
            {
                bIsActiveSKillEquip = true;
            }
            this.m_selectedEquipItems[(int) equipSelectedOrigin].m_equipInfo = null;
            this.m_selectedEquipItems[(int) equipSelectedOrigin].m_equipItemTransform = null;
            this.m_selectedEquipItems[(int) equipSelectedOrigin].m_positionInBag = -1;
            if (equipItemTransform != null)
            {
                this.SetEquipItemSelectFlag(equipItemTransform, false, equipSelectedOrigin, bIsActiveSKillEquip);
            }
        }

        public void CloseEquipFormRightPanel()
        {
            if (null != this.m_equipFormScript)
            {
                GameObject widget = this.m_equipFormScript.GetWidget(6);
                if (widget != null)
                {
                    widget.CustomSetActive(false);
                }
                GameObject obj3 = this.m_equipFormScript.GetWidget(7);
                if (obj3 != null)
                {
                    obj3.CustomSetActive(false);
                }
                GameObject obj4 = this.m_equipFormScript.GetWidget(5);
                if (obj4 != null)
                {
                    obj4.CustomSetActive(false);
                }
            }
        }

        private void CloseEquipTreePanel()
        {
            if (this.m_equipFormScript != null)
            {
                GameObject widget = this.m_equipFormScript.GetWidget(12);
                if (widget != null)
                {
                    widget.CustomSetActive(true);
                }
                GameObject obj3 = this.m_equipFormScript.GetWidget(13);
                if (obj3 != null)
                {
                    obj3.CustomSetActive(false);
                }
                this.m_isEquipTreeOpened = false;
                this.m_equipTree.Clear();
                this.ClearSelectedEquipItem(enSelectedEquipOrigin.EquipTree);
                if (this.m_selectedEquipOrigin == enSelectedEquipOrigin.EquipTree)
                {
                    if (this.m_selectedEquipItems[0].m_equipInfo != null)
                    {
                        this.m_selectedEquipOrigin = enSelectedEquipOrigin.EquipLibaray;
                    }
                    else
                    {
                        this.m_selectedEquipOrigin = enSelectedEquipOrigin.None;
                    }
                }
                if (this.m_selectedEquipOrigin == enSelectedEquipOrigin.None)
                {
                    this.CloseEquipFormRightPanel();
                }
                else
                {
                    this.RefreshEquipFormRightPanel(false);
                }
            }
        }

        private void EnableOpenEquipTreeButton(bool enabled)
        {
            if (this.m_equipFormScript != null)
            {
                GameObject widget = this.m_equipFormScript.GetWidget(14);
                if (widget != null)
                {
                    widget.CustomSetActive(enabled);
                }
            }
        }

        public void ExecChooseEquipSkillCmd(int iEquipSlotIndex, ref PoolObjHandle<ActorRoot> actor)
        {
            if (((iEquipSlotIndex >= 0) && (iEquipSlotIndex < 6)) && (((actor != 0) && (actor.handle.EquipComponent != null)) && (actor.handle.EquipComponent.GetEquipActiveSkillSlotInfo(iEquipSlotIndex) == ENUM_EQUIP_ACTIVESKILL_STATUS.ENM_EQUIP_ACTIVESKILL_STATUS_NOTSHOW)))
            {
                SkillSlotType showEquipActiveSkillSlot = actor.handle.EquipComponent.GetShowEquipActiveSkillSlot();
                int equipSlotBySkillSlot = actor.handle.EquipComponent.GetEquipSlotBySkillSlot(showEquipActiveSkillSlot);
                if ((equipSlotBySkillSlot >= 0) && (equipSlotBySkillSlot < 6))
                {
                    SkillSlot slot;
                    if ((actor.handle.SkillControl != null) && actor.handle.SkillControl.TryGetSkillSlot(showEquipActiveSkillSlot, out slot))
                    {
                        int curSkillCD = (int) slot.CurSkillCD;
                        actor.handle.EquipComponent.AddEquipActiveSkillCdInfo(equipSlotBySkillSlot, curSkillCD);
                    }
                    actor.handle.EquipComponent.SetEquipActiveSkillSlotInfo(equipSlotBySkillSlot, ENUM_EQUIP_ACTIVESKILL_STATUS.ENM_EQUIP_ACTIVESKILL_STATUS_NOTSHOW);
                    if (actor == this.m_hostCtrlHero)
                    {
                        this.SetEquipSkillBeUsingBtnState(false, equipSlotBySkillSlot);
                    }
                }
                if (actor.handle.EquipComponent.ChangeEquipSkillSlot(showEquipActiveSkillSlot, iEquipSlotIndex))
                {
                    actor.handle.EquipComponent.SetEquipActiveSkillSlotInfo(iEquipSlotIndex, ENUM_EQUIP_ACTIVESKILL_STATUS.ENM_EQUIP_ACTIVESKILL_STATUS_ISSHOWING);
                    if (actor == this.m_hostCtrlHero)
                    {
                        if (actor.handle.EquipComponent.GetEquipActiveSkillInitFlag(showEquipActiveSkillSlot) && (Singleton<CBattleSystem>.GetInstance().FightForm != null))
                        {
                            Singleton<CBattleSystem>.GetInstance().FightForm.ResetSkillButtonManager(actor, true, showEquipActiveSkillSlot);
                        }
                        this.SetEquipSkillBtnState(true, showEquipActiveSkillSlot);
                        this.SetEquipSkillBeUsingBtnState(true, iEquipSlotIndex);
                        this.SetEquipSkillShowBtnState(false, iEquipSlotIndex);
                    }
                }
            }
        }

        public void ExecuteBuyEquipFrameCommand(ushort equipID, ref PoolObjHandle<ActorRoot> actor)
        {
            this.BuyEquip(equipID, ref actor);
        }

        public void ExecuteBuyHorizonEquipFrameCommand(ushort equipID, ref PoolObjHandle<ActorRoot> actor)
        {
            this.BuyHorizonEquip(equipID, ref actor);
        }

        public void ExecuteInOutEquipShopFrameCommand(byte inOut, ref PoolObjHandle<ActorRoot> actor)
        {
            if (Singleton<WatchController>.GetInstance().IsWatching || actor.handle.IsHostCamp())
            {
                if (inOut > 0)
                {
                    actor.handle.HudControl.AddInEquipShopHud();
                }
                else
                {
                    actor.handle.HudControl.RemoveInEquipShopHud();
                }
            }
        }

        public void ExecuteSellEquipFrameCommand(int equipIndex, ref PoolObjHandle<ActorRoot> actor)
        {
            this.SellEquip(equipIndex, ref actor);
        }

        private Transform GetBagEquipItem(int index)
        {
            if ((index >= 0) && (index < this.m_bagEquipItemList.Count))
            {
                return this.m_bagEquipItemList[index];
            }
            return null;
        }

        public void GetEquipBuyPrice(ushort equipID, ref PoolObjHandle<ActorRoot> actor, ref CEquipBuyPrice equipBuyPrice)
        {
            equipBuyPrice.Clear();
            if ((this.m_isEnabled && (equipID != 0)) && (actor != 0))
            {
                ResEquipInBattle dataByKey = GameDataMgr.m_equipInBattleDatabin.GetDataByKey((uint) equipID);
                if (dataByKey != null)
                {
                    uint dwBuyPrice = (uint) dataByKey.dwBuyPrice;
                    uint num2 = 0;
                    CExistEquipInfoSet freeExistEquipInfoSet = this.GetFreeExistEquipInfoSet();
                    freeExistEquipInfoSet.Clone(actor.handle.EquipComponent.GetExistEquipInfoSet());
                    freeExistEquipInfoSet.ResetCalculateAmount();
                    num2 = this.GetPreEquipSwappedPrice(equipID, ref freeExistEquipInfoSet, ref equipBuyPrice);
                    freeExistEquipInfoSet.m_used = false;
                    if (dwBuyPrice >= num2)
                    {
                        dwBuyPrice -= num2;
                    }
                    else
                    {
                        dwBuyPrice = 0;
                    }
                    equipBuyPrice.m_buyPrice = (CrypticInt32) dwBuyPrice;
                }
            }
        }

        private CEquipInfo GetEquipInfo(ushort equipID)
        {
            CEquipInfo info = null;
            if (this.m_equipInfoDictionary.TryGetValue(equipID, out info))
            {
                return info;
            }
            return null;
        }

        private Transform GetEquipItem(int level, int index)
        {
            index = ((level - 1) * 12) + index;
            if ((index >= 0) && (index < this.m_equipItemList.Count))
            {
                return this.m_equipItemList[index];
            }
            return null;
        }

        private CEquipBuyPrice GetFreeEquipBuyPrice()
        {
            for (int i = 0; i < this.m_equipBuyPricePool.Count; i++)
            {
                if (!this.m_equipBuyPricePool[i].m_used)
                {
                    this.m_equipBuyPricePool[i].Clear();
                    this.m_equipBuyPricePool[i].m_used = true;
                    return this.m_equipBuyPricePool[i];
                }
            }
            CEquipBuyPrice item = new CEquipBuyPrice(0);
            this.m_equipBuyPricePool.Add(item);
            item.m_used = true;
            return item;
        }

        private CExistEquipInfoSet GetFreeExistEquipInfoSet()
        {
            for (int i = 0; i < this.m_existEquipInfoSetPool.Count; i++)
            {
                if (!this.m_existEquipInfoSetPool[i].m_used)
                {
                    this.m_existEquipInfoSetPool[i].Clear();
                    this.m_existEquipInfoSetPool[i].m_used = true;
                    return this.m_existEquipInfoSetPool[i];
                }
            }
            CExistEquipInfoSet item = new CExistEquipInfoSet();
            this.m_existEquipInfoSetPool.Add(item);
            item.m_used = true;
            return item;
        }

        public ushort[] GetHostCtrlHeroQuicklyBuyEquipList()
        {
            return this.m_hostPlayerQuicklyBuyEquipIDs;
        }

        private CEquipBuyPrice GetHostPlayerCachedEquipBuyPrice(ushort equipID)
        {
            CEquipBuyPrice price = null;
            if (this.m_hostPlayerCachedEquipBuyPrice.TryGetValue(equipID, out price))
            {
                return price;
            }
            return null;
        }

        private uint GetPreEquipSwappedPrice(ushort equipID, ref CExistEquipInfoSet existEquipInfoSet, ref CEquipBuyPrice equipBuyPrice)
        {
            if (equipID == 0)
            {
                return 0;
            }
            ResEquipInBattle dataByKey = GameDataMgr.m_equipInBattleDatabin.GetDataByKey((uint) equipID);
            if (dataByKey == null)
            {
                return 0;
            }
            uint num = 0;
            for (int i = 0; i < dataByKey.PreEquipID.Length; i++)
            {
                if (dataByKey.PreEquipID[i] <= 0)
                {
                    continue;
                }
                bool flag = false;
                for (int j = 0; j < existEquipInfoSet.m_existEquipInfoCount; j++)
                {
                    if ((existEquipInfoSet.m_existEquipInfos[j].m_equipID == dataByKey.PreEquipID[i]) && (existEquipInfoSet.m_existEquipInfos[j].m_calculateAmount > 0))
                    {
                        flag = true;
                        num += (uint) existEquipInfoSet.m_existEquipInfos[j].m_unitBuyPrice;
                        existEquipInfoSet.m_existEquipInfos[j].m_calculateAmount--;
                        equipBuyPrice.AddSwappedPreEquipInfo(dataByKey.PreEquipID[i]);
                        break;
                    }
                }
                if (!flag)
                {
                    num += this.GetPreEquipSwappedPrice(dataByKey.PreEquipID[i], ref existEquipInfoSet, ref equipBuyPrice);
                }
            }
            return num;
        }

        public ushort[] GetQuicklyBuyEquipList(ref PoolObjHandle<ActorRoot> actor)
        {
            for (int i = 0; i < 2L; i++)
            {
                this.m_tempQuicklyBuyEquipIDs[i] = 0;
            }
            if ((actor != 0) && (actor.handle.EquipComponent != null))
            {
                ListView<CRecommendEquipInfo> recommendEquipInfos = actor.handle.EquipComponent.GetRecommendEquipInfos();
                Dictionary<ushort, uint> equipBoughtHistory = actor.handle.EquipComponent.GetEquipBoughtHistory();
                bool flag = false;
                List<ushort> usageLevelEquipList = this.GetUsageLevelEquipList(enEquipUsage.Move, 2);
                if (usageLevelEquipList != null)
                {
                    for (int k = 0; k < usageLevelEquipList.Count; k++)
                    {
                        if (equipBoughtHistory.ContainsKey(usageLevelEquipList[k]))
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                CExistEquipInfoSet freeExistEquipInfoSet = this.GetFreeExistEquipInfoSet();
                freeExistEquipInfoSet.Clone(actor.handle.EquipComponent.GetExistEquipInfoSet());
                int index = 0;
                for (int j = 0; j < recommendEquipInfos.Count; j++)
                {
                    if (!recommendEquipInfos[j].m_hasBeenBought && ((recommendEquipInfos[j].m_resEquipInBattle.bUsage != 4) || !flag))
                    {
                        this.m_tempRelatedRecommondEquips.Clear();
                        freeExistEquipInfoSet.ResetCalculateAmount();
                        this.GetRelatedRecommondEquips(recommendEquipInfos[j].m_equipID, true, ref actor, ref freeExistEquipInfoSet, ref this.m_tempRelatedRecommondEquips);
                        for (int m = 0; m < index; m++)
                        {
                            for (int num6 = 0; num6 < this.m_tempRelatedRecommondEquips.Count; num6++)
                            {
                                if (this.m_tempQuicklyBuyEquipIDs[m] == this.m_tempRelatedRecommondEquips[num6].m_equipID)
                                {
                                    this.m_tempRelatedRecommondEquips.RemoveAt(num6);
                                    break;
                                }
                            }
                        }
                        if (this.m_tempRelatedRecommondEquips.Count == 0)
                        {
                            break;
                        }
                        this.m_tempRelatedRecommondEquips.Sort();
                        int num7 = Math.Min(this.m_tempRelatedRecommondEquips.Count, 2 - index);
                        for (int n = 0; n < num7; n++)
                        {
                            this.m_tempQuicklyBuyEquipIDs[index] = this.m_tempRelatedRecommondEquips[n].m_equipID;
                            index++;
                        }
                        if (index >= 2L)
                        {
                            break;
                        }
                    }
                }
                freeExistEquipInfoSet.m_used = false;
            }
            return this.m_tempQuicklyBuyEquipIDs;
        }

        public void GetRelatedRecommondEquips(ushort equipID, bool isRootEquip, ref PoolObjHandle<ActorRoot> actor, ref CExistEquipInfoSet existEquipInfoSet, ref ListView<CEquipInfo> relatedRecommondEquips)
        {
            CEquipInfo info = null;
            if (this.m_equipInfoDictionary.TryGetValue(equipID, out info))
            {
                if (!isRootEquip)
                {
                    for (int i = 0; i < existEquipInfoSet.m_existEquipInfoCount; i++)
                    {
                        if ((existEquipInfoSet.m_existEquipInfos[i].m_equipID == equipID) && (existEquipInfoSet.m_existEquipInfos[i].m_calculateAmount > 0))
                        {
                            existEquipInfoSet.m_existEquipInfos[i].m_calculateAmount--;
                            return;
                        }
                    }
                }
                CEquipBuyPrice freeEquipBuyPrice = this.GetFreeEquipBuyPrice();
                if (this.IsEquipCanBought(equipID, ref actor, ref freeEquipBuyPrice))
                {
                    if (!relatedRecommondEquips.Contains(info))
                    {
                        relatedRecommondEquips.Add(info);
                    }
                    freeEquipBuyPrice.m_used = false;
                }
                else
                {
                    freeEquipBuyPrice.m_used = false;
                    for (int j = 0; j < info.m_resEquipInBattle.PreEquipID.Length; j++)
                    {
                        this.GetRelatedRecommondEquips(info.m_resEquipInBattle.PreEquipID[j], false, ref actor, ref existEquipInfoSet, ref relatedRecommondEquips);
                    }
                }
            }
        }

        private ushort[] GetRequiredEquipIDs(ushort equipID)
        {
            CEquipInfo info = null;
            if (this.m_equipInfoDictionary.TryGetValue(equipID, out info))
            {
                return info.m_requiredEquipIDs;
            }
            return null;
        }

        public List<ushort> GetUsageLevelEquipList(enEquipUsage equipUsage, int level)
        {
            if (((equipUsage > enEquipUsage.Recommend) && (equipUsage <= enEquipUsage.Horizon)) && ((level > 0) && (level <= 3)))
            {
                return this.m_equipList[(int) equipUsage][level - 1];
            }
            return null;
        }

        private void HandleEquipActiveSkillWhenEquipChange(uint actorObjectID, bool bIsAdd, int iEquipSlotIndex)
        {
            if (((this.m_hostCtrlHero != 0) && (actorObjectID == this.m_hostCtrlHero.handle.ObjID)) && (this.m_hostCtrlHero.handle.EquipComponent != null))
            {
                ENUM_EQUIP_ACTIVESKILL_STATUS equipActiveSkillSlotInfo = this.m_hostCtrlHero.handle.EquipComponent.GetEquipActiveSkillSlotInfo(iEquipSlotIndex);
                SkillSlotType showingSkillSlotByEquipSlot = this.m_hostCtrlHero.handle.EquipComponent.GetShowingSkillSlotByEquipSlot(iEquipSlotIndex);
                switch (equipActiveSkillSlotInfo)
                {
                    case ENUM_EQUIP_ACTIVESKILL_STATUS.ENM_EQUIP_ACTIVESKILL_STATUS_WHITHOUTACTIVESKILL:
                        this.SetEquipSkillShowBtnState(false, iEquipSlotIndex);
                        this.SetEquipSkillBeUsingBtnState(false, iEquipSlotIndex);
                        if (!bIsAdd)
                        {
                            this.SetEquipActiveSkillCdState(iEquipSlotIndex, -1);
                            for (int i = 0; i < 1; i++)
                            {
                                if (this.m_hostCtrlHero.handle.EquipComponent.GetEquipActiveSkillShowFlag((SkillSlotType) (i + 8)))
                                {
                                    int equipSlotBySkillSlot = this.m_hostCtrlHero.handle.EquipComponent.GetEquipSlotBySkillSlot((SkillSlotType) (i + 8));
                                    this.SetEquipSkillBeUsingBtnState(true, equipSlotBySkillSlot);
                                }
                            }
                        }
                        return;

                    case ENUM_EQUIP_ACTIVESKILL_STATUS.ENM_EQUIP_ACTIVESKILL_STATUS_NOTSHOW:
                        this.UpdateEquipActiveSkillCd(iEquipSlotIndex, false);
                        this.SetEquipSkillBeUsingBtnState(false, iEquipSlotIndex);
                        return;

                    case ENUM_EQUIP_ACTIVESKILL_STATUS.ENM_EQUIP_ACTIVESKILL_STATUS_ISSHOWING:
                        if (!this.m_hostCtrlHero.handle.EquipComponent.GetEquipActiveSkillInitFlag(showingSkillSlotByEquipSlot))
                        {
                            this.SetEquipSkillBtnState(true, showingSkillSlotByEquipSlot);
                        }
                        else if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
                        {
                            Singleton<CBattleSystem>.GetInstance().FightForm.ResetSkillButtonManager(this.m_hostCtrlHero, true, showingSkillSlotByEquipSlot);
                        }
                        this.UpdateEquipActiveSkillCd(iEquipSlotIndex, false);
                        this.SetEquipSkillBeUsingBtnState(true, iEquipSlotIndex);
                        return;

                    case ENUM_EQUIP_ACTIVESKILL_STATUS.ENM_EQUIP_ACTIVESKILL_STATUS_WHITHOUTACTIVESKILL_REMOVECURSKILL:
                        this.SetEquipActiveSkillCdState(iEquipSlotIndex, -1);
                        for (int j = 0; j < 1; j++)
                        {
                            if (!this.m_hostCtrlHero.handle.EquipComponent.GetEquipActiveSkillShowFlag((SkillSlotType) (j + 8)))
                            {
                                this.SetEquipSkillBtnState(false, (SkillSlotType) (j + 8));
                            }
                        }
                        this.SetEquipSkillShowBtnState(false, iEquipSlotIndex);
                        this.SetEquipSkillBeUsingBtnState(false, iEquipSlotIndex);
                        return;
                }
            }
        }

        private bool HasEquipChangeFlag(enEquipChangeFlag flag)
        {
            return ((((enEquipChangeFlag) this.m_equipChangedFlags) & flag) > ((enEquipChangeFlag) 0));
        }

        private void InitBagEquipItemList()
        {
            this.m_bagEquipItemList.Clear();
            GameObject widget = this.m_equipFormScript.GetWidget(4);
            if (null != widget)
            {
                Transform transform = widget.get_transform();
                for (int i = 0; i < 6; i++)
                {
                    Transform item = transform.Find(string.Format("equipItem{0}", i));
                    this.m_bagEquipItemList.Add(item);
                }
            }
        }

        private void InitEquipActiveSkillTimerTxt()
        {
            for (int i = 0; i < 6; i++)
            {
                if (this.m_objEquipActiveTimerTxt[i] == null)
                {
                    Transform bagEquipItem = this.GetBagEquipItem(i);
                    if (bagEquipItem != null)
                    {
                        Transform transform2 = bagEquipItem.FindChild("TimerTxt");
                        if (transform2 != null)
                        {
                            this.m_objEquipActiveTimerTxt[i] = transform2.FindChild("TimerTxt").get_gameObject();
                        }
                    }
                    this.m_arrEquipActiveSkillCd[i] = 0;
                }
            }
        }

        private void InitEquipItemHorizontalLine(Transform equipPanel, int level)
        {
            if (null != equipPanel)
            {
                Transform equipItem = null;
                Transform transform2 = null;
                Transform transform3 = null;
                int index = 0;
                for (int i = 0; i < 12; i++)
                {
                    equipItem = this.GetEquipItem(level, i);
                    if (null != equipItem)
                    {
                        transform2 = equipItem.Find("imgLineFront");
                        if (level > 1)
                        {
                            index = (level <= 2) ? 0 : 1;
                            this.m_equipRelationPath.InitializeHorizontalLine(index, i, CEquipLineSet.enHorizontalLineType.Right, transform2.get_gameObject());
                        }
                        transform3 = equipItem.Find("imgLineBack");
                        if (level < 3)
                        {
                            index = (level >= 2) ? 1 : 0;
                            this.m_equipRelationPath.InitializeHorizontalLine(index, i, CEquipLineSet.enHorizontalLineType.Left, transform3.get_gameObject());
                        }
                    }
                }
            }
        }

        private void InitEquipLevelPanel()
        {
            this.m_equipItemList.Clear();
            for (int i = 0; i < 3; i++)
            {
                GameObject widget = this.m_equipFormScript.GetWidget(1 + i);
                if (widget != null)
                {
                    Transform transform = widget.get_transform();
                    for (int j = 0; j < 12; j++)
                    {
                        Transform item = transform.Find(string.Format("equipItem{0}", j));
                        this.m_equipItemList.Add(item);
                        if (item != null)
                        {
                            item.get_gameObject().CustomSetActive(true);
                        }
                    }
                }
            }
        }

        private void InitEquipPathLine()
        {
            if (this.m_equipFormScript != null)
            {
                this.m_equipRelationPath.Clear();
                Transform transform = this.m_equipFormScript.GetWidget(8).get_transform();
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 11; j++)
                    {
                        this.m_equipRelationPath.InitializeVerticalLine(i, j, j + 1, transform.Find(string.Format("imgLine_{0}_{1}", i, j)).get_gameObject());
                    }
                }
                this.InitEquipItemHorizontalLine(this.m_equipFormScript.GetWidget(1).get_transform(), 1);
                this.InitEquipItemHorizontalLine(this.m_equipFormScript.GetWidget(2).get_transform(), 2);
                this.InitEquipItemHorizontalLine(this.m_equipFormScript.GetWidget(3).get_transform(), 3);
            }
        }

        public void Initialize(CUIFormScript battleFormScript, PoolObjHandle<ActorRoot> hostCtrlHero, bool enableEquipSystem, bool isInEquipLimitedLevel)
        {
            this.Clear();
            this.m_battleFormScript = battleFormScript;
            this.m_hostCtrlHero = hostCtrlHero;
            DebugHelper.Assert((bool) this.m_hostCtrlHero, "Initialize EquipSystem with null host ctrl hero.");
            this.m_isEnabled = enableEquipSystem;
            this.m_isInEquipLimitedLevel = isInEquipLimitedLevel;
            this.m_hostCtrlHeroPermitedToBuyEquip = false;
            EquipComponent.s_equipEffectSequence = 0;
            if (this.m_isEnabled)
            {
                this.RefreshHostPlayerCachedEquipBuyPrice();
                this.OnEquipFormOpen(null);
                Singleton<CUIManager>.GetInstance().CloseForm(s_equipFormPath);
                Singleton<EventRouter>.GetInstance().AddEventHandler<uint, stEquipInfo[], bool, int>("HeroEquipInBattleChange", new Action<uint, stEquipInfo[], bool, int>(this, (IntPtr) this.OnHeroEquipInBattleChanged));
                Singleton<EventRouter>.GetInstance().AddEventHandler<uint>("HeroRecommendEquipInit", new Action<uint>(this.OnHeroRecommendEquipInit));
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleEquip_ShowOrHideInBattle, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipShowOrHideInBattleBtnClicked));
            }
        }

        private void InitializeRecommendEquipList()
        {
            this.ClearEquipList(enEquipUsage.Recommend);
            DebugHelper.Assert(this.m_hostCtrlHero == 1, "InitializeEquipList m_hostCtrlHero is null");
            if (this.m_hostCtrlHero != 0)
            {
                ListView<CRecommendEquipInfo> recommendEquipInfos = this.m_hostCtrlHero.handle.EquipComponent.GetRecommendEquipInfos();
                if (recommendEquipInfos != null)
                {
                    for (int i = 0; i < recommendEquipInfos.Count; i++)
                    {
                        this.AddRecommendPreEquip(recommendEquipInfos[i].m_equipID, true);
                    }
                }
            }
        }

        private bool IsActiveEquipButNotHorizon(ushort equipId)
        {
            CEquipInfo equipInfo = this.GetEquipInfo(equipId);
            return (((equipInfo != null) && (equipInfo.m_resEquipInBattle.dwActiveSkillID > 0)) && (equipInfo.m_resEquipInBattle.bUsage != 6));
        }

        public bool IsEquipCanBought(ushort equipID, ref PoolObjHandle<ActorRoot> actor, ref CEquipBuyPrice equipBuyPrice)
        {
            if ((!this.m_isEnabled || (equipID == 0)) || (actor == 0))
            {
                return false;
            }
            ResEquipInBattle dataByKey = GameDataMgr.m_equipInBattleDatabin.GetDataByKey((uint) equipID);
            if ((dataByKey == null) || (dataByKey.bInvalid > 0))
            {
                return false;
            }
            if ((dataByKey.bNeedPunish > 0) && !actor.handle.SkillControl.HasPunishSkill())
            {
                return false;
            }
            if ((dataByKey.wGroup > 0) && actor.handle.EquipComponent.HasEquipInGroup(dataByKey.wGroup))
            {
                return false;
            }
            ushort[] requiredEquipIDs = this.GetRequiredEquipIDs(equipID);
            if ((requiredEquipIDs != null) && (requiredEquipIDs.Length > 0))
            {
                bool flag = false;
                for (int i = 0; i < requiredEquipIDs.Length; i++)
                {
                    if (actor.handle.EquipComponent.HasEquip(requiredEquipIDs[i], 1))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    return false;
                }
            }
            if (actor == this.m_hostCtrlHero)
            {
                equipBuyPrice.Clone(this.GetHostPlayerCachedEquipBuyPrice(equipID));
            }
            else
            {
                this.GetEquipBuyPrice(equipID, ref actor, ref equipBuyPrice);
            }
            if (((ulong) actor.handle.ValueComponent.GetGoldCoinInBattle()) < ((long) equipBuyPrice.m_buyPrice))
            {
                return false;
            }
            if (!actor.handle.EquipComponent.IsEquipCanAddedToGrid(equipID, ref equipBuyPrice))
            {
                return false;
            }
            return true;
        }

        public bool IsHorizonEquipCanBought(ushort equipID, ref PoolObjHandle<ActorRoot> actor, ref int price)
        {
            if ((this.m_isEnabled && (equipID != 0)) && (actor != 0))
            {
                ResEquipInBattle dataByKey = GameDataMgr.m_equipInBattleDatabin.GetDataByKey((uint) equipID);
                if ((dataByKey == null) || (dataByKey.bInvalid > 0))
                {
                    return false;
                }
                price = (int) dataByKey.dwBuyPrice;
                SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                if ((curLvelContext != null) && curLvelContext.m_bEnableOrnamentSlot)
                {
                    SkillSlot slot = actor.handle.SkillControl.SkillSlotArray[7];
                    if (((dataByKey.dwActiveSkillID > 0) && ((slot == null) || (dataByKey.dwActiveSkillID != slot.SkillObj.SkillID))) && (actor.handle.ValueComponent.GetGoldCoinInBattle() >= dataByKey.dwBuyPrice))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsHorizonEquipOwn(ushort equipID, ref PoolObjHandle<ActorRoot> actor)
        {
            if ((this.m_isEnabled && (equipID != 0)) && (actor != 0))
            {
                ResEquipInBattle dataByKey = GameDataMgr.m_equipInBattleDatabin.GetDataByKey((uint) equipID);
                if ((dataByKey == null) || (dataByKey.bInvalid > 0))
                {
                    return false;
                }
                SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                if ((curLvelContext != null) && curLvelContext.m_bEnableOrnamentSlot)
                {
                    SkillSlot slot = actor.handle.SkillControl.SkillSlotArray[7];
                    if ((slot != null) && (slot.SkillObj != null))
                    {
                        return (dataByKey.dwActiveSkillID == slot.SkillObj.SkillID);
                    }
                }
            }
            return false;
        }

        public bool IsHostCtrlHeroPermitedToBuyEquip()
        {
            return this.m_hostCtrlHeroPermitedToBuyEquip;
        }

        public bool IsInEquipLimitedLevel()
        {
            return this.m_isInEquipLimitedLevel;
        }

        public void OnActorGoldChange(int changeValue, int currentValue)
        {
            this.SetEquipChangeFlag(enEquipChangeFlag.GoldCoinInBattleChanged);
        }

        public void OnBattleEquipBackEquipListMoreLeftClicked(CUIEvent uiEvent)
        {
            if (this.m_backEquipListScript != null)
            {
                this.m_backEquipListScript.MoveContent(new Vector2(this.m_backEquipListScript.GetEelementDefaultSize().x, 0f));
            }
        }

        public void OnBattleEquipBackEquipListMoreRightClicked(CUIEvent uiEvent)
        {
            if (this.m_backEquipListScript != null)
            {
                this.m_backEquipListScript.MoveContent(new Vector2(this.m_backEquipListScript.GetEelementDefaultSize().x * -1f, 0f));
            }
        }

        public void OnBattleEquipBackEquipListScrollChanged(CUIEvent uiEvent)
        {
            CUIListScript srcWidgetScript = uiEvent.m_srcWidgetScript as CUIListScript;
            if (srcWidgetScript != null)
            {
                Vector2 contentSize = srcWidgetScript.GetContentSize();
                Vector2 scrollAreaSize = srcWidgetScript.GetScrollAreaSize();
                Vector2 contentPosition = srcWidgetScript.GetContentPosition();
                GameObject widget = uiEvent.m_srcFormScript.GetWidget(0x10);
                GameObject obj3 = uiEvent.m_srcFormScript.GetWidget(0x11);
                if (srcWidgetScript.IsNeedScroll())
                {
                    if (contentPosition.x >= 0f)
                    {
                        widget.CustomSetActive(false);
                        obj3.CustomSetActive(true);
                    }
                    else if ((contentPosition.x + contentSize.x) <= scrollAreaSize.x)
                    {
                        widget.CustomSetActive(true);
                        obj3.CustomSetActive(false);
                    }
                    else
                    {
                        widget.CustomSetActive(true);
                        obj3.CustomSetActive(true);
                    }
                }
                else
                {
                    widget.CustomSetActive(false);
                    obj3.CustomSetActive(false);
                }
            }
        }

        public void OnBattleEquipBackEquipListSelectedChanged(CUIEvent uiEvent)
        {
            List<ushort> backEquipIDs = null;
            int num = 0;
            if ((this.m_selectedEquipOrigin != enSelectedEquipOrigin.None) && (this.m_selectedEquipItems[(int) this.m_selectedEquipOrigin].m_equipInfo != null))
            {
                backEquipIDs = this.m_selectedEquipItems[(int) this.m_selectedEquipOrigin].m_equipInfo.m_backEquipIDs;
                num = (backEquipIDs != null) ? backEquipIDs.Count : 0;
            }
            CUIListScript srcWidgetScript = uiEvent.m_srcWidgetScript as CUIListScript;
            if (srcWidgetScript != null)
            {
                int selectedIndex = srcWidgetScript.GetSelectedIndex();
                if ((selectedIndex >= 0) && (selectedIndex < num))
                {
                    ushort rootEquipID = backEquipIDs[selectedIndex];
                    this.m_equipTree.Create(rootEquipID, this.m_equipInfoDictionary);
                    GameObject widget = uiEvent.m_srcFormScript.GetWidget(13);
                    Transform equipItemTransform = (widget == null) ? null : widget.get_transform().Find("rootEquipItem");
                    this.SetSelectedEquipItem(enSelectedEquipOrigin.EquipTree, this.GetEquipInfo(rootEquipID), equipItemTransform, -1);
                    this.m_selectedEquipOrigin = enSelectedEquipOrigin.EquipTree;
                    this.RefreshEquipTreePanel(false);
                    this.RefreshEquipFormRightPanel(false);
                }
            }
        }

        public void OnBattleEquipCloseEquipTree(CUIEvent uiEvent)
        {
            this.CloseEquipTreePanel();
            this.RefreshEquipLibraryPanel(false);
            if ((this.m_selectedEquipOrigin == enSelectedEquipOrigin.EquipTree) || (this.m_selectedEquipOrigin == enSelectedEquipOrigin.None))
            {
                this.ClearSelectedEquipItem();
                this.CloseEquipFormRightPanel();
            }
            else
            {
                this.ClearSelectedEquipItem(enSelectedEquipOrigin.EquipTree);
                this.RefreshEquipFormRightPanel(false);
                this.SetFocusEquipInEquipLibrary(this.m_selectedEquipItems[(int) this.m_selectedEquipOrigin].m_equipInfo, false);
            }
        }

        public void OnBattleEquipOpenEquipTree(CUIEvent uiEvent)
        {
            if ((this.m_selectedEquipOrigin == enSelectedEquipOrigin.EquipLibaray) || (this.m_selectedEquipOrigin == enSelectedEquipOrigin.EquipBag))
            {
                CEquipInfo equipInfo = this.m_selectedEquipItems[(int) this.m_selectedEquipOrigin].m_equipInfo;
                if (equipInfo != null)
                {
                    this.OpenEquipTreePanel(equipInfo, this.m_selectedEquipOrigin == enSelectedEquipOrigin.EquipLibaray);
                }
            }
        }

        public void OnBattleEquipQuicklyBuy(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.battleEquipPar.equipInfo != null)
            {
                this.SendBuyEquipFrameCommand(uiEvent.m_eventParams.battleEquipPar.equipInfo.m_equipID, true);
            }
        }

        public void OnBattleEquipSelectItemInEquipTree(CUIEvent uiEvent)
        {
            if (this.m_selectedEquipOrigin == enSelectedEquipOrigin.EquipBag)
            {
                this.ClearSelectedEquipItem(enSelectedEquipOrigin.EquipBag);
            }
            else if (this.m_selectedEquipOrigin == enSelectedEquipOrigin.EquipTree)
            {
                CEquipInfo equipInfo = this.m_selectedEquipItems[(int) this.m_selectedEquipOrigin].m_equipInfo;
                if (((equipInfo != null) && (equipInfo.m_equipID == uiEvent.m_eventParams.commonUInt16Param1)) && (this.m_selectedEquipItems[(int) this.m_selectedEquipOrigin].m_equipItemTransform == uiEvent.m_srcWidget.get_transform()))
                {
                    return;
                }
            }
            this.SetSelectedEquipItem(enSelectedEquipOrigin.EquipTree, this.GetEquipInfo(uiEvent.m_eventParams.commonUInt16Param1), uiEvent.m_srcWidget.get_transform(), -1);
            this.m_selectedEquipOrigin = enSelectedEquipOrigin.EquipTree;
            this.RefreshBackEquipList(false);
            this.RefreshEquipFormRightPanel(false);
        }

        private void OnBattleEquipShowOrHideInBattleBtnClicked(CUIEvent uiEvent)
        {
            if (((uiEvent.m_srcWidget != null) && (uiEvent.m_srcWidget.get_transform() != null)) && (uiEvent.m_srcWidget.get_transform().get_parent() != null))
            {
                string str = uiEvent.m_srcWidget.get_transform().get_parent().get_name();
                int num = int.Parse(str.Substring(str.IndexOf("equipItem") + 9));
                if ((num >= 0) && (num < 6))
                {
                    FrameCommand<PlayerChooseEquipSkillCommand> command = FrameCommandFactory.CreateFrameCommand<PlayerChooseEquipSkillCommand>();
                    command.cmdData.m_iEquipSlotIndex = num;
                    command.Send();
                }
            }
        }

        public void OnEquipBagItemSelect(CUIEvent uiEvent)
        {
            if (((this.m_selectedEquipOrigin != enSelectedEquipOrigin.EquipBag) || (this.m_selectedEquipItems[(int) this.m_selectedEquipOrigin].m_equipInfo == null)) || ((this.m_selectedEquipItems[(int) this.m_selectedEquipOrigin].m_equipInfo.m_equipID != uiEvent.m_eventParams.battleEquipPar.equipInfo.m_equipID) || (this.m_selectedEquipItems[(int) this.m_selectedEquipOrigin].m_equipItemTransform != uiEvent.m_srcWidget.get_transform())))
            {
                this.ClearSelectedEquipItem(enSelectedEquipOrigin.EquipLibaray);
                this.ClearSelectedEquipItem(enSelectedEquipOrigin.EquipTree);
                this.SetFocusEquipInEquipLibrary(uiEvent.m_eventParams.battleEquipPar.equipInfo, true);
                this.SetSelectedEquipItem(enSelectedEquipOrigin.EquipBag, uiEvent.m_eventParams.battleEquipPar.equipInfo, uiEvent.m_srcWidget.get_transform(), uiEvent.m_eventParams.battleEquipPar.pos);
                this.m_selectedEquipOrigin = enSelectedEquipOrigin.EquipBag;
                if (this.m_isEquipTreeOpened)
                {
                    this.m_equipTree.Create(uiEvent.m_eventParams.battleEquipPar.equipInfo.m_equipID, this.m_equipInfoDictionary);
                    this.RefreshEquipTreePanel(false);
                }
                this.RefreshEquipFormRightPanel(false);
                this.EnableOpenEquipTreeButton(true);
            }
        }

        public void OnEquipBuyBtnClick(CUIEvent uiEvent)
        {
            CEquipInfo equipInfo = this.GetEquipInfo(uiEvent.m_eventParams.commonUInt16Param1);
            if (equipInfo != null)
            {
                if (equipInfo.m_resEquipInBattle.bUsage == 6)
                {
                    this.SendBuyHorizonEquipFrameCommand(equipInfo.m_equipID);
                }
                else
                {
                    this.SendBuyEquipFrameCommand(equipInfo.m_equipID, false);
                }
            }
            this.ClearSelectedEquipItem();
            this.CloseEquipFormRightPanel();
        }

        public void OnEquipFormClose(CUIEvent uiEvent)
        {
            this.SendInOutEquipShopFrameCommand(0);
            this.ClearSelectedEquipItem();
            this.m_equipFormScript = null;
            this.m_backEquipListScript = null;
            Singleton<CUIParticleSystem>.instance.Show(null);
        }

        public void OnEquipFormOpen(CUIEvent uiEvent)
        {
            this.m_equipFormScript = Singleton<CUIManager>.GetInstance().OpenForm(s_equipFormPath, true, true);
            this.SendInOutEquipShopFrameCommand(1);
            if (this.m_equipFormScript != null)
            {
                if (this.m_uiEquipItemHeight == 0f)
                {
                    GameObject obj2 = this.m_equipFormScript.GetWidget(10);
                    if (obj2 != null)
                    {
                        this.m_uiEquipItemHeight = (obj2.get_transform() as RectTransform).get_rect().get_height();
                    }
                }
                if (this.m_uiEquipItemContentDefaultHeight == 0f)
                {
                    GameObject obj3 = this.m_equipFormScript.GetWidget(11);
                    if (obj3 != null)
                    {
                        this.m_uiEquipItemContentDefaultHeight = (obj3.get_transform() as RectTransform).get_rect().get_height();
                    }
                }
                GameObject widget = this.m_equipFormScript.GetWidget(15);
                if (widget != null)
                {
                    this.m_backEquipListScript = widget.GetComponent<CUIListScript>();
                }
                this.InitEquipLevelPanel();
                this.InitBagEquipItemList();
                this.InitEquipPathLine();
                this.CloseEquipTreePanel();
                this.RefreshEquipBagPanel();
                this.RefreshGoldCoin();
                this.ClearSelectedEquipItem();
                this.CloseEquipFormRightPanel();
                GameObject listObj = this.m_equipFormScript.GetWidget(0);
                CTextManager instance = Singleton<CTextManager>.GetInstance();
                if (Singleton<BattleLogic>.instance.GetCurLvelContext().m_bEnableShopHorizonTab)
                {
                    string[] titleList = new string[] { instance.GetText("Equip_Usage_Recommend"), instance.GetText("Equip_Usage_PhyAttack"), instance.GetText("Equip_Usage_MagicAttack"), instance.GetText("Equip_Usage_Defence"), instance.GetText("Equip_Usage_Move"), instance.GetText("Equip_Usage_Jungle"), instance.GetText("Equip_Usage_Horizon") };
                    CUICommonSystem.InitMenuPanel(listObj, titleList, (int) this.m_curEquipUsage, true);
                }
                else
                {
                    string[] strArray2 = new string[] { instance.GetText("Equip_Usage_Recommend"), instance.GetText("Equip_Usage_PhyAttack"), instance.GetText("Equip_Usage_MagicAttack"), instance.GetText("Equip_Usage_Defence"), instance.GetText("Equip_Usage_Move"), instance.GetText("Equip_Usage_Jungle") };
                    CUICommonSystem.InitMenuPanel(listObj, strArray2, (int) this.m_curEquipUsage, true);
                }
                this.InitEquipActiveSkillTimerTxt();
                this.UpdateEquipActiveSkillCd(true);
            }
            Singleton<CUIParticleSystem>.instance.Hide(null);
        }

        public void OnEquipItemSelect(CUIEvent uiEvent)
        {
            if (((this.m_selectedEquipOrigin != enSelectedEquipOrigin.EquipLibaray) || (this.m_selectedEquipItems[(int) this.m_selectedEquipOrigin].m_equipInfo != uiEvent.m_eventParams.battleEquipPar.equipInfo)) || (this.m_selectedEquipItems[(int) this.m_selectedEquipOrigin].m_equipItemTransform != uiEvent.m_srcWidget.get_transform()))
            {
                this.ClearSelectedEquipItem(enSelectedEquipOrigin.EquipTree);
                this.ClearSelectedEquipItem(enSelectedEquipOrigin.EquipBag);
                this.m_selectedEquipOrigin = enSelectedEquipOrigin.EquipLibaray;
                this.SetFocusEquipInEquipLibrary(uiEvent.m_eventParams.battleEquipPar.equipInfo, false);
                this.SetSelectedEquipItem(enSelectedEquipOrigin.EquipLibaray, uiEvent.m_eventParams.battleEquipPar.equipInfo, uiEvent.m_srcWidget.get_transform(), -1);
                this.RefreshEquipFormRightPanel(false);
                this.EnableOpenEquipTreeButton(true);
            }
        }

        public void OnEquipSaleBtnClick(CUIEvent uiEvent)
        {
            this.SendSellEquipFrameCommand(uiEvent.m_eventParams.selectIndex);
            this.ClearSelectedEquipItem();
            this.CloseEquipFormRightPanel();
        }

        public void OnEquipTypeListSelect(CUIEvent uiEvent)
        {
            this.ClearSelectedEquipItem(enSelectedEquipOrigin.EquipLibaray);
            this.ClearSelectedEquipItem(enSelectedEquipOrigin.EquipBag);
            if ((this.m_selectedEquipOrigin == enSelectedEquipOrigin.EquipLibaray) || (this.m_selectedEquipOrigin == enSelectedEquipOrigin.EquipBag))
            {
                if (this.m_equipRelationPath != null)
                {
                    this.m_equipRelationPath.Reset();
                }
                this.EnableOpenEquipTreeButton(false);
                this.CloseEquipFormRightPanel();
            }
            CUIListScript srcWidgetScript = uiEvent.m_srcWidgetScript;
            if (srcWidgetScript != null)
            {
                this.m_curEquipUsage = (enEquipUsage) srcWidgetScript.GetSelectedIndex();
                this.RefreshEquipLibraryPanel(false);
            }
            if (this.m_equipFormScript != null)
            {
                this.m_equipFormScript.GetWidget(0x12).CustomSetActive(this.m_curEquipUsage == enEquipUsage.Horizon);
            }
        }

        private void OnHeroEquipInBattleChanged(uint actorObjectID, stEquipInfo[] equips, bool bIsAdd, int iEquipSlotIndex)
        {
            if (this.m_isEnabled)
            {
                if ((this.m_hostCtrlHero != 0) && (actorObjectID == this.m_hostCtrlHero.handle.ObjID))
                {
                    this.RefreshHostPlayerCachedEquipBuyPrice();
                    this.SetEquipChangeFlag(enEquipChangeFlag.EquipInBattleChanged);
                }
                this.HandleEquipActiveSkillWhenEquipChange(actorObjectID, bIsAdd, iEquipSlotIndex);
            }
        }

        private void OnHeroRecommendEquipInit(uint actorObjectID)
        {
            if (this.m_isEnabled && ((this.m_hostCtrlHero != 0) && (actorObjectID == this.m_hostCtrlHero.handle.ObjID)))
            {
                this.InitializeRecommendEquipList();
            }
        }

        private void OpenEquipTreePanel(CEquipInfo rootEquipInfo, bool selectRootItem)
        {
            if (this.m_equipFormScript != null)
            {
                GameObject widget = this.m_equipFormScript.GetWidget(12);
                if (widget != null)
                {
                    widget.CustomSetActive(false);
                }
                GameObject obj3 = this.m_equipFormScript.GetWidget(13);
                if (obj3 != null)
                {
                    obj3.CustomSetActive(true);
                }
                this.m_isEquipTreeOpened = true;
                this.m_equipTree.Create(rootEquipInfo.m_equipID, this.m_equipInfoDictionary);
                if (selectRootItem)
                {
                    Transform equipItemTransform = (obj3 == null) ? null : obj3.get_transform().Find("rootEquipItem");
                    this.SetSelectedEquipItem(enSelectedEquipOrigin.EquipTree, rootEquipInfo, equipItemTransform, -1);
                    this.m_selectedEquipOrigin = enSelectedEquipOrigin.EquipTree;
                }
                this.RefreshEquipTreePanel(false);
                this.RefreshEquipFormRightPanel(false);
            }
        }

        private void RefreshBackEquipList(bool onlyRefreshPriceAndOwned)
        {
            if (this.m_isEquipTreeOpened && (this.m_equipFormScript != null))
            {
                List<ushort> backEquipIDs = null;
                int amount = 0;
                if ((this.m_selectedEquipOrigin != enSelectedEquipOrigin.None) && (this.m_selectedEquipItems[(int) this.m_selectedEquipOrigin].m_equipInfo != null))
                {
                    backEquipIDs = this.m_selectedEquipItems[(int) this.m_selectedEquipOrigin].m_equipInfo.m_backEquipIDs;
                    amount = (backEquipIDs != null) ? backEquipIDs.Count : 0;
                }
                if (this.m_backEquipListScript != null)
                {
                    if (!onlyRefreshPriceAndOwned || (this.m_backEquipListScript.GetElementAmount() != amount))
                    {
                        this.m_backEquipListScript.SetElementAmount(amount);
                        this.m_backEquipListScript.SelectElement(-1, false);
                        this.m_backEquipListScript.ResetContentPosition();
                        GameObject widget = this.m_equipFormScript.GetWidget(0x10);
                        GameObject obj3 = this.m_equipFormScript.GetWidget(0x11);
                        if (!this.m_backEquipListScript.IsNeedScroll())
                        {
                            widget.CustomSetActive(false);
                            obj3.CustomSetActive(false);
                        }
                        else
                        {
                            widget.CustomSetActive(false);
                            obj3.CustomSetActive(true);
                        }
                    }
                    for (int i = 0; i < amount; i++)
                    {
                        this.RefreshEquipTreeItem(this.m_backEquipListScript.GetElemenet(i).get_transform(), backEquipIDs[i], onlyRefreshPriceAndOwned);
                    }
                }
            }
        }

        private void RefreshEquipBagPanel()
        {
            if ((this.m_equipFormScript.GetWidget(4) != null) && (this.m_hostCtrlHero != 0))
            {
                stEquipInfo[] equips = this.m_hostCtrlHero.handle.EquipComponent.GetEquips();
                for (int i = 0; i < 6; i++)
                {
                    Transform bagEquipItem = this.GetBagEquipItem(i);
                    if (null != bagEquipItem)
                    {
                        Image component = bagEquipItem.Find("imgIcon").GetComponent<Image>();
                        CUIMiniEventScript script = bagEquipItem.GetComponent<CUIMiniEventScript>();
                        if (equips[i].m_amount >= 1)
                        {
                            script.set_enabled(true);
                            component.get_gameObject().CustomSetActive(true);
                            CEquipInfo equipInfo = this.GetEquipInfo(equips[i].m_equipID);
                            if (equipInfo != null)
                            {
                                component.SetSprite(equipInfo.m_equipIconPath, this.m_equipFormScript, true, false, false, false);
                                if (this.IsActiveEquipButNotHorizon(equipInfo.m_equipID))
                                {
                                    Transform transform2 = bagEquipItem.FindChild("imgActiveEquip");
                                    if ((transform2 != null) && !transform2.get_gameObject().get_activeSelf())
                                    {
                                        transform2.get_gameObject().CustomSetActive(true);
                                    }
                                }
                                script.m_onClickEventID = enUIEventID.BattleEquip_BagItem_Select;
                                script.m_onClickEventParams.battleEquipPar.equipInfo = equipInfo;
                                script.m_onClickEventParams.battleEquipPar.pos = i;
                            }
                        }
                        else
                        {
                            script.set_enabled(false);
                            component.get_gameObject().CustomSetActive(false);
                            Transform transform3 = bagEquipItem.FindChild("imgActiveEquip");
                            if ((transform3 != null) && transform3.get_gameObject().get_activeSelf())
                            {
                                transform3.get_gameObject().CustomSetActive(false);
                            }
                        }
                    }
                }
                SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                GameObject widget = this.m_equipFormScript.GetWidget(0x13);
                if ((curLvelContext != null) && (widget != null))
                {
                    if (curLvelContext.m_bEnableOrnamentSlot && curLvelContext.m_bEnableShopHorizonTab)
                    {
                        widget.CustomSetActive(true);
                        this.RefreshHorizonEquipGrid(widget);
                    }
                    else
                    {
                        widget.CustomSetActive(false);
                    }
                }
            }
        }

        private void RefreshEquipBuyPanel(CEquipInfo equipInfo, Transform buyPanel)
        {
            if ((null != buyPanel) && (equipInfo != null))
            {
                CEquipBuyPrice freeEquipBuyPrice = this.GetFreeEquipBuyPrice();
                bool flag = false;
                if (equipInfo.m_resEquipInBattle.bUsage == 6)
                {
                    int price = 0;
                    flag = this.IsHorizonEquipCanBought(equipInfo.m_equipID, ref this.m_hostCtrlHero, ref price);
                }
                else
                {
                    flag = this.IsEquipCanBought(equipInfo.m_equipID, ref this.m_hostCtrlHero, ref freeEquipBuyPrice);
                }
                buyPanel.Find("buyPriceText").GetComponent<Text>().set_text(freeEquipBuyPrice.m_buyPrice.ToString());
                Button component = buyPanel.Find("buyBtn").GetComponent<Button>();
                CUICommonSystem.SetButtonEnableWithShader(component, flag && this.m_hostCtrlHeroPermitedToBuyEquip, true);
                CUIEventScript script = buyPanel.Find("buyBtn").GetComponent<CUIEventScript>();
                if (flag && this.m_hostCtrlHeroPermitedToBuyEquip)
                {
                    stUIEventParams eventParams = new stUIEventParams();
                    eventParams.commonUInt16Param1 = equipInfo.m_equipID;
                    script.SetUIEvent(enUIEventType.Click, enUIEventID.BattleEquip_BuyBtn_Click, eventParams);
                }
                GameObject obj2 = component.get_transform().FindChild("Text").get_gameObject();
                GameObject obj3 = component.get_transform().FindChild("CantBuyText").get_gameObject();
                obj2.CustomSetActive(this.m_hostCtrlHeroPermitedToBuyEquip);
                obj3.CustomSetActive(!this.m_hostCtrlHeroPermitedToBuyEquip);
                freeEquipBuyPrice.m_used = false;
            }
        }

        private void RefreshEquipFormRightPanel(bool onlyRefreshPrice)
        {
            if (this.m_equipFormScript != null)
            {
                if ((this.m_selectedEquipOrigin == enSelectedEquipOrigin.None) || (this.m_selectedEquipItems[(int) this.m_selectedEquipOrigin].m_equipInfo == null))
                {
                    this.CloseEquipFormRightPanel();
                }
                else
                {
                    CEquipInfo equipInfo = this.m_selectedEquipItems[(int) this.m_selectedEquipOrigin].m_equipInfo;
                    if (!onlyRefreshPrice)
                    {
                        GameObject widget = this.m_equipFormScript.GetWidget(5);
                        widget.CustomSetActive(true);
                        this.RefreshEquipInfoPanel(equipInfo, widget.get_transform());
                    }
                    if (this.m_selectedEquipOrigin == enSelectedEquipOrigin.EquipBag)
                    {
                        this.m_equipFormScript.GetWidget(6).CustomSetActive(false);
                        GameObject obj4 = this.m_equipFormScript.GetWidget(7);
                        obj4.CustomSetActive(true);
                        if (!onlyRefreshPrice)
                        {
                            this.RefreshEquipSalePanel(equipInfo, this.m_selectedEquipItems[(int) this.m_selectedEquipOrigin].m_positionInBag, obj4.get_transform());
                        }
                    }
                    else
                    {
                        this.m_equipFormScript.GetWidget(7).CustomSetActive(false);
                        GameObject obj6 = this.m_equipFormScript.GetWidget(6);
                        obj6.CustomSetActive(true);
                        this.RefreshEquipBuyPanel(equipInfo, obj6.get_transform());
                    }
                }
            }
        }

        private void RefreshEquipInfoPanel(CEquipInfo equipInfo, Transform infoPanel)
        {
            if ((null != infoPanel) && (equipInfo != null))
            {
                infoPanel.Find("equipNameText").GetComponent<Text>().set_text(equipInfo.m_equipName);
                Text component = infoPanel.Find("Panel_equipPropertyDesc/equipPropertyDescText").GetComponent<Text>();
                component.set_text(equipInfo.m_equipPropertyDesc);
                (component.get_transform() as RectTransform).set_anchoredPosition(new Vector2(0f, 0f));
            }
        }

        private void RefreshEquipItem(Transform equipItem, ushort equipID, bool onlyRefreshPriceAndOwned)
        {
            if (null != equipItem)
            {
                CEquipInfo equipInfo = this.GetEquipInfo(equipID);
                if (equipInfo == null)
                {
                    object[] inParameters = new object[] { equipID };
                    DebugHelper.Assert(equipInfo != null, "GetEquipInfo is null equipId = {0}", inParameters);
                }
                else if (this.m_hostCtrlHero == 0)
                {
                    DebugHelper.Assert(this.m_hostCtrlHero == 1, " RefreshEquipItem m_hostCtrlHero is null");
                }
                else if (equipInfo.m_resEquipInBattle != null)
                {
                    Image component = equipItem.Find("imgIcon").GetComponent<Image>();
                    if (!onlyRefreshPriceAndOwned)
                    {
                        component.SetSprite(equipInfo.m_equipIconPath, this.m_equipFormScript, true, false, false, false);
                    }
                    CEquipBuyPrice freeEquipBuyPrice = this.GetFreeEquipBuyPrice();
                    bool bActive = false;
                    bool flag2 = false;
                    Text text = equipItem.Find("priceText").GetComponent<Text>();
                    if (equipInfo.m_resEquipInBattle.bUsage == 6)
                    {
                        int price = 0;
                        flag2 = this.IsHorizonEquipCanBought(equipInfo.m_equipID, ref this.m_hostCtrlHero, ref price);
                        bActive = this.IsHorizonEquipOwn(equipInfo.m_equipID, ref this.m_hostCtrlHero);
                        text.set_text(price.ToString());
                    }
                    else
                    {
                        flag2 = this.IsEquipCanBought(equipID, ref this.m_hostCtrlHero, ref freeEquipBuyPrice);
                        bActive = this.m_hostCtrlHero.handle.EquipComponent.HasEquip(equipInfo.m_equipID, 1);
                        text.set_text(freeEquipBuyPrice.m_buyPrice.ToUintString());
                    }
                    component.set_color((!flag2 && !bActive) ? CUIUtility.s_Color_GrayShader : CUIUtility.s_Color_White);
                    Transform transform = equipItem.Find("imgOwn");
                    if (transform != null)
                    {
                        transform.get_gameObject().CustomSetActive(bActive);
                    }
                    if (!onlyRefreshPriceAndOwned)
                    {
                        equipItem.Find("nameText").GetComponent<Text>().set_text(equipInfo.m_equipName);
                        CUIMiniEventScript script = equipItem.GetComponent<CUIMiniEventScript>();
                        if (script != null)
                        {
                            script.m_onClickEventID = enUIEventID.BattleEquip_Item_Select;
                            script.m_onClickEventParams.battleEquipPar.equipInfo = equipInfo;
                        }
                    }
                    Transform transform2 = equipItem.FindChild("imgActiveEquip");
                    Transform transform3 = equipItem.FindChild("imgEyeEquip");
                    if ((transform3 != null) && (transform2 != null))
                    {
                        if (equipInfo.m_resEquipInBattle.dwActiveSkillID > 0)
                        {
                            if (equipInfo.m_resEquipInBattle.bUsage == 6)
                            {
                                transform3.get_gameObject().CustomSetActive(true);
                            }
                            else
                            {
                                transform2.get_gameObject().CustomSetActive(true);
                            }
                        }
                        else
                        {
                            transform2.get_gameObject().CustomSetActive(false);
                            transform3.get_gameObject().CustomSetActive(false);
                        }
                    }
                    freeEquipBuyPrice.m_used = false;
                }
            }
        }

        private void RefreshEquipLevelPanel(Transform equipPanel, int level, bool onlyRefreshPriceAndOwned)
        {
            if (null != equipPanel)
            {
                List<ushort> list = this.m_equipList[(int) this.m_curEquipUsage][level - 1];
                int count = list.Count;
                int index = 0;
                for (index = 0; index < 12; index++)
                {
                    Transform equipItem = this.GetEquipItem(level, index);
                    if (equipItem != null)
                    {
                        if (index < count)
                        {
                            if (!onlyRefreshPriceAndOwned)
                            {
                                equipItem.get_gameObject().CustomSetActive(true);
                            }
                            this.RefreshEquipItem(equipItem, list[index], onlyRefreshPriceAndOwned);
                        }
                        else if (!onlyRefreshPriceAndOwned)
                        {
                            equipItem.get_gameObject().CustomSetActive(false);
                        }
                    }
                }
            }
        }

        private void RefreshEquipLibraryPanel(bool onlyRefreshPriceAndOwned)
        {
            if ((this.m_equipFormScript != null) && (this.m_hostCtrlHero != 0))
            {
                GameObject widget = this.m_equipFormScript.GetWidget(1);
                if (widget != null)
                {
                    this.RefreshEquipLevelPanel(widget.get_transform(), 1, onlyRefreshPriceAndOwned);
                }
                GameObject obj3 = this.m_equipFormScript.GetWidget(2);
                if (obj3 != null)
                {
                    this.RefreshEquipLevelPanel(obj3.get_transform(), 2, onlyRefreshPriceAndOwned);
                }
                GameObject obj4 = this.m_equipFormScript.GetWidget(3);
                if (obj4 != null)
                {
                    this.RefreshEquipLevelPanel(obj4.get_transform(), 3, onlyRefreshPriceAndOwned);
                }
                if (!onlyRefreshPriceAndOwned && (this.m_equipList != null))
                {
                    int count = 0;
                    List<ushort>[] listArray = this.m_equipList[(int) this.m_curEquipUsage];
                    if (listArray != null)
                    {
                        for (int i = 0; i < listArray.Length; i++)
                        {
                            if (listArray[i].Count > count)
                            {
                                count = listArray[i].Count;
                            }
                        }
                        float num3 = this.m_uiEquipItemContentDefaultHeight - ((12 - count) * this.m_uiEquipItemHeight);
                        GameObject obj5 = this.m_equipFormScript.GetWidget(11);
                        if (obj5 != null)
                        {
                            RectTransform transform = obj5.get_transform() as RectTransform;
                            transform.set_offsetMin(new Vector2(0f, -num3));
                            transform.set_offsetMax(new Vector2(0f, 0f));
                        }
                    }
                }
            }
        }

        private void RefreshEquipSalePanel(CEquipInfo equipInfo, int positionInBag, Transform salePanel)
        {
            if ((null != salePanel) && (equipInfo != null))
            {
                salePanel.Find("salePriceText").GetComponent<Text>().set_text(((uint) equipInfo.m_resEquipInBattle.dwSalePrice).ToString());
                stUIEventParams eventParams = new stUIEventParams();
                eventParams.selectIndex = positionInBag;
                CUICommonSystem.SetButtonEnableWithShader(salePanel.Find("saleBtn").GetComponent<Button>(), (positionInBag != 6) && this.m_hostCtrlHeroPermitedToBuyEquip, true);
                if ((positionInBag != 6) && this.m_hostCtrlHeroPermitedToBuyEquip)
                {
                    salePanel.Find("saleBtn").GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, enUIEventID.BattleEquip_SaleBtn_Click, eventParams);
                }
            }
        }

        private void RefreshEquipTreeItem(Transform equipItemTransform, ushort equipID, bool onlyRefreshPriceAndOwned)
        {
            if ((equipItemTransform != null) && (equipID != 0))
            {
                CEquipInfo equipInfo = this.GetEquipInfo(equipID);
                if ((equipInfo == null) || (equipInfo.m_resEquipInBattle == null))
                {
                    DebugHelper.Assert(equipInfo != null, "GetEquipInfo is null equipId = " + equipID);
                }
                else
                {
                    Transform transform = equipItemTransform.Find("imgIcon");
                    Image image = (transform == null) ? null : transform.get_gameObject().GetComponent<Image>();
                    if (!onlyRefreshPriceAndOwned && (image != null))
                    {
                        image.SetSprite(equipInfo.m_equipIconPath, this.m_equipFormScript, true, false, false, false);
                    }
                    int buyPrice = 0;
                    bool bActive = false;
                    bool flag2 = false;
                    if (equipInfo.m_resEquipInBattle.bUsage == 6)
                    {
                        flag2 = this.IsHorizonEquipCanBought(equipInfo.m_equipID, ref this.m_hostCtrlHero, ref buyPrice);
                        bActive = this.IsHorizonEquipOwn(equipInfo.m_equipID, ref this.m_hostCtrlHero);
                    }
                    else
                    {
                        CEquipBuyPrice freeEquipBuyPrice = this.GetFreeEquipBuyPrice();
                        flag2 = this.IsEquipCanBought(equipID, ref this.m_hostCtrlHero, ref freeEquipBuyPrice);
                        bActive = this.m_hostCtrlHero.handle.EquipComponent.HasEquip(equipInfo.m_equipID, 1);
                        buyPrice = (int) freeEquipBuyPrice.m_buyPrice;
                        freeEquipBuyPrice.m_used = false;
                    }
                    Transform transform2 = equipItemTransform.Find("Price");
                    if (transform2 != null)
                    {
                        Text component = transform2.get_gameObject().GetComponent<Text>();
                        if (component != null)
                        {
                            component.set_text(buyPrice.ToString());
                        }
                    }
                    if (image != null)
                    {
                        image.set_color((!flag2 && !bActive) ? CUIUtility.s_Color_GrayShader : CUIUtility.s_Color_White);
                    }
                    Transform transform3 = equipItemTransform.Find("imgOwn");
                    if (transform3 != null)
                    {
                        transform3.get_gameObject().CustomSetActive(bActive);
                    }
                    if (!onlyRefreshPriceAndOwned)
                    {
                        CUIMiniEventScript script = equipItemTransform.get_gameObject().GetComponent<CUIMiniEventScript>();
                        if (script != null)
                        {
                            script.m_onClickEventParams.commonUInt16Param1 = equipID;
                        }
                    }
                }
            }
        }

        private void RefreshEquipTreePanel(bool onlyRefreshPriceAndOwned)
        {
            if (this.m_isEquipTreeOpened && (this.m_equipFormScript != null))
            {
                GameObject widget = this.m_equipFormScript.GetWidget(13);
                if (widget != null)
                {
                    Transform equipItemTransform = widget.get_transform().Find("rootEquipItem");
                    this.RefreshEquipTreeItem(equipItemTransform, this.m_equipTree.m_rootEquipID, onlyRefreshPriceAndOwned);
                    Transform lineGroupPanel = widget.get_transform().Find("lineGroupPanel");
                    this.RefreshLineGroupPanel(lineGroupPanel, 3, (int) this.m_equipTree.m_2ndEquipCount);
                    Transform transform3 = widget.get_transform().Find("preEquipGroupPanel");
                    if (null != transform3)
                    {
                        ushort equipID = 0;
                        for (int i = 0; i < 3; i++)
                        {
                            equipID = this.m_equipTree.m_2ndEquipIDs[i];
                            Transform transform4 = transform3.Find("preEquipGroup" + i);
                            if (transform4 != null)
                            {
                                transform4.get_gameObject().CustomSetActive(equipID > 0);
                                if (equipID > 0)
                                {
                                    Transform transform5 = transform4.Find("2ndEquipItem");
                                    this.RefreshEquipTreeItem(transform5, equipID, onlyRefreshPriceAndOwned);
                                    lineGroupPanel = transform4.get_transform().Find("lineGroupPanel");
                                    this.RefreshLineGroupPanel(lineGroupPanel, 2, (int) this.m_equipTree.m_3rdEquipCounts[i]);
                                    ushort num3 = 0;
                                    for (int j = 0; j < 2; j++)
                                    {
                                        num3 = this.m_equipTree.m_3rdEquipIDs[i][j];
                                        Transform transform6 = transform4.Find("preEquipPanel/3rdEquipItem" + j);
                                        transform6.get_gameObject().CustomSetActive(num3 > 0);
                                        this.RefreshEquipTreeItem(transform6, num3, onlyRefreshPriceAndOwned);
                                    }
                                }
                            }
                        }
                        this.RefreshBackEquipList(onlyRefreshPriceAndOwned);
                    }
                }
            }
        }

        private void RefreshGoldCoin()
        {
            if ((this.m_equipFormScript != null) && (this.m_hostCtrlHero != 0))
            {
                GameObject widget = this.m_equipFormScript.GetWidget(9);
                if (widget != null)
                {
                    widget.GetComponent<Text>().set_text(this.m_hostCtrlHero.handle.ValueComponent.GetGoldCoinInBattle().ToString());
                }
            }
        }

        private void RefreshHorizonEquipGrid(GameObject horizonEquipGrid)
        {
            if ((null != horizonEquipGrid) && (this.m_hostCtrlHero != 0))
            {
                Transform transform = horizonEquipGrid.get_transform();
                Image component = transform.Find("imgIcon").GetComponent<Image>();
                CUIMiniEventScript script = transform.GetComponent<CUIMiniEventScript>();
                CEquipInfo equipInfo = this.GetEquipInfo(this.m_hostCtrlHero.handle.EquipComponent.m_horizonEquipId);
                if (equipInfo != null)
                {
                    component.get_gameObject().CustomSetActive(true);
                    component.SetSprite(equipInfo.m_equipIconPath, this.m_equipFormScript, true, false, false, false);
                    script.set_enabled(true);
                    script.m_onClickEventID = enUIEventID.BattleEquip_BagItem_Select;
                    script.m_onClickEventParams.battleEquipPar.equipInfo = equipInfo;
                    script.m_onClickEventParams.battleEquipPar.pos = 6;
                }
                else
                {
                    component.get_gameObject().CustomSetActive(false);
                    script.set_enabled(false);
                }
            }
        }

        private void RefreshHostPlayerCachedEquipBuyPrice()
        {
            Dictionary<ushort, CEquipInfo>.Enumerator enumerator = this.m_equipInfoDictionary.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<ushort, CEquipInfo> current = enumerator.Current;
                ushort key = current.Key;
                CEquipBuyPrice price = null;
                if (this.m_hostPlayerCachedEquipBuyPrice.TryGetValue(key, out price))
                {
                    this.GetEquipBuyPrice(key, ref this.m_hostCtrlHero, ref price);
                }
                else
                {
                    price = new CEquipBuyPrice(0);
                    this.GetEquipBuyPrice(key, ref this.m_hostCtrlHero, ref price);
                    this.m_hostPlayerCachedEquipBuyPrice.Add(key, price);
                }
            }
        }

        private void RefreshHostPlayerQuicklyBuyEquipList()
        {
            if (this.m_hostCtrlHero != 0)
            {
                ushort[] quicklyBuyEquipList = this.GetQuicklyBuyEquipList(ref this.m_hostCtrlHero);
                bool isQuicklyBuyEquipListChanged = false;
                for (int i = 0; i < 2L; i++)
                {
                    if (this.m_hostPlayerQuicklyBuyEquipIDs[i] != quicklyBuyEquipList[i])
                    {
                        this.m_hostPlayerQuicklyBuyEquipIDs[i] = quicklyBuyEquipList[i];
                        if (!isQuicklyBuyEquipListChanged)
                        {
                            isQuicklyBuyEquipListChanged = true;
                        }
                    }
                }
                if (isQuicklyBuyEquipListChanged)
                {
                    this.RefreshQuicklyBuyPanel(isQuicklyBuyEquipListChanged);
                }
            }
        }

        private void RefreshHostPlayerQuicklyBuyEquipPanel(bool hostCtrlHeroPermitedToBuyEquip)
        {
            CUIFormScript fightFormScript = Singleton<CBattleSystem>.GetInstance().FightFormScript;
            if (fightFormScript != null)
            {
                GameObject widget = fightFormScript.GetWidget(0x3a);
                if (widget != null)
                {
                    Image component = widget.GetComponent<Image>();
                    if (component != null)
                    {
                        component.set_color(new Color(!hostCtrlHeroPermitedToBuyEquip ? ((float) 0) : ((float) 1), 1f, 1f, 1f));
                    }
                }
                GameObject obj3 = fightFormScript.GetWidget(0x3b);
                if (obj3 != null)
                {
                    obj3.CustomSetActive(hostCtrlHeroPermitedToBuyEquip);
                }
            }
        }

        private void RefreshLineGroupPanel(Transform lineGroupPanel, int maxLineCnt, int curLineCnt)
        {
            if (null != lineGroupPanel)
            {
                for (int i = 0; i < maxLineCnt; i++)
                {
                    Transform transform = lineGroupPanel.Find("linePanel" + i);
                    if (transform != null)
                    {
                        transform.get_gameObject().CustomSetActive((i + 1) == curLineCnt);
                    }
                }
            }
        }

        private void RefreshQuicklyBuyPanel(bool isQuicklyBuyEquipListChanged)
        {
            if (null != this.m_battleFormScript)
            {
                for (int i = 0; i < this.m_hostPlayerQuicklyBuyEquipIDs.Length; i++)
                {
                    ushort equipID = this.m_hostPlayerQuicklyBuyEquipIDs[i];
                    GameObject widget = this.m_battleFormScript.GetWidget(0x2f + i);
                    if (widget != null)
                    {
                        CEquipInfo equipInfo = this.GetEquipInfo(equipID);
                        if (equipInfo != null)
                        {
                            widget.CustomSetActive(true);
                            Transform transform = widget.get_transform();
                            transform.Find("imgIcon").GetComponent<Image>().SetSprite(equipInfo.m_equipIconPath, this.m_battleFormScript, true, false, false, false);
                            transform.Find("Panel_Info/descText").GetComponent<Text>().set_text(string.Format("<color=#ffa500>{0}</color>\n{1}", equipInfo.m_equipName, equipInfo.m_equipDesc));
                            uint buyPrice = (uint) this.GetHostPlayerCachedEquipBuyPrice(equipInfo.m_equipID).m_buyPrice;
                            transform.Find("moneyText").GetComponent<Text>().set_text(buyPrice.ToString());
                            CUIMiniEventScript component = widget.GetComponent<CUIMiniEventScript>();
                            stUIEventParams eventParams = new stUIEventParams();
                            eventParams.battleEquipPar.equipInfo = equipInfo;
                            eventParams.battleEquipPar.m_indexInQuicklyBuyList = i;
                            component.SetUIEvent(enUIEventType.Click, enUIEventID.BattleEquip_RecommendEquip_Buy, eventParams);
                        }
                        else
                        {
                            widget.CustomSetActive(false);
                        }
                    }
                }
                if (isQuicklyBuyEquipListChanged)
                {
                    this.m_tickFadeTime = 0x2710;
                    this.m_bPlayAnimation = false;
                    this.SetQuicklyBuyInfoPanelAlpha(1f);
                }
            }
        }

        private void SellEquip(int equipIndexInGrid, ref PoolObjHandle<ActorRoot> actor)
        {
            if ((this.m_isEnabled && (equipIndexInGrid >= 0)) && (((equipIndexInGrid < 6) && (actor != 0)) && actor.handle.EquipComponent.IsPermitedToBuyEquip(this.m_isInEquipLimitedLevel)))
            {
                stEquipInfo[] equips = actor.handle.EquipComponent.GetEquips();
                if ((equips[equipIndexInGrid].m_equipID > 0) && (equips[equipIndexInGrid].m_amount > 0))
                {
                    ResEquipInBattle dataByKey = GameDataMgr.m_equipInBattleDatabin.GetDataByKey((uint) equips[equipIndexInGrid].m_equipID);
                    if (dataByKey != null)
                    {
                        actor.handle.EquipComponent.RemoveEquip(equipIndexInGrid);
                        actor.handle.EquipComponent.UpdateEquipEffect();
                        actor.handle.ValueComponent.ChangeGoldCoinInBattle((int) dataByKey.dwSalePrice, false, false, new Vector3(), false, new PoolObjHandle<ActorRoot>());
                    }
                }
            }
        }

        public void SendBuyEquipFrameCommand(ushort equipID, bool isQuicklyBuy)
        {
            FrameCommand<PlayerBuyEquipCommand> command = FrameCommandFactory.CreateFrameCommand<PlayerBuyEquipCommand>();
            command.cmdData.m_equipID = equipID;
            command.Send();
            if (isQuicklyBuy)
            {
                Singleton<CSoundManager>.GetInstance().PostEvent("UI_junei_ani_goumai", null);
                if (this.m_hostCtrlHero != 0)
                {
                    this.m_hostCtrlHero.handle.EquipComponent.m_iFastBuyEquipCount++;
                }
            }
            else if (this.m_hostCtrlHero != 0)
            {
                this.m_hostCtrlHero.handle.EquipComponent.m_iBuyEquipCount++;
            }
        }

        public void SendBuyHorizonEquipFrameCommand(ushort equipID)
        {
            FrameCommand<PlayerBuyHorizonEquipCommand> command = FrameCommandFactory.CreateFrameCommand<PlayerBuyHorizonEquipCommand>();
            command.cmdData.m_equipID = equipID;
            command.Send();
        }

        public void SendInOutEquipShopFrameCommand(byte inOut)
        {
            FrameCommand<PlayerInOutEquipShopCommand> command = FrameCommandFactory.CreateFrameCommand<PlayerInOutEquipShopCommand>();
            command.cmdData.m_inOut = inOut;
            command.Send();
        }

        public void SendSellEquipFrameCommand(int equipIndexInGrid)
        {
            FrameCommand<PlayerSellEquipCommand> command = FrameCommandFactory.CreateFrameCommand<PlayerSellEquipCommand>();
            command.cmdData.m_equipIndex = equipIndexInGrid;
            command.Send();
        }

        private void SetEquipActiveSkillCdState(int iEquipSlotIndex, int icd)
        {
            if (((iEquipSlotIndex >= 0) && (iEquipSlotIndex <= 6)) && (this.m_objEquipActiveTimerTxt[iEquipSlotIndex] != null))
            {
                if (icd > 0)
                {
                    Text component = this.m_objEquipActiveTimerTxt[iEquipSlotIndex].GetComponent<Text>();
                    if (component != null)
                    {
                        component.set_text(SimpleNumericString.GetNumeric(Mathf.CeilToInt((float) (icd / 0x3e8)) + 1));
                    }
                    if (!this.m_objEquipActiveTimerTxt[iEquipSlotIndex].get_transform().get_parent().get_gameObject().get_activeSelf())
                    {
                        this.m_objEquipActiveTimerTxt[iEquipSlotIndex].get_transform().get_parent().get_gameObject().CustomSetActive(true);
                    }
                }
                else if (this.m_objEquipActiveTimerTxt[iEquipSlotIndex].get_transform().get_parent().get_gameObject().get_activeSelf())
                {
                    this.m_objEquipActiveTimerTxt[iEquipSlotIndex].get_transform().get_parent().get_gameObject().CustomSetActive(false);
                }
            }
        }

        private void SetEquipChangeFlag(enEquipChangeFlag flag)
        {
            this.m_equipChangedFlags = (uint) (((enEquipChangeFlag) this.m_equipChangedFlags) | flag);
        }

        private void SetEquipItemSelectFlag(Transform equipItemObj, bool bSelect, enSelectedEquipOrigin equipSelectedOrigin, bool bIsActiveSKillEquip)
        {
            if (equipItemObj != null)
            {
                if (equipSelectedOrigin != enSelectedEquipOrigin.EquipBag)
                {
                    Transform transform = equipItemObj.Find("selectImg");
                    if (transform != null)
                    {
                        transform.get_gameObject().CustomSetActive(bSelect);
                    }
                }
                else
                {
                    Transform transform2 = null;
                    if (bIsActiveSKillEquip)
                    {
                        if (equipItemObj.FindChild("Beingused").get_gameObject().get_activeSelf())
                        {
                            transform2 = equipItemObj.Find("selectImg");
                        }
                        else
                        {
                            transform2 = equipItemObj.Find("SelectImg_InBattle");
                            Transform transform4 = equipItemObj.FindChild("BtnShowInBattle");
                            if ((transform4 != null) && (transform4.get_gameObject().get_activeSelf() != bSelect))
                            {
                                transform4.get_gameObject().CustomSetActive(bSelect);
                            }
                        }
                    }
                    else
                    {
                        transform2 = equipItemObj.Find("selectImg");
                    }
                    if (transform2 != null)
                    {
                        transform2.get_gameObject().CustomSetActive(bSelect);
                    }
                }
            }
        }

        private void SetEquipSkillBeUsingBtnState(bool bIsBtnShow, int iEquipSlotIndex)
        {
            if (this.m_bagEquipItemList[iEquipSlotIndex] != null)
            {
                Transform transform = this.m_bagEquipItemList[iEquipSlotIndex].FindChild("Beingused");
                if ((transform != null) && (transform.get_gameObject().get_activeSelf() != bIsBtnShow))
                {
                    transform.get_gameObject().CustomSetActive(bIsBtnShow);
                }
            }
        }

        private void SetEquipSkillBtnState(bool bIsShow, SkillSlotType slot)
        {
            if ((Singleton<CBattleSystem>.GetInstance() != null) && (Singleton<CBattleSystem>.GetInstance().FightForm != null))
            {
                CSkillButtonManager skillButtonManager = Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager;
                if (skillButtonManager != null)
                {
                    SkillButton button = skillButtonManager.GetButton(slot);
                    if (((button != null) && (button.m_button != null)) && (button.m_button.get_activeSelf() != bIsShow))
                    {
                        button.m_button.CustomSetActive(bIsShow);
                        if ((!bIsShow && (button.m_cdText != null)) && (button.m_cdText.get_activeSelf() != bIsShow))
                        {
                            button.m_cdText.CustomSetActive(bIsShow);
                        }
                    }
                }
            }
        }

        private void SetEquipSkillShowBtnState(bool bIsBtnShow, int iEquipSlotIndex)
        {
            if (this.m_bagEquipItemList[iEquipSlotIndex] != null)
            {
                Transform transform = this.m_bagEquipItemList[iEquipSlotIndex].FindChild("SelectImg_InBattle");
                if ((transform != null) && (transform.get_gameObject().get_activeSelf() != bIsBtnShow))
                {
                    transform.get_gameObject().CustomSetActive(bIsBtnShow);
                }
                Transform transform2 = this.m_bagEquipItemList[iEquipSlotIndex].FindChild("BtnShowInBattle");
                if ((transform2 != null) && (transform2.get_gameObject().get_activeSelf() != bIsBtnShow))
                {
                    transform2.get_gameObject().CustomSetActive(bIsBtnShow);
                }
            }
        }

        private void SetFocusEquipInEquipLibrary(CEquipInfo equipInfo, bool switchUsage)
        {
            if ((equipInfo != null) && (this.m_equipFormScript != null))
            {
                if (switchUsage && (((enEquipUsage) equipInfo.m_resEquipInBattle.bUsage) != this.m_curEquipUsage))
                {
                    GameObject widget = this.m_equipFormScript.GetWidget(0);
                    if (widget != null)
                    {
                        CUIListScript component = widget.GetComponent<CUIListScript>();
                        if (component != null)
                        {
                            component.SelectElement(equipInfo.m_resEquipInBattle.bUsage, true);
                        }
                    }
                }
                this.m_equipRelationPath.Display(equipInfo.m_equipID, this.m_equipList[(int) this.m_curEquipUsage], this.m_equipInfoDictionary);
            }
        }

        private void SetQuicklyBuyInfoPanelAlpha(float alphaValue)
        {
            if (null != this.m_battleFormScript)
            {
                for (int i = 0; i < 2L; i++)
                {
                    GameObject widget = this.m_battleFormScript.GetWidget(0x2f + i);
                    if ((widget != null) && widget.get_activeSelf())
                    {
                        Transform transform = widget.get_transform().Find("Panel_Info");
                        if (transform != null)
                        {
                            CanvasGroup component = transform.GetComponent<CanvasGroup>();
                            if (component != null)
                            {
                                component.set_alpha(alphaValue);
                            }
                        }
                    }
                }
            }
        }

        private void SetSelectedEquipItem(enSelectedEquipOrigin equipSelectedOrigin, CEquipInfo equipInfo, Transform equipItemTransform, int positionInBag)
        {
            Transform equipItemObj = this.m_selectedEquipItems[(int) equipSelectedOrigin].m_equipItemTransform;
            bool bIsActiveSKillEquip = false;
            if (((this.m_selectedEquipItems[(int) equipSelectedOrigin].m_equipInfo != null) && (this.m_selectedEquipItems[(int) equipSelectedOrigin].m_equipInfo.m_resEquipInBattle != null)) && (this.m_selectedEquipItems[(int) equipSelectedOrigin].m_equipInfo.m_resEquipInBattle.dwActiveSkillID > 0))
            {
                bIsActiveSKillEquip = true;
            }
            this.m_selectedEquipItems[(int) equipSelectedOrigin].m_equipInfo = equipInfo;
            this.m_selectedEquipItems[(int) equipSelectedOrigin].m_equipItemTransform = equipItemTransform;
            this.m_selectedEquipItems[(int) equipSelectedOrigin].m_positionInBag = positionInBag;
            bool flag2 = this.IsActiveEquipButNotHorizon(equipInfo.m_equipID);
            if (equipItemObj != equipItemTransform)
            {
                if (equipItemObj != null)
                {
                    this.SetEquipItemSelectFlag(equipItemObj, false, equipSelectedOrigin, bIsActiveSKillEquip);
                }
                if (equipItemTransform != null)
                {
                    this.SetEquipItemSelectFlag(equipItemTransform, true, equipSelectedOrigin, flag2);
                }
            }
        }

        public void Update()
        {
            if (this.m_isEnabled)
            {
                if (this.HasEquipChangeFlag(enEquipChangeFlag.EquipInBattleChanged) || this.HasEquipChangeFlag(enEquipChangeFlag.GoldCoinInBattleChanged))
                {
                    if (this.m_hostCtrlHeroPermitedToBuyEquip)
                    {
                        this.RefreshHostPlayerQuicklyBuyEquipList();
                    }
                    if (this.m_equipFormScript != null)
                    {
                        if (this.m_isEquipTreeOpened)
                        {
                            this.RefreshEquipTreePanel(true);
                        }
                        else
                        {
                            this.RefreshEquipLibraryPanel(true);
                        }
                        this.RefreshEquipFormRightPanel(true);
                        if (this.HasEquipChangeFlag(enEquipChangeFlag.EquipInBattleChanged))
                        {
                            this.RefreshEquipBagPanel();
                        }
                        if (this.HasEquipChangeFlag(enEquipChangeFlag.GoldCoinInBattleChanged))
                        {
                            this.RefreshGoldCoin();
                        }
                    }
                }
                this.ClearEquipChangeFlag();
                if (this.m_hostCtrlHero != 0)
                {
                    bool flag = this.m_hostCtrlHero.handle.EquipComponent.IsPermitedToBuyEquip(this.m_isInEquipLimitedLevel);
                    if (this.m_hostCtrlHeroPermitedToBuyEquip != flag)
                    {
                        this.m_hostCtrlHeroPermitedToBuyEquip = flag;
                        if (this.m_hostCtrlHeroPermitedToBuyEquip)
                        {
                            this.RefreshHostPlayerQuicklyBuyEquipList();
                        }
                        this.RefreshHostPlayerQuicklyBuyEquipPanel(this.m_hostCtrlHeroPermitedToBuyEquip);
                        if (((this.m_equipFormScript != null) && (this.m_selectedEquipOrigin != enSelectedEquipOrigin.None)) && (this.m_selectedEquipOrigin != enSelectedEquipOrigin.EquipBag))
                        {
                            this.RefreshEquipBuyPanel(this.m_selectedEquipItems[(int) this.m_selectedEquipOrigin].m_equipInfo, this.m_equipFormScript.GetWidget(6).get_transform());
                        }
                    }
                }
                if (this.m_equipFormScript != null)
                {
                    this.UpdateEquipActiveSkillCd(false);
                }
            }
        }

        private void UpdateEquipActiveSkillCd(bool bIsOpenForm = false)
        {
            if (this.m_hostCtrlHero != 0)
            {
                for (int i = 0; i < 6; i++)
                {
                    this.UpdateEquipActiveSkillCd(i, bIsOpenForm);
                }
            }
        }

        private void UpdateEquipActiveSkillCd(int iEquipSlotIndex, bool bIsOpenForm = false)
        {
            if ((iEquipSlotIndex >= 0) && (iEquipSlotIndex <= 6))
            {
                EquipComponent equipComponent = this.m_hostCtrlHero.handle.EquipComponent;
                stEquipInfo[] equips = equipComponent.GetEquips();
                if ((equips != null) && (equips[iEquipSlotIndex].m_equipID > 0))
                {
                    switch (equipComponent.GetEquipActiveSkillSlotInfo(iEquipSlotIndex))
                    {
                        case ENUM_EQUIP_ACTIVESKILL_STATUS.ENM_EQUIP_ACTIVESKILL_STATUS_ISSHOWING:
                        {
                            SkillSlot slot;
                            SkillSlotType showingSkillSlotByEquipSlot = equipComponent.GetShowingSkillSlotByEquipSlot(iEquipSlotIndex);
                            if ((this.m_hostCtrlHero.handle.SkillControl != null) && this.m_hostCtrlHero.handle.SkillControl.TryGetSkillSlot(showingSkillSlotByEquipSlot, out slot))
                            {
                                int curSkillCD = (int) slot.CurSkillCD;
                                if ((curSkillCD != this.m_arrEquipActiveSkillCd[iEquipSlotIndex]) || bIsOpenForm)
                                {
                                    this.m_arrEquipActiveSkillCd[iEquipSlotIndex] = curSkillCD;
                                    this.SetEquipActiveSkillCdState(iEquipSlotIndex, curSkillCD);
                                    equipComponent.AddEquipActiveSkillCdInfo(iEquipSlotIndex, curSkillCD);
                                }
                            }
                            break;
                        }
                        case ENUM_EQUIP_ACTIVESKILL_STATUS.ENM_EQUIP_ACTIVESKILL_STATUS_NOTSHOW:
                        {
                            int cd = 0;
                            equipComponent.GetEquipActiveSkillCdInfo(iEquipSlotIndex, out cd);
                            if ((cd != this.m_arrEquipActiveSkillCd[iEquipSlotIndex]) || bIsOpenForm)
                            {
                                this.m_arrEquipActiveSkillCd[iEquipSlotIndex] = cd;
                                this.SetEquipActiveSkillCdState(iEquipSlotIndex, cd);
                            }
                            break;
                        }
                    }
                }
            }
        }

        public void UpdateLogic(int delta)
        {
            if (this.m_tickFadeTime > 0)
            {
                this.m_tickFadeTime -= delta;
                if (this.m_tickFadeTime <= 0)
                {
                    this.m_bPlayAnimation = true;
                    this.m_tickAnimationTime = 0x7d0;
                }
            }
            if (this.m_bPlayAnimation)
            {
                this.m_tickAnimationTime -= delta;
                this.SetQuicklyBuyInfoPanelAlpha(((float) this.m_tickAnimationTime) / 2000f);
                this.m_bPlayAnimation = this.m_tickAnimationTime > 0;
            }
        }

        public class CEquipBuyPrice
        {
            public CrypticInt32 m_buyPrice;
            public int m_swappedPreEquipInfoCount;
            public stSwappedPreEquipInfo[] m_swappedPreEquipInfos;
            public bool m_used;

            public CEquipBuyPrice(uint buyPrice)
            {
                this.m_buyPrice = (CrypticInt32) buyPrice;
                this.m_swappedPreEquipInfos = new stSwappedPreEquipInfo[6];
                this.m_swappedPreEquipInfoCount = 0;
                this.m_used = true;
            }

            public void AddSwappedPreEquipInfo(ushort equipID)
            {
                for (int i = 0; i < this.m_swappedPreEquipInfoCount; i++)
                {
                    if (this.m_swappedPreEquipInfos[i].m_equipID == equipID)
                    {
                        this.m_swappedPreEquipInfos[i].m_swappedAmount++;
                        return;
                    }
                }
                if (this.m_swappedPreEquipInfoCount < 6)
                {
                    this.m_swappedPreEquipInfos[this.m_swappedPreEquipInfoCount].m_equipID = equipID;
                    this.m_swappedPreEquipInfos[this.m_swappedPreEquipInfoCount].m_swappedAmount = 1;
                    this.m_swappedPreEquipInfoCount++;
                }
            }

            public void Clear()
            {
                this.m_buyPrice = 0;
                for (int i = 0; i < 6; i++)
                {
                    this.m_swappedPreEquipInfos[i].m_equipID = 0;
                    this.m_swappedPreEquipInfos[i].m_swappedAmount = 0;
                }
                this.m_swappedPreEquipInfoCount = 0;
            }

            public void Clone(CBattleEquipSystem.CEquipBuyPrice equipBuyPrice)
            {
                if (equipBuyPrice != null)
                {
                    this.m_buyPrice = equipBuyPrice.m_buyPrice;
                    for (int i = 0; i < 6; i++)
                    {
                        this.m_swappedPreEquipInfos[i] = equipBuyPrice.m_swappedPreEquipInfos[i];
                    }
                    this.m_swappedPreEquipInfoCount = equipBuyPrice.m_swappedPreEquipInfoCount;
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct stSwappedPreEquipInfo
            {
                public ushort m_equipID;
                public uint m_swappedAmount;
            }
        }

        private enum enEquipChangeFlag
        {
            EquipInBattleChanged = 2,
            GoldCoinInBattleChanged = 1
        }

        private enum enSelectedEquipOrigin
        {
            Count = 3,
            EquipBag = 2,
            EquipLibaray = 0,
            EquipTree = 1,
            None = -1
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct stSelectedEquipItem
        {
            public CEquipInfo m_equipInfo;
            public Transform m_equipItemTransform;
            public int m_positionInBag;
        }
    }
}

