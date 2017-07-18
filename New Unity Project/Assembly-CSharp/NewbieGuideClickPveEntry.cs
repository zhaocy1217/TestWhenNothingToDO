﻿using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickPveEntry : NewbieGuideBaseScript
{
    protected override void Initialize()
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CLobbySystem.LOBBY_FORM_PATH);
        GameObject baseGo = form.get_transform().FindChild("BtnCon/PveBtn").get_gameObject();
        base.AddHighLightGameObject(baseGo, true, form, true);
        base.Initialize();
    }

    protected override bool IsDelegateClickEvent()
    {
        return true;
    }
}

