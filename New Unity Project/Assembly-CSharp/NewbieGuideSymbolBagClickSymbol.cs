using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideSymbolBagClickSymbol : NewbieGuideBaseScript
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
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSymbolSystem.s_symbolFormPath);
            if (form != null)
            {
                Transform transform = form.get_transform().Find("SymbolMake/Panel_SymbolMake/symbolMakeList");
                if (transform != null)
                {
                    CUIListElementScript elemenet = transform.GetComponent<CUIListScript>().GetElemenet(base.currentConf.Param[0]);
                    if (elemenet != null)
                    {
                        base.AddHighLightGameObject(elemenet.get_gameObject(), true, form, true);
                        base.Initialize();
                    }
                }
            }
        }
    }
}

