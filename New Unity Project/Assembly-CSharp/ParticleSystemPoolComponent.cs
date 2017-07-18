using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class ParticleSystemPoolComponent : MonoBehaviour
{
    public ParticleSystemCache[] cache;

    [StructLayout(LayoutKind.Sequential)]
    public struct ParticleSystemCache
    {
        public ParticleSystem par;
        public bool emmitState;
    }
}

