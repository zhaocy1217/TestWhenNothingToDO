using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickJungleSword : NewbieGuideBaseScript
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
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CBattleEquipSystem.s_equipFormPath);
            if (form != null)
            {
                GameObject baseGo = form.GetWidget(1).get_transform().FindChild("equipItem0").get_gameObject();
                DebugHelper.Assert(baseGo != null, "Can't find equipItem0~!!");
                base.AddHighLightGameObject(baseGo, true, form, true);
                base.Initialize();
            }
        }
    }
}

