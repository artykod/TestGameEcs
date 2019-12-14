using Unity.Entities;
using Unity.Transforms;

public class DestroyOutOfBoundSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        var gameData = World.GetExistingSystem<GameSystem>().Data;

        Entities.WithAll<DestroyOutOfBound>().ForEach((Entity entity, ref Translation position) =>
        {
            if (!CollisionHelper.IsPointInArea(position.Value, gameData.FieldSize))
            {
                PostUpdateCommands.DestroyEntity(entity);
            }
        });
    }
}