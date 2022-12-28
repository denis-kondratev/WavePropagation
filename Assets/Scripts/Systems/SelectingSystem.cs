using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

namespace WavePropagation
{
    public partial class SelectingSystem : SystemBase
    {
        private const float RaycastDistance = 1000;
        private Camera _camera;

        protected override void OnCreate()
        {
            _camera = Camera.main;
        }
        
        protected override void OnUpdate()
        {
            if (!Input.GetMouseButton(0))
            {
                return;
            }
            
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            var entity = Raycast(ray.origin, ray.GetPoint(RaycastDistance));
            var impulse = SystemAPI.GetSingleton<Impulse>();

            if (entity != Entity.Null)
            {
                SetupImpulse(entity, impulse);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetupImpulse(Entity centerEntity, Impulse impulse)
        {
            var axesSqr = new float2(impulse.SemiAxes.x * impulse.SemiAxes.x, impulse.SemiAxes.y * impulse.SemiAxes.y);
            SetHeight(centerEntity, impulse.Height, axesSqr, 0, 0);
            PropagateImpulseAlongX(Direction.East, centerEntity, impulse.Height, impulse.SemiAxes, axesSqr);
            PropagateImpulseAlongX(Direction.West, centerEntity, impulse.Height, impulse.SemiAxes, axesSqr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetHeight(Entity entity, float height, float2 axesSqr, int x, int y)
        {
            var pos = x * x / axesSqr.x + y * y / axesSqr.y;
            var value = 0f;

            if (pos < 1)
            {
                var z = math.sqrt(1 - pos);
                value = z * height;
            }

            EntityManager.SetComponentData(entity, new Height
            {
                Value = value
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PropagateImpulseAlongX(Direction direction, Entity entity, float height, int2 semiAxes, float2 axesSqr)
        {
            for (var x = 1; x <= semiAxes.x; x++)
            {
                var cell = EntityManager.GetComponentData<Cell>(entity);
                entity = cell[direction];
                
                if (entity == Entity.Null)
                {
                    break;
                }

                var maxY = (int) math.round(semiAxes.y * math.sqrt(1 - x * x / axesSqr.x));
                
                SetHeight(entity, height, axesSqr, x, 0);
                PropagateImpulseAlongY(Direction.North, entity, height, axesSqr, maxY, x);
                PropagateImpulseAlongY(Direction.South, entity, height, axesSqr, maxY, x);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PropagateImpulseAlongY(Direction direction, Entity entity, float height, float2 axesSqr, int maxY, int x)
        {
            for (var y = 0; y < maxY; y++)
            {
                var cell = EntityManager.GetComponentData<Cell>(entity);
                entity = cell[direction];
                
                if (entity == Entity.Null)
                {
                    break;
                }
                
                SetHeight(entity, height, axesSqr, x, y);
            }
        }
        
        private static Entity Raycast(float3 rayFrom, float3 rayTo)
        {
            var builder = new EntityQueryBuilder(Allocator.Temp).WithAll<PhysicsWorldSingleton>();
            var singletonQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(builder);
            var collisionWorld = singletonQuery.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
            singletonQuery.Dispose();

            var input = new RaycastInput
            {
                Start = rayFrom,
                End = rayTo,
                Filter = new CollisionFilter
                {
                    BelongsTo = ~0u,
                    CollidesWith = ~0u,
                    GroupIndex = 0
                }
            };

            return collisionWorld.CastRay(input, out var hit) ? hit.Entity : Entity.Null;
        }
    }
}