using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickMallMenu : NewbieGuideBaseScript
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
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<CMallSystem>.instance.sMallFormPath);
            if (form != null)
            {
                CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(form.get_gameObject(), "TopCommon/Panel_Menu/ListMenu");
                if (componetInChild != null)
                {
                    int tabIndex = Singleton<CMallSystem>.instance.GetTabIndex((CMallSystem.Tab) base.currentConf.Param[0]);
                    GameObject baseGo = componetInChild.GetElemenet(tabIndex).get_gameObject();
                    base.AddHighLightGameObject(baseGo, true, form, true);
                    base.Initialize();
                }
            }
        }
    }
}

