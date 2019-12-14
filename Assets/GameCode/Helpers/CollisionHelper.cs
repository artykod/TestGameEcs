using Unity.Mathematics;

public static class CollisionHelper
{
    public static bool IsPointInArea(float3 point, float3 areaSize)
    {
        return math.abs(point.x) <= areaSize.x && math.abs(point.y) <= areaSize.y && math.abs(point.z) <= areaSize.z;
    }

    public static bool IsSphereVsSegmentCollision(float3 segmentStart, float3 segmentEnd, float3 sphereCenter, float sphereRadius)
    {
        var d = segmentEnd - segmentStart;
        var f = segmentStart - sphereCenter;
        var a = math.dot(d, d);
        var b = math.dot(f, d) * 2f;
        var c = math.dot(f, f) - sphereRadius * sphereRadius;

        var discriminant = b * b - 4f * a * c;
        if (discriminant < 0f) return false;
        discriminant = math.sqrt(discriminant);

        var t1 = (-b - discriminant) / (2f * a);
        if (t1 >= 0f && t1 <= 1f) return true;

        var t2 = (-b + discriminant) / (2f * a);
        if (t2 >= 0f && t2 <= 1f) return true;

        return false;
    }
}