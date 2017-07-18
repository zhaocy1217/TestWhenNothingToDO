namespace Assets.Scripts.GameLogic
{
    using System;

    public class EnergyAttribute : Attribute
    {
        public int attributeType;

        public EnergyAttribute(EnergyType _type)
        {
            this.attributeType = (int) _type;
        }
    }
}

