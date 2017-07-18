using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickSymbolRewardBack : NewbieGuideBaseScript
{
    private const float DelayTimer = 2f;
    private float m_timer;

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
            this.m_timer += Time.get_deltaTime();
            if (this.m_timer >= 2f)
            {
                CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CUICommonSystem.s_newSymbolFormPath);
                if (form != null)
                {
                    Transform transform = form.get_transform().Find("Btn_Continue");
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
}

