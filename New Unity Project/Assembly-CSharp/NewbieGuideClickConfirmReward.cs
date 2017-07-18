using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickConfirmReward : NewbieGuideBaseScript
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
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(string.Format("{0}{1}", "UGUI/Form/Common/", "Form_Award"));
            if (form != null)
            {
                GameObject baseGo = form.get_transform().FindChild("btnGroup/Button_Back").get_gameObject();
                base.AddHighLightGameObject(baseGo, true, form, true);
                base.Initialize();
            }
        }
    }
}

