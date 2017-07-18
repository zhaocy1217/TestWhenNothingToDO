using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

internal class NewbieGuideClickSettingMenu : NewbieGuideBaseScript
{
    protected override void Initialize()
    {
    }

    protected override bool IsDelegateClickEvent()
    {
        return true;
    }

    public override bool IsTimeOutSkip()
    {
        return false;
    }

    protected override void Update()
    {
        if (base.isInitialize)
        {
            base.Update();
        }
        else
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(FightForm.s_battleUIForm);
            if (form != null)
            {
                GameObject baseGo = form.get_transform().FindChild("PanelBtn/MenuBtn").get_gameObject();
                if (baseGo.get_activeInHierarchy())
                {
                    base.AddHighLightGameObject(baseGo, true, form, true);
                    base.Initialize();
                }
            }
        }
    }
}

