using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace WavePropagation
{
    public partial struct TransformingJon : IJobEntity
    {
        public float HeightScale;
        public float DeltaTime;
        public float FieldViscosity;
        public float MinHeight;

        private void Execute(ref LocalTransform transform, ref Height height, in Velocity velocity)
        {
            height.Value += velocity.Value * DeltaTime / FieldViscosity;
            var position = transform.Position;
            position.y = height.Value * HeightScale;

            if (math.abs(position.y) < MinHeight)
            {
                position.y = 0;
            }
            
            transform.Position = position;
        }
    }
}