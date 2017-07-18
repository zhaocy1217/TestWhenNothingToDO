namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using System;

    public class BULLET_INFO : PooledClassObject
    {
        public bool bDistOnly;
        private const int DataSize_ = 3;
        private const int DataSizeBytes_ = 1;
        public VInt3 location = VInt3.zero;
        public int radius;

        public static int GetDataSize()
        {
            return 3;
        }

        public static int GetDataSizeBytes()
        {
            return 1;
        }

        public override void OnRelease()
        {
            this.location = VInt3.zero;
            this.radius = 0;
            this.bDistOnly = false;
            base.OnRelease();
        }

        public override void OnUse()
        {
            base.OnUse();
            this.location = VInt3.zero;
            this.radius = 0;
            this.bDistOnly = false;
        }
    }
}

