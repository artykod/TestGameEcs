using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class GameCameraSystem : ComponentSystem
{
    private Vector3 _previousMousePosition;

    protected override void OnUpdate()
    {
        var mousePositionDelta = Input.mousePosition - _previousMousePosition;
        _previousMousePosition = Input.mousePosition;

        Entities.ForEach((ref GameCamera camera, ref Translation cameraPosition, ref Rotation cameraRotation) =>
        {
            var dt = Time.DeltaTime;

            if (!EntityManager.Exists(camera.Target))
            {
                return;
            }

            var targetAiming = EntityManager.HasComponent<AimingMarker>(camera.Target);
            var targetPosition = EntityManager.GetComponentData<Translation>(camera.Target).Value;

            camera.EulerRotation.x = math.radians(targetAiming ? 10f : 20f);
            camera.EulerRotation.y += math.radians(mousePositionDelta.x) * 0.5f;

            cameraRotation.Value = math.slerp(cameraRotation.Value, quaternion.EulerXYZ(camera.EulerRotation), dt * 8f);

            var offsetFromTarget = targetAiming ? new float3(-1f, 1f, -2f) : new float3(0f, 2f, -10f);
            cameraPosition.Value = math.lerp(cameraPosition.Value, targetPosition + math.mul(cameraRotation.Value, offsetFromTarget), dt * 8f);

            EntityManager.AddComponentData(camera.Target, new GameCameraListener { Rotation = cameraRotation.Value });
        });
    }
}