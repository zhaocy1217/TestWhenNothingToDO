namespace AGE
{
    using System;
    using UnityEngine;

    public class RootMotionHelper : MonoBehaviour
    {
        private Vector3 posOffset = new Vector3();
        public Transform rootTransform;

        public void ForceLateUpdate()
        {
            this.rootTransform.set_localPosition(this.posOffset);
        }

        public void ForceStart()
        {
            this.posOffset = this.rootTransform.get_localPosition();
        }

        private void LateUpdate()
        {
            this.rootTransform.set_localPosition(this.posOffset);
        }

        private void Start()
        {
            this.posOffset = this.rootTransform.get_localPosition();
        }
    }
}

