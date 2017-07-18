namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using System;
    using UnityEngine;

    public class InBattle3DTouch
    {
        public void Clear()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_3DTouch_FullScreen, new CUIEventManager.OnUIEventHandler(this.OnBattle_3DTouch_FullScreen));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_3DTouch_FullScreen_Scene, new CUIEventManager.OnUIEventHandler(this.OnBattle_3DTouch_FullScreen_Scene));
        }

        public void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_3DTouch_FullScreen, new CUIEventManager.OnUIEventHandler(this.OnBattle_3DTouch_FullScreen));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_3DTouch_FullScreen_Scene, new CUIEventManager.OnUIEventHandler(this.OnBattle_3DTouch_FullScreen_Scene));
        }

        private void OnBattle_3DTouch_FullScreen(CUIEvent uievent)
        {
            if (GameSettings.Unity3DTouchEnable)
            {
                this.Porcess_CloseOtherForm();
            }
        }

        private void OnBattle_3DTouch_FullScreen_Scene(CUIEvent uievent)
        {
            if (GameSettings.Unity3DTouchEnable && (uievent.m_pointerEventData != null))
            {
                if ((uievent.m_pointerEventData.get_position().x <= (Screen.get_width() * 0.5f)) && (uievent.m_pointerEventData.get_position().x >= 0f))
                {
                    this.Process_MiniMap();
                }
                else
                {
                    this.Process_InBattleShortCutMsg();
                }
            }
        }

        public bool Porcess_CloseOtherForm()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CBattleEquipSystem.s_equipFormPath);
            if ((form != null) && (!form.IsHided() || form.GetComponent<Canvas>().get_enabled()))
            {
                Singleton<CUIManager>.instance.CloseForm(form);
                return true;
            }
            CUIFormScript formScript = Singleton<CUIManager>.instance.GetForm(CSettingsSys.SETTING_FORM);
            if ((formScript != null) && !formScript.IsHided())
            {
                Singleton<CUIManager>.instance.CloseForm(formScript);
                return true;
            }
            CUIFormScript script3 = Singleton<CUIManager>.instance.GetForm(BattleStatView.s_battleStateViewUIForm);
            if (((script3 != null) && (!script3.IsHided() || script3.GetComponent<Canvas>().get_enabled())) && (Singleton<CBattleSystem>.instance.BattleStatView != null))
            {
                Singleton<CBattleSystem>.instance.BattleStatView.Hide();
                return true;
            }
            return false;
        }

        public void Process_InBattleShortCutMsg()
        {
            if (Singleton<InBattleMsgMgr>.instance.m_shortcutChat != null)
            {
                Singleton<InBattleMsgMgr>.instance.m_shortcutChat.Send_Config_Chat(0);
            }
        }

        public void Process_MiniMap()
        {
            MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
            if ((theMinimapSys != null) && (theMinimapSys.CurMapType() == MinimapSys.EMapType.Mini))
            {
                Singleton<CUIEventManager>.instance.DispatchUIEvent(enUIEventID.BigMap_Open_BigMap);
            }
        }
    }
}

