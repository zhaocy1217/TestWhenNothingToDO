using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickSymbolBuyClose : NewbieGuideBaseScript
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
                Transform transform = form.get_transform().Find("Panel_SymbolTranform/Panel_Title/btnClose");
                if (transform != null)
                {
                    base.AddHighLightGameObject(transform.get_gameObject(), true, form, true);
                    base.Initialize();
                }
            }
        }
    }
}

