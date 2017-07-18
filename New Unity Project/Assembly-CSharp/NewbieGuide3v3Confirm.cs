using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuide3v3Confirm : NewbieGuideBaseScript
{
    private CUIEventScript CanCelBtnScript;
    private CUIEventScript ConfirmBtnScript;
    private CUIFormScript Guide3v3ConfirmForm;

    protected override void ClickHandler(CUIEvent uiEvt)
    {
        if (this.CanCelBtnScript != null)
        {
            this.CanCelBtnScript.onClick = (CUIEventScript.OnUIEventHandler) Delegate.Remove(this.CanCelBtnScript.onClick, new CUIEventScript.OnUIEventHandler(this.ClickHandler));
        }
        if (this.ConfirmBtnScript != null)
        {
            this.ConfirmBtnScript.onClick = (CUIEventScript.OnUIEventHandler) Delegate.Remove(this.ConfirmBtnScript.onClick, new CUIEventScript.OnUIEventHandler(this.ClickHandler));
        }
        if (uiEvt.m_eventParams.tag == 1)
        {
            this.openTrainEntry();
            this.CompleteHandler();
        }
        else
        {
            this.CompleteAllHandler();
        }
    }

    protected override void Initialize()
    {
        this.Guide3v3ConfirmForm = Singleton<CUIManager>.GetInstance().OpenForm(NewbieGuideManager.FORM_3v3GUIDE_CONFIRM, false, true);
        GameObject obj2 = this.Guide3v3ConfirmForm.get_transform().FindChild("Bg/btnCancel").get_gameObject();
        GameObject obj3 = this.Guide3v3ConfirmForm.get_transform().FindChild("Bg/btnConfirm").get_gameObject();
        DebugHelper.Assert((obj2 != null) && (obj3 != null), string.Format("{0}can't find cancel and confirm button in {1}", base.logTitle, NewbieGuideManager.FORM_3v3GUIDE_CONFIRM));
        this.CanCelBtnScript = obj2.GetComponent<CUIEventScript>();
        if (this.CanCelBtnScript != null)
        {
            this.CanCelBtnScript.m_onClickEventParams.tag = 0;
            this.CanCelBtnScript.onClick = (CUIEventScript.OnUIEventHandler) Delegate.Combine(this.CanCelBtnScript.onClick, new CUIEventScript.OnUIEventHandler(this.ClickHandler));
        }
        this.ConfirmBtnScript = obj3.GetComponent<CUIEventScript>();
        if (this.ConfirmBtnScript != null)
        {
            this.ConfirmBtnScript.m_onClickEventParams.tag = 1;
            this.ConfirmBtnScript.onClick = (CUIEventScript.OnUIEventHandler) Delegate.Combine(this.ConfirmBtnScript.onClick, new CUIEventScript.OnUIEventHandler(this.ClickHandler));
        }
        base.Initialize();
    }

    private void openTrainEntry()
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
        if (form == null)
        {
            form = Singleton<CUIManager>.GetInstance().OpenForm(CMatchingSystem.PATH_MATCHING_ENTRY, false, true);
        }
        form.GetWidget(3).CustomSetActive(false);
        form.GetWidget(4).CustomSetActive(false);
        form.GetWidget(2).CustomSetActive(false);
        form.GetWidget(5).CustomSetActive(false);
        form.GetWidget(6).CustomSetActive(false);
        form.GetWidget(7).CustomSetActive(false);
        form.GetWidget(9).CustomSetActive(true);
        form.GetWidget(10).CustomSetActive(false);
        Singleton<CMatchingSystem>.instance.ShowBonusImage(form);
    }
}

