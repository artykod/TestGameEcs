using Unity.Mathematics;

public static class mathExt
{
    public const float pi = UnityEngine.Mathf.PI;
    public const float pi2 = UnityEngine.Mathf.PI * 2f;

    public static float3 lerpXZ(float3 a, float3 b, float w)
    {
        var angle1 = math.atan2(a.z, a.x);
        var angle2 = math.atan2(b.z, b.x);
        var angleLerp = angle1 + shortAngleDistance(angle1, angle2) * w;
        return new float3 { x = math.cos(angleLerp), y = math.lerp(a.y, b.y, w), z = math.sin(angleLerp) };
    }

    private static float shortAngleDistance(float a0, float a1)
    {
        var da = (a1 - a0) % pi2;
        return 2f * da % pi2 - da;
    }
}