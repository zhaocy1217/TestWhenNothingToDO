namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class CHeroSkinBuyManager : Singleton<CHeroSkinBuyManager>
    {
        private ListView<COMDT_FRIEND_INFO> detailFriendList = new ListLinqView<COMDT_FRIEND_INFO>();
        private uint m_buyHeroIDForFriend;
        private uint m_buyPriceForFriend;
        private uint m_buySkinIDForFriend;
        private ListView<COMDT_FRIEND_INFO> m_friendList;
        private bool m_isBuySkinForFriend;
        public const int MAX_PRESENT_MSG_LENGTH = 40;
        public static string s_buyHeroSkin3DFormPath = "UGUI/Form/System/HeroInfo/Form_Buy_HeroSkin_3D.prefab";
        public static string s_buyHeroSkinFormPath = "UGUI/Form/System/HeroInfo/Form_Buy_HeroSkin.prefab";
        public static string s_heroBuyFormPath = "UGUI/Form/System/Mall/Form_MallBuyHero.prefab";
        public static string s_heroBuyFriendPath = "UGUI/Form/System/HeroInfo/Form_BuyForFriend.prefab";
        public static string s_leaveMsgPath = "UGUI/Form/System/HeroInfo/Form_LeaveMessage.prefab";

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_OpenBuyHeroForm, new CUIEventManager.OnUIEventHandler(this.OnOpenBuyHeroForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_OpenBuySkinForm, new CUIEventManager.OnUIEventHandler(this.OnOpenBuySkinForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_Buy, new CUIEventManager.OnUIEventHandler(this.OnHeroSkin_Buy));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_BuyConfirm, new CUIEventManager.OnUIEventHandler(this.OnHeroSkinBuyConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_BuyHero, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_BuyHero));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_ConfirmBuyHero, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_ConfirmBuyHero));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_OpenBuyHeroForFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_OpenBuyHeroForFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_BuyHeroForFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_BuyHeroForFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_ConfirmBuyHeroForFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_ConfirmBuyHeroForFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_SecurePwdConfirmBuyHeroForFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_SecurePwdConfirmBuyHeroForFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_OpenBuyHeroSkinForFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_OpenBuyHeroSkinForFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_SecurePwdConfirmBuyHeroSkinForFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_SecurePwdConfirmBuySkinForFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_BuyHeroSkinForFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_BuyHeroSkinForFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_ConfirmBuyHeroSkinForFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_ConfirmBuyHeroSkinForFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_LeaveMsgForFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_BuySkinLeaveMsgForFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_LeaveMsgForFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_BuyHeroLeaveMsgForFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_SearchFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_SearchFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_OnFriendListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnFriendListElementEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_OnCloseBuySkinForm, new CUIEventManager.OnUIEventHandler(this.OnCloseBuySkinForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_OnUseSkinExpCard, new CUIEventManager.OnUIEventHandler(this.OnUseSkinExpCard));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPcik_factoyShopTipsForm, new CUIEventManager.OnUIEventHandler(this.OnBuyPcik_factoyShopTipsForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPcik_factoyShopTipsCancelForm, new CUIEventManager.OnUIEventHandler(this.OnBuyPcik_factoyShopTipsCancelForm));
        }

        private void InitBuyForFriendForm(CUIFormScript form, bool bSkin, uint heroId, uint skinId = 0, ulong friendUid = 0, uint worldId = 0, bool isSns = false)
        {
            Transform transform9;
            CUIEventScript script;
            uint payValue = 0;
            if (!bSkin)
            {
                ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
                DebugHelper.Assert(dataByKey != null);
                if (dataByKey == null)
                {
                    goto Label_03FD;
                }
                form.get_transform().Find("Panel/Title/titleText").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Hero_Give_Title"));
                GameObject obj3 = form.get_transform().Find("Panel/skinBgImage/skinIconImage").get_gameObject();
                form.get_transform().Find("Panel/skinBgImage/skinNameText").GetComponent<Text>().set_text(StringHelper.UTF8BytesToString(ref dataByKey.szName));
                form.get_transform().Find("Panel/skinBgImage/skinIconImage").GetComponent<Image>().SetSprite(CUIUtility.s_Sprite_Dynamic_BustHero_Dir + StringHelper.UTF8BytesToString(ref dataByKey.szImagePath), form, false, true, true, true);
                form.get_transform().Find("Panel/Panel_Prop").get_gameObject().CustomSetActive(false);
                Transform transform6 = form.get_transform().Find("Panel/skinPricePanel");
                Transform costIcon = transform6.Find("costImage");
                SetPayCostIcon(form, costIcon, enPayType.DianQuan);
                SetPayCostTypeText(transform6.Find("costTypeText"), enPayType.DianQuan);
                transform9 = transform6.Find("costPanel");
                if (transform9 == null)
                {
                    goto Label_03FD;
                }
                ResHeroPromotion resPromotion = CHeroDataFactory.CreateHeroData(heroId).promotion();
                stPayInfoSet payInfoSetOfGood = CMallSystem.GetPayInfoSetOfGood(dataByKey, resPromotion);
                for (int i = 0; i < payInfoSetOfGood.m_payInfoCount; i++)
                {
                    if (((payInfoSetOfGood.m_payInfos[i].m_payType == enPayType.Diamond) || (payInfoSetOfGood.m_payInfos[i].m_payType == enPayType.DianQuan)) || (payInfoSetOfGood.m_payInfos[i].m_payType == enPayType.DiamondAndDianQuan))
                    {
                        payValue = payInfoSetOfGood.m_payInfos[i].m_payValue;
                        break;
                    }
                }
            }
            else
            {
                ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
                DebugHelper.Assert(heroSkin != null);
                if (heroSkin != null)
                {
                    form.get_transform().Find("Panel/Title/titleText").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_Give_Title"));
                    Image image = form.get_transform().Find("Panel/skinBgImage/skinIconImage").GetComponent<Image>();
                    string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_BustHero_Dir, StringHelper.UTF8BytesToString(ref heroSkin.szSkinPicID));
                    image.SetSprite(prefabPath, form, false, true, true, true);
                    form.get_transform().Find("Panel/skinBgImage/skinNameText").GetComponent<Text>().set_text(StringHelper.UTF8BytesToString(ref heroSkin.szSkinName));
                    form.get_transform().Find("Panel/Panel_Prop").get_gameObject().CustomSetActive(true);
                    GameObject listObj = form.get_transform().Find("Panel/Panel_Prop/List_Prop").get_gameObject();
                    CSkinInfo.GetHeroSkinProp(heroId, skinId, ref CHeroInfoSystem2.s_propArr, ref CHeroInfoSystem2.s_propPctArr, ref CHeroInfoSystem2.s_propImgArr);
                    CUICommonSystem.SetListProp(listObj, ref CHeroInfoSystem2.s_propArr, ref CHeroInfoSystem2.s_propPctArr);
                    Transform transform = form.get_transform().Find("Panel/skinPricePanel");
                    Transform transform2 = transform.Find("costImage");
                    SetPayCostIcon(form, transform2, enPayType.DianQuan);
                    SetPayCostTypeText(transform.Find("costTypeText"), enPayType.DianQuan);
                    Transform transform4 = transform.Find("costPanel");
                    if (transform4 != null)
                    {
                        stPayInfoSet skinPayInfoSet = CSkinInfo.GetSkinPayInfoSet(heroId, skinId);
                        for (int j = 0; j < skinPayInfoSet.m_payInfoCount; j++)
                        {
                            if (((skinPayInfoSet.m_payInfos[j].m_payType == enPayType.Diamond) || (skinPayInfoSet.m_payInfos[j].m_payType == enPayType.DianQuan)) || (skinPayInfoSet.m_payInfos[j].m_payType == enPayType.DiamondAndDianQuan))
                            {
                                payValue = skinPayInfoSet.m_payInfos[j].m_payValue;
                                break;
                            }
                        }
                        SetPayCurrentPrice(transform4.Find("costText"), payValue);
                    }
                }
                goto Label_03FD;
            }
            SetPayCurrentPrice(transform9.Find("costText"), payValue);
        Label_03FD:
            script = form.get_transform().Find("Panel/SearchFriend/Button").GetComponent<CUIEventScript>();
            script.m_onClickEventParams.friendHeroSkinPar.bSkin = bSkin;
            script.m_onClickEventParams.friendHeroSkinPar.heroId = heroId;
            script.m_onClickEventParams.friendHeroSkinPar.skinId = skinId;
            script.m_onClickEventParams.friendHeroSkinPar.price = payValue;
            Text component = form.get_transform().Find("Panel/TipTxt").GetComponent<Text>();
            uint[] conditionParam = Singleton<CFunctionUnlockSys>.instance.GetConditionParam(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_PRESENTHERO, RES_UNLOCKCONDITION_TYPE.RES_UNLOCKCONDITIONTYPE_ABOVELEVEL);
            uint num4 = (conditionParam.Length <= 1) ? 1 : conditionParam[0];
            string[] args = new string[] { num4.ToString() };
            component.set_text(Singleton<CTextManager>.GetInstance().GetText("Buy_For_Friend_Tip", args));
            ListView<COMDT_FRIEND_INFO> allFriend = Singleton<CFriendContoller>.GetInstance().model.GetAllFriend(true);
            CUIListScript list = form.get_transform().Find("Panel/List").GetComponent<CUIListScript>();
            this.UpdateFriendList(ref allFriend, ref list, bSkin, heroId, skinId, payValue, friendUid, worldId, isSns);
        }

        private void InitLeaveMsgForFriendForm(CUIFormScript form, bool bSkin, uint heroId, uint skinId = 0, int friendIndex = 0)
        {
            Transform transform9;
            Text text5;
            uint payValue = 0;
            if (!bSkin)
            {
                ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
                DebugHelper.Assert(dataByKey != null);
                if (dataByKey == null)
                {
                    goto Label_03EB;
                }
                form.get_transform().Find("Panel/Title/titleText").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Hero_Give_Title"));
                GameObject obj3 = form.get_transform().Find("Panel/skinBgImage/skinIconImage").get_gameObject();
                form.get_transform().Find("Panel/skinBgImage/skinNameText").GetComponent<Text>().set_text(StringHelper.UTF8BytesToString(ref dataByKey.szName));
                form.get_transform().Find("Panel/skinBgImage/skinIconImage").GetComponent<Image>().SetSprite(CUIUtility.s_Sprite_Dynamic_BustHero_Dir + StringHelper.UTF8BytesToString(ref dataByKey.szImagePath), form, false, true, true, true);
                form.get_transform().Find("Panel/Panel_Prop").get_gameObject().CustomSetActive(false);
                Transform transform6 = form.get_transform().Find("Panel/skinPricePanel");
                Transform costIcon = transform6.Find("costImage");
                SetPayCostIcon(form, costIcon, enPayType.DianQuan);
                SetPayCostTypeText(transform6.Find("costTypeText"), enPayType.DianQuan);
                transform9 = transform6.Find("costPanel");
                if (transform9 == null)
                {
                    goto Label_03EB;
                }
                ResHeroPromotion resPromotion = CHeroDataFactory.CreateHeroData(heroId).promotion();
                stPayInfoSet payInfoSetOfGood = CMallSystem.GetPayInfoSetOfGood(dataByKey, resPromotion);
                for (int i = 0; i < payInfoSetOfGood.m_payInfoCount; i++)
                {
                    if (((payInfoSetOfGood.m_payInfos[i].m_payType == enPayType.Diamond) || (payInfoSetOfGood.m_payInfos[i].m_payType == enPayType.DianQuan)) || (payInfoSetOfGood.m_payInfos[i].m_payType == enPayType.DiamondAndDianQuan))
                    {
                        payValue = payInfoSetOfGood.m_payInfos[i].m_payValue;
                        break;
                    }
                }
            }
            else
            {
                ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
                form.get_transform().Find("Panel/Title/titleText").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_Give_Title"));
                Image image = form.get_transform().Find("Panel/skinBgImage/skinIconImage").GetComponent<Image>();
                string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_BustHero_Dir, StringHelper.UTF8BytesToString(ref heroSkin.szSkinPicID));
                image.SetSprite(prefabPath, form, false, true, true, true);
                form.get_transform().Find("Panel/skinBgImage/skinNameText").GetComponent<Text>().set_text(StringHelper.UTF8BytesToString(ref heroSkin.szSkinName));
                form.get_transform().Find("Panel/Panel_Prop").get_gameObject().CustomSetActive(true);
                GameObject listObj = form.get_transform().Find("Panel/Panel_Prop/List_Prop").get_gameObject();
                CSkinInfo.GetHeroSkinProp(heroId, skinId, ref CHeroInfoSystem2.s_propArr, ref CHeroInfoSystem2.s_propPctArr, ref CHeroInfoSystem2.s_propImgArr);
                CUICommonSystem.SetListProp(listObj, ref CHeroInfoSystem2.s_propArr, ref CHeroInfoSystem2.s_propPctArr);
                Transform transform = form.get_transform().Find("Panel/skinPricePanel");
                Transform transform2 = transform.Find("costImage");
                SetPayCostIcon(form, transform2, enPayType.DianQuan);
                SetPayCostTypeText(transform.Find("costTypeText"), enPayType.DianQuan);
                Transform transform4 = transform.Find("costPanel");
                if (transform4 != null)
                {
                    stPayInfoSet skinPayInfoSet = CSkinInfo.GetSkinPayInfoSet(heroId, skinId);
                    for (int j = 0; j < skinPayInfoSet.m_payInfoCount; j++)
                    {
                        if (((skinPayInfoSet.m_payInfos[j].m_payType == enPayType.Diamond) || (skinPayInfoSet.m_payInfos[j].m_payType == enPayType.DianQuan)) || (skinPayInfoSet.m_payInfos[j].m_payType == enPayType.DiamondAndDianQuan))
                        {
                            payValue = skinPayInfoSet.m_payInfos[j].m_payValue;
                            break;
                        }
                    }
                    SetPayCurrentPrice(transform4.Find("costText"), payValue);
                }
                goto Label_03EB;
            }
            SetPayCurrentPrice(transform9.Find("costText"), payValue);
        Label_03EB:
            text5 = form.get_transform().Find("Panel/TipTxt").GetComponent<Text>();
            uint[] conditionParam = Singleton<CFunctionUnlockSys>.instance.GetConditionParam(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_PRESENTHERO, RES_UNLOCKCONDITION_TYPE.RES_UNLOCKCONDITIONTYPE_ABOVELEVEL);
            uint num4 = (conditionParam.Length <= 1) ? 1 : conditionParam[0];
            string[] args = new string[] { num4.ToString() };
            text5.set_text(Singleton<CTextManager>.GetInstance().GetText("Buy_For_Friend_Tip", args));
            RefreshFriendListElementForGift(Utility.FindChild(form.get_gameObject(), "Panel/targetFriend"), this.m_friendList[friendIndex], this.m_isBuySkinForFriend);
            CUIEventScript component = Utility.FindChild(form.get_gameObject(), "Panel/InviteButton").GetComponent<CUIEventScript>();
            component.m_onClickEventParams.commonUInt64Param1 = this.m_friendList[friendIndex].stUin.ullUid;
            component.m_onClickEventParams.tagUInt = this.m_friendList[friendIndex].stUin.dwLogicWorldId;
            component.m_onClickEventParams.tagStr = CUIUtility.RemoveEmoji(Utility.UTF8Convert(this.m_friendList[friendIndex].szUserName));
            if (this.m_isBuySkinForFriend)
            {
                component.m_onClickEventID = enUIEventID.HeroSkin_BuyHeroSkinForFriend;
                component.m_onClickEventParams.heroSkinParam.heroId = this.m_buyHeroIDForFriend;
                component.m_onClickEventParams.heroSkinParam.skinId = this.m_buySkinIDForFriend;
                component.m_onClickEventParams.commonUInt32Param1 = this.m_buyPriceForFriend;
            }
            else
            {
                component.m_onClickEventID = enUIEventID.HeroView_BuyHeroForFriend;
                component.m_onClickEventParams.commonUInt32Param1 = this.m_buyPriceForFriend;
                component.m_onClickEventParams.tag = (int) this.m_buyHeroIDForFriend;
            }
            InputField field = Utility.FindChild(form.get_gameObject(), "Panel/msgContainer/InputField").GetComponent<InputField>();
            if (field != null)
            {
                field.get_onValueChange().AddListener(new UnityAction<string>(this, (IntPtr) this.On_InputFiled_ValueChange));
            }
        }

        private void On_InputFiled_ValueChange(string arg0)
        {
            if (Singleton<CUIManager>.GetInstance().GetForm("Form_MessageBox.prefab") == null)
            {
                string str = CUIUtility.RemoveEmoji(Utility.UTF8Convert(arg0));
                if (str.Length > 40)
                {
                    CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_leaveMsgPath);
                    if (form == null)
                    {
                        Singleton<CUIManager>.GetInstance().OpenMessageBox(string.Format(Singleton<CTextManager>.instance.GetText("chat_input_max"), 40), false);
                    }
                    else
                    {
                        InputField component = Utility.FindChild(form.get_gameObject(), "Panel/msgContainer/InputField").GetComponent<InputField>();
                        if (component != null)
                        {
                            component.DeactivateInputField();
                            component.set_text(str.Substring(0, 40));
                            Singleton<CUIManager>.GetInstance().OpenMessageBox(string.Format(Singleton<CTextManager>.instance.GetText("chat_input_max"), 40), false);
                        }
                    }
                }
            }
        }

        [MessageHandler(0x71a)]
        public static void OnBuyHero(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (msg.stPkgData.stBuyHeroRsp.iResult == 0)
            {
                DebugHelper.Assert(GameDataMgr.heroDatabin.GetDataByKey(msg.stPkgData.stBuyHeroRsp.dwHeroID) != null);
                Singleton<CHeroInfoSystem2>.GetInstance().OnNtyAddHero(msg.stPkgData.stBuyHeroRsp.dwHeroID);
                CUICommonSystem.ShowNewHeroOrSkin(msg.stPkgData.stBuyHeroRsp.dwHeroID, 0, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, false, null, enFormPriority.Priority1, 0, 0);
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(0x71a, msg.stPkgData.stBuyHeroRsp.iResult), false, 1.5f, null, new object[0]);
            }
        }

        [MessageHandler(0x727)]
        public static void OnBuyHeroForFriend(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (msg.stPkgData.stPresentHeroRsp.iResult != 0)
            {
                string strContent = Utility.ProtErrCodeToStr(0x729, msg.stPkgData.stPresentHeroRsp.iResult);
                Singleton<CUIManager>.GetInstance().OpenTips(strContent, false, 1.5f, null, new object[0]);
            }
        }

        [MessageHandler(0x71c)]
        public static void OnBuyHeroSkinRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_BUYHEROSKIN_RSP stBuyHeroSkinRsp = msg.stPkgData.stBuyHeroSkinRsp;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            DebugHelper.Assert(masterRoleInfo != null, "OnBuyHeroSkinRsp::Master Role Info Is Null");
            if (masterRoleInfo != null)
            {
                if (stBuyHeroSkinRsp.iResult == 0)
                {
                    masterRoleInfo.OnAddHeroSkin(stBuyHeroSkinRsp.dwHeroID, stBuyHeroSkinRsp.dwSkinID);
                    Singleton<CHeroInfoSystem2>.GetInstance().OnHeroSkinBuySuc(stBuyHeroSkinRsp.dwHeroID);
                    CUICommonSystem.ShowNewHeroOrSkin(stBuyHeroSkinRsp.dwHeroID, stBuyHeroSkinRsp.dwSkinID, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, false, null, enFormPriority.Priority1, 0, 0);
                }
                else
                {
                    CS_ADDHEROSKIN_ERRCODE iResult = (CS_ADDHEROSKIN_ERRCODE) stBuyHeroSkinRsp.iResult;
                    CTextManager instance = Singleton<CTextManager>.GetInstance();
                    switch (iResult)
                    {
                        case CS_ADDHEROSKIN_ERRCODE.CS_ADDHEROSKIN_SKININVALID:
                            Singleton<CUIManager>.GetInstance().OpenTips(instance.GetText("BuySkin_Error_Invalid"), false, 1.5f, null, new object[0]);
                            return;

                        case CS_ADDHEROSKIN_ERRCODE.CS_ADDHEROSKIN_PROMOTION:
                            Singleton<CUIManager>.GetInstance().OpenTips(instance.GetText("BuySkin_Error_WrongSale"), false, 1.5f, null, new object[0]);
                            return;

                        case CS_ADDHEROSKIN_ERRCODE.CS_ADDHEROSKIN_BUYFAIL:
                            Singleton<CUIManager>.GetInstance().OpenTips(instance.GetText("BuySkin_Error_WrongMethod"), false, 1.5f, null, new object[0]);
                            return;

                        case CS_ADDHEROSKIN_ERRCODE.CS_ADDHEROSKIN_NOHERO:
                            Singleton<CUIManager>.GetInstance().OpenTips(instance.GetText("BuySkin_Error_NoHero"), false, 1.5f, null, new object[0]);
                            return;

                        case CS_ADDHEROSKIN_ERRCODE.CS_ADDHEROSKIN_SKINHAS:
                            if (((stBuyHeroSkinRsp.dwHeroID == 0) || (stBuyHeroSkinRsp.dwSkinID == 0)) || masterRoleInfo.IsHaveHeroSkin(stBuyHeroSkinRsp.dwHeroID, stBuyHeroSkinRsp.dwSkinID, false))
                            {
                                Singleton<CUIManager>.GetInstance().OpenTips(instance.GetText("BuySkin_Error_AlreadyHave"), false, 1.5f, null, new object[0]);
                                return;
                            }
                            masterRoleInfo.OnAddHeroSkin(stBuyHeroSkinRsp.dwHeroID, stBuyHeroSkinRsp.dwSkinID);
                            Singleton<CHeroInfoSystem2>.GetInstance().OnHeroSkinBuySuc(stBuyHeroSkinRsp.dwHeroID);
                            CUICommonSystem.ShowNewHeroOrSkin(stBuyHeroSkinRsp.dwHeroID, stBuyHeroSkinRsp.dwSkinID, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, false, null, enFormPriority.Priority1, 0, 0);
                            return;

                        case CS_ADDHEROSKIN_ERRCODE.CS_ADDHEROSKIN_COINLIMIT:
                            Singleton<CUIManager>.GetInstance().OpenTips(instance.GetText("BuySkin_Error_Money"), false, 1.5f, null, new object[0]);
                            return;

                        case CS_ADDHEROSKIN_ERRCODE.CS_ADDHEROSKIN_COUPONS:
                            Singleton<CUIManager>.GetInstance().OpenTips(instance.GetText("BuySkin_Error_Dianjuan"), false, 1.5f, null, new object[0]);
                            return;

                        case CS_ADDHEROSKIN_ERRCODE.CS_ADDHEROSKIN_OTHER:
                            Singleton<CUIManager>.GetInstance().OpenTips(instance.GetText("BuySkin_Error_Other"), false, 1.5f, null, new object[0]);
                            return;

                        case CS_ADDHEROSKIN_ERRCODE.CS_ADDHEROSKIN_RANKGRADE:
                            Singleton<CUIManager>.GetInstance().OpenTips(instance.GetText("BuySkin_Error_RankGrade"), false, 1.5f, null, new object[0]);
                            return;

                        default:
                            Singleton<CUIManager>.GetInstance().OpenTips(iResult.ToString(), false, 1.5f, null, new object[0]);
                            return;
                    }
                }
            }
        }

        private void OnBuyPcik_factoyShopTipsCancelForm(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.tag3 == 1)
            {
                OpenBuyHeroForm(uiEvent.m_srcFormScript, uiEvent.m_eventParams.heroId, new stPayInfoSet(), enUIEventID.None);
            }
            else if (uiEvent.m_eventParams.tag3 == 2)
            {
                OpenBuyHeroSkinForm(uiEvent.m_eventParams.heroSkinParam.heroId, uiEvent.m_eventParams.heroSkinParam.skinId, uiEvent.m_eventParams.heroSkinParam.isCanCharge, new stPayInfoSet(), enUIEventID.None);
            }
        }

        private void OnBuyPcik_factoyShopTipsForm(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.heroId > 0)
            {
                ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(uiEvent.m_eventParams.heroId);
                ResHeroShop shop = null;
                if (dataByKey != null)
                {
                    GameDataMgr.heroShopInfoDict.TryGetValue(dataByKey.dwCfgID, out shop);
                    if ((shop != null) && (shop.dwFactoryID > 0))
                    {
                        Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mall_Open_Factory_Shop_Tab);
                        CMallFactoryShopController.ShopProduct theSp = new CMallFactoryShopController.ShopProduct(GameDataMgr.specSaleDict[shop.dwFactoryID]);
                        Singleton<CMallFactoryShopController>.GetInstance().StartShopProduct(theSp);
                    }
                }
            }
            else if ((uiEvent.m_eventParams.heroSkinParam.heroId > 0) && (uiEvent.m_eventParams.heroSkinParam.skinId > 0))
            {
                ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(uiEvent.m_eventParams.heroSkinParam.heroId, uiEvent.m_eventParams.heroSkinParam.skinId);
                if (heroSkin != null)
                {
                    ResHeroSkinShop shop2 = null;
                    GameDataMgr.skinShopInfoDict.TryGetValue(heroSkin.dwID, out shop2);
                    if ((shop2 != null) && (shop2.dwFactoryID > 0))
                    {
                        Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mall_Open_Factory_Shop_Tab);
                        CMallFactoryShopController.ShopProduct product2 = new CMallFactoryShopController.ShopProduct(GameDataMgr.specSaleDict[shop2.dwFactoryID]);
                        Singleton<CMallFactoryShopController>.GetInstance().StartShopProduct(product2);
                    }
                }
            }
        }

        [MessageHandler(0x729)]
        public static void OnBuySkinForFriend(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (msg.stPkgData.stPresentSkinRsp.iResult != 0)
            {
                string strContent = Utility.ProtErrCodeToStr(0x729, msg.stPkgData.stPresentSkinRsp.iResult);
                Singleton<CUIManager>.GetInstance().OpenTips(strContent, false, 1.5f, null, new object[0]);
            }
        }

        private void OnCloseBuySkinForm(CUIEvent uiEvent)
        {
            Singleton<CHeroSelectNormalSystem>.instance.ResetHero3DObj();
        }

        private void OnFriendListElementEnable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            RefreshFriendListElementForGift(uiEvent.m_srcWidget, this.m_friendList[srcWidgetIndexInBelongedList], this.m_isBuySkinForFriend);
            CUIEventScript component = uiEvent.m_srcWidget.get_transform().FindChild("InviteButton").GetComponent<CUIEventScript>();
            if (this.m_isBuySkinForFriend)
            {
                component.m_onClickEventID = enUIEventID.HeroSkin_LeaveMsgForFriend;
                component.m_onClickEventParams.heroSkinParam.heroId = this.m_buyHeroIDForFriend;
                component.m_onClickEventParams.heroSkinParam.skinId = this.m_buySkinIDForFriend;
                component.m_onClickEventParams.commonUInt32Param1 = this.m_buyPriceForFriend;
                component.m_onClickEventParams.tag2 = srcWidgetIndexInBelongedList;
            }
            else
            {
                component.m_onClickEventID = enUIEventID.HeroView_LeaveMsgForFriend;
                component.m_onClickEventParams.commonUInt32Param1 = this.m_buyPriceForFriend;
                component.m_onClickEventParams.tag = (int) this.m_buyHeroIDForFriend;
                component.m_onClickEventParams.tag2 = srcWidgetIndexInBelongedList;
            }
        }

        public void OnHeroInfo_BuyHero(CUIEvent uiEvent)
        {
            enPayType tag = (enPayType) uiEvent.m_eventParams.tag;
            uint payValue = uiEvent.m_eventParams.commonUInt32Param1;
            uint heroId = uiEvent.m_eventParams.heroId;
            ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
            DebugHelper.Assert(dataByKey != null);
            CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
            uIEvent.m_eventID = enUIEventID.HeroView_ConfirmBuyHero;
            uIEvent.m_eventParams.heroId = heroId;
            switch (tag)
            {
                case enPayType.GoldCoin:
                    uIEvent.m_eventParams.tag = 1;
                    break;

                case enPayType.DianQuan:
                    uIEvent.m_eventParams.tag = 0;
                    break;

                case enPayType.Diamond:
                    uIEvent.m_eventParams.tag = 2;
                    break;

                case enPayType.DiamondAndDianQuan:
                    uIEvent.m_eventParams.tag = 3;
                    break;
            }
            CMallSystem.TryToPay(enPayPurpose.Buy, StringHelper.UTF8BytesToString(ref dataByKey.szName), tag, payValue, uIEvent.m_eventID, ref uIEvent.m_eventParams, enUIEventID.None, false, true, false);
        }

        private void OnHeroInfo_BuyHeroForFriend(CUIEvent uiEvent)
        {
            uiEvent.m_eventID = enUIEventID.HeroView_ConfirmBuyHeroForFriend;
            uint payValue = uiEvent.m_eventParams.commonUInt32Param1;
            int tag = uiEvent.m_eventParams.tag;
            ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey((long) tag);
            string tagStr = uiEvent.m_eventParams.tagStr;
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            if (srcFormScript != null)
            {
                InputField component = Utility.FindChild(srcFormScript.get_gameObject(), "Panel/msgContainer/InputField").GetComponent<InputField>();
                if (component != null)
                {
                    uiEvent.m_eventParams.tagStr1 = CUIUtility.RemoveEmoji(Utility.UTF8Convert(component.get_text()));
                }
                DebugHelper.Assert(dataByKey != null);
                if (dataByKey != null)
                {
                    string goodName = string.Format(Singleton<CTextManager>.GetInstance().GetText("BuyForFriendWithName"), dataByKey.szName, tagStr);
                    CMallSystem.TryToPay(enPayPurpose.Buy, goodName, enPayType.DianQuan, payValue, uiEvent.m_eventID, ref uiEvent.m_eventParams, enUIEventID.None, true, true, true);
                }
            }
        }

        private void OnHeroInfo_BuyHeroLeaveMsgForFriend(CUIEvent uiEvent)
        {
            uint heroId = uiEvent.m_eventParams.heroSkinParam.heroId;
            int friendIndex = uiEvent.m_eventParams.tag2;
            CUIFormScript form = Singleton<CUIManager>.GetInstance().OpenForm(s_leaveMsgPath, false, true);
            if (form != null)
            {
                this.InitLeaveMsgForFriendForm(form, false, heroId, 0, friendIndex);
            }
        }

        private void OnHeroInfo_BuyHeroSkinForFriend(CUIEvent uiEvent)
        {
            uiEvent.m_eventID = enUIEventID.HeroSkin_ConfirmBuyHeroSkinForFriend;
            uint payValue = uiEvent.m_eventParams.commonUInt32Param1;
            uint heroId = uiEvent.m_eventParams.heroSkinParam.heroId;
            uint skinId = uiEvent.m_eventParams.heroSkinParam.skinId;
            string tagStr = uiEvent.m_eventParams.tagStr;
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            if (srcFormScript != null)
            {
                InputField component = Utility.FindChild(srcFormScript.get_gameObject(), "Panel/msgContainer/InputField").GetComponent<InputField>();
                if (component != null)
                {
                    uiEvent.m_eventParams.tagStr1 = CUIUtility.RemoveEmoji(Utility.UTF8Convert(component.get_text()));
                }
                ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
                DebugHelper.Assert(heroSkin != null);
                if (heroSkin != null)
                {
                    string goodName = string.Format(Singleton<CTextManager>.GetInstance().GetText("BuyForFriendWithName"), heroSkin.szSkinName, tagStr);
                    CMallSystem.TryToPay(enPayPurpose.Buy, goodName, enPayType.DianQuan, payValue, uiEvent.m_eventID, ref uiEvent.m_eventParams, enUIEventID.None, true, true, true);
                }
            }
        }

        private void OnHeroInfo_BuySkinLeaveMsgForFriend(CUIEvent uiEvent)
        {
            uint heroId = uiEvent.m_eventParams.heroSkinParam.heroId;
            uint skinId = uiEvent.m_eventParams.heroSkinParam.skinId;
            int friendIndex = uiEvent.m_eventParams.tag2;
            ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
            if (heroSkin == null)
            {
                DebugHelper.Assert(heroSkin != null, "heroSkin is null");
            }
            else
            {
                CUIFormScript form = Singleton<CUIManager>.GetInstance().OpenForm(s_leaveMsgPath, false, true);
                if (form != null)
                {
                    this.InitLeaveMsgForFriendForm(form, true, heroId, skinId, friendIndex);
                }
            }
        }

        public void OnHeroInfo_ConfirmBuyHero(CUIEvent uiEvent)
        {
            int tag = uiEvent.m_eventParams.tag;
            ReqBuyHero(uiEvent.m_eventParams.heroId, tag);
        }

        private void OnHeroInfo_ConfirmBuyHeroForFriend(CUIEvent uiEvent)
        {
            CSecurePwdSystem.TryToValidate(enOpPurpose.BUY_HERO_FOR_FRIEND, enUIEventID.HeroView_SecurePwdConfirmBuyHeroForFriend, uiEvent.m_eventParams);
        }

        private void OnHeroInfo_ConfirmBuyHeroSkinForFriend(CUIEvent uiEvent)
        {
            CSecurePwdSystem.TryToValidate(enOpPurpose.BUY_SKIN_FOR_FRIEND, enUIEventID.HeroView_SecurePwdConfirmBuyHeroSkinForFriend, uiEvent.m_eventParams);
        }

        private void OnHeroInfo_OpenBuyHeroForFriend(CUIEvent uiEvent)
        {
            uint heroId = uiEvent.m_eventParams.heroId;
            CUIFormScript form = Singleton<CUIManager>.GetInstance().OpenForm(s_heroBuyFriendPath, false, true);
            if (form != null)
            {
                this.InitBuyForFriendForm(form, false, heroId, 0, uiEvent.m_eventParams.commonUInt64Param1, uiEvent.m_eventParams.commonUInt32Param1, uiEvent.m_eventParams.commonBool);
            }
        }

        private void OnHeroInfo_OpenBuyHeroSkinForFriend(CUIEvent uiEvent)
        {
            uint heroId = uiEvent.m_eventParams.heroSkinParam.heroId;
            uint skinId = uiEvent.m_eventParams.heroSkinParam.skinId;
            CUIFormScript form = Singleton<CUIManager>.GetInstance().OpenForm(s_heroBuyFriendPath, false, true);
            if (form != null)
            {
                this.InitBuyForFriendForm(form, true, heroId, skinId, uiEvent.m_eventParams.commonUInt64Param1, uiEvent.m_eventParams.commonUInt32Param1, uiEvent.m_eventParams.commonBool);
            }
        }

        private void OnHeroInfo_SearchFriend(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            CUIListScript component = srcFormScript.get_transform().Find("Panel/List").GetComponent<CUIListScript>();
            InputField field = srcFormScript.get_transform().Find("Panel/SearchFriend/InputField").GetComponent<InputField>();
            if (field != null)
            {
                ListView<COMDT_FRIEND_INFO> allFriend = Singleton<CFriendContoller>.GetInstance().model.GetAllFriend(true);
                if (field.get_text() != string.Empty)
                {
                    ListView<COMDT_FRIEND_INFO> view2 = allFriend;
                    allFriend = new ListView<COMDT_FRIEND_INFO>();
                    for (int i = 0; i < view2.Count; i++)
                    {
                        COMDT_FRIEND_INFO item = view2[i];
                        if (StringHelper.UTF8BytesToString(ref item.szUserName).Contains(field.get_text()))
                        {
                            allFriend.Add(item);
                        }
                    }
                }
                bool bSkin = uiEvent.m_eventParams.friendHeroSkinPar.bSkin;
                uint heroId = uiEvent.m_eventParams.friendHeroSkinPar.heroId;
                uint skinId = uiEvent.m_eventParams.friendHeroSkinPar.skinId;
                uint price = uiEvent.m_eventParams.friendHeroSkinPar.price;
                if (allFriend.Count == 0)
                {
                    Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Friend_SearchNoResult"), false, 1.5f, null, new object[0]);
                }
                this.UpdateFriendList(ref allFriend, ref component, bSkin, heroId, skinId, price, 0L, 0, false);
            }
        }

        private void OnHeroInfo_SecurePwdConfirmBuyHeroForFriend(CUIEvent uiEvent)
        {
            int tag = uiEvent.m_eventParams.tag;
            COMDT_ACNT_UNIQ friendUin = new COMDT_ACNT_UNIQ();
            friendUin.ullUid = uiEvent.m_eventParams.commonUInt64Param1;
            friendUin.dwLogicWorldId = uiEvent.m_eventParams.tagUInt;
            string pwd = uiEvent.m_eventParams.pwd;
            string msg = uiEvent.m_eventParams.tagStr1;
            ReqBuyHeroForFriend((uint) tag, ref friendUin, pwd, msg);
        }

        private void OnHeroInfo_SecurePwdConfirmBuySkinForFriend(CUIEvent uiEvent)
        {
            uint heroId = uiEvent.m_eventParams.heroSkinParam.heroId;
            uint skinId = uiEvent.m_eventParams.heroSkinParam.skinId;
            COMDT_ACNT_UNIQ friendUin = new COMDT_ACNT_UNIQ();
            friendUin.ullUid = uiEvent.m_eventParams.commonUInt64Param1;
            friendUin.dwLogicWorldId = uiEvent.m_eventParams.tagUInt;
            string pwd = uiEvent.m_eventParams.pwd;
            string msg = uiEvent.m_eventParams.tagStr1;
            ReqBuySkinForFriend(heroId, skinId, ref friendUin, pwd, msg);
        }

        public void OnHeroSkin_Buy(CUIEvent uiEvent)
        {
            enPayType tag = (enPayType) uiEvent.m_eventParams.tag;
            uint payValue = uiEvent.m_eventParams.commonUInt32Param1;
            uint heroId = uiEvent.m_eventParams.heroSkinParam.heroId;
            uint skinId = uiEvent.m_eventParams.heroSkinParam.skinId;
            bool isCanCharge = uiEvent.m_eventParams.heroSkinParam.isCanCharge;
            string goodName = StringHelper.UTF8BytesToString(ref CSkinInfo.GetHeroSkin(heroId, skinId).szSkinName);
            CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
            uIEvent.m_eventID = enUIEventID.HeroSkin_BuyConfirm;
            uIEvent.m_eventParams.heroSkinParam.heroId = heroId;
            uIEvent.m_eventParams.heroSkinParam.skinId = skinId;
            uIEvent.m_eventParams.tag = (int) tag;
            uIEvent.m_eventParams.commonUInt32Param1 = payValue;
            CMallSystem.TryToPay(enPayPurpose.Buy, goodName, tag, payValue, uIEvent.m_eventID, ref uIEvent.m_eventParams, enUIEventID.None, false, isCanCharge, false);
        }

        public void OnHeroSkinBuyConfirm(CUIEvent uiEvent)
        {
            enPayType tag = (enPayType) uiEvent.m_eventParams.tag;
            uint heroId = uiEvent.m_eventParams.heroSkinParam.heroId;
            uint skinId = uiEvent.m_eventParams.heroSkinParam.skinId;
            BUY_HEROSKIN_TYPE buyType = BUY_HEROSKIN_TYPE.BUY_HEROSKIN_TYPE_DIAMOND;
            switch (tag)
            {
                case enPayType.DianQuan:
                    buyType = BUY_HEROSKIN_TYPE.BUY_HEROSKIN_TYPE_COUPONS;
                    break;

                case enPayType.Diamond:
                    buyType = BUY_HEROSKIN_TYPE.BUY_HEROSKIN_TYPE_DIAMOND;
                    break;

                case enPayType.DiamondAndDianQuan:
                    buyType = BUY_HEROSKIN_TYPE.BUY_HEROSKIN_TYPE_MIXPAY;
                    break;

                default:
                    buyType = BUY_HEROSKIN_TYPE.BUY_HEROSKIN_TYPE_DIAMOND;
                    break;
            }
            ReqBuyHeroSkin(heroId, skinId, buyType, false);
        }

        private void OnOpenBuyHeroForm(CUIEvent uiEvent)
        {
            ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(uiEvent.m_eventParams.heroId);
            ResHeroShop shop = null;
            if (dataByKey != null)
            {
                GameDataMgr.heroShopInfoDict.TryGetValue(dataByKey.dwCfgID, out shop);
                if ((shop != null) && (shop.dwFactoryID > 0))
                {
                    ResSpecSale sale = null;
                    GameDataMgr.specSaleDict.TryGetValue(shop.dwFactoryID, out sale);
                    if (sale != null)
                    {
                        int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
                        if ((currentUTCTime >= sale.dwOnTimeGen) && (currentUTCTime < sale.dwOffTimeGen))
                        {
                            CUseable useable = CUseableManager.CreateUseable((COM_ITEM_TYPE) sale.dwSpecSaleType, sale.dwSpecSaleId, 0);
                            if (useable != null)
                            {
                                string strContent = string.Format(Singleton<CTextManager>.GetInstance().GetText("FactoyBuy_Tips_Confirm"), shop.szName, useable.m_name);
                                uiEvent.m_eventParams.tag3 = 1;
                                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent, enUIEventID.BuyPcik_factoyShopTipsForm, enUIEventID.BuyPcik_factoyShopTipsCancelForm, uiEvent.m_eventParams, "去看看", "不用了", false);
                                return;
                            }
                        }
                    }
                }
            }
            OpenBuyHeroForm(uiEvent.m_srcFormScript, uiEvent.m_eventParams.heroId, new stPayInfoSet(), enUIEventID.None);
        }

        private void OnOpenBuySkinForm(CUIEvent uiEvent)
        {
            ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(uiEvent.m_eventParams.heroSkinParam.heroId, uiEvent.m_eventParams.heroSkinParam.skinId);
            if (heroSkin != null)
            {
                ResHeroSkinShop shop = null;
                GameDataMgr.skinShopInfoDict.TryGetValue(heroSkin.dwID, out shop);
                if ((shop != null) && (shop.dwFactoryID > 0))
                {
                    ResSpecSale sale = null;
                    GameDataMgr.specSaleDict.TryGetValue(shop.dwFactoryID, out sale);
                    if (sale != null)
                    {
                        int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
                        if ((currentUTCTime >= sale.dwOnTimeGen) && (currentUTCTime < sale.dwOffTimeGen))
                        {
                            CUseable useable = CUseableManager.CreateUseable((COM_ITEM_TYPE) sale.dwSpecSaleType, sale.dwSpecSaleId, 0);
                            if (useable != null)
                            {
                                string strContent = string.Format(Singleton<CTextManager>.GetInstance().GetText("FactoyBuy_Tips_Confirm"), shop.szSkinName, useable.m_name);
                                uiEvent.m_eventParams.tag3 = 2;
                                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent, enUIEventID.BuyPcik_factoyShopTipsForm, enUIEventID.BuyPcik_factoyShopTipsCancelForm, uiEvent.m_eventParams, "去看看", "不用了", false);
                                return;
                            }
                        }
                    }
                }
            }
            OpenBuyHeroSkinForm(uiEvent.m_eventParams.heroSkinParam.heroId, uiEvent.m_eventParams.heroSkinParam.skinId, uiEvent.m_eventParams.heroSkinParam.isCanCharge, new stPayInfoSet(), enUIEventID.None);
        }

        private void OnUseSkinExpCard(CUIEvent uiEvent)
        {
            ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(uiEvent.m_eventParams.heroSkinParam.heroId, uiEvent.m_eventParams.heroSkinParam.skinId);
            if (heroSkin != null)
            {
                CBagSystem.UseSkinExpCard(heroSkin.dwID);
            }
        }

        public static void OpenBuyHeroForm(CUIFormScript srcform, uint heroId, stPayInfoSet payInfoSet, enUIEventID btnClickEventID = 0)
        {
            CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm(s_heroBuyFormPath, false, true);
            if (formScript != null)
            {
                ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
                if (dataByKey != null)
                {
                    formScript.get_transform().Find("heroInfoPanel/title/Text").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Hero_Buy_Title"));
                    GameObject item = formScript.get_transform().Find("heroInfoPanel/heroItem").get_gameObject();
                    Text component = item.get_transform().Find("heroNameText").GetComponent<Text>();
                    CUICommonSystem.SetHeroItemImage(formScript, item, StringHelper.UTF8BytesToString(ref dataByKey.szImagePath), enHeroHeadType.enBust, false, true);
                    component.set_text(StringHelper.UTF8BytesToString(ref dataByKey.szName));
                    GameObject pricePanel = formScript.get_transform().Find("heroInfoPanel/heroPricePanel").get_gameObject();
                    if (payInfoSet.m_payInfoCount > 0)
                    {
                        SetHeroBuyPricePanel(formScript, pricePanel, ref payInfoSet, heroId, btnClickEventID);
                    }
                    else
                    {
                        ResHeroPromotion resPromotion = CHeroDataFactory.CreateHeroData(heroId).promotion();
                        stPayInfoSet payInfoSetOfGood = new stPayInfoSet();
                        payInfoSetOfGood = CMallSystem.GetPayInfoSetOfGood(dataByKey, resPromotion);
                        SetHeroBuyPricePanel(formScript, pricePanel, ref payInfoSetOfGood, heroId, btnClickEventID);
                    }
                    Transform transform = formScript.get_transform().Find("heroInfoPanel/buyForFriendBtn");
                    if (transform != null)
                    {
                        if (ShouldShowBuyForFriend(false, heroId, 0, btnClickEventID == enUIEventID.Mall_Mystery_On_Buy_Item))
                        {
                            transform.get_gameObject().CustomSetActive(true);
                            CUIEventScript script2 = transform.GetComponent<CUIEventScript>();
                            if (script2 != null)
                            {
                                script2.m_onClickEventParams.heroId = heroId;
                            }
                        }
                        else
                        {
                            transform.get_gameObject().CustomSetActive(false);
                        }
                    }
                }
            }
        }

        public static void OpenBuyHeroSkinForm(uint heroId, uint skinId, bool isCanCharge, stPayInfoSet payInfoSet, enUIEventID btnClickEventID = 0)
        {
            CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm(s_buyHeroSkinFormPath, false, true);
            if (formScript != null)
            {
                ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
                if (heroSkin != null)
                {
                    Transform transform = formScript.get_gameObject().get_transform().Find("Panel");
                    Image component = transform.Find("skinBgImage/skinIconImage").GetComponent<Image>();
                    string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_BustHero_Dir, StringHelper.UTF8BytesToString(ref heroSkin.szSkinPicID));
                    component.SetSprite(prefabPath, formScript, true, false, false, true);
                    transform.Find("skinNameText").GetComponent<Text>().set_text(StringHelper.UTF8BytesToString(ref heroSkin.szSkinName));
                    GameObject widget = formScript.GetWidget(2);
                    CSkinInfo.GetHeroSkinProp(heroId, skinId, ref CHeroInfoSystem2.s_propArr, ref CHeroInfoSystem2.s_propPctArr, ref CHeroInfoSystem2.s_propImgArr);
                    int num = 0;
                    int num2 = 0x25;
                    if ((CHeroInfoSystem2.s_propArr == null) || (CHeroInfoSystem2.s_propPctArr == null))
                    {
                        num = 0;
                    }
                    else
                    {
                        for (int j = 0; j < num2; j++)
                        {
                            if ((CHeroInfoSystem2.s_propArr[j] > 0) || (CHeroInfoSystem2.s_propPctArr[j] > 0))
                            {
                                num++;
                            }
                        }
                    }
                    CUIListScript script2 = widget.GetComponent<CUIListScript>();
                    int cnt = 0;
                    bool flag = CSkinInfo.GetSkinFeatureCnt(heroId, skinId, out cnt);
                    int amount = num + cnt;
                    script2.SetElementAmount(amount);
                    SetSkinListProp(widget, ref CHeroInfoSystem2.s_propArr, ref CHeroInfoSystem2.s_propPctArr, ref CHeroInfoSystem2.s_propImgArr);
                    for (int i = 0; i < cnt; i++)
                    {
                        CUIListElementScript elemenet = script2.GetElemenet(i + num);
                        GameObject obj3 = elemenet.GetWidget(0);
                        string str2 = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_SkinFeature_Dir, heroSkin.astFeature[i].szIconPath);
                        obj3.GetComponent<Image>().SetSprite(str2, elemenet.m_belongedFormScript, true, false, false, false);
                        elemenet.GetWidget(1).GetComponent<Text>().set_text(heroSkin.astFeature[i].szDesc);
                    }
                    CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                    if (payInfoSet.m_payInfoCount == 0)
                    {
                        ResSkinPromotion resPromotion = new ResSkinPromotion();
                        resPromotion = CSkinInfo.GetSkinPromotion(heroId, skinId);
                        payInfoSet = CMallSystem.GetPayInfoSetOfGood(heroSkin, resPromotion);
                    }
                    Transform skinPricePanel = transform.Find("skinPricePanel");
                    GameObject buyButtonGo = formScript.GetWidget(1);
                    GameObject obj6 = formScript.GetWidget(3);
                    if (payInfoSet.m_payInfoCount > 0)
                    {
                        SetSkinPricePanel(formScript, skinPricePanel, ref payInfoSet.m_payInfos[0]);
                        if (masterRoleInfo != null)
                        {
                            if (!masterRoleInfo.IsHaveHero(heroId, false))
                            {
                                if (buyButtonGo != null)
                                {
                                    Transform transform3 = buyButtonGo.get_transform().Find("Text");
                                    if (transform3 != null)
                                    {
                                        Text text2 = transform3.GetComponent<Text>();
                                        if (text2 != null)
                                        {
                                            text2.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_Text_1"));
                                        }
                                    }
                                    CUIEventScript script4 = buyButtonGo.GetComponent<CUIEventScript>();
                                    if (script4 != null)
                                    {
                                        stUIEventParams eventParams = new stUIEventParams();
                                        eventParams.openHeroFormPar.heroId = heroId;
                                        eventParams.openHeroFormPar.skinId = skinId;
                                        eventParams.openHeroFormPar.openSrc = enHeroFormOpenSrc.SkinBuyClick;
                                        script4.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenForm, eventParams);
                                    }
                                }
                            }
                            else if (buyButtonGo != null)
                            {
                                CUIEventScript script5 = buyButtonGo.GetComponent<CUIEventScript>();
                                if (script5 != null)
                                {
                                    CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                                    if (btnClickEventID == enUIEventID.None)
                                    {
                                        uIEvent.m_eventID = enUIEventID.HeroSkin_Buy;
                                    }
                                    else
                                    {
                                        uIEvent.m_eventID = btnClickEventID;
                                    }
                                    uIEvent.m_eventParams.tag = (int) payInfoSet.m_payInfos[0].m_payType;
                                    uIEvent.m_eventParams.commonUInt32Param1 = payInfoSet.m_payInfos[0].m_payValue;
                                    uIEvent.m_eventParams.heroSkinParam.heroId = heroId;
                                    uIEvent.m_eventParams.heroSkinParam.skinId = skinId;
                                    uIEvent.m_eventParams.heroSkinParam.isCanCharge = isCanCharge;
                                    script5.SetUIEvent(enUIEventType.Click, uIEvent.m_eventID, uIEvent.m_eventParams);
                                }
                            }
                        }
                    }
                    if (obj6 != null)
                    {
                        if (ShouldShowBuyForFriend(true, heroId, skinId, btnClickEventID == enUIEventID.Mall_Mystery_On_Buy_Item))
                        {
                            obj6.CustomSetActive(true);
                            CUIEventScript script6 = obj6.GetComponent<CUIEventScript>();
                            if (script6 != null)
                            {
                                script6.m_onClickEventParams.heroSkinParam.heroId = heroId;
                                script6.m_onClickEventParams.heroSkinParam.skinId = skinId;
                            }
                        }
                        else
                        {
                            obj6.CustomSetActive(false);
                        }
                    }
                    GameObject rankLimitTextGo = formScript.GetWidget(0);
                    SetRankLimitWidgets(heroId, skinId, rankLimitTextGo, buyButtonGo);
                }
            }
        }

        public static void OpenBuyHeroSkinForm3D(uint heroId, uint skinId, bool isCanCharge)
        {
            if (skinId != 0)
            {
                CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm(s_buyHeroSkin3DFormPath, false, true);
                if (formScript != null)
                {
                    ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
                    if (heroSkin != null)
                    {
                        Transform transform = formScript.get_transform().Find("Panel");
                        transform.Find("skinNameText").GetComponent<Text>().set_text(StringHelper.UTF8BytesToString(ref heroSkin.szSkinName));
                        GameObject listObj = transform.Find("Panel_Prop/List_Prop").get_gameObject();
                        CSkinInfo.GetHeroSkinProp(heroId, skinId, ref CHeroInfoSystem2.s_propArr, ref CHeroInfoSystem2.s_propPctArr, ref CHeroInfoSystem2.s_propImgArr);
                        CUICommonSystem.SetListProp(listObj, ref CHeroInfoSystem2.s_propArr, ref CHeroInfoSystem2.s_propPctArr);
                        CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                        if (masterRoleInfo == null)
                        {
                            DebugHelper.Assert(false, "OpenBuyHeroSkinForm3D role is null");
                        }
                        else
                        {
                            Transform transform2 = transform.Find("BtnGroup/useExpCardButton");
                            if (transform2 != null)
                            {
                                transform2.get_gameObject().CustomSetActive(false);
                                if (CBagSystem.CanUseSkinExpCard(heroSkin.dwID))
                                {
                                    transform2.get_gameObject().CustomSetActive(true);
                                    CUIEventScript script2 = transform2.GetComponent<CUIEventScript>();
                                    if (script2 != null)
                                    {
                                        stUIEventParams eventParams = new stUIEventParams();
                                        eventParams.heroSkinParam.heroId = heroId;
                                        eventParams.heroSkinParam.skinId = skinId;
                                        script2.SetUIEvent(enUIEventType.Click, enUIEventID.HeroSkin_OnUseSkinExpCard, eventParams);
                                    }
                                }
                            }
                            Transform transform3 = transform.Find("pricePanel");
                            Transform transform4 = transform.Find("getPathText");
                            Transform transform5 = transform.Find("BtnGroup/buyButton");
                            if ((transform3 != null) && (transform5 != null))
                            {
                                transform3.get_gameObject().CustomSetActive(false);
                                transform5.get_gameObject().CustomSetActive(false);
                            }
                            if (transform4 != null)
                            {
                                transform4.get_gameObject().CustomSetActive(false);
                            }
                            if (masterRoleInfo.IsHaveHero(heroId, false) && CSkinInfo.IsCanBuy(heroId, skinId))
                            {
                                stPayInfoSet skinPayInfoSet = CSkinInfo.GetSkinPayInfoSet(heroSkin.dwHeroID, heroSkin.dwSkinID);
                                if (skinPayInfoSet.m_payInfoCount <= 0)
                                {
                                    return;
                                }
                                if ((transform3 != null) && (transform5 != null))
                                {
                                    transform3.get_gameObject().CustomSetActive(true);
                                    transform5.get_gameObject().CustomSetActive(true);
                                    Transform transform6 = transform3.Find("costImage");
                                    if (transform6 != null)
                                    {
                                        Image image = transform6.get_gameObject().GetComponent<Image>();
                                        if (image != null)
                                        {
                                            image.SetSprite(CMallSystem.GetPayTypeIconPath(skinPayInfoSet.m_payInfos[0].m_payType), formScript, true, false, false, false);
                                        }
                                    }
                                    Transform transform7 = transform3.Find("priceText");
                                    if (transform7 != null)
                                    {
                                        Text text2 = transform7.get_gameObject().GetComponent<Text>();
                                        if (text2 != null)
                                        {
                                            text2.set_text(skinPayInfoSet.m_payInfos[0].m_payValue.ToString());
                                        }
                                    }
                                    if (transform5 != null)
                                    {
                                        CUIEventScript script3 = transform5.get_gameObject().GetComponent<CUIEventScript>();
                                        if (script3 != null)
                                        {
                                            CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                                            uIEvent.m_eventID = enUIEventID.HeroSkin_Buy;
                                            uIEvent.m_eventParams.tag = (int) skinPayInfoSet.m_payInfos[0].m_payType;
                                            uIEvent.m_eventParams.commonUInt32Param1 = skinPayInfoSet.m_payInfos[0].m_payValue;
                                            uIEvent.m_eventParams.heroSkinParam.heroId = heroId;
                                            uIEvent.m_eventParams.heroSkinParam.skinId = skinId;
                                            uIEvent.m_eventParams.heroSkinParam.isCanCharge = isCanCharge;
                                            script3.SetUIEvent(enUIEventType.Click, uIEvent.m_eventID, uIEvent.m_eventParams);
                                        }
                                    }
                                }
                            }
                            else if (transform4 != null)
                            {
                                transform4.get_gameObject().CustomSetActive(true);
                                if (masterRoleInfo.IsHaveHero(heroId, false))
                                {
                                    transform4.GetComponent<Text>().set_text(CHeroInfoSystem2.GetSkinCannotBuyStr(heroSkin));
                                }
                                else
                                {
                                    transform4.GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("HeroSelect_GetHeroFirstTip"));
                                }
                            }
                            CUI3DImageScript component = transform.Find("3DImage").get_gameObject().GetComponent<CUI3DImageScript>();
                            ObjNameData data = CUICommonSystem.GetHeroPrefabPath(heroId, (int) skinId, true);
                            GameObject model = component.AddGameObject(data.ObjectName, false, false);
                            if (model != null)
                            {
                                if (data.ActorInfo != null)
                                {
                                    model.get_transform().set_localScale(new Vector3(data.ActorInfo.LobbyScale, data.ActorInfo.LobbyScale, data.ActorInfo.LobbyScale));
                                }
                                DynamicShadow.EnableDynamicShow(component.get_gameObject(), true);
                                CHeroAnimaSystem instance = Singleton<CHeroAnimaSystem>.GetInstance();
                                instance.Set3DModel(model);
                                instance.InitAnimatList();
                                instance.InitAnimatSoundList(heroId, skinId);
                                instance.OnModePlayAnima("Come");
                            }
                            GameObject widget = formScript.GetWidget(0);
                            GameObject buyButtonGo = formScript.GetWidget(1);
                            SetRankLimitWidgets(heroId, skinId, widget, buyButtonGo);
                        }
                    }
                }
            }
        }

        public static void RefreshFriendListElementForGift(GameObject element, COMDT_FRIEND_INFO friend, bool bSkin)
        {
            CInviteView.UpdateFriendListElementBase(element, ref friend);
            Transform transform = element.get_transform().Find("Gender");
            if (transform != null)
            {
                COM_SNSGENDER bGender = (COM_SNSGENDER) friend.bGender;
                transform.get_gameObject().CustomSetActive(bGender != COM_SNSGENDER.COM_SNSGENDER_NONE);
                switch (bGender)
                {
                    case COM_SNSGENDER.COM_SNSGENDER_MALE:
                        CUIUtility.SetImageSprite(transform.GetComponent<Image>(), string.Format("{0}icon/Ico_boy", "UGUI/Sprite/Dynamic/"), null, true, false, false, false);
                        break;

                    case COM_SNSGENDER.COM_SNSGENDER_FEMALE:
                        CUIUtility.SetImageSprite(transform.GetComponent<Image>(), string.Format("{0}icon/Ico_girl", "UGUI/Sprite/Dynamic/"), null, true, false, false, false);
                        break;
                }
            }
        }

        public static void ReqBuyHero(uint HeroId, int BuyType)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x719);
            msg.stPkgData.stBuyHeroReq.dwHeroID = HeroId;
            msg.stPkgData.stBuyHeroReq.bBuyType = (byte) BuyType;
            IHeroData data = CHeroDataFactory.CreateHeroData(HeroId);
            if (data != null)
            {
                if (data.promotion() != null)
                {
                    msg.stPkgData.stBuyHeroReq.bIsPromotion = Convert.ToByte(true);
                }
                else
                {
                    msg.stPkgData.stBuyHeroReq.bIsPromotion = 0;
                }
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
            }
        }

        public static void ReqBuyHeroForFriend(uint heroId, ref COMDT_ACNT_UNIQ friendUin, string pwd = "", string msg = "")
        {
            CSPkg pkg = NetworkModule.CreateDefaultCSPKG(0x726);
            pkg.stPkgData.stPresentHeroReq.stFriendUin = friendUin;
            pkg.stPkgData.stPresentHeroReq.dwHeroID = heroId;
            StringHelper.StringToUTF8Bytes(pwd, ref pkg.stPkgData.stPresentHeroReq.szPswdInfo);
            StringHelper.StringToUTF8Bytes(msg, ref pkg.stPkgData.stPresentHeroReq.szPresentMsg);
            IHeroData data = CHeroDataFactory.CreateHeroData(heroId);
            if (data != null)
            {
                if (data.promotion() != null)
                {
                    pkg.stPkgData.stPresentHeroReq.bIsPromotion = Convert.ToByte(true);
                }
                else
                {
                    pkg.stPkgData.stPresentHeroReq.bIsPromotion = 0;
                }
            }
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref pkg, true);
        }

        public static void ReqBuyHeroSkin(uint heroId, uint skinId, BUY_HEROSKIN_TYPE buyType, bool isSendGameSvr = false)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x71b);
            msg.stPkgData.stBuyHeroSkinReq.dwHeroID = heroId;
            msg.stPkgData.stBuyHeroSkinReq.dwSkinID = skinId;
            msg.stPkgData.stBuyHeroSkinReq.bBuyType = (byte) buyType;
            ResSkinPromotion promotion = new ResSkinPromotion();
            stPayInfoSet set = new stPayInfoSet();
            if (CSkinInfo.GetSkinPromotion(heroId, skinId) != null)
            {
                msg.stPkgData.stBuyHeroSkinReq.bIsPromotion = Convert.ToByte(true);
            }
            else
            {
                msg.stPkgData.stBuyHeroSkinReq.bIsPromotion = 0;
            }
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void ReqBuySkinForFriend(uint heroId, uint skinId, ref COMDT_ACNT_UNIQ friendUin, string pwd = "", string msg = "")
        {
            CSPkg pkg = NetworkModule.CreateDefaultCSPKG(0x728);
            pkg.stPkgData.stPresentSkinReq.stFriendUin = friendUin;
            pkg.stPkgData.stPresentSkinReq.dwSkinID = CSkinInfo.GetSkinCfgId(heroId, skinId);
            StringHelper.StringToUTF8Bytes(pwd, ref pkg.stPkgData.stPresentSkinReq.szPswdInfo);
            StringHelper.StringToUTF8Bytes(msg, ref pkg.stPkgData.stPresentSkinReq.szPresentMsg);
            if (CSkinInfo.GetSkinPromotion(heroId, skinId) != null)
            {
                pkg.stPkgData.stPresentSkinReq.bIsPromotion = Convert.ToByte(true);
            }
            else
            {
                pkg.stPkgData.stPresentSkinReq.bIsPromotion = 0;
            }
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref pkg, true);
        }

        public static void SetHeroBuyPricePanel(CUIFormScript formScript, GameObject pricePanel, ref stPayInfoSet payInfoSet, uint heroID, enUIEventID btnClickEventID = 0)
        {
            if ((null != formScript) && (pricePanel != null))
            {
                Transform payInfoPanel = pricePanel.get_transform().Find("pnlCoinBuy");
                Transform transform2 = pricePanel.get_transform().Find("pnlDiamondBuy");
                Transform transform3 = pricePanel.get_transform().Find("Text");
                if ((payInfoPanel != null) && (transform2 != null))
                {
                    payInfoPanel.get_gameObject().CustomSetActive(false);
                    transform2.get_gameObject().CustomSetActive(false);
                    if (transform3 != null)
                    {
                        transform3.get_gameObject().CustomSetActive(payInfoSet.m_payInfoCount > 1);
                    }
                    GameObject obj2 = Utility.FindChild(formScript.get_gameObject(), "heroInfoPanel/BtnGroup/coinBuyBtn");
                    GameObject obj3 = Utility.FindChild(formScript.get_gameObject(), "heroInfoPanel/BtnGroup/diamondBuyBtn");
                    obj2.CustomSetActive(false);
                    obj3.CustomSetActive(false);
                    GameObject obj4 = null;
                    for (int i = 0; i < payInfoSet.m_payInfoCount; i++)
                    {
                        stPayInfo payInfo = payInfoSet.m_payInfos[i];
                        if (payInfoSet.m_payInfos[i].m_payType == enPayType.GoldCoin)
                        {
                            payInfoPanel.get_gameObject().CustomSetActive(true);
                            SetPayInfoPanel(formScript, payInfoPanel, ref payInfo, heroID, btnClickEventID);
                            obj4 = obj2;
                        }
                        else
                        {
                            transform2.get_gameObject().CustomSetActive(true);
                            SetPayInfoPanel(formScript, transform2, ref payInfo, heroID, btnClickEventID);
                            obj4 = obj3;
                        }
                        obj4.CustomSetActive(true);
                        if (obj4 != null)
                        {
                            Text componetInChild = Utility.GetComponetInChild<Text>(obj4, "Text");
                            if (componetInChild != null)
                            {
                                componetInChild.set_text(CMallSystem.GetPriceTypeBuyString(payInfo.m_payType));
                            }
                            CUIEventScript component = obj4.GetComponent<CUIEventScript>();
                            stUIEventParams eventParams = new stUIEventParams();
                            eventParams.tag = (int) payInfo.m_payType;
                            eventParams.commonUInt32Param1 = payInfo.m_payValue;
                            eventParams.heroId = heroID;
                            if (btnClickEventID != enUIEventID.None)
                            {
                                component.SetUIEvent(enUIEventType.Click, btnClickEventID, eventParams);
                            }
                            else
                            {
                                component.SetUIEvent(enUIEventType.Click, enUIEventID.HeroView_BuyHero, eventParams);
                            }
                        }
                    }
                }
            }
        }

        public static void SetPayCostIcon(CUIFormScript formScript, Transform costIcon, enPayType payType)
        {
            if ((null != formScript) && (null != costIcon))
            {
                Image component = costIcon.GetComponent<Image>();
                if (component != null)
                {
                    component.SetSprite(CMallSystem.GetPayTypeIconPath(payType), formScript, true, false, false, false);
                }
            }
        }

        public static void SetPayCostTypeText(Transform costTypeText, enPayType payType)
        {
            if (costTypeText != null)
            {
                Text component = costTypeText.GetComponent<Text>();
                if (component != null)
                {
                    component.set_text(CMallSystem.GetPayTypeText(payType));
                }
            }
        }

        public static void SetPayCurrentPrice(Transform currentPrice, uint payValue)
        {
            if (currentPrice != null)
            {
                Text component = currentPrice.GetComponent<Text>();
                if (component != null)
                {
                    component.set_text(payValue.ToString());
                }
            }
        }

        private static void SetPayInfoPanel(CUIFormScript formScript, Transform payInfoPanel, ref stPayInfo payInfo, uint heroID, enUIEventID btnClickEventID)
        {
            Transform costIcon = payInfoPanel.Find("costImage");
            SetPayCostIcon(formScript, costIcon, payInfo.m_payType);
            SetPayCostTypeText(payInfoPanel.Find("costTypeText"), payInfo.m_payType);
            Transform transform3 = payInfoPanel.Find("costPanel");
            if (transform3 != null)
            {
                Transform transform4 = transform3.Find("oldPricePanel");
                if (transform4 != null)
                {
                    transform4.get_gameObject().CustomSetActive(payInfo.m_oriValue != payInfo.m_payValue);
                    SetPayOldPrice(transform4.Find("oldPriceText"), payInfo.m_oriValue);
                }
                SetPayCurrentPrice(transform3.Find("costText"), payInfo.m_payValue);
            }
        }

        public static void SetPayOldPrice(Transform oldPrice, uint oriValue)
        {
            if (oldPrice != null)
            {
                Text component = oldPrice.GetComponent<Text>();
                if (component != null)
                {
                    component.set_text(oriValue.ToString());
                }
            }
        }

        private static void SetRankLimitWidgets(uint heroId, uint skinId, GameObject rankLimitTextGo, GameObject buyButtonGo)
        {
            RES_RANK_LIMIT_TYPE res_rank_limit_type;
            byte num;
            bool flag;
            ulong num2;
            if (CSkinInfo.IsBuyForbiddenForRankBigGrade(heroId, skinId, out res_rank_limit_type, out num, out num2, out flag))
            {
                CUICommonSystem.SetButtonEnable(buyButtonGo.GetComponent<Button>(), false, false, true);
            }
            if (CSkinInfo.IsCanBuy(heroId, skinId) && flag)
            {
                rankLimitTextGo.CustomSetActive(true);
                Text component = rankLimitTextGo.GetComponent<Text>();
                component.set_text(string.Empty);
                string rankBigGradeName = CLadderView.GetRankBigGradeName(num);
                switch (res_rank_limit_type)
                {
                    case RES_RANK_LIMIT_TYPE.RES_RANK_LIMIT_CURGRADE:
                    {
                        string[] args = new string[] { rankBigGradeName };
                        component.set_text(Singleton<CTextManager>.GetInstance().GetText("Ladder_Buy_Skin_Ladder_Limit_Current_Grade", args));
                        return;
                    }
                    case RES_RANK_LIMIT_TYPE.RES_RANK_LIMIT_SEASONGRADE:
                    {
                        string[] textArray2 = new string[] { rankBigGradeName };
                        component.set_text(Singleton<CTextManager>.GetInstance().GetText("Ladder_Buy_Skin_Ladder_Limit_Season_Highest_Grade", textArray2));
                        return;
                    }
                    case RES_RANK_LIMIT_TYPE.RES_RANK_LIMIT_MAXGRADE:
                    {
                        string[] textArray3 = new string[] { rankBigGradeName };
                        component.set_text(Singleton<CTextManager>.GetInstance().GetText("Ladder_Buy_Skin_Ladder_Limit_History_Highest_Grade", textArray3));
                        return;
                    }
                    case RES_RANK_LIMIT_TYPE.RES_RANK_LIMIT_HISTORYGRADE:
                    {
                        string[] textArray4 = new string[] { Singleton<CLadderSystem>.GetInstance().GetLadderSeasonName(num2), rankBigGradeName };
                        component.set_text(Singleton<CTextManager>.GetInstance().GetText("Ladder_Buy_Skin_Ladder_Limit_History_Grade", textArray4));
                        return;
                    }
                }
            }
            else
            {
                rankLimitTextGo.CustomSetActive(false);
            }
        }

        private static void SetSkinListProp(GameObject listObj, ref int[] propArr, ref int[] propPctArr, ref string[] propImgArr)
        {
            int num = 0x25;
            if (((listObj == null) || (propArr == null)) || (((propPctArr == null) || (propArr.Length != num)) || (propPctArr.Length != num)))
            {
                listObj.CustomSetActive(false);
            }
            else
            {
                int index = 0;
                int num3 = 0;
                for (index = 0; index < num; index++)
                {
                    if ((propArr[index] > 0) || (propPctArr[index] > 0))
                    {
                        num3++;
                    }
                }
                listObj.CustomSetActive(true);
                CUIListScript component = listObj.GetComponent<CUIListScript>();
                num3 = 0;
                for (index = 0; index < num; index++)
                {
                    if ((propArr[index] > 0) || (propPctArr[index] > 0))
                    {
                        CUIListElementScript elemenet = component.GetElemenet(num3);
                        if (elemenet != null)
                        {
                            Image image = elemenet.GetWidget(0).GetComponent<Image>();
                            if ((image != null) && !string.IsNullOrEmpty(propImgArr[index]))
                            {
                                string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_SkinFeature_Dir, propImgArr[index]);
                                image.SetSprite(prefabPath, component.m_belongedFormScript, true, false, false, false);
                            }
                            Text text = elemenet.GetWidget(1).GetComponent<Text>();
                            if (text != null)
                            {
                                if (propArr[index] > 0)
                                {
                                    text.set_text(string.Format("{0} +{1}", CUICommonSystem.s_attNameList[index], propArr[index]));
                                }
                                else if (propPctArr[index] > 0)
                                {
                                    text.set_text(string.Format("{0} +{1}", CUICommonSystem.s_attNameList[index], CUICommonSystem.GetValuePercent(propPctArr[index])));
                                }
                            }
                            num3++;
                        }
                    }
                }
            }
        }

        private static void SetSkinPricePanel(CUIFormScript formScript, Transform skinPricePanel, ref stPayInfo payInfo)
        {
            if ((null != formScript) && (null != skinPricePanel))
            {
                Transform costIcon = skinPricePanel.Find("costImage");
                SetPayCostIcon(formScript, costIcon, payInfo.m_payType);
                SetPayCostTypeText(skinPricePanel.Find("costTypeText"), payInfo.m_payType);
                Transform transform3 = skinPricePanel.Find("costPanel");
                if (transform3 != null)
                {
                    Transform transform4 = transform3.Find("oldPricePanel");
                    if (transform4 != null)
                    {
                        transform4.get_gameObject().CustomSetActive(payInfo.m_oriValue != payInfo.m_payValue);
                        SetPayOldPrice(transform4.Find("oldPriceText"), payInfo.m_oriValue);
                    }
                    SetPayCurrentPrice(transform3.Find("costText"), payInfo.m_payValue);
                }
            }
        }

        public static bool ShouldShowBuyForFriend(bool bSkin, uint heroId, uint skinId = 0, bool forceNotShow = false)
        {
            if (!forceNotShow)
            {
                if (bSkin)
                {
                    ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
                    DebugHelper.Assert(heroSkin != null);
                    if (heroSkin != null)
                    {
                        stPayInfoSet payInfoSetOfGood = CMallSystem.GetPayInfoSetOfGood(heroSkin);
                        return ((Singleton<CFunctionUnlockSys>.GetInstance().FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_PRESENTHERO) && GameDataMgr.IsSkinCanBuyNow(heroSkin.dwID)) && GameDataMgr.IsSkinCanBuyForFriend(heroSkin.dwID));
                    }
                }
                else
                {
                    ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
                    DebugHelper.Assert(dataByKey != null);
                    if (dataByKey != null)
                    {
                        stPayInfoSet set2 = CMallSystem.GetPayInfoSetOfGood(dataByKey);
                        bool flag = false;
                        for (int i = 0; i < set2.m_payInfoCount; i++)
                        {
                            if (((set2.m_payInfos[i].m_payType == enPayType.Diamond) || (set2.m_payInfos[i].m_payType == enPayType.DianQuan)) || (set2.m_payInfos[i].m_payType == enPayType.DiamondAndDianQuan))
                            {
                                flag = true;
                                break;
                            }
                        }
                        return (((Singleton<CFunctionUnlockSys>.GetInstance().FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_PRESENTHERO) && flag) && GameDataMgr.IsHeroAvailableAtShop(dataByKey.dwCfgID)) && GameDataMgr.IsHeroCanBuyForFriend(dataByKey.dwCfgID));
                    }
                }
            }
            return false;
        }

        public override void UnInit()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BuyPcik_factoyShopTipsForm, new CUIEventManager.OnUIEventHandler(this.OnBuyPcik_factoyShopTipsForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BuyPcik_factoyShopTipsCancelForm, new CUIEventManager.OnUIEventHandler(this.OnBuyPcik_factoyShopTipsCancelForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroSkin_LeaveMsgForFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_BuySkinLeaveMsgForFriend));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroView_LeaveMsgForFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_BuyHeroLeaveMsgForFriend));
        }

        private void UpdateFriendList(ref ListView<COMDT_FRIEND_INFO> allFriends, ref CUIListScript list, bool bSkin, uint heroId, uint skinId, uint price, ulong friendUid = 0, uint worldId = 0, bool isSns = false)
        {
            if ((friendUid > 0L) && (worldId > 0))
            {
                this.detailFriendList.Clear();
                for (int i = 0; i < allFriends.Count; i++)
                {
                    if ((allFriends[i].stUin.ullUid == friendUid) && (allFriends[i].stUin.dwLogicWorldId == worldId))
                    {
                        this.detailFriendList.Add(allFriends[i]);
                        this.m_friendList = this.detailFriendList;
                        break;
                    }
                }
            }
            else
            {
                this.m_friendList = allFriends;
            }
            this.m_isBuySkinForFriend = bSkin;
            this.m_buyHeroIDForFriend = heroId;
            this.m_buySkinIDForFriend = skinId;
            this.m_buyPriceForFriend = price;
            list.SetElementAmount(0);
            list.SetElementAmount(this.m_friendList.Count);
        }

        public enum enBuyHeroSkin3DFormWidget
        {
            Buy_Rank_Limit_Text,
            Buy_Button
        }

        public enum enBuyHeroSkinFormWidget
        {
            Buy_Rank_Limit_Text,
            Buy_Button,
            Feature_List,
            Buy_For_Friend_Button
        }
    }
}

