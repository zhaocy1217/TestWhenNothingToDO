using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickBackToHall : NewbieGuideBaseScript
{
    protected override void Initialize()
    {
    }

    protected override bool IsDelegateClickEvent()
    {
        return true;
    }

    private void SetActiveHighlit()
    {
        for (int i = 0; i < NewbieGuideBaseScript.ms_originalGo.Count; i++)
        {
            GameObject obj2 = NewbieGuideBaseScript.ms_originalGo[i];
            NewbieGuideBaseScript.ms_highlitGo[i].CustomSetActive(obj2.get_activeInHierarchy());
        }
    }

    protected override void Update()
    {
        if (base.isInitialize)
        {
            base.Update();
            this.SetActiveHighlit();
        }
        else
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PVESettleSys.PATH_ITEM);
            if (form != null)
            {
                Transform transform = form.get_transform().FindChild("Root/Panel_Interactable/Button_ReturnLobby");
                if (transform != null)
                {
                    GameObject baseGo = transform.get_gameObject();
                    if (baseGo.get_activeInHierarchy())
                    {
                        base.AddHighLightGameObject(baseGo, true, form, true);
                        this.SetActiveHighlit();
                        base.Initialize();
                    }
                }
            }
        }
    }
}

