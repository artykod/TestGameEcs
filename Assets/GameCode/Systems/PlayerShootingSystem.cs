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
            Entities.WithAll<PlayerMarker, AimingMarker>().WithNone<Shooting>().ForEach((Entity entity, ref Gun gun) =>
            {
                EntityManager.AddComponentData(entity, new Shooting { Gun = gun });
            });
        }
        else
        {
            Entities.WithAll<PlayerMarker, Shooting>().ForEach((Entity entity) =>
            {
                EntityManager.RemoveComponent<Shooting>(entity);
            });
        }
    }
}