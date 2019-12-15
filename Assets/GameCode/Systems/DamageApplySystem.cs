using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public class DamageApplySystem : JobComponentSystem
{
    public NativeMultiHashMap<Entity, Damage> DealtDamage
    {
        get;
        private set;
    }

    protected override void OnCreate()
    {
        base.OnCreate();

        DealtDamage = new NativeMultiHashMap<Entity, Damage>(256, Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        DealtDamage.Dispose();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        new DamageApplyJob { DealtDamage = DealtDamage }.Schedule(this, inputDeps).Complete();
        DealtDamage.Clear();
        return inputDeps;
    }

    private struct DamageApplyJob : IJobForEachWithEntity<Health>
    {
        [ReadOnly] public NativeMultiHashMap<Entity, Damage> DealtDamage;

        public void Execute(Entity entity, int index, ref Health health)
        {
            if (DealtDamage.TryGetFirstValue(entity, out var damage, out var iterator))
            {
                do
                {
                    health.Value -= damage.Value;
                }
                while (DealtDamage.TryGetNextValue(out damage, ref iterator));
            }
        }
    }
}