using UnityEngine;

public static class VectorExt
{
    public static Vector3 DirectionToPoint(this in Vector3 vector, in Vector3 point)
    {
        Vector3 heading = point - vector;
        float distance = heading.magnitude;
        return heading / distance;
    }

    public static Vector2 DirectionToPoint(this in Vector2 vector, in Vector2 point)
    {
        Vector2 heading = point - vector;
        float distance = heading.magnitude;
        return heading / distance;
    }

    public static Vector3 Abs(this in Vector3 vector)
    {
        return new(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
    }

    public static Vector2 Abs(this in Vector2 vector)
    {
        return new(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
    }
}
