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
            Entities.WithAllReadOnly<PlayerMarker>().WithNone<AimingMarker>().ForEach((Entity entity) =>
            {
                PostUpdateCommands.AddComponent<AimingMarker>(entity);
            });
        }
        else
        {
            Entities.WithAllReadOnly<PlayerMarker, AimingMarker>().ForEach((Entity entity) =>
            {
                PostUpdateCommands.RemoveComponent<AimingMarker>(entity);
            });
        }
    }
}