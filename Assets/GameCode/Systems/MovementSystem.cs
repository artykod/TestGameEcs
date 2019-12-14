using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public class MovementSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return new MovementJob { DeltaTime = Time.DeltaTime }.Schedule(this, inputDeps);
    }

    [Unity.Burst.BurstCompile]
    private struct MovementJob : IJobForEach<Translation, Movement, Speed>
    {
        public float DeltaTime;

        public void Execute(ref Translation position, [ReadOnly] ref Movement movement, [ReadOnly] ref Speed speed)
        {
            position.Value += movement.Value * speed.Value * DeltaTime;
        }
    }
}