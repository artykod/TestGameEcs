using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[AlwaysUpdateSystem]
public class PlayerMovementSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        var direction = default(float3);

        if (Input.GetKey(KeyCode.A))
        {
            direction += new float3 { x = -1 };
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += new float3 { x = 1 };
        }
        if (Input.GetKey(KeyCode.W))
        {
            direction += new float3 { z = 1 };
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction += new float3 { z = -1 };
        }

        var isMoving = math.lengthsq(direction) > math.FLT_MIN_NORMAL;
        if (isMoving)
        {
            direction = math.normalize(direction);

            Entities.WithAllReadOnly<PlayerMarker>().WithNone<MovingMarker>().ForEach((Entity entity) =>
            {
                EntityManager.AddComponent<MovingMarker>(entity);
            });
        }
        else
        {
            Entities.WithAll<PlayerMarker, MovingMarker>().ForEach((Entity entity) =>
            {
                EntityManager.RemoveComponent<MovingMarker>(entity);
            });
        }

        Entities.WithAllReadOnly<PlayerMarker>().ForEach((ref Movement movement, ref GameCameraListener cameraData) =>
        {
            var cameraRotation = cameraData.Rotation;
            var directionInCamera = math.mul(cameraRotation, direction);

            movement.Value = math.lerp(movement.Value, new float3 { x = directionInCamera.x, z = directionInCamera.z }, Time.DeltaTime * 8f);
        });
    }
}