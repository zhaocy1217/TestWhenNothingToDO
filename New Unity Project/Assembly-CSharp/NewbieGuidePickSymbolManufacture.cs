using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

internal class NewbieGuidePickSymbolManufacture : NewbieGuideBaseScript
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
                int index = base.currentConf.Param[0];
                string str = "Panel_SymbolMake/symbolMakeList";
                Transform transform = form.get_transform().FindChild(str);
                if (transform != null)
                {
                    CUIListScript component = transform.get_gameObject().GetComponent<CUIListScript>();
                    if (component != null)
                    {
                        CUIListElementScript elemenet = component.GetElemenet(index);
                        if (elemenet != null)
                        {
                            GameObject baseGo = elemenet.get_gameObject();
                            if (baseGo.get_activeInHierarchy())
                            {
                                base.AddHighLightGameObject(baseGo, true, form, true);
                                base.Initialize();
                            }
                        }
                    }
                }
            }
        }
    }
}

