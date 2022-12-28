using System;
using Unity.Entities;

namespace WavePropagation
{
    public struct Cell : IComponentData
    {
        public Entity North;
        public Entity South;
        public Entity West;
        public Entity East;

        public const int Size = 4;

        public Entity this[Direction direction] => this[(int)direction];

        public Entity this[int index]
        {
            get
            {
                return index switch
                {
                    (int)Direction.North => North,
                    (int)Direction.South => South,
                    (int)Direction.West => West,
                    (int)Direction.East => East,
                    _ => throw new IndexOutOfRangeException()
                };
            }
        }
    }
}