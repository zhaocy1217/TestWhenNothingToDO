using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickSymbolBuy : NewbieGuideBaseScript
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
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSymbolMakeController.s_symbolTransformPath);
            if (form != null)
            {
                GameObject baseGo = form.get_transform().Find("Panel_SymbolTranform/Panel_Content/btnMake").get_gameObject();
                if (baseGo != null)
                {
                    base.AddHighLightGameObject(baseGo, true, form, true);
                    base.Initialize();
                }
            }
        }
    }
}

