namespace Assets.Scripts.GameSystem
{
    using System;
    using UnityEngine;

    public abstract class ActivityWidget
    {
        private GameObject _root;
        private ActivityView _view;

        public ActivityWidget(GameObject node, ActivityView view)
        {
            this._root = node;
            this._view = view;
        }

        public abstract void Clear();
        public virtual void OnShow()
        {
        }

        public void SetPosY(float val)
        {
            this._root.get_transform().set_localPosition(new Vector3(this._root.get_transform().get_localPosition().x, val, this._root.get_transform().get_localPosition().z));
        }

        public virtual void Update()
        {
        }

        public abstract void Validate();

        public virtual float Height
        {
            get
            {
                return this._root.GetComponent<RectTransform>().get_rect().get_height();
            }
        }

        public GameObject root
        {
            get
            {
                return this._root;
            }
        }

        public ActivityView view
        {
            get
            {
                return this._view;
            }
        }
    }
}

