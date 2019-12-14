using Unity.Entities;

public class HealthControlSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref Health health) =>
        {
            if (health.Value <= 0f)
            {
                EntityManager.DestroyEntity(entity);
            }
        });
    }
}