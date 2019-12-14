public struct JobRandom
{
    private const long a = 25214903917L;
    private const long c = 11L;
    private long _seed;

    public JobRandom(long seed)
    {
        _seed = seed;
    }

    public static JobRandom New()
    {
        return new JobRandom(unchecked((int)System.DateTime.UtcNow.Ticks));
    }

    private int NextBits(int bits)
    {
        _seed = (_seed * a + c) & ((1L << 48) - 1);
        return (int)(_seed >> (48 - bits));
    }

    public double Next()
    {
        return (((long)NextBits(26) << 27) + NextBits(27)) / (double)(1L << 53);
    }

    public float NextFloat()
    {
        return (((long)NextBits(26) << 27) + NextBits(27)) / (float)(1L << 53);
    }

    public double Range(double from, double to)
    {
        return from + (to - from) * Next();
    }

    public float Range(float from, float to)
    {
        return from + (to - from) * NextFloat();
    }

    public int Range(int from, int to)
    {
        return (int)(from + (to - from) * NextFloat());
    }
}