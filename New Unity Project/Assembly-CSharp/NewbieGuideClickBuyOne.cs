using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickBuyOne : NewbieGuideBaseScript
{
    protected override void Initialize()
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
        CUIListScript component = form.get_transform().FindChild("TopCommon/Panel_Menu/ListMenu").get_gameObject().GetComponent<CUIListScript>();
        if (component != null)
        {
            component.MoveElementInScrollArea(0, true);
        }
        GameObject baseGo = form.get_transform().FindChild("pnlBodyBg/pnlLottery/pnlAction/pnlBuyOneFree/btnPanel/Button").get_gameObject();
        base.AddHighLightGameObject(baseGo, true, form, true);
        base.Initialize();
    }

    protected override bool IsDelegateClickEvent()
    {
        return true;
    }
}

