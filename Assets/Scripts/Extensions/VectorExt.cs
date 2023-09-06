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
        return vector.DirectionToPoint(point);
    } 
}
