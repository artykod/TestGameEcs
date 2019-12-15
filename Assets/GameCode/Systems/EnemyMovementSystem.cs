using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class EnemyMovementSystem : JobComponentSystem
{
    private EntityQuery _enemyQuery;
    private EntityQuery _targetsQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        _enemyQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[]
            {
                ComponentType.ReadWrite<Heading>(),
                ComponentType.ReadWrite<Movement>(),
                ComponentType.ReadOnly<EnemyMarker>(),
                ComponentType.ReadOnly<Translation>(),
            }
        });

        _targetsQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[]
            {
                ComponentType.ReadOnly<Translation>(),
            },
            Any = new ComponentType[]
            {
                ComponentType.ReadOnly<PlayerMarker>(),
            }
        });
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var enemyChunks = _enemyQuery.CreateArchetypeChunkArray(Allocator.TempJob);
        var targetChunks = _targetsQuery.CreateArchetypeChunkArray(Allocator.TempJob);

        return new EnemyMovementJob
        {
            EnemyChunks = enemyChunks,
            TargetChunks = targetChunks,
            HeadingType = GetArchetypeChunkComponentType<Heading>(),
            MovementType = GetArchetypeChunkComponentType<Movement>(),
            TranslationType = GetArchetypeChunkComponentType<Translation>(true),
        }.Schedule(enemyChunks.Length, 32, inputDeps);
    }

    [BurstCompile]
    private struct EnemyMovementJob : IJobParallelFor
    {
        [DeallocateOnJobCompletion] public NativeArray<ArchetypeChunk> EnemyChunks;
        [ReadOnly, DeallocateOnJobCompletion] public NativeArray<ArchetypeChunk> TargetChunks;

        public ArchetypeChunkComponentType<Heading> HeadingType;
        public ArchetypeChunkComponentType<Movement> MovementType;
        [ReadOnly] public ArchetypeChunkComponentType<Translation> TranslationType;

        public void Execute(int index)
        {
            var enemyChunk = EnemyChunks[index];
            var enemyHeadings = enemyChunk.GetNativeArray(HeadingType);
            var enemyMovements = enemyChunk.GetNativeArray(MovementType);
            var enemyPositions = enemyChunk.GetNativeArray(TranslationType);

            for (int enemyIdx = 0, enemyCount = enemyChunk.Count; enemyIdx < enemyCount; ++enemyIdx)
            {
                var enemyPosition = enemyPositions[enemyIdx];
                var closestPosition = enemyPosition.Value;
                var closestDistance = float.MaxValue;

                for (int targetChunkIdx = 0, targetChunkCount = TargetChunks.Length; targetChunkIdx < targetChunkCount; ++targetChunkIdx)
                {
                    var targetChunk = TargetChunks[targetChunkIdx];
                    var targetPositions = targetChunk.GetNativeArray(TranslationType);

                    for (int targetIdx = 0, targetCount = targetChunk.Count; targetIdx < targetCount; ++targetIdx)
                    {
                        var targetPosition = targetPositions[targetIdx];
                        var targetDistance = math.distancesq(targetPosition.Value, enemyPosition.Value);

                        if (targetDistance < closestDistance)
                        {
                            closestPosition = targetPosition.Value;
                            closestDistance = targetDistance;
                        }
                    }
                }

                var forward = math.normalizesafe(closestPosition - enemyPosition.Value);
                var movement = math.normalizesafe(closestPosition - forward - enemyPosition.Value);

                enemyHeadings[enemyIdx] = new Heading { Value = movement };
                enemyMovements[enemyIdx] = new Movement { Value = movement };
            }
        }
    }
}