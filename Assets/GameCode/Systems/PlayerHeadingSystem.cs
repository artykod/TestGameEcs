using Unity.Entities;
using Unity.Mathematics;

[AlwaysUpdateSystem]
public class PlayerHeadingSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<PlayerMarker, AimingMarker>().ForEach((ref Heading heading, ref GameCameraListener cameraData) =>
        {
            var cameraDirection = math.mul(cameraData.Rotation, new float3 { z = 1f });
            heading.Value = mathExt.lerpXZ(heading.Value, new float3 { x = cameraDirection.x, z = cameraDirection.z }, Time.DeltaTime * 16f);
        });

        Entities.WithAll<PlayerMarker, MovingMarker>().WithNone<AimingMarker>().ForEach((Entity entity, ref Heading heading, ref Movement movement) =>
        {
            heading.Value = mathExt.lerpXZ(heading.Value, new float3 { x = movement.Value.x, z = movement.Value.z }, Time.DeltaTime * 8f);
        });
    }
}