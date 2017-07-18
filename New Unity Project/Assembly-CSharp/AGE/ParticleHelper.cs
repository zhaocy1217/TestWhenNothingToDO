namespace AGE
{
    using System;
    using UnityEngine;

    public class ParticleHelper
    {
        private static int _particleActiveNumber;

        public static void DecParticleActiveNumber()
        {
            _particleActiveNumber--;
            if (_particleActiveNumber < 0)
            {
            }
        }

        public static int GetParticleActiveNumber()
        {
            return _particleActiveNumber;
        }

        public static void IncParticleActiveNumber()
        {
            _particleActiveNumber++;
        }

        public static ParticleSystem[] Init(GameObject gameObj, Vector3 scaling)
        {
            ParticleSystem[] componentsInChildren = gameObj.GetComponentsInChildren<ParticleSystem>();
            if ((componentsInChildren == null) || (componentsInChildren.Length == 0))
            {
                return null;
            }
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                ParticleSystem system = componentsInChildren[i];
                system.set_startSize(system.get_startSize() * scaling.x);
                system.set_startLifetime(system.get_startLifetime() * scaling.y);
                system.set_startSpeed(system.get_startSpeed() * scaling.z);
                Transform transform1 = system.get_transform();
                transform1.set_localScale((Vector3) (transform1.get_localScale() * scaling.x));
                if (!system.get_playOnAwake())
                {
                    system.set_playOnAwake(true);
                    system.Play();
                }
            }
            return componentsInChildren;
        }
    }
}

