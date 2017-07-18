using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickMainTask : NewbieGuideBaseScript
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
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CTaskSys.TASK_FORM_PATH);
            CUIListScript component = form.get_transform().FindChild("TopCommon/Panel_Menu/List").GetComponent<CUIListScript>();
            GameObject baseGo = null;
            int index = base.currentConf.Param[0];
            if (component != null)
            {
                component.MoveElementInScrollArea(index, true);
                baseGo = component.GetElemenet(index).get_gameObject();
            }
            if (baseGo != null)
            {
                base.AddHighLightGameObject(baseGo, true, form, true);
                base.Initialize();
            }
        }
    }
}

