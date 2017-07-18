namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class Lobby3DInteraction
    {
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map2;
        public const float CAMERA_MAX = 7.5f;
        public const float CAMERA_MIN = 0.8f;
        private int InteractLayerMask = (((int) 1) << LayerMask.NameToLayer("Lobby_Interactable"));
        private Transform mouseDownObj;
        private const string NAME_ARENA = "Arena";
        private const string NAME_LOTTERY = "Lottery";
        private const string NAME_PROBE = "Probe";
        private const string NAME_PVE = "PVE";
        private const string NAME_PVP = "PVP";
        private const string NAME_SHOP = "Shop";
        private const string NAME_SOCIAL = "Social";
        public static string PATH_3DINTERACTION_FORM = "UGUI/Form/System/Lobby/Form_LobbyInteractable.prefab";
        private CUIEvent uiEvt;

        public void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_OnMouseDown, new CUIEventManager.OnUIEventHandler(this.onMouseDown));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_OnMouseUp, new CUIEventManager.OnUIEventHandler(this.onMouseUp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_OnMouseClick, new CUIEventManager.OnUIEventHandler(this.onMouseClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_OnDragStart, new CUIEventManager.OnUIEventHandler(this.onDragStart));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_OnDragging, new CUIEventManager.OnUIEventHandler(this.onDragging));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_OnDragEnd, new CUIEventManager.OnUIEventHandler(this.onDragEnd));
            this.uiEvt = new CUIEvent();
        }

        private void onDragEnd(CUIEvent uiEvent)
        {
        }

        private void onDragging(CUIEvent uiEvent)
        {
            Transform transform = Camera.get_main().get_transform();
            transform.set_position(new Vector3(transform.get_position().x - (uiEvent.m_pointerEventData.get_delta().x / 100f), transform.get_position().y, transform.get_position().z));
            if (transform.get_position().x < 0.8f)
            {
                transform.set_position(new Vector3(0.8f, transform.get_position().y, transform.get_position().z));
            }
            if (transform.get_position().x > 7.5f)
            {
                transform.set_position(new Vector3(7.5f, transform.get_position().y, transform.get_position().z));
            }
        }

        private void onDragStart(CUIEvent uiEvent)
        {
        }

        private void onMouseClick(CUIEvent uiEvent)
        {
            RaycastHit hit;
            if (!Physics.Raycast(Camera.get_main().ScreenPointToRay(uiEvent.m_pointerEventData.get_position()), ref hit, float.PositiveInfinity, this.InteractLayerMask))
            {
                return;
            }
            string key = hit.get_collider().get_gameObject().get_transform().get_gameObject().get_name();
            if (key != null)
            {
                int num;
                if (<>f__switch$map2 == null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(7);
                    dictionary.Add("PVP", 0);
                    dictionary.Add("PVE", 1);
                    dictionary.Add("Probe", 2);
                    dictionary.Add("Lottery", 3);
                    dictionary.Add("Arena", 4);
                    dictionary.Add("Social", 5);
                    dictionary.Add("Shop", 6);
                    <>f__switch$map2 = dictionary;
                }
                if (<>f__switch$map2.TryGetValue(key, out num))
                {
                    switch (num)
                    {
                        case 0:
                            this.uiEvt.m_eventID = enUIEventID.Matching_OpenEntry;
                            goto Label_01AE;

                        case 1:
                            this.uiEvt.m_eventID = enUIEventID.Adv_OpenChapterForm;
                            goto Label_01AE;

                        case 2:
                            this.uiEvt.m_eventID = enUIEventID.Explore_OpenForm;
                            goto Label_01AE;

                        case 3:
                            this.uiEvt.m_eventID = enUIEventID.Lottery_OpenForm;
                            goto Label_01AE;

                        case 4:
                            this.uiEvt.m_eventID = enUIEventID.Arena_OpenForm;
                            goto Label_01AE;

                        case 5:
                            this.uiEvt.m_eventID = enUIEventID.Guild_OpenForm;
                            goto Label_01AE;

                        case 6:
                            this.uiEvt.m_eventID = enUIEventID.Shop_OpenForm;
                            goto Label_01AE;
                    }
                }
            }
            this.uiEvt.m_eventID = enUIEventID.None;
        Label_01AE:
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(this.uiEvt);
        }

        private void onMouseDown(CUIEvent uiEvent)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.get_main().ScreenPointToRay(uiEvent.m_pointerEventData.get_position()), ref hit, float.PositiveInfinity, this.InteractLayerMask))
            {
                this.mouseDownObj = hit.get_collider().get_gameObject().get_transform();
                this.mouseDownObj.set_position(new Vector3(this.mouseDownObj.get_position().x, this.mouseDownObj.get_position().y + 0.2f, this.mouseDownObj.get_position().z));
            }
        }

        private void onMouseUp(CUIEvent uiEvent)
        {
            if (this.mouseDownObj != null)
            {
                this.mouseDownObj.set_position(new Vector3(this.mouseDownObj.get_position().x, this.mouseDownObj.get_position().y - 0.2f, this.mouseDownObj.get_position().z));
                this.mouseDownObj = null;
            }
        }

        public void OpenForm()
        {
            Singleton<CUIManager>.GetInstance().OpenForm(PATH_3DINTERACTION_FORM, false, true);
        }

        public void UnInit()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_OnMouseDown, new CUIEventManager.OnUIEventHandler(this.onMouseDown));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_OnMouseUp, new CUIEventManager.OnUIEventHandler(this.onMouseUp));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_OnMouseClick, new CUIEventManager.OnUIEventHandler(this.onMouseClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_OnDragStart, new CUIEventManager.OnUIEventHandler(this.onDragStart));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_OnDragging, new CUIEventManager.OnUIEventHandler(this.onDragging));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_OnDragEnd, new CUIEventManager.OnUIEventHandler(this.onDragEnd));
            this.uiEvt = null;
        }
    }
}

