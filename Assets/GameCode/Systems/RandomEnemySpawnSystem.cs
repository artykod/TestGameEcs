using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class RandomEnemySpawnSystem : ComponentSystem
{
    private float _spawnTimer;

    protected override void OnUpdate()
    {
        _spawnTimer -= Time.DeltaTime;
        if (_spawnTimer > 0f)
        {
            return;
        }
        _spawnTimer = 1f;

        var random = JobRandom.New();
        var gameData = World.GetExistingSystem<GameSystem>().Data;
        var fieldSize = gameData.FieldSize;
        var enemyEntity = EntityManager.Instantiate(gameData.EnemyPrefab);
        var spawnPoint = new float3 { x = random.Range(-fieldSize.x / 4f, fieldSize.x / 4f), z = random.Range(-fieldSize.z / 4f, fieldSize.z / 4f) };

        EntityManager.SetComponentData(enemyEntity, new Translation { Value = spawnPoint });
    }
}