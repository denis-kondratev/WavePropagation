using Unity.Entities;
using UnityEngine;

namespace WavePropagation
{
    public class FieldAuthoring : MonoBehaviour
    {
        public GameObject CellPrefab;
        public Field Field;
        public Impulse Impulse;
        
        public class Baker : Baker<FieldAuthoring>
        {
            public override void Bake(FieldAuthoring authoring)
            {
                AddComponent(authoring.Field);
                AddComponent(authoring.Impulse);
                
                AddComponent(new CellPrefab
                {
                    Value = GetEntity(authoring.CellPrefab)
                });
            }
        }
    }
}
