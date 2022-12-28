using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace WavePropagation
{
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
    [BurstCompile]
    public partial struct InitializingSystem : ISystem
    {
        public void OnCreate(ref SystemState state) { }

        public void OnDestroy(ref SystemState state) { }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var field = SystemAPI.GetSingleton<Field>();
            var cellPrefab = SystemAPI.GetSingleton<CellPrefab>().Value;

            try
            {
                CreateField(ref state, ref field, ref cellPrefab);
            }
            finally
            {
                state.Enabled = false;
            }
        }

        [BurstCompile]
        private void CreateField(ref SystemState state, ref Field field, ref Entity cellPrefab)
        {
            var cellCount = field.Size.x * field.Size.y;
            var cells = state.EntityManager.Instantiate(cellPrefab, cellCount, Allocator.Temp);
            var fieldOffset = new float3(field.Size.x, 0, field.Size.y) / 2 * field.Step;

            for (var x = 0; x < field.Size.x; x++)
            {
                for (var y = 0; y < field.Size.y; y++)
                {
                    var index = GetIndex(x, y, ref field);
                    var northIndex = GetIndex(x, y + 1, ref field);
                    var southIndex = GetIndex(x, y - 1, ref field);
                    var eastIndex = GetIndex(x + 1, y, ref field);
                    var westIndex = GetIndex(x - 1, y, ref field);

                    var cell = new Cell
                    {
                        North = GetCell(northIndex, ref cells),
                        South = GetCell(southIndex, ref cells),
                        East = GetCell(eastIndex, ref cells),
                        West = GetCell(westIndex, ref cells),
                    };

                    state.EntityManager.SetComponentData(cells[index], cell);
                    var position = new float3(x * field.Step, 0, y * field.Step) - fieldOffset;
                    state.EntityManager.SetComponentData(cells[index], LocalTransform.FromPosition(position));
                    
                    state.EntityManager.SetComponentData(cells[index], new Mass
                    {
                        Value = field.CellMass
                    });
                    
                    state.EntityManager.SetComponentData(cells[index], new Spring
                    {
                        Value = field.CellSpring
                    });
                }
            }
        }
        
        [BurstCompile]
        private static int GetIndex(int x, int y, ref Field field)
        {
            if (x < 0 || x >= field.Size.x || y < 0 || y >= field.Size.y)
            {
                return -1;
            }
            
            return x + y * field.Size.x;
        }
        
        [BurstCompile]
        private Entity GetCell(int index, ref NativeArray<Entity> cells)
        {
            if (index >= cells.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            
            return index >= 0 ? cells[index] : Entity.Null;
        }
    }
}