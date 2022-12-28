using Unity.Burst;
using Unity.Entities;

namespace WavePropagation
{
    [BurstCompile]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(OscillatingSystem))]
    public partial struct TransformingSystem : ISystem
    {
        public void OnCreate(ref SystemState state) { }

        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var field = SystemAPI.GetSingleton<Field>();
            
            var job = new TransformingJon
            {
                HeightScale = field.CellHeightScale,
                DeltaTime = SystemAPI.Time.fixedDeltaTime,
                FieldViscosity = field.Viscosity,
                MinHeight = field.MinHeight
            };

            job.ScheduleParallel();
        }
    }
}