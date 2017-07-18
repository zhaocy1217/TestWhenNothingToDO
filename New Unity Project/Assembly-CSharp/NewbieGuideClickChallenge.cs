using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickChallenge : NewbieGuideBaseScript
{
    private CUIStepListScript m_stepList;

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
            if (this.m_stepList != null)
            {
                int count = NewbieGuideBaseScript.ms_highlitGo.Count;
                for (int i = 0; i < count; i++)
                {
                    GameObject obj2 = NewbieGuideBaseScript.ms_highlitGo[i];
                    GameObject obj3 = NewbieGuideBaseScript.ms_originalGo[i];
                    RectTransform transform = obj2.get_transform() as RectTransform;
                    transform.set_localScale(obj3.get_transform().get_localScale());
                    transform.set_localScale((Vector3) (transform.get_localScale() * 1.2f));
                }
            }
        }
        else
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CAdventureSys.EXLPORE_FORM_PATH);
            if (form != null)
            {
                Transform transform2 = form.get_transform().FindChild("ExploreList");
                if (transform2 != null)
                {
                    this.m_stepList = transform2.get_gameObject().GetComponent<CUIStepListScript>();
                    int index = base.currentConf.Param[0];
                    this.m_stepList.SelectElementImmediately(index);
                    CUIListElementScript elemenet = this.m_stepList.GetElemenet(index);
                    if (elemenet != null)
                    {
                        GameObject baseGo = elemenet.get_gameObject();
                        if (baseGo.get_activeInHierarchy())
                        {
                            if (index == 1)
                            {
                                Singleton<CAdventureSys>.instance.currentDifficulty = 1;
                                Singleton<CAdventureSys>.instance.currentChapter = 1;
                                Singleton<CAdventureSys>.instance.currentLevelSeq = 1;
                            }
                            base.AddHighLightGameObject(baseGo, true, form, true);
                            base.Initialize();
                        }
                    }
                }
            }
        }
    }
}

