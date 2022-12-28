using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace WavePropagation
{
    [BurstCompile]
    public partial struct OscillatingJob : IJobEntity
    {
        public float DeltaTime;
        [ReadOnly] public ComponentLookup<Height> Heights;
        [ReadOnly] public ComponentLookup<Spring> Springs;
        
        [BurstCompile]
        private void Execute(ref Velocity velocity, in Height height, in Spring spring, in Mass mass, in Cell cell)
        {
            var acceleration = 0f;

            for (var i = 0; i < Cell.Size; i++)
            {
                acceleration += GetRestoringForce(cell[i], height.Value, spring.Value) / mass.Value;
            }

            velocity.Value += acceleration * DeltaTime;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetRestoringForce(Entity entity, float height, float spring)
        {
            var entityExists = entity != Entity.Null;
            var anotherHeight = entityExists ? Heights[entity].Value : 0;
            var vector = anotherHeight - height;
            spring += entityExists ? Springs[entity].Value : spring;
            return vector * spring;
        }
    }
}