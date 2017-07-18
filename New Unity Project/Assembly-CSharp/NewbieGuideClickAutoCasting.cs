using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

internal class NewbieGuideClickAutoCasting : NewbieGuideBaseScript
{
    protected override void Initialize()
    {
    }

    protected override bool IsDelegateClickEvent()
    {
        return true;
    }

    protected override void Update()
    {
        if (base.isInitialize)
        {
            base.Update();
        }
        else
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSettingsSys.SETTING_FORM);
            if (form != null)
            {
                GameObject baseGo = form.get_transform().FindChild("OpSetting/CastToggle/OpAutoCast").get_gameObject();
                if (baseGo.get_activeInHierarchy())
                {
                    base.AddHighLightGameObject(baseGo, true, form, true);
                    base.Initialize();
                }
            }
        }
    }
}

