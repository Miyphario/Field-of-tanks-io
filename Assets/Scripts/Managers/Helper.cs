using UnityEngine;

public static class Helper
{
    public static Vector3 DirectionToPoint(Vector3 startPosition, Vector3 endPosition)
    {
        Vector3 heading = endPosition - startPosition;
        float distance = heading.magnitude;
        return heading / distance;
    }
}
