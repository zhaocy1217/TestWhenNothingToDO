using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickChallengeNext : NewbieGuideBaseScript
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
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CAdventureSys.ADVENTURE_SELECT_FORM);
            if (form != null)
            {
                form.get_transform().FindChild("LevelList").GetComponent<CUIListScript>().SelectElement(0, true);
                CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                uIEvent.m_eventParams.tag = 1;
                uIEvent.m_eventID = enUIEventID.Adv_SelectLevel;
                uIEvent.m_srcFormScript = form;
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uIEvent);
                Transform transform = form.get_transform().FindChild("ButtonEnter");
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

