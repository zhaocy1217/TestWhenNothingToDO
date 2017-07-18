using Assets.Scripts.UI;
using System;
using UnityEngine;

internal class NewbieGuideClickMysteryShop : NewbieGuideBaseScript
{
    protected override void Initialize()
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<CShopSystem>.GetInstance().sShopFormPath);
        GameObject baseGo = form.get_transform().FindChild("pnlShop/Tab/ScrollRect/Content/ListElement_1").get_gameObject();
        base.AddHighLightGameObject(baseGo, true, form, true);
        base.Initialize();
    }

    protected override bool IsDelegateClickEvent()
    {
        return true;
    }
}

