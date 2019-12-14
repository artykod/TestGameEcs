using Unity.Collections;
using Unity.Entities;

public class DamageApplySystem : ComponentSystem
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

        if (DealtDamage.IsCreated)
        {
            DealtDamage.Dispose();
        }
    }

    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref Health health) =>
        {
            if (DealtDamage.TryGetFirstValue(entity, out var damage, out var iterator))
            {
                do
                {
                    health.Value -= damage.Value;
                }
                while (DealtDamage.TryGetNextValue(out damage, ref iterator));
            }
        });
        DealtDamage.Clear();
    }
}