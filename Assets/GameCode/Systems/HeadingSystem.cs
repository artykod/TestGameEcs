using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class HeadingSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return new HeadingJob().Schedule(this, inputDeps);
    }

    private struct HeadingJob : IJobForEach<Heading, Rotation>
    {
        public void Execute([ReadOnly] ref Heading heading, ref Rotation rotation)
        {
            rotation.Value = quaternion.LookRotation(heading.Value, new float3 { y = 1f });
        }
    }
}