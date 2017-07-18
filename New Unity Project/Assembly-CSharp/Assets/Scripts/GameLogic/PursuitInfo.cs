namespace Assets.Scripts.GameLogic
{
    using System;

    [Serializable]
    public class PursuitInfo
    {
        public int CosHalfAngle;
        public int PursuitAngle;
        public VInt3 PursuitDir = VInt3.right;
        public VInt3 PursuitOrigin;
        public int PursuitRadius;

        public bool IsVaild()
        {
            return ((this.PursuitAngle > 0) && (this.PursuitRadius > 0));
        }
    }
}

