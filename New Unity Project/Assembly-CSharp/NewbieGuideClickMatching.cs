using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

internal class NewbieGuideClickMatching : NewbieGuideBaseScript
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
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_MULTI);
            if (form != null)
            {
                GameObject baseGo = form.get_transform().FindChild("Panel_Main/Btn_Matching").get_gameObject();
                if (baseGo != null)
                {
                    base.AddHighLightGameObject(baseGo, true, form, true);
                    base.Initialize();
                }
            }
        }
    }
}

