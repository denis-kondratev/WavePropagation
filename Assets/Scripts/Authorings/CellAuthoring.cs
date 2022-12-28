using Unity.Entities;
using UnityEngine;

namespace WavePropagation
{
    public class CellAuthoring : MonoBehaviour
    {
        public class Baker : Baker<CellAuthoring>
        {
            public override void Bake(CellAuthoring authoring)
            {
                AddComponent<Cell>();
                AddComponent<Height>();
                AddComponent<Mass>();
                AddComponent<Spring>();
                AddComponent<Velocity>();
            }
        }
    }
}