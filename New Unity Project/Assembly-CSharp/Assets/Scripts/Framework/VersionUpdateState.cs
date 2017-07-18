namespace Assets.Scripts.Framework
{
    using Assets.Scripts.GameSystem;
    using Assets.Scripts.UI;
    using System;
    using UnityEngine;

    [GameState]
    public class VersionUpdateState : BaseState
    {
        private void OnExitGame(CUIEvent uiEvent)
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CheckDevice_Quit, new CUIEventManager.OnUIEventHandler(this.OnExitGame));
            CVersionUpdateSystem.QuitApp();
        }

        public override void OnStateEnter()
        {
            Singleton<ResourceLoader>.GetInstance().LoadScene("EmptySceneWithCamera", null);
            if (Application.get_internetReachability() == null)
            {
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CheckDevice_Quit, new CUIEventManager.OnUIEventHandler(this.OnExitGame));
                Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("VersionUpdate_NetworkUnReachable"), enUIEventID.CheckDevice_Quit, false);
            }
            else
            {
                MonoSingleton<CVersionUpdateSystem>.GetInstance().StartVersionUpdate(new Assets.Scripts.GameSystem.CVersionUpdateSystem.OnVersionUpdateComplete(this.OnVersionUpdateComplete));
            }
        }

        public override void OnStateLeave()
        {
        }

        private void OnVersionUpdateComplete()
        {
            Singleton<GameStateCtrl>.GetInstance().GotoState("MovieState");
        }
    }
}

