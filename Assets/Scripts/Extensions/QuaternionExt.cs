using UnityEngine;

public static class QuaternionExt
{
    public static float AngleToPoint(Vector3 position, Vector3 point, float offset)
    {
        Vector3 difference = position.DirectionToPoint(point);
        return Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg + offset;
    }

    public static Quaternion RotationToPoint(this Quaternion _, Vector3 startPosition, Vector3 endPosition, float offset)
    {
        float rotZ = AngleToPoint(startPosition, endPosition, offset);
        return Quaternion.Euler(0, 0, rotZ);
    }

    public static Quaternion RotationToDirection(this Quaternion _, Quaternion rotation, Vector3 direction, Vector3 offset)
    {
        Quaternion toRot = Quaternion.LookRotation(Vector3.forward, direction);
        toRot.eulerAngles += offset;
        return Quaternion.RotateTowards(rotation, toRot, Time.deltaTime * 1000f);
    }

    public static Quaternion RotationToDirection(this Quaternion qt, Quaternion rotation, Vector3 direction)
    {
        return qt.RotationToDirection(rotation, direction, Vector3.zero);
    }
}
