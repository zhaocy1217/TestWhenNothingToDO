namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using System;

    public class ACTOR_INFO : PooledClassObject
    {
        public bool bDistOnly;
        public int[] camps;
        private const int DataSize_ = 6;
        private const int DataSizeBytes_ = 1;
        public VInt3[] location;

        public static int GetDataSize()
        {
            return 6;
        }

        public static int GetDataSizeBytes()
        {
            return 1;
        }

        public override void OnRelease()
        {
            this.location = null;
            this.camps = null;
            this.bDistOnly = false;
            base.OnRelease();
        }

        public override void OnUse()
        {
            base.OnUse();
            this.location = null;
            this.camps = null;
            this.bDistOnly = false;
        }
    }
}

