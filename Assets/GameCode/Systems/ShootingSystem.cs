using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

public class ShootingSystem : ComponentSystem
{
    private struct BulletSpawnData
    {
        public Gun Gun;
        public TeamId TeamId;
        public Heading Heading;
        public Translation Position;
        public Rotation Rotation;
    }

    protected override void OnUpdate()
    {
        var bulletSpawnDatas = new NativeList<BulletSpawnData>(Allocator.Temp);

        Entities.ForEach((ref Shooting shooting, ref TeamId teamId, ref Translation position, ref Rotation rotation, ref Heading heading) =>
        {
            shooting.Timer -= Time.DeltaTime;
            if (shooting.Timer > 0f)
            {
                return;
            }
            shooting.Timer = shooting.Gun.ShootingFrequency;

            bulletSpawnDatas.Add(new BulletSpawnData
            {
                Gun = shooting.Gun,
                TeamId = teamId,
                Heading = heading,
                Position = new Translation { Value = position.Value + heading.Value * 0.5f},
                Rotation = rotation,
            });
        });

        var gameData = World.GetExistingSystem<GameSystem>().Data;
        var bulletEntities = new NativeArray<Entity>(bulletSpawnDatas.Length, Allocator.Temp);

        EntityManager.Instantiate(gameData.BulletPrefab, bulletEntities);

        for (int i = 0, l = bulletEntities.Length; i < l; ++i)
        {
            var entity = bulletEntities[i];
            var data = bulletSpawnDatas[i];

            EntityManager.AddComponentData(entity, data.TeamId);
            EntityManager.AddComponentData(entity, data.Heading);
            EntityManager.AddComponentData(entity, data.Position);
            EntityManager.AddComponentData(entity, data.Rotation);
            EntityManager.AddComponentData(entity, new Speed { Value = data.Gun.BulletSpeed });
            EntityManager.AddComponentData(entity, new Movement { Value = data.Heading.Value });
        }

        bulletEntities.Dispose();
        bulletSpawnDatas.Dispose();
    }
}