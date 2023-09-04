using System;

public static class MACExtensions
{
    public static float Range(this Random random, float minValue, float maxValue)
    {
        return (float)(random.NextDouble() * (maxValue - minValue) + minValue);
    }

    public static int Range(this Random random, int minValue, int maxValue)
    {
        return random.Next(minValue, maxValue);
    }
}
