using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

internal class NewbieGuideClickAddedSkillForBattle : NewbieGuideBaseScript
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
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroSelectNormalSystem.s_heroSelectFormPath);
            if (form != null)
            {
                int index = base.currentConf.Param[0];
                Transform transform = form.get_transform().FindChild("PanelAddSkill/ToggleList");
                if (transform != null)
                {
                    CUIToggleListScript component = transform.get_gameObject().GetComponent<CUIToggleListScript>();
                    if (component != null)
                    {
                        component.MoveElementInScrollArea(index, true);
                        CUIToggleListElementScript elemenet = component.GetElemenet(index) as CUIToggleListElementScript;
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

