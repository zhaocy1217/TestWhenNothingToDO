using Assets.Scripts.UI;
using System;
using UnityEngine;

internal class NewbieGuideClickSymbolLottery : NewbieGuideBaseScript
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
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<CMallSystem>.instance.sMallFormPath);
            if (form != null)
            {
                Transform transform = form.get_transform().FindChild("pnlBodyBg/pnlSymbolGift/pnlAction/pnlSeniorBuy/btnBuyOneFree/BuyButton");
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

