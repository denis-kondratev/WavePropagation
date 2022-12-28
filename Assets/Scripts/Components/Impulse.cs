using System;
using Unity.Entities;
using Unity.Mathematics;

namespace WavePropagation
{
    [Serializable]
    public struct Impulse : IComponentData
    {
        public int2 SemiAxes;
        public float Height;
    }
}