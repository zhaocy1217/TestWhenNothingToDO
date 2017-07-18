using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickSymbolManufacture : NewbieGuideBaseScript
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
                Transform transform = form.get_transform().FindChild("TopCommon/Panel_Menu/ListMenu");
                if (transform != null)
                {
                    CUIListScript component = transform.get_gameObject().GetComponent<CUIListScript>();
                    if (component != null)
                    {
                        CUIListElementScript elemenet = component.GetElemenet(2);
                        if (elemenet != null)
                        {
                            GameObject baseGo = elemenet.get_transform().get_gameObject();
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

