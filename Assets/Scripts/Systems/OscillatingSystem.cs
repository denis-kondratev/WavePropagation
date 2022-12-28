using Unity.Burst;
using Unity.Entities;

namespace WavePropagation
{
    [BurstCompile]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct OscillatingSystem : ISystem
    {
        private ComponentLookup<Height> _heights;
        private ComponentLookup<Spring> _springs;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _heights = state.GetComponentLookup<Height>();
            _springs = state.GetComponentLookup<Spring>();
        }

        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _heights.Update(ref state);
            _springs.Update(ref state);
            
            var oscillatingJob = new OscillatingJob
            {
                DeltaTime = SystemAPI.Time.fixedDeltaTime,
                Heights = _heights,
                Springs = _springs
            };

            oscillatingJob.ScheduleParallel();
        }
    }
}