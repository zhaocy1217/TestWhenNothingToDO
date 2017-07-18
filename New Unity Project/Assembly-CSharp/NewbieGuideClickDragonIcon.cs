using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickDragonIcon : NewbieGuideBaseScript
{
    protected override void Initialize()
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(FightForm.s_battleUIForm);
        GameObject baseGo = form.get_transform().Find("DragonInfo/Tuch").get_gameObject();
        base.AddHighLightGameObject(baseGo, true, form, true);
        base.Initialize();
    }

    protected override bool IsDelegateClickEvent()
    {
        return true;
    }
}

