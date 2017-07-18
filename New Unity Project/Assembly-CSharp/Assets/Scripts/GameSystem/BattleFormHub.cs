namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using System;
    using UnityEngine;

    public static class BattleFormHub
    {
        public static bool IsFormShow
        {
            get
            {
                return (!Singleton<WatchController>.GetInstance().IsWatching ? Singleton<CBattleSystem>.GetInstance().m_isInBattle : Singleton<CWatchSystem>.GetInstance().IsFormShow);
            }
        }

        public static CUIFormScript TheGameForm
        {
            get
            {
                return (!Singleton<WatchController>.GetInstance().IsWatching ? Singleton<CBattleSystem>.GetInstance().m_FormScript : Singleton<CWatchSystem>.GetInstance().Form);
            }
        }

        public static KillNotify TheKillNotify
        {
            get
            {
                return (!Singleton<WatchController>.GetInstance().IsWatching ? Singleton<CBattleSystem>.GetInstance().GetKillNotifation() : Singleton<CWatchSystem>.GetInstance().TheKillNotify);
            }
        }

        public static MinimapSys TheMinimapSys
        {
            get
            {
                return (!Singleton<WatchController>.GetInstance().IsWatching ? Singleton<CBattleSystem>.GetInstance().GetMinimapSys() : Singleton<CWatchSystem>.GetInstance().TheMinimapSys);
            }
        }

        public static SignalPanel TheSignalPanel
        {
            get
            {
                return (!Singleton<WatchController>.GetInstance().IsWatching ? Singleton<CBattleSystem>.GetInstance().GetSignalPanel() : null);
            }
        }

        public static CUIContainerScript TheTextHud
        {
            get
            {
                if (Singleton<WatchController>.GetInstance().IsWatching)
                {
                    return Singleton<CWatchSystem>.GetInstance().TheTextHud;
                }
                CUIFormScript formScript = Singleton<CBattleSystem>.GetInstance().m_FormScript;
                if (null != formScript)
                {
                    GameObject widget = formScript.GetWidget(0x18);
                    if (widget != null)
                    {
                        return widget.GetComponent<CUIContainerScript>();
                    }
                }
                return null;
            }
        }
    }
}

