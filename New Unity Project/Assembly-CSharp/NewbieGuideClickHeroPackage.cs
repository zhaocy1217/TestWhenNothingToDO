﻿using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickHeroPackage : NewbieGuideBaseScript
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
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
            if (form != null)
            {
                Transform transform = form.get_transform().FindChild("TopCommon/Panel_Menu/ListMenu/ScrollRect/Content/ListElement_0");
                if (transform != null)
                {
                    GameObject baseGo = transform.get_gameObject();
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
