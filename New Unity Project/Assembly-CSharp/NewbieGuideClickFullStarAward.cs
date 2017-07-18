﻿using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

internal class NewbieGuideClickFullStarAward : NewbieGuideBaseScript
{
    protected override void Initialize()
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CAdventureSys.ADVENTURE_SELECT_FORM);
        GameObject baseGo = form.get_transform().FindChild("ChapterReward").get_gameObject();
        base.AddHighLightGameObject(baseGo, true, form, true);
        base.Initialize();
    }

    protected override bool IsDelegateClickEvent()
    {
        return true;
    }
}

