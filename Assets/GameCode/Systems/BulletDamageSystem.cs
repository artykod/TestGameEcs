using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

public class BulletDamageSystem : ComponentSystem
{
    private struct UnitInfo
    {
        public TeamId team;
        public Entity entity;
        public Translation position;
        public SphereCollider collider;
    }

    protected override void OnUpdate()
    {
        var allDamage = World.GetExistingSystem<DamageApplySystem>().DealtDamage;
        var units = new NativeList<UnitInfo>(Allocator.Temp);

        Entities.ForEach((Entity entity, ref TeamId team, ref Translation position, ref SphereCollider collider) =>
        {
            units.Add(new UnitInfo
            {
                team = team,
                entity = entity,
                position = position,
                collider = collider,
            });
        });

        Entities.WithAllReadOnly<BulletMarker>().ForEach((Entity entity, ref Speed speed, ref TeamId team, ref Damage damage, ref Heading heading, ref Translation position) =>
        {
            for (int i = 0, l = units.Length; i < l; ++i)
            {
                var unit = units[i];

                if (unit.team.Value == team.Value)
                {
                    continue;
                }

                var velocity = heading.Value * speed.Value;
                var nextFramePosition = position.Value + velocity * Time.DeltaTime;

                if (!CollisionHelper.IsSphereVsSegmentCollision(position.Value, nextFramePosition, unit.position.Value, unit.collider.Radius))
                {
                    continue;
                }

                allDamage.Add(unit.entity, damage);
                PostUpdateCommands.DestroyEntity(entity);
            }
        });

        units.Dispose();
    }
}