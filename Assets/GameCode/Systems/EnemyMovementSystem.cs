using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class EnemyMovementSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        var targetPositions = new NativeList<float3>(Allocator.Temp);

        Entities.WithAll<PlayerMarker>().ForEach((ref Translation position) =>
        {
            targetPositions.Add(position.Value);
        });

        Entities.WithAll<EnemyMarker>().ForEach((ref Translation position, ref Movement movement, ref Heading heading) =>
        {
            var closestPosition = position.Value;
            var closestDistance = float.MaxValue;

            for (int i = 0, l = targetPositions.Length; i < l; ++i)
            {
                var targetPosition = targetPositions[i];
                var targetDistance = math.distancesq(targetPosition, position.Value);
                if (targetDistance < closestDistance)
                {
                    closestPosition = targetPosition;
                    closestDistance = targetDistance;
                }
            }

            var forward = math.normalizesafe(closestPosition - position.Value);
            movement.Value = math.normalizesafe(closestPosition - forward - position.Value);
            heading.Value = movement.Value;
        });

        targetPositions.Dispose();
    }
}