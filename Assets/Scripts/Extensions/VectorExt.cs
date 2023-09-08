using UnityEngine;

public static class VectorExt
{
    public static Vector3 DirectionToPoint(this Vector3 vector, Vector3 point)
    {
        Vector3 heading = point - vector;
        float distance = heading.magnitude;
        return heading / distance;
    }

    public static Vector2 DirectionToPoint(this Vector2 vector, Vector2 point)
    {
        Vector2 heading = point - vector;
        float distance = heading.magnitude;
        return heading / distance;
    }

    public static Vector3 Abs(this Vector3 vector)
    {
        return new(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
    }

    public static Vector2 Abs(this Vector2 vector)
    {
        return new(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
    }
}
