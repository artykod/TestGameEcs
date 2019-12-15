using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public class BulletDamageSystem : JobComponentSystem
{
    private EntityQuery _unitsQuery;
    private EntityQuery _bulletsQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        _unitsQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] 
            {
                ComponentType.ReadOnly<TeamId>(),
                ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<SphereCollider>(),
            }
        });

        _bulletsQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[]
            {
                ComponentType.ReadOnly<BulletMarker>(),
                ComponentType.ReadOnly<Speed>(),
                ComponentType.ReadOnly<TeamId>(),
                ComponentType.ReadOnly<Damage>(),
                ComponentType.ReadOnly<Heading>(),
                ComponentType.ReadOnly<Translation>(),
            }
        });
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var gameSystem = World.GetExistingSystem<DamageApplySystem>();
        var ecbSource = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        var unitChunks = _unitsQuery.CreateArchetypeChunkArray(Allocator.TempJob);
        var bulletChunks = _bulletsQuery.CreateArchetypeChunkArray(Allocator.TempJob);

        return new BulletDamageJob
        {
            DeltaTime = Time.DeltaTime,

            UnitChunks = unitChunks,
            BulletChunks = bulletChunks,

            EntityType = GetArchetypeChunkEntityType(),
            SpeedType = GetArchetypeChunkComponentType<Speed>(true),
            DamageType = GetArchetypeChunkComponentType<Damage>(true),
            TeamIdType = GetArchetypeChunkComponentType<TeamId>(true),
            HeadingType = GetArchetypeChunkComponentType<Heading>(true),
            TranslationType = GetArchetypeChunkComponentType<Translation>(true),
            SphereColliderType = GetArchetypeChunkComponentType<SphereCollider>(true),

            DealtDamage = gameSystem.DealtDamage.AsParallelWriter(),
            CommandBuffer = ecbSource.CreateCommandBuffer().ToConcurrent(),
        }.Schedule(bulletChunks.Length, 32, inputDeps);
    }

    [BurstCompile]
    private struct BulletDamageJob : IJobParallelFor
    {
        public float DeltaTime;

        [ReadOnly, DeallocateOnJobCompletion] public NativeArray<ArchetypeChunk> UnitChunks;
        [ReadOnly, DeallocateOnJobCompletion] public NativeArray<ArchetypeChunk> BulletChunks;

        [ReadOnly] public ArchetypeChunkEntityType EntityType;
        [ReadOnly] public ArchetypeChunkComponentType<Speed> SpeedType;
        [ReadOnly] public ArchetypeChunkComponentType<Damage> DamageType;
        [ReadOnly] public ArchetypeChunkComponentType<TeamId> TeamIdType;
        [ReadOnly] public ArchetypeChunkComponentType<Heading> HeadingType;
        [ReadOnly] public ArchetypeChunkComponentType<Translation> TranslationType;
        [ReadOnly] public ArchetypeChunkComponentType<SphereCollider> SphereColliderType;

        [WriteOnly] public EntityCommandBuffer.Concurrent CommandBuffer;
        [WriteOnly] public NativeMultiHashMap<Entity, Damage>.ParallelWriter DealtDamage;

        public void Execute(int bulletChunkIdx)
        {
            var bulletChunk = BulletChunks[bulletChunkIdx];
            var bulletEntities = bulletChunk.GetNativeArray(EntityType);
            var bulletSpeeds = bulletChunk.GetNativeArray(SpeedType);
            var bulletTeamIds = bulletChunk.GetNativeArray(TeamIdType);
            var bulletDamages = bulletChunk.GetNativeArray(DamageType);
            var bulletHeadings = bulletChunk.GetNativeArray(HeadingType);
            var bulletPositions = bulletChunk.GetNativeArray(TranslationType);

            for (int bulletIdx = 0, bulletCount = bulletChunk.Count; bulletIdx < bulletCount; ++bulletIdx)
            {
                var bulletEntity = bulletEntities[bulletIdx];
                var bulletSpeed = bulletSpeeds[bulletIdx];
                var bulletTeamId = bulletTeamIds[bulletIdx];
                var bulletDamage = bulletDamages[bulletIdx];
                var bulletHeading = bulletHeadings[bulletIdx];
                var bulletPosition = bulletPositions[bulletIdx];
                var bulletVelocity = bulletHeading.Value * bulletSpeed.Value;
                var bulletNextFramePosition = bulletPosition.Value + bulletVelocity * DeltaTime;

                for (int unitChunkIdx = 0, unitChunkCount = UnitChunks.Length; unitChunkIdx < unitChunkCount; ++unitChunkIdx)
                {
                    var unitChunk = UnitChunks[unitChunkIdx];
                    var unitEntities = unitChunk.GetNativeArray(EntityType);
                    var unitTeamIds = unitChunk.GetNativeArray(TeamIdType);
                    var unitPositions = unitChunk.GetNativeArray(TranslationType);
                    var unitSphereColliders = unitChunk.GetNativeArray(SphereColliderType);

                    for (int unitIdx = 0, unitCount = unitChunk.Count; unitIdx < unitCount; ++unitIdx)
                    {
                        var unitTeamId = unitTeamIds[unitIdx];
                        var unitEntity = unitEntities[unitIdx];
                        var unitPosition = unitPositions[unitIdx];
                        var unitSphereCollider = unitSphereColliders[unitIdx];

                        if (unitTeamId.Value == bulletTeamId.Value)
                        {
                            continue;
                        }

                        if (!CollisionHelper.IsSphereVsSegmentCollision(bulletPosition.Value, bulletNextFramePosition, unitPosition.Value, unitSphereCollider.Radius))
                        {
                            continue;
                        }

                        DealtDamage.Add(unitEntity, bulletDamage);
                        CommandBuffer.DestroyEntity(bulletChunkIdx, bulletEntity);
                    }
                }
            }
        }
    }
}