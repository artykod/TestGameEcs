using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class DestroyOutOfBoundSystem : JobComponentSystem
{
    private EntityQuery _outOfBoundQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        _outOfBoundQuery = GetEntityQuery(ComponentType.ReadOnly<Translation>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var gameData = World.GetExistingSystem<GameSystem>().Data;
        var ecbSource = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var chunks = _outOfBoundQuery.CreateArchetypeChunkArray(Allocator.TempJob);

        new DestroyOutOfBoundJob
        {
            FieldSize = gameData.FieldSize,
            EntityType = GetArchetypeChunkEntityType(),
            TranslationType = GetArchetypeChunkComponentType<Translation>(),
            Chunks = chunks,
            CommandBuffer = ecbSource.CreateCommandBuffer().ToConcurrent(),
        }.Schedule(chunks.Length, 32, inputDeps).Complete();

        return inputDeps;
    }

    private struct DestroyOutOfBoundJob : IJobParallelFor
    {
        public float3 FieldSize;

        [ReadOnly] public ArchetypeChunkEntityType EntityType;
        [ReadOnly] public ArchetypeChunkComponentType<Translation> TranslationType;

        [WriteOnly] public EntityCommandBuffer.Concurrent CommandBuffer;

        [ReadOnly, DeallocateOnJobCompletion] public NativeArray<ArchetypeChunk> Chunks;


        public void Execute(int index)
        {
            var chunk = Chunks[index];
            var entities = chunk.GetNativeArray(EntityType);
            var positions = chunk.GetNativeArray(TranslationType);

            for (int i = 0, l = chunk.Count; i < l; ++i)
            {
                if (!CollisionHelper.IsPointInArea(positions[i].Value, FieldSize))
                {
                    CommandBuffer.DestroyEntity(index, entities[i]);
                }
            }
        }
    }
}