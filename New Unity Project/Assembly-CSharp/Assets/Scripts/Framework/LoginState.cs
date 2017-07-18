namespace Assets.Scripts.Framework
{
    using ResData;
    using System;
    using UnityEngine;

    [GameState]
    public class LoginState : BaseState
    {
        private void OnLoginSceneCompleted()
        {
            Singleton<CSoundManager>.GetInstance().PostEvent("Login_Play", null);
            Singleton<CLoginSystem>.GetInstance().Draw();
            if (((GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_CURMEMNOTENOUGH_TIPS) == 1) && !DeviceCheckSys.CheckAvailMemory()) && (DeviceCheckSys.GetRecordCurMemNotEnoughPopTimes() < 3))
            {
                Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("CheckDevice_QuitGame_CurMemNotEnough"), false);
                DeviceCheckSys.RecordCurMemNotEnoughPopTimes();
            }
        }

        public override void OnStateEnter()
        {
            Singleton<CUIManager>.GetInstance().CloseAllForm(null, true, true);
            Singleton<ResourceLoader>.GetInstance().LoadScene("EmptyScene", new ResourceLoader.LoadCompletedDelegate(this.OnLoginSceneCompleted));
            Singleton<CSoundManager>.CreateInstance();
        }

        public override void OnStateLeave()
        {
            base.OnStateLeave();
            Singleton<CLoginSystem>.GetInstance().CloseLogin();
            Debug.Log("CloseLogin...");
            enResourceType[] resourceTypes = new enResourceType[5];
            resourceTypes[1] = enResourceType.UI3DImage;
            resourceTypes[2] = enResourceType.UIForm;
            resourceTypes[3] = enResourceType.UIPrefab;
            resourceTypes[4] = enResourceType.UISprite;
            Singleton<CResourceManager>.GetInstance().RemoveCachedResources(resourceTypes);
        }
    }
}

