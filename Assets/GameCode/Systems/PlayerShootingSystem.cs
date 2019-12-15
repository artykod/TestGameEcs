using Unity.Entities;
using UnityEngine;

[AlwaysUpdateSystem]
public class PlayerShootingSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        var isShooting = Input.GetMouseButton(0);
        if (isShooting)
        {
            Entities.WithAllReadOnly<PlayerMarker, AimingMarker>().WithNone<Shooting>().ForEach((Entity entity, ref Gun gun) =>
            {
                PostUpdateCommands.AddComponent(entity, new Shooting { Gun = gun });
            });

            Entities.WithAllReadOnly<PlayerMarker, Shooting>().WithNone<AimingMarker>().ForEach((Entity entity) =>
            {
                PostUpdateCommands.RemoveComponent<Shooting>(entity);
            });
        }
        else
        {
            Entities.WithAllReadOnly<PlayerMarker, Shooting>().ForEach((Entity entity) =>
            {
                PostUpdateCommands.RemoveComponent<Shooting>(entity);
            });
        }
    }
}