using UnityEngine;

public static class QuaternionExt
{
    public static float AngleToPoint(in Vector3 position, in Vector3 point, float offset)
    {
        Vector3 difference = position.DirectionToPoint(point);
        return Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg + offset;
    }

    public static Quaternion RotationToPoint(this in Quaternion _, in Vector3 startPosition, in Vector3 endPosition, float offset)
    {
        float rotZ = AngleToPoint(startPosition, endPosition, offset);
        return Quaternion.Euler(0, 0, rotZ);
    }

    public static Quaternion RotationToDirection(in Vector3 direction, in Vector3 offset)
    {
        Quaternion toRot = Quaternion.LookRotation(Vector3.forward, direction);
        toRot.eulerAngles += offset;
        return toRot;
    }

    public static Quaternion RotationToDirection(in Vector3 direction)
    {
        return RotationToDirection(direction, Vector3.zero);
    }
}
