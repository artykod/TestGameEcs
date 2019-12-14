using Unity.Entities;
using UnityEngine;

[AlwaysUpdateSystem]
public class PlayerAimingSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        var isAiming = Input.GetMouseButton(1);
        if (isAiming)
        {
            Entities.WithAll<PlayerMarker>().WithNone<AimingMarker>().ForEach((Entity entity) =>
            {
                EntityManager.AddComponent<AimingMarker>(entity);
            });
        }
        else
        {
            Entities.WithAll<PlayerMarker, AimingMarker>().ForEach((Entity entity) =>
            {
                EntityManager.RemoveComponent<AimingMarker>(entity);
            });
        }
    }
}