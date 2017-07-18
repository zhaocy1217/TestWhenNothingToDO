namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class CSecurePwdSystem : Singleton<CSecurePwdSystem>
    {
        public PwdCloseStatus CloseStatus;
        public int CloseTimerSeq;
        public PwdStatus EnableStatus;
        private uint m_CloseTime;
        public static string[] s_OpRiskNameKeys = new string[] { "SecurePwd_OpRisk_Use_Battle_Record_Card", "SecurePwd_OpRisk_Buy_Hero_For_Friend", "SecurePwd_OpRisk_Buy_Skin_For_Friend", "SecurePwd_OpRisk_Break_Symbol", "SecurePwd_OpRisk_Guild_Transfer_Chairman", "SecurePwd_OpRisk_Sale_Symbol" };
        public static string sApplyClosePwdFormPath = "UGUI/Form/System/SecurePwd/Form_ApplyClose.prefab";
        public static string sClosePwdFormPath = "UGUI/Form/System/SecurePwd/Form_ClosePwd.prefab";
        public static string sModifyPwdFormPath = "UGUI/Form/System/SecurePwd/Form_ModifyPwd.prefab";
        public static string sSetPwdFormPath = "UGUI/Form/System/SecurePwd/Form_SetPwd.prefab";
        public static string sValidatePwdFormPath = "UGUI/Form/System/SecurePwd/Form_Validate.prefab";

        private void ApplyCancelForceCloseConfirm(CUIEvent uiEvent)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x775);
            CSPKG_ACNT_PSWDINFO_FORCECAL cspkg_acnt_pswdinfo_forcecal = new CSPKG_ACNT_PSWDINFO_FORCECAL();
            cspkg_acnt_pswdinfo_forcecal.bOpt = 1;
            msg.stPkgData.stAcntPswdFroceCalReq = cspkg_acnt_pswdinfo_forcecal;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public void ApplyClose(string pwd)
        {
            if (this.EnableStatus != PwdStatus.Enable)
            {
                Singleton<EventRouter>.GetInstance().BroadCastEvent<OpResult>(EventID.SECURE_PWD_CLOSE_RESULT, OpResult.Illegal);
            }
            else if (!Check(pwd))
            {
                Singleton<EventRouter>.GetInstance().BroadCastEvent<OpResult>(EventID.SECURE_PWD_CLOSE_RESULT, OpResult.Invalid);
            }
            else
            {
                this.ReqClose(pwd);
            }
        }

        public void ApplyForceClose()
        {
            if (this.EnableStatus != PwdStatus.Enable)
            {
                Singleton<EventRouter>.GetInstance().BroadCastEvent<OpResult>(EventID.SECURE_PWD_FORCE_CLOSE_RESULT, OpResult.Illegal);
            }
            else
            {
                float dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 13).dwConfValue;
                float num2 = dwConfValue / 86000f;
                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(string.Format(Singleton<CTextManager>.GetInstance().GetText("SecurePwd_Force_Close_Desc"), num2.ToString("F0")), enUIEventID.SecurePwd_ApplyForceCloseConfirm, enUIEventID.None, false);
            }
        }

        private void ApplyForceCloseConfirm(CUIEvent uiEvent)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x773);
            CSPKG_ACNT_PSWDINFO_FORCE cspkg_acnt_pswdinfo_force = new CSPKG_ACNT_PSWDINFO_FORCE();
            cspkg_acnt_pswdinfo_force.bOpt = 1;
            msg.stPkgData.stAcntPswdForceReq = cspkg_acnt_pswdinfo_force;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static bool Check(string pwd)
        {
            if (pwd == null)
            {
                return false;
            }
            char[] trimChars = new char[] { '\r', '\n' };
            pwd = pwd.TrimEnd(trimChars);
            if (string.IsNullOrEmpty(pwd))
            {
                return false;
            }
            string pattern = "^[a-zA-Z\\d-`=\\\\\\[\\];',./~!@#$%^&*()_+|{}:\"<>?]{6,12}$";
            if (!Regex.IsMatch(pwd, pattern))
            {
                return false;
            }
            if (Encoding.UTF8.GetBytes(pwd).Length > 0x40)
            {
                return false;
            }
            return true;
        }

        private void HandleOpError(OpResult err)
        {
            switch (err)
            {
                case OpResult.Fail:
                    Singleton<CUIManager>.GetInstance().OpenTips("设置密码失败,请稍后重试", false, 1.5f, null, new object[0]);
                    break;

                case OpResult.NotEuqal:
                    Singleton<CUIManager>.GetInstance().OpenTips("两次密码不相同，请重新输入", false, 1.5f, null, new object[0]);
                    break;

                case OpResult.Invalid:
                    Singleton<CUIManager>.GetInstance().OpenTips("密码不符合规则，请重新输入", false, 1.5f, null, new object[0]);
                    break;

                case OpResult.Illegal:
                    Singleton<CUIManager>.GetInstance().OpenTips("当前操作不允许", false, 1.5f, null, new object[0]);
                    break;
            }
        }

        public override void Init()
        {
            this.EnableStatus = PwdStatus.Disable;
            this.CloseStatus = PwdCloseStatus.Close;
            this.m_CloseTime = 0;
            this.CloseTimerSeq = -1;
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SecurePwd_ApplyForceCloseConfirm, new CUIEventManager.OnUIEventHandler(this.ApplyForceCloseConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SecurePwd_ApplyCancelForceCloseConfirm, new CUIEventManager.OnUIEventHandler(this.ApplyCancelForceCloseConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SecurePwd_OpenSetPwdForm, new CUIEventManager.OnUIEventHandler(this.OpenSetForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SecurePwd_OpenModifyPwdForm, new CUIEventManager.OnUIEventHandler(this.OpenModifyForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SecurePwd_OpenClosePwdForm, new CUIEventManager.OnUIEventHandler(this.OpenCloseForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SecurePwd_OpenApplyClosePwdForm, new CUIEventManager.OnUIEventHandler(this.OpenApplyClosePwdForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SecurePwd_OnSetPwd, new CUIEventManager.OnUIEventHandler(this.OnSetPwd));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SecurePwd_OnModifyPwd, new CUIEventManager.OnUIEventHandler(this.OnModifyPwd));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SecurePwd_OnClosePwd, new CUIEventManager.OnUIEventHandler(this.OnClosePwd));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SecurePwd_OnApplyClose, new CUIEventManager.OnUIEventHandler(this.OnApplyClosePwd));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SecurePwd_OnCancelApplyClose, new CUIEventManager.OnUIEventHandler(this.OnCancelApplyClosePwd));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SecurePwd_OnOpCancel, new CUIEventManager.OnUIEventHandler(this.OnOpCancel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SecurePwd_OnValidateConfirm, new CUIEventManager.OnUIEventHandler(this.OnValidateConfirm));
            Singleton<EventRouter>.GetInstance().AddEventHandler<OpResult>(EventID.SECURE_PWD_SET_RESULT, new Action<OpResult>(this.OnPwdSet));
            Singleton<EventRouter>.GetInstance().AddEventHandler<OpResult>(EventID.SECURE_PWD_MODIFY_RESULT, new Action<OpResult>(this.OnPwdModify));
            Singleton<EventRouter>.GetInstance().AddEventHandler<OpResult>(EventID.SECURE_PWD_CLOSE_RESULT, new Action<OpResult>(this.OnPwdClose));
            Singleton<EventRouter>.GetInstance().AddEventHandler<OpResult>(EventID.SECURE_PWD_FORCE_CLOSE_RESULT, new Action<OpResult>(this.OnPwdForceClose));
            Singleton<EventRouter>.GetInstance().AddEventHandler<OpResult>(EventID.SECURE_PWD_CANCEL_FORCE_CLOSE_RESULT, new Action<OpResult>(this.OnPwdCancelForceClose));
            Singleton<EventRouter>.GetInstance().AddEventHandler<OpResult>(EventID.SECURE_PWD_VALIDATE_RESULT, new Action<OpResult>(this.OnPwdValidate));
        }

        public void ModifyPwd(string newPwd, string newPwdConfirm, string oldPwd)
        {
            if (this.EnableStatus != PwdStatus.Enable)
            {
                Singleton<EventRouter>.GetInstance().BroadCastEvent<OpResult>(EventID.SECURE_PWD_MODIFY_RESULT, OpResult.Illegal);
            }
            else if (newPwd != newPwdConfirm)
            {
                Singleton<EventRouter>.GetInstance().BroadCastEvent<OpResult>(EventID.SECURE_PWD_MODIFY_RESULT, OpResult.NotEuqal);
            }
            else if (!Check(oldPwd))
            {
                Singleton<CUIManager>.GetInstance().OpenTips("旧密码错误，请重新输入", false, 1.5f, null, new object[0]);
            }
            else if (!Check(newPwd) || !Check(newPwdConfirm))
            {
                Singleton<CUIManager>.GetInstance().OpenTips("新密码不符合规则，请重新输入", false, 1.5f, null, new object[0]);
            }
            else
            {
                this.ReqModify(newPwd, oldPwd);
            }
        }

        private void OnApplyClosePwd(CUIEvent uiEvent)
        {
            if ((this.EnableStatus != PwdStatus.Disable) && (this.CloseStatus != PwdCloseStatus.Open))
            {
                this.ReqApplyClose();
            }
        }

        private void OnCancelApplyClosePwd(CUIEvent uiEvent)
        {
            if ((this.EnableStatus != PwdStatus.Disable) && (this.CloseStatus != PwdCloseStatus.Close))
            {
                this.ReqCancelApplyClose();
            }
        }

        private void OnClosePwd(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(sClosePwdFormPath);
            if (form != null)
            {
                Transform transform = form.get_transform().Find("pnlBg/Panel_Main/PwdContainer/InputField");
                if (transform == null)
                {
                    DebugHelper.Assert(false, "Password InputField is null!");
                }
                else
                {
                    InputField component = transform.GetComponent<InputField>();
                    if (component == null)
                    {
                        DebugHelper.Assert(false, "Password InputField Component is null!");
                    }
                    else
                    {
                        string pwd = component.get_text();
                        this.ApplyClose(pwd);
                    }
                }
            }
        }

        private void OnModifyPwd(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(sModifyPwdFormPath);
            if (form != null)
            {
                Transform transform = form.get_transform().Find("pnlBg/Panel_Main/OldPwdContainer/InputField");
                Transform transform2 = form.get_transform().Find("pnlBg/Panel_Main/PwdContainer/InputField");
                Transform transform3 = form.get_transform().Find("pnlBg/Panel_Main/PwdConfirmContainer/InputField");
                if (transform == null)
                {
                    DebugHelper.Assert(false, "Old Password InputField is null!");
                }
                else if (transform2 == null)
                {
                    DebugHelper.Assert(false, "Password InputField is null!");
                }
                else if (transform3 == null)
                {
                    DebugHelper.Assert(false, "ConfirmPassword InputField is null!");
                }
                else
                {
                    InputField component = transform.GetComponent<InputField>();
                    InputField field2 = transform2.GetComponent<InputField>();
                    InputField field3 = transform3.GetComponent<InputField>();
                    if (component == null)
                    {
                        DebugHelper.Assert(false, "Old Password InputField Component is null!");
                    }
                    else if (field2 == null)
                    {
                        DebugHelper.Assert(false, "Password InputField Component is null!");
                    }
                    else if (field3 == null)
                    {
                        DebugHelper.Assert(false, "ConfirmPassword InputField Component is null!");
                    }
                    else
                    {
                        string oldPwd = component.get_text();
                        string newPwd = field2.get_text();
                        string newPwdConfirm = field3.get_text();
                        this.ModifyPwd(newPwd, newPwdConfirm, oldPwd);
                    }
                }
            }
        }

        private void OnOpCancel(CUIEvent uiEvent)
        {
            Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.SECURE_PWD_OP_CANCEL);
        }

        private void OnPwdCancelForceClose(OpResult rs)
        {
            if (rs == OpResult.Success)
            {
                Singleton<CUIManager>.GetInstance().CloseForm(sApplyClosePwdFormPath);
                Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("SecurePwd_Cancel_Force_Close_Success"), false, 1.5f, null, new object[0]);
                Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.SECURE_PWD_STATUS_CHANGE);
            }
            else
            {
                this.HandleOpError(rs);
            }
        }

        private void OnPwdClose(OpResult rs)
        {
            if (rs == OpResult.Success)
            {
                Singleton<CUIManager>.GetInstance().CloseForm(sClosePwdFormPath);
                Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("SecurePwd_Close_Pwd_Success"), false, 1.5f, null, new object[0]);
                Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.SECURE_PWD_STATUS_CHANGE);
            }
            else
            {
                this.HandleOpError(rs);
            }
        }

        private void OnPwdForceClose(OpResult rs)
        {
            if (rs == OpResult.Success)
            {
                Singleton<CUIManager>.GetInstance().CloseForm(sApplyClosePwdFormPath);
                Singleton<CUIManager>.GetInstance().CloseForm(sModifyPwdFormPath);
                Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("SecurePwd_Apply_Force_Close_Success"), false, 1.5f, null, new object[0]);
                Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.SECURE_PWD_STATUS_CHANGE);
            }
            else
            {
                this.HandleOpError(rs);
            }
        }

        private void OnPwdModify(OpResult rs)
        {
            if (rs == OpResult.Success)
            {
                Singleton<CUIManager>.GetInstance().CloseForm(sModifyPwdFormPath);
                Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("SecurePwd_Modify_Pwd_Success"), false, 1.5f, null, new object[0]);
                Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.SECURE_PWD_STATUS_CHANGE);
            }
            else
            {
                this.HandleOpError(rs);
            }
        }

        private void OnPwdSet(OpResult rs)
        {
            if (rs == OpResult.Success)
            {
                Singleton<CUIManager>.GetInstance().CloseForm(sSetPwdFormPath);
                Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("SecurePwd_Set_Pwd_Success"), false, 1.5f, null, new object[0]);
                Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.SECURE_PWD_STATUS_CHANGE);
            }
            else
            {
                this.HandleOpError(rs);
            }
        }

        private void OnPwdValidate(OpResult rs)
        {
            this.HandleOpError(rs);
        }

        private void OnSetPwd(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(sSetPwdFormPath);
            if (form != null)
            {
                Transform transform = form.get_transform().Find("pnlBg/Panel_Main/PwdContainer/InputField");
                Transform transform2 = form.get_transform().Find("pnlBg/Panel_Main/PwdConfirmContainer/InputField");
                if (transform == null)
                {
                    DebugHelper.Assert(false, "Password InputField is null!");
                }
                else if (transform2 == null)
                {
                    DebugHelper.Assert(false, "ConfirmPassword InputField is null!");
                }
                else
                {
                    InputField component = transform.GetComponent<InputField>();
                    InputField field2 = transform2.GetComponent<InputField>();
                    if (component == null)
                    {
                        DebugHelper.Assert(false, "Password InputField Component is null!");
                    }
                    else if (field2 == null)
                    {
                        DebugHelper.Assert(false, "ConfirmPassword InputField Component is null!");
                    }
                    else
                    {
                        string newPwd = component.get_text();
                        string newPwdConfirm = field2.get_text();
                        this.SetPwd(newPwd, newPwdConfirm);
                    }
                }
            }
        }

        private void OnValidateConfirm(CUIEvent uiEvent)
        {
            stUIEventParams eventParams = uiEvent.m_eventParams;
            if (this.EnableStatus == PwdStatus.Enable)
            {
                CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(sValidatePwdFormPath);
                if (form == null)
                {
                    return;
                }
                Transform transform = form.get_transform().Find("pnlBg/Panel_Main/PwdContainer/InputField");
                if (transform == null)
                {
                    DebugHelper.Assert(false, "Password InputField is null!");
                    return;
                }
                InputField component = transform.GetComponent<InputField>();
                if (component == null)
                {
                    DebugHelper.Assert(false, "Password InputField Component is null!");
                    return;
                }
                string pwd = component.get_text();
                if (!Check(pwd))
                {
                    Singleton<EventRouter>.GetInstance().BroadCastEvent<OpResult>(EventID.SECURE_PWD_VALIDATE_RESULT, OpResult.Invalid);
                    return;
                }
                Singleton<CUIManager>.GetInstance().CloseForm(sValidatePwdFormPath);
                eventParams.pwd = pwd;
            }
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(eventParams.srcUIEventID, eventParams);
        }

        private void OpenApplyClosePwdForm(CUIEvent uiEvent)
        {
            if (this.EnableStatus != PwdStatus.Disable)
            {
                CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(sApplyClosePwdFormPath, false, true);
                if (script == null)
                {
                    DebugHelper.Assert(false, "Apply Close Pwd Form Is Null");
                }
                else
                {
                    Transform transform = script.get_transform().Find("pnlBg/Panel_Main/LeftTime");
                    Transform transform2 = script.get_transform().Find("pnlBg/Panel_Main/Button");
                    Transform transform3 = script.get_transform().Find("pnlBg/Panel_Main/Desc");
                    if (transform == null)
                    {
                        DebugHelper.Assert(false, "SecurePwdSys left time trans is null");
                    }
                    else if (transform2 == null)
                    {
                        DebugHelper.Assert(false, "SecurePwdSys op btn trans is null");
                    }
                    else if (transform3 == null)
                    {
                        DebugHelper.Assert(false, "SecurePwdSys desc trans is null");
                    }
                    else
                    {
                        Text component = transform3.GetComponent<Text>();
                        if ((component != null) && GameDataMgr.svr2CltCfgDict.ContainsKey(13))
                        {
                            ResGlobalInfo info = new ResGlobalInfo();
                            if (GameDataMgr.svr2CltCfgDict.TryGetValue(13, out info))
                            {
                                float dwConfValue = info.dwConfValue;
                                float num2 = dwConfValue / 86000f;
                                component.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("SecurePwd_Force_Close_Desc"), num2.ToString("F0")));
                            }
                        }
                        switch (this.CloseStatus)
                        {
                            case PwdCloseStatus.Open:
                            {
                                transform.get_gameObject().CustomSetActive(true);
                                CUITimerScript script2 = transform.Find("Timer").GetComponent<CUITimerScript>();
                                if (script2 == null)
                                {
                                    DebugHelper.Assert(false, "SecurePwdSys left timer is null");
                                    return;
                                }
                                DateTime time = Utility.ToUtcTime2Local((long) CRoleInfo.GetCurrentUTCTime());
                                TimeSpan span = (TimeSpan) (Utility.ToUtcTime2Local((long) this.m_CloseTime) - time);
                                if (span.TotalSeconds > 0.0)
                                {
                                    script2.SetTotalTime((float) span.TotalSeconds);
                                    script2.ReStartTimer();
                                    CUIEventScript script3 = transform2.GetComponent<CUIEventScript>();
                                    if (script3 != null)
                                    {
                                        script3.SetUIEvent(enUIEventType.Up, enUIEventID.SecurePwd_OnCancelApplyClose);
                                    }
                                    Text componetInChild = Utility.GetComponetInChild<Text>(transform2.get_gameObject(), "Text");
                                    if (componetInChild != null)
                                    {
                                        componetInChild.set_text(Singleton<CTextManager>.GetInstance().GetText("SecurePwd_Cancel_Force_Close_Btn"));
                                    }
                                }
                                else
                                {
                                    this.EnableStatus = PwdStatus.Disable;
                                    this.CloseStatus = PwdCloseStatus.Close;
                                    this.CloseTime = 0;
                                    Singleton<CUIManager>.GetInstance().CloseForm(sApplyClosePwdFormPath);
                                    Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.SECURE_PWD_STATUS_CHANGE);
                                }
                                break;
                            }
                            case PwdCloseStatus.Close:
                            {
                                transform.get_gameObject().CustomSetActive(false);
                                CUIEventScript script4 = transform2.GetComponent<CUIEventScript>();
                                if (script4 != null)
                                {
                                    script4.SetUIEvent(enUIEventType.Up, enUIEventID.SecurePwd_OnApplyClose);
                                }
                                Text text3 = Utility.GetComponetInChild<Text>(transform2.get_gameObject(), "Text");
                                if (text3 != null)
                                {
                                    text3.set_text(Singleton<CTextManager>.GetInstance().GetText("SecurePwd_Force_Close_Btn"));
                                }
                                return;
                            }
                        }
                    }
                }
            }
        }

        private void OpenCloseForm(CUIEvent uiEvent)
        {
            if (this.EnableStatus != PwdStatus.Disable)
            {
                if (this.CloseStatus == PwdCloseStatus.Open)
                {
                    DateTime time = Utility.ToUtcTime2Local((long) CRoleInfo.GetCurrentUTCTime());
                    TimeSpan span = (TimeSpan) (Utility.ToUtcTime2Local((long) this.m_CloseTime) - time);
                    if ((span.TotalSeconds < 0.0) || (span.TotalSeconds < 2.0))
                    {
                        return;
                    }
                }
                switch (this.CloseStatus)
                {
                    case PwdCloseStatus.Open:
                        Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.SecurePwd_OpenApplyClosePwdForm);
                        break;

                    case PwdCloseStatus.Close:
                    {
                        CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(sClosePwdFormPath, false, true);
                        return;
                    }
                }
            }
        }

        private void OpenModifyForm(CUIEvent uiEvent)
        {
            if ((this.EnableStatus != PwdStatus.Disable) && (this.CloseStatus != PwdCloseStatus.Open))
            {
                CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(sModifyPwdFormPath, false, true);
            }
        }

        private void OpenSetForm(CUIEvent uiEvent)
        {
            if (this.EnableStatus != PwdStatus.Enable)
            {
                CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(sSetPwdFormPath, false, true);
            }
        }

        [MessageHandler(0x776)]
        public static void ReceiveCancelForceCloseRs(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (msg.stPkgData.stAcntPswdFroceCalRsp.iResult == 0)
            {
                Singleton<CSecurePwdSystem>.GetInstance().CloseStatus = PwdCloseStatus.Close;
                Singleton<CSecurePwdSystem>.GetInstance().CloseTime = 0;
                Singleton<EventRouter>.GetInstance().BroadCastEvent<OpResult>(EventID.SECURE_PWD_CANCEL_FORCE_CLOSE_RESULT, OpResult.Success);
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(0x776, msg.stPkgData.stAcntPswdFroceCalRsp.iResult), false, 1.5f, null, new object[0]);
            }
        }

        [MessageHandler(0x770)]
        public static void ReceiveCloseRs(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (msg.stPkgData.stAcntPswdCloseRsp.iResult == 0)
            {
                Singleton<CSecurePwdSystem>.GetInstance().EnableStatus = PwdStatus.Disable;
                Singleton<EventRouter>.GetInstance().BroadCastEvent<OpResult>(EventID.SECURE_PWD_CLOSE_RESULT, OpResult.Success);
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(0x770, msg.stPkgData.stAcntPswdCloseRsp.iResult), false, 1.5f, null, new object[0]);
            }
        }

        [MessageHandler(0x774)]
        public static void ReceiveForceCloseRs(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (msg.stPkgData.stAcntPswdForceRsp.iResult == 0)
            {
                Singleton<CSecurePwdSystem>.GetInstance().CloseStatus = PwdCloseStatus.Open;
                Singleton<CSecurePwdSystem>.GetInstance().CloseTime = msg.stPkgData.stAcntPswdForceRsp.dwForceTime;
                Singleton<EventRouter>.GetInstance().BroadCastEvent<OpResult>(EventID.SECURE_PWD_FORCE_CLOSE_RESULT, OpResult.Success);
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(0x774, msg.stPkgData.stAcntPswdForceRsp.iResult), false, 1.5f, null, new object[0]);
            }
        }

        [MessageHandler(0x772)]
        public static void ReceiveModifyRs(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (msg.stPkgData.stAcntPswdChgRsp.iResult == 0)
            {
                Singleton<EventRouter>.GetInstance().BroadCastEvent<OpResult>(EventID.SECURE_PWD_MODIFY_RESULT, OpResult.Success);
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(0x772, msg.stPkgData.stAcntPswdChgRsp.iResult), false, 1.5f, null, new object[0]);
            }
        }

        [MessageHandler(0x76e)]
        public static void ReceiveSetRs(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (msg.stPkgData.stAcntPswdOpenRsp.iResult == 0)
            {
                Singleton<CSecurePwdSystem>.GetInstance().EnableStatus = PwdStatus.Enable;
                Singleton<EventRouter>.GetInstance().BroadCastEvent<OpResult>(EventID.SECURE_PWD_SET_RESULT, OpResult.Success);
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(0x76e, msg.stPkgData.stAcntPswdOpenRsp.iResult), false, 1.5f, null, new object[0]);
            }
        }

        private void RefreshStatus(int seq)
        {
            this.m_CloseTime = 0;
            this.CloseTimerSeq = -1;
            this.CloseStatus = PwdCloseStatus.Close;
            this.EnableStatus = PwdStatus.Disable;
            Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.SECURE_PWD_STATUS_CHANGE);
        }

        private void ReqApplyClose()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x773);
            CSPKG_ACNT_PSWDINFO_FORCE cspkg_acnt_pswdinfo_force = new CSPKG_ACNT_PSWDINFO_FORCE();
            cspkg_acnt_pswdinfo_force.bOpt = 1;
            msg.stPkgData.stAcntPswdForceReq = cspkg_acnt_pswdinfo_force;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        private void ReqCancelApplyClose()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x775);
            CSPKG_ACNT_PSWDINFO_FORCECAL cspkg_acnt_pswdinfo_forcecal = new CSPKG_ACNT_PSWDINFO_FORCECAL();
            cspkg_acnt_pswdinfo_forcecal.bOpt = 0;
            msg.stPkgData.stAcntPswdFroceCalReq = cspkg_acnt_pswdinfo_forcecal;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        private void ReqClose(string pwd)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x76f);
            CSPKG_ACNT_PSWDINFO_CLOSE cspkg_acnt_pswdinfo_close = new CSPKG_ACNT_PSWDINFO_CLOSE();
            StringHelper.StringToUTF8Bytes(pwd, ref cspkg_acnt_pswdinfo_close.szPswdStr);
            msg.stPkgData.stAcntPswdCloseReq = cspkg_acnt_pswdinfo_close;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        private void ReqModify(string pwd, string oldPwd = new string())
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x771);
            CSPKG_ACNT_PSWDINFO_CHG cspkg_acnt_pswdinfo_chg = new CSPKG_ACNT_PSWDINFO_CHG();
            StringHelper.StringToUTF8Bytes(pwd, ref cspkg_acnt_pswdinfo_chg.szNewPswdStr);
            StringHelper.StringToUTF8Bytes(oldPwd, ref cspkg_acnt_pswdinfo_chg.szOldPswdStr);
            msg.stPkgData.stAcntPswdChgReq = cspkg_acnt_pswdinfo_chg;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        private void ReqSet(string pwd)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x76d);
            CSPKG_ACNT_PSWDINFO_OPEN cspkg_acnt_pswdinfo_open = new CSPKG_ACNT_PSWDINFO_OPEN();
            StringHelper.StringToUTF8Bytes(pwd, ref cspkg_acnt_pswdinfo_open.szPswdStr);
            msg.stPkgData.stAcntPswdOpenReq = cspkg_acnt_pswdinfo_open;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public void SetPwd(string newPwd, string newPwdConfirm)
        {
            if (this.EnableStatus != PwdStatus.Disable)
            {
                Singleton<EventRouter>.GetInstance().BroadCastEvent<OpResult>(EventID.SECURE_PWD_SET_RESULT, OpResult.Illegal);
            }
            else if (newPwd != newPwdConfirm)
            {
                Singleton<EventRouter>.GetInstance().BroadCastEvent<OpResult>(EventID.SECURE_PWD_SET_RESULT, OpResult.NotEuqal);
            }
            else if (!Check(newPwd) || !Check(newPwdConfirm))
            {
                Singleton<EventRouter>.GetInstance().BroadCastEvent<OpResult>(EventID.SECURE_PWD_SET_RESULT, OpResult.Invalid);
            }
            else
            {
                this.ReqSet(newPwd);
            }
        }

        public static void TryToValidate(enOpPurpose purpose, enUIEventID confirmEventID, [Optional] stUIEventParams confirmEventParams)
        {
            if (Singleton<CSecurePwdSystem>.GetInstance().EnableStatus == PwdStatus.Enable)
            {
                CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(sValidatePwdFormPath, false, true);
                if (script != null)
                {
                    Transform transform = script.get_transform().Find("pnlBg/Panel_Main/BtnConfirm");
                    Transform transform2 = script.get_transform().Find("pnlBg/Panel_Main/BtnCancel");
                    if ((transform != null) && (transform2 != null))
                    {
                        CUIEventScript component = transform.GetComponent<CUIEventScript>();
                        CUIEventScript script3 = transform2.GetComponent<CUIEventScript>();
                        if ((component != null) && (script3 != null))
                        {
                            confirmEventParams.srcUIEventID = confirmEventID;
                            component.SetUIEvent(enUIEventType.Up, enUIEventID.SecurePwd_OnValidateConfirm, confirmEventParams);
                        }
                    }
                }
            }
            else
            {
                confirmEventParams.srcUIEventID = confirmEventID;
                string text = Singleton<CTextManager>.GetInstance().GetText(s_OpRiskNameKeys[(int) purpose]);
                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.SecurePwd_OnValidateConfirm, enUIEventID.SecurePwd_OpenSetPwdForm, confirmEventParams, "暂时不用", "前往设置", true);
            }
        }

        public override void UnInit()
        {
            base.UnInit();
            this.CloseStatus = PwdCloseStatus.Close;
            this.m_CloseTime = 0;
            Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this.CloseTimerSeq);
            this.CloseTimerSeq = -1;
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SecurePwd_ApplyForceCloseConfirm, new CUIEventManager.OnUIEventHandler(this.ApplyForceCloseConfirm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SecurePwd_ApplyCancelForceCloseConfirm, new CUIEventManager.OnUIEventHandler(this.ApplyCancelForceCloseConfirm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SecurePwd_OpenSetPwdForm, new CUIEventManager.OnUIEventHandler(this.OpenSetForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SecurePwd_OpenModifyPwdForm, new CUIEventManager.OnUIEventHandler(this.OpenModifyForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SecurePwd_OpenClosePwdForm, new CUIEventManager.OnUIEventHandler(this.OpenCloseForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SecurePwd_OpenApplyClosePwdForm, new CUIEventManager.OnUIEventHandler(this.OpenApplyClosePwdForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SecurePwd_OnSetPwd, new CUIEventManager.OnUIEventHandler(this.OnSetPwd));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SecurePwd_OnModifyPwd, new CUIEventManager.OnUIEventHandler(this.OnModifyPwd));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SecurePwd_OnClosePwd, new CUIEventManager.OnUIEventHandler(this.OnClosePwd));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SecurePwd_OnApplyClose, new CUIEventManager.OnUIEventHandler(this.OnApplyClosePwd));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SecurePwd_OnCancelApplyClose, new CUIEventManager.OnUIEventHandler(this.OnCancelApplyClosePwd));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SecurePwd_OnOpCancel, new CUIEventManager.OnUIEventHandler(this.OnOpCancel));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SecurePwd_OnValidateConfirm, new CUIEventManager.OnUIEventHandler(this.OnValidateConfirm));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<OpResult>(EventID.SECURE_PWD_SET_RESULT, new Action<OpResult>(this.OnPwdSet));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<OpResult>(EventID.SECURE_PWD_MODIFY_RESULT, new Action<OpResult>(this.OnPwdModify));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<OpResult>(EventID.SECURE_PWD_CLOSE_RESULT, new Action<OpResult>(this.OnPwdClose));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<OpResult>(EventID.SECURE_PWD_FORCE_CLOSE_RESULT, new Action<OpResult>(this.OnPwdForceClose));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<OpResult>(EventID.SECURE_PWD_CANCEL_FORCE_CLOSE_RESULT, new Action<OpResult>(this.OnPwdCancelForceClose));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<OpResult>(EventID.SECURE_PWD_VALIDATE_RESULT, new Action<OpResult>(this.OnPwdValidate));
        }

        public uint CloseTime
        {
            get
            {
                return this.m_CloseTime;
            }
            set
            {
                this.m_CloseTime = value;
                if (this.m_CloseTime > 0)
                {
                    if (this.CloseTimerSeq > 0)
                    {
                        Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this.CloseTimerSeq);
                    }
                    DateTime time = Utility.ToUtcTime2Local((long) CRoleInfo.GetCurrentUTCTime());
                    TimeSpan span = (TimeSpan) (Utility.ToUtcTime2Local((long) this.m_CloseTime) - time);
                    if (span.TotalSeconds > 0.0)
                    {
                        long num = (long) (span.TotalSeconds * 1000.0);
                        if ((num > 0L) && (num < 0x7fffffffL))
                        {
                            this.CloseTimerSeq = Singleton<CTimerManager>.GetInstance().AddTimer((int) num, 1, new CTimer.OnTimeUpHandler(this.RefreshStatus));
                        }
                    }
                }
                else if (this.CloseTimerSeq > 0)
                {
                    Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this.CloseTimerSeq);
                }
            }
        }
    }
}

