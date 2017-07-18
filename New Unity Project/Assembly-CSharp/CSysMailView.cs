using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CSysMailView
{
    private CUIFormScript m_CUIForm;
    private CUIListScript m_CUIListScript;

    private void Draw(CMail mail)
    {
        if (this.m_CUIForm != null)
        {
            this.m_CUIForm.get_transform().FindChild("PanelAccess").get_gameObject().CustomSetActive(true);
            Text component = this.m_CUIForm.get_transform().FindChild("PanelAccess/MailContent").GetComponent<Text>();
            Text text2 = this.m_CUIForm.get_transform().FindChild("PanelAccess/MailTitle").GetComponent<Text>();
            component.set_text(mail.mailContent);
            text2.set_text(mail.subject);
            this.m_CUIListScript.SetElementAmount(mail.accessUseable.Count);
            for (int i = 0; i < mail.accessUseable.Count; i++)
            {
                GameObject itemCell = this.m_CUIListScript.GetElemenet(i).get_transform().FindChild("itemCell").get_gameObject();
                CUICommonSystem.SetItemCell(this.m_CUIForm, itemCell, mail.accessUseable[i], true, true, false, false);
            }
            GameObject obj3 = this.m_CUIForm.get_transform().FindChild("PanelAccess/GetAccess").get_gameObject();
            obj3.CustomSetActive(mail.accessUseable.Count > 0);
            GameObject target = this.m_CUIForm.get_transform().FindChild("PanelAccess/CheckAccess").get_gameObject();
            if (CHyperLink.Bind(target, mail.mailHyperlink))
            {
                target.CustomSetActive(true);
                obj3.CustomSetActive(false);
            }
            else
            {
                target.CustomSetActive(false);
            }
        }
    }

    public CUIFormScript Form
    {
        set
        {
            this.m_CUIForm = value;
            this.m_CUIListScript = this.m_CUIForm.get_transform().FindChild("PanelAccess/List").GetComponent<CUIListScript>();
        }
    }

    public CMail Mail
    {
        set
        {
            this.Draw(value);
        }
    }

    public COM_MAIL_TYPE mailType
    {
        set
        {
            if (this.m_CUIForm != null)
            {
                if (value == COM_MAIL_TYPE.COM_MAIL_SYSTEM)
                {
                    this.m_CUIForm.get_transform().FindChild("PanelAccess/GetAccess").GetComponent<CUIEventScript>().m_onClickEventID = enUIEventID.Mail_SysAccess;
                }
                else if (value == COM_MAIL_TYPE.COM_MAIL_FRIEND)
                {
                    this.m_CUIForm.get_transform().FindChild("PanelAccess/GetAccess").GetComponent<CUIEventScript>().m_onClickEventID = enUIEventID.Mail_FriendAccess;
                }
            }
        }
    }
}

