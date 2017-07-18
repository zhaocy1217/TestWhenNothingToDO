namespace Assets.Scripts.GameSystem
{
    using System;

    [MessageHandlerClass]
    public class CMentorSystem : Singleton<CMentorSystem>
    {
        public override void Init()
        {
            this.InitUIEventListener();
        }

        private void InitUIEventListener()
        {
        }

        private void UinitUIEventListener()
        {
        }

        public override void UnInit()
        {
            this.UinitUIEventListener();
        }
    }
}

