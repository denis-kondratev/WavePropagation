using System;
using Unity.Entities;
using Unity.Mathematics;

namespace WavePropagation
{
    [Serializable]
    public struct Field : IComponentData
    {
        public float Step;
        public int2 Size;
        public float CellMass;
        public float CellSpring;
        public float Viscosity;
        public float CellHeightScale;
        public float MinHeight;
    }
}