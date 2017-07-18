namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class CSignalButton
    {
        private GameObject m_button;
        private Image m_buttonImage;
        private Image m_cooldownImage;
        private Image m_highLightImage;
        private Image m_iconImage;
        private uint m_maxCooldownTime;
        public int m_signalID;
        public ResSignalInfo m_signalInfo;
        private ulong m_startCooldownTimestamp;
        private CUIEventScript m_uiEventScript;

        public CSignalButton(GameObject button, int signalID)
        {
            this.m_signalID = signalID;
            if (button != null)
            {
                this.m_button = button;
                this.m_buttonImage = this.m_button.GetComponent<Image>();
                Transform transform = button.get_transform().FindChild("high");
                if (transform != null)
                {
                    this.m_highLightImage = transform.get_gameObject().GetComponent<Image>();
                }
                Transform transform2 = button.get_transform().FindChild("icon");
                if (transform2 != null)
                {
                    this.m_iconImage = transform2.get_gameObject().GetComponent<Image>();
                }
                Transform transform3 = button.get_transform().FindChild("cd");
                if (transform3 != null)
                {
                    this.m_cooldownImage = transform3.get_gameObject().GetComponent<Image>();
                }
                this.m_uiEventScript = button.GetComponent<CUIEventScript>();
            }
        }

        public void Disable()
        {
            if (this.m_button != null)
            {
                this.m_button.CustomSetActive(false);
            }
        }

        public void Initialize(CUIFormScript formScript)
        {
            this.m_signalInfo = GameDataMgr.signalDatabin.GetDataByKey((long) this.m_signalID);
            if (this.m_signalInfo != null)
            {
                if (this.m_buttonImage != null)
                {
                    this.m_buttonImage.SetSprite(CUIUtility.s_battleSignalPrefabDir + ((this.m_signalInfo.bSignalType != 0) ? "Signal_Btn_Voice.prefab" : "Signal_Btn_Normal.prefab"), formScript, true, false, false, false);
                }
                if (this.m_iconImage != null)
                {
                    this.m_iconImage.SetSprite(StringHelper.UTF8BytesToString(ref this.m_signalInfo.szUIIcon), formScript, true, false, false, false);
                }
                if (this.m_highLightImage != null)
                {
                    this.m_highLightImage.SetSprite(CUIUtility.s_battleSignalPrefabDir + ((this.m_signalInfo.bSignalType != 0) ? "Signal_Btn_Voice_Highlight.prefab" : "Signal_Btn_Normal_Highlight.prefab"), formScript, true, false, false, false);
                }
                if (this.m_uiEventScript != null)
                {
                    this.m_uiEventScript.m_onClickEventID = enUIEventID.Battle_OnSignalButtonClicked;
                    this.m_uiEventScript.m_onClickEventParams.tag = this.m_signalID;
                }
                this.SetHighLight(false);
                this.StartCooldown(0);
            }
        }

        public bool IsInCooldown()
        {
            return (this.m_startCooldownTimestamp > 0L);
        }

        public void SetHighLight(bool highLight)
        {
            if (this.m_highLightImage != null)
            {
                this.m_highLightImage.get_gameObject().CustomSetActive(highLight);
            }
        }

        public void StartCooldown(uint maxCooldownTime)
        {
            this.m_maxCooldownTime = maxCooldownTime;
            this.SetHighLight(false);
            if (this.m_cooldownImage != null)
            {
                if (maxCooldownTime > 0)
                {
                    this.m_cooldownImage.set_enabled(true);
                    this.m_cooldownImage.set_type(3);
                    this.m_cooldownImage.set_fillMethod(4);
                    this.m_cooldownImage.set_fillOrigin(2);
                    this.m_startCooldownTimestamp = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
                    this.m_cooldownImage.CustomFillAmount(1f);
                }
                else
                {
                    this.m_startCooldownTimestamp = 0L;
                    this.m_cooldownImage.set_enabled(false);
                }
            }
        }

        public void UpdateCooldown()
        {
            if ((this.m_startCooldownTimestamp != 0) && (this.m_maxCooldownTime != 0))
            {
                uint num = (uint) (Singleton<FrameSynchr>.GetInstance().LogicFrameTick - this.m_startCooldownTimestamp);
                if (num >= this.m_maxCooldownTime)
                {
                    this.m_startCooldownTimestamp = 0L;
                    if (this.m_cooldownImage != null)
                    {
                        this.m_cooldownImage.set_enabled(false);
                    }
                }
                else if (this.m_cooldownImage != null)
                {
                    this.m_cooldownImage.CustomFillAmount(((float) (this.m_maxCooldownTime - num)) / ((float) this.m_maxCooldownTime));
                }
            }
        }
    }
}

