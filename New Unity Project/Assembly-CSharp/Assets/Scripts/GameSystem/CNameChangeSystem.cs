namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class CNameChangeSystem : Singleton<CNameChangeSystem>
    {
        private RES_CHANGE_NAME_TYPE m_curChangeType = RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_PLAYER;
        private int m_guildNameChangeCount;
        private int m_playerNameChangeCount;
        public const string NameChangeFormPrefabPath = "UGUI/Form/Common/Form_NameChange.prefab";

        private bool CheckNameChangeCardCount()
        {
            int nameChangeHaveItemCount = this.GetNameChangeHaveItemCount(this.m_curChangeType);
            int nameChangeCostItemCount = this.GetNameChangeCostItemCount(this.m_curChangeType);
            if (nameChangeHaveItemCount >= nameChangeCostItemCount)
            {
                return true;
            }
            if (CSysDynamicBlock.bLobbyEntryBlocked)
            {
                string text = Singleton<CTextManager>.GetInstance().GetText("NameChange_ErrorItemNotEnough");
                Singleton<CUIManager>.GetInstance().OpenMessageBox(text, false);
            }
            else
            {
                uint nameChangeCardShopProductKey = this.GetNameChangeCardShopProductKey(this.m_curChangeType);
                CMallFactoryShopController.ShopProduct product = Singleton<CMallFactoryShopController>.GetInstance().GetProduct(nameChangeCardShopProductKey);
                if (product == null)
                {
                    DebugHelper.Assert(false, "错误的特卖商品ID");
                    return false;
                }
                ResPropInfo dataByKey = GameDataMgr.itemDatabin.GetDataByKey(product.ID);
                if (dataByKey == null)
                {
                    DebugHelper.Assert(false, "错误的商品ID");
                    return false;
                }
                CUseable useable = CUseableManager.CreateUseable(product.Type, 0L, product.ID, (int) product.LimitCount, 0);
                uint num4 = product.ConvertWithRealDiscount(useable.GetBuyPrice(product.CoinType));
                int num5 = nameChangeCostItemCount - nameChangeHaveItemCount;
                string[] args = new string[] { num5.ToString(), dataByKey.szName, (num5 * num4).ToString() };
                string strContent = Singleton<CTextManager>.GetInstance().GetText("NameChange_GuideToMall", args);
                stUIEventParams par = new stUIEventParams();
                par.tag = num5;
                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent, enUIEventID.NameChange_GuideToMall, enUIEventID.None, par, false);
            }
            return false;
        }

        private uint GetNameChangeCardShopProductKey(RES_CHANGE_NAME_TYPE chgNameType)
        {
            uint num = 0;
            RES_CHANGE_NAME_TYPE res_change_name_type = chgNameType;
            if (res_change_name_type != RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_PLAYER)
            {
                if (res_change_name_type != RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_GUILD)
                {
                    return num;
                }
            }
            else
            {
                return GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xc9).dwConfValue;
            }
            return GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xca).dwConfValue;
        }

        private int GetNameChangeCostItemCount(RES_CHANGE_NAME_TYPE changeType)
        {
            <GetNameChangeCostItemCount>c__AnonStorey62 storey = new <GetNameChangeCostItemCount>c__AnonStorey62();
            storey.changeType = changeType;
            storey.nameChangeCount = (storey.changeType != RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_PLAYER) ? this.m_guildNameChangeCount : this.m_playerNameChangeCount;
            ResChangeName name = GameDataMgr.changeNameDatabin.FindIf(new Func<ResChangeName, bool>(storey, (IntPtr) this.<>m__63));
            if (name != null)
            {
                return (int) name.dwCostItemCnt;
            }
            return 0;
        }

        private int GetNameChangeHaveItemCount(RES_CHANGE_NAME_TYPE changeType)
        {
            int num = 0;
            CUseableContainer useableContainer = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
            if (useableContainer != null)
            {
                int curUseableCount = useableContainer.GetCurUseableCount();
                for (int i = 0; i < curUseableCount; i++)
                {
                    CUseable useableByIndex = useableContainer.GetUseableByIndex(i);
                    if ((useableByIndex != null) && (useableByIndex.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP))
                    {
                        if ((changeType == RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_PLAYER) && CItem.IsPlayerNameChangeCard(useableByIndex.m_baseID))
                        {
                            num += useableByIndex.m_stackCount;
                        }
                        else if ((changeType == RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_GUILD) && CItem.IsGuildNameChangeCard(useableByIndex.m_baseID))
                        {
                            num += useableByIndex.m_stackCount;
                        }
                    }
                }
            }
            return num;
        }

        private uint GetTimeToCD(RES_CHANGE_NAME_TYPE changeType)
        {
            DateTime time2;
            uint num2;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo == null)
            {
                return 0;
            }
            DateTime date = Utility.ToUtcTime2Local((long) CRoleInfo.GetCurrentUTCTime());
            uint globeValue = 0;
            RES_CHANGE_NAME_TYPE res_change_name_type = changeType;
            if (res_change_name_type != RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_PLAYER)
            {
                if (res_change_name_type != RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_GUILD)
                {
                    return 0;
                }
            }
            else
            {
                time2 = Utility.ToUtcTime2Local((long) masterRoleInfo.chgNameCD.dwLastChgPlayerNameTime);
                globeValue = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_CHG_PLAYER_NAME_CD);
                goto Label_007D;
            }
            time2 = Utility.ToUtcTime2Local((long) masterRoleInfo.chgNameCD.dwLastChgGuildNameTime);
            globeValue = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_CHG_GUILD_NAME_CD);
        Label_007D:
            num2 = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 10).dwConfValue;
            int num3 = Utility.Hours2Second((int) (num2 / 100)) + Utility.Minutes2Seconds((int) (num2 % 100));
            date = date.AddSeconds((double) -num3).Date;
            time2 = time2.AddSeconds((double) -num3).Date;
            TimeSpan span = (TimeSpan) (date - time2);
            int days = span.Days;
            return ((days < globeValue) ? (globeValue - ((uint) days)) : ((uint) 0L));
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.NameChange_OpenPlayerNameChangeForm, new CUIEventManager.OnUIEventHandler(this.OnOpenPlayerNameChangeForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.NameChange_OpenGuildNameChangeForm, new CUIEventManager.OnUIEventHandler(this.OnOpenGuildNameChangeForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.NameChange_ChangeName, new CUIEventManager.OnUIEventHandler(this.OnChangeName));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.NameChange_GuideToMall, new CUIEventManager.OnUIEventHandler(this.OnGuideToMall));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.NameChange_BuyNameChangeCardConfirm, new CUIEventManager.OnUIEventHandler(this.OnBuyNameChgCardConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_OpenBuyDianQuanPanel, new CUIEventManager.OnUIEventHandler(this.OnOpenBuyDianQuanPanel));
        }

        private void OnBuyNameChgCardConfirm(CUIEvent uiEvent)
        {
            uint key = (uint) uiEvent.m_eventParams.commonUInt64Param1;
            uint count = uiEvent.m_eventParams.commonUInt32Param1;
            CMallFactoryShopController.ShopProduct shopProduct = Singleton<CMallFactoryShopController>.GetInstance().GetProduct(key);
            if (shopProduct != null)
            {
                Singleton<CMallFactoryShopController>.GetInstance().RequestBuy(shopProduct, count);
            }
        }

        private void OnChangeName(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            if (srcFormScript != null)
            {
                string str = CUIUtility.RemoveEmoji(srcFormScript.GetWidget(0).GetComponent<InputField>().get_text()).Trim();
                if (this.m_curChangeType != RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_PLAYER)
                {
                    if (this.m_curChangeType == RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_GUILD)
                    {
                        if (string.IsNullOrEmpty(str))
                        {
                            Singleton<CUIManager>.GetInstance().OpenTips("Guild_Input_Guild_Name_Empty", true, 1.5f, null, new object[0]);
                        }
                        else if (!Utility.IsValidText(str))
                        {
                            Singleton<CUIManager>.GetInstance().OpenTips("Guild_Input_Guild_Name_Invalid", true, 1.5f, null, new object[0]);
                        }
                        else if (this.CheckNameChangeCardCount())
                        {
                            this.SendChangeNameReq(str);
                        }
                    }
                    else if (this.m_curChangeType == RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_TRANSDATA_PLAYER)
                    {
                        switch (Utility.CheckRoleName(str))
                        {
                            case Utility.NameResult.Vaild:
                                this.SendTransDataRenameReq(str);
                                return;

                            case Utility.NameResult.Null:
                                Singleton<CUIManager>.GetInstance().OpenTips("NameChange_Null", true, 1.5f, null, new object[0]);
                                return;

                            case Utility.NameResult.OutOfLength:
                                Singleton<CUIManager>.GetInstance().OpenTips("RoleRegister_NameOutOfLength", true, 1.5f, null, new object[0]);
                                return;

                            case Utility.NameResult.InVaildChar:
                                Singleton<CUIManager>.GetInstance().OpenTips("NameChange_InvalidName", true, 1.5f, null, new object[0]);
                                return;
                        }
                    }
                }
                else
                {
                    switch (Utility.CheckRoleName(str))
                    {
                        case Utility.NameResult.Vaild:
                            if (this.CheckNameChangeCardCount())
                            {
                                this.SendChangeNameReq(str);
                                return;
                            }
                            return;

                        case Utility.NameResult.Null:
                            Singleton<CUIManager>.GetInstance().OpenTips("NameChange_Null", true, 1.5f, null, new object[0]);
                            return;

                        case Utility.NameResult.OutOfLength:
                            Singleton<CUIManager>.GetInstance().OpenTips("RoleRegister_NameOutOfLength", true, 1.5f, null, new object[0]);
                            return;

                        case Utility.NameResult.InVaildChar:
                            Singleton<CUIManager>.GetInstance().OpenTips("NameChange_InvalidName", true, 1.5f, null, new object[0]);
                            return;
                    }
                }
            }
        }

        private void OnGuideToMall(CUIEvent uiEvent)
        {
            uint nameChangeCardShopProductKey = this.GetNameChangeCardShopProductKey(this.m_curChangeType);
            CMallFactoryShopController.ShopProduct shopProduct = Singleton<CMallFactoryShopController>.GetInstance().GetProduct(nameChangeCardShopProductKey);
            CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
            uIEvent.m_eventID = enUIEventID.NameChange_BuyNameChangeCardConfirm;
            uIEvent.m_eventParams.commonUInt64Param1 = nameChangeCardShopProductKey;
            Singleton<CMallFactoryShopController>.GetInstance().BuyShopProduct(shopProduct, (uint) uiEvent.m_eventParams.tag, false, uIEvent);
        }

        private void OnOpenBuyDianQuanPanel(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().CloseForm(CPlayerInfoSystem.sPlayerInfoFormPath);
            Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/Common/Form_NameChange.prefab");
        }

        private void OnOpenGuildNameChangeForm(CUIEvent uiEvent)
        {
            uint timeToCD = this.GetTimeToCD(RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_GUILD);
            if (timeToCD == 0)
            {
                this.OpenNameChangeForm(RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_GUILD);
            }
            else
            {
                object[] replaceArr = new object[] { timeToCD };
                Singleton<CUIManager>.GetInstance().OpenTips("NameChange_ErrorInCD", true, 1.5f, null, replaceArr);
            }
        }

        private void OnOpenPlayerNameChangeForm(CUIEvent uiEvent)
        {
            uint timeToCD = this.GetTimeToCD(RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_PLAYER);
            if (timeToCD == 0)
            {
                this.OpenNameChangeForm(RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_PLAYER);
            }
            else
            {
                object[] replaceArr = new object[] { timeToCD };
                Singleton<CUIManager>.GetInstance().OpenTips("NameChange_ErrorInCD", true, 1.5f, null, replaceArr);
            }
        }

        private void OpenNameChangeForm(RES_CHANGE_NAME_TYPE changeType)
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/Common/Form_NameChange.prefab", false, true);
            if (script != null)
            {
                InputField component = script.GetWidget(0).GetComponent<InputField>();
                Text text = script.GetWidget(1).GetComponent<Text>();
                if ((changeType == RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_PLAYER) || (changeType == RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_GUILD))
                {
                    bool flag = changeType == RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_PLAYER;
                    component.get_placeholder().GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText(!flag ? "NameChange_GuildInputNewName" : "NameChange_InputNewName"));
                    if (!flag)
                    {
                        component.set_characterLimit(7);
                    }
                    text.set_text(Singleton<CTextManager>.GetInstance().GetText(!flag ? "NameChange_GuildCostDesc" : "NameChange_CostDesc"));
                }
                else if (changeType == RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_TRANSDATA_PLAYER)
                {
                    component.get_placeholder().GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("NameChange_InputNewName"));
                    text.set_text(Singleton<CTextManager>.GetInstance().GetText("NameChange_TransData_PlayerDesc"));
                    script.get_transform().FindChild("Panel/title/ClostBtn").get_gameObject().CustomSetActive(false);
                }
                this.m_curChangeType = changeType;
            }
        }

        [MessageHandler(0xc81)]
        public static void ReceiveChangeNameRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_CHANGE_NAME_RSP stChangeNameRsp = msg.stPkgData.stChangeNameRsp;
            CS_ERRORCODE_DEF iResult = (CS_ERRORCODE_DEF) stChangeNameRsp.iResult;
            if (iResult == CS_ERRORCODE_DEF.CS_SUCCESS)
            {
                Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/Common/Form_NameChange.prefab");
                string str = StringHelper.UTF8BytesToString(ref stChangeNameRsp.szNewName);
                if (stChangeNameRsp.bType == 1)
                {
                    Singleton<CNameChangeSystem>.GetInstance().m_playerNameChangeCount = (int) stChangeNameRsp.dwChangeCnt;
                    if (!string.IsNullOrEmpty(str))
                    {
                        Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().Name = str;
                        Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.NAMECHANGE_PLAYER_NAME_CHANGE);
                        Singleton<CUIManager>.GetInstance().OpenTips("NameChange_Success", true, 1.5f, null, new object[0]);
                    }
                    else
                    {
                        DebugHelper.Assert(false, "CNameChangeSystem.ReceiveChangeNameRsp(): error, empty player new name!!!");
                    }
                }
                else if (stChangeNameRsp.bType == 2)
                {
                    Singleton<CNameChangeSystem>.GetInstance().m_guildNameChangeCount = (int) stChangeNameRsp.dwChangeCnt;
                    if (!string.IsNullOrEmpty(str))
                    {
                        Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.name = str;
                        Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.NAMECHANGE_GUILD_NAME_CHANGE);
                        Singleton<CUIManager>.GetInstance().OpenTips("NameChange_Success", true, 1.5f, null, new object[0]);
                    }
                    else
                    {
                        DebugHelper.Assert(false, "CNameChangeSystem.ReceiveChangeNameRsp(): error, empty guild new name!!!");
                    }
                }
            }
            else
            {
                Singleton<CNameChangeSystem>.GetInstance().ShowErrorCode(iResult);
            }
        }

        [MessageHandler(0x157e)]
        public static void ReceiveChgNameCD(CSPkg msg)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                masterRoleInfo.chgNameCD = msg.stPkgData.stChgNameCDNtf.stChgNameCD;
            }
        }

        [MessageHandler(0xc82)]
        public static void ReceiveGuildNameChangeNtf(CSPkg msg)
        {
            Singleton<CNameChangeSystem>.GetInstance().m_guildNameChangeCount = (int) msg.stPkgData.stGuildNameChgNtf.dwChgNameCnt;
            Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.name = StringHelper.UTF8BytesToString(ref msg.stPkgData.stGuildNameChgNtf.szNewName);
            Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.NAMECHANGE_GUILD_NAME_CHANGE);
        }

        [MessageHandler(0x1473)]
        public static void ReceiveTransDataRenameNtf(CSPkg msg)
        {
            Singleton<CNameChangeSystem>.GetInstance().OpenNameChangeForm(RES_CHANGE_NAME_TYPE.RES_CHANGE_NAME_TYPE_TRANSDATA_PLAYER);
        }

        [MessageHandler(0x1475)]
        public static void ReceiveTransDataRenameRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            CS_ERRORCODE_DEF dwResult = (CS_ERRORCODE_DEF) msg.stPkgData.stTransDataRenameRes.dwResult;
            if (dwResult == CS_ERRORCODE_DEF.CS_SUCCESS)
            {
                Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/Common/Form_NameChange.prefab");
                Singleton<CUIManager>.GetInstance().OpenTips("NameChange_Success", true, 1.5f, null, new object[0]);
            }
            else
            {
                Singleton<CNameChangeSystem>.GetInstance().ShowErrorCode(dwResult);
            }
        }

        private void SendChangeNameReq(string name)
        {
            Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0xc80);
            msg.stPkgData.stChangeNameReq.bType = (byte) this.m_curChangeType;
            StringHelper.StringToUTF8Bytes(name, ref msg.stPkgData.stChangeNameReq.szNewName);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void SendTransDataRenameReq(string name)
        {
            Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x1474);
            StringHelper.StringToUTF8Bytes(name, ref msg.stPkgData.stTransDataRenameReq.szNewName);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        public void SetGuildNameChangeCount(int guildNameChangeCount)
        {
            this.m_guildNameChangeCount = guildNameChangeCount;
        }

        public void SetPlayerNameChangeCount(int playerNameChangeCount)
        {
            this.m_playerNameChangeCount = playerNameChangeCount;
        }

        private void ShowErrorCode(CS_ERRORCODE_DEF result)
        {
            if (result == CS_ERRORCODE_DEF.CS_ERR_REGISTER_NAME_DUP_FAIL)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("RoleRegister_NameExists", true, 1.5f, null, new object[0]);
            }
            else if (result == CS_ERRORCODE_DEF.CS_ERR_REGISTERNAME)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("NameChange_ErrorInvalid", true, 1.5f, null, new object[0]);
            }
            else if (result == CS_ERRORCODE_DEF.CS_ERR_CHANGE_NAME_TYPE_INVALID)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("NameChange_ErrorTypeInvalid", true, 1.5f, null, new object[0]);
            }
            else if (result == CS_ERRORCODE_DEF.CS_ERR_CHANGE_NAME_ITEM_NOT_ENOUGH)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("NameChange_ErrorItemNotEnough", true, 1.5f, null, new object[0]);
            }
            else if (result == CS_ERRORCODE_DEF.CS_ERR_DB)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("NameChange_ErrorDB", true, 1.5f, null, new object[0]);
            }
            else if (result == CS_ERRORCODE_DEF.CS_ERR_CHANGE_GUILD_NAME_AUTHORITY)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("NameChange_ErrorAuthority", true, 1.5f, null, new object[0]);
            }
        }

        [CompilerGenerated]
        private sealed class <GetNameChangeCostItemCount>c__AnonStorey62
        {
            internal RES_CHANGE_NAME_TYPE changeType;
            internal int nameChangeCount;

            internal bool <>m__63(ResChangeName x)
            {
                return (((x.bType == ((byte) this.changeType)) && (x.dwChgCntLow <= this.nameChangeCount)) && (this.nameChangeCount <= x.dwChgCntHigh));
            }
        }

        public enum enNameChangeFormWidget
        {
            NameInputField,
            DescText
        }
    }
}

