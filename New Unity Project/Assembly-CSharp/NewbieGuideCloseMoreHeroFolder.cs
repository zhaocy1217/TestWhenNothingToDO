using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideCloseMoreHeroFolder : NewbieGuideBaseScript
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
                Transform transform = form.get_gameObject().get_transform().Find("PanelLeft/ListHostHeroInfo");
                Transform transform2 = form.get_gameObject().get_transform().Find("PanelLeft/ListHostHeroInfoFull");
                if ((transform2 != null) && transform2.get_gameObject().get_activeInHierarchy())
                {
                    Transform transform3 = transform2.FindChild("btnOpenFullHeroPanel");
                    if (transform3 != null)
                    {
                        GameObject baseGo = transform3.get_gameObject();
                        if (baseGo.get_activeInHierarchy())
                        {
                            base.AddHighLightGameObject(baseGo, true, form, true);
                            base.Initialize();
                        }
                    }
                }
                else if ((transform != null) && transform.get_gameObject().get_activeInHierarchy())
                {
                    this.CompleteHandler();
                }
            }
        }
    }
}

