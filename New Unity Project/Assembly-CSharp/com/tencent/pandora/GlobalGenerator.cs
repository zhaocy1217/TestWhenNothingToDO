namespace com.tencent.pandora
{
    using System;
    using UnityEngine;

    public class GlobalGenerator : MonoBehaviour
    {
        private static GlobalGenerator glo;

        public static GlobalGenerator getInstance()
        {
            if (Pandora.stopConnectAll)
            {
                return null;
            }
            if (glo == null)
            {
                GameObject obj2 = new GameObject("GameManager");
                if (Pandora.GetInstance().get_gameObject() != null)
                {
                    obj2.get_transform().set_parent(Pandora.GetInstance().get_gameObject().get_transform());
                }
                glo = obj2.AddComponent<GlobalGenerator>();
                Logger.d("加载GlobalGenerator");
                Logger.d(Time.get_time().ToString());
                glo.init();
            }
            return glo;
        }

        public void init()
        {
            string str = "GameManager";
            if (GameObject.Find(str) != null)
            {
                Logger.d("启动Appfacade");
                AppFacade.Instance.StartUp();
            }
            else
            {
                Logger.d("没找到GameManager");
            }
        }

        public void InitGameMangager()
        {
        }

        public void OnActionListRefresh(object data)
        {
        }

        private void Start()
        {
        }
    }
}

