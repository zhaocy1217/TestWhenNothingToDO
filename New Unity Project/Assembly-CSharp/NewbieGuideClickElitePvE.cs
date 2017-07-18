using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickElitePvE : NewbieGuideBaseScript
{
    protected override void Initialize()
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CAdventureSys.ADVENTURE_SELECT_FORM);
        form.get_gameObject().get_transform().Find("ChapterList").get_gameObject().GetComponent<CUIStepListScript>().SelectElementImmediately(0);
        GameObject baseGo = form.get_transform().Find("TopCommon/Panel_Menu/ListMenu/ScrollRect/Content/ListElement_1").get_gameObject();
        base.AddHighLightGameObject(baseGo, true, form, true);
        base.Initialize();
    }

    protected override bool IsDelegateClickEvent()
    {
        return true;
    }
}

