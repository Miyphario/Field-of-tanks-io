using UnityEngine;

public class TankController : MonoBehaviour
{
    public Tank Tank { get; protected set; }
    public Vector2 Velocity => _rb.velocity;

    protected Rigidbody2D _rb;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        Tank = GetComponent<Tank>();
    }

    protected virtual void Start() { }

    /* protected void RotateToPoint(Vector2 point)
    {
        var rot = RotationToPoint(point);
        transform.rotation = rot;
    }

    protected void RotateToPoint(Vector2 point, float offset)
    {
        var rot = RotationToPoint(point, offset);
        transform.rotation = rot;
    }

    protected float AngleToPoint(Vector2 point)
    {
        return AngleToPoint(point, 0f);
    }

    protected float AngleToPoint(Vector2 point, float offset)
    {
        Vector3 difference = transform.position.DirectionToPoint(point);
        return Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg + offset;
    }

    protected Quaternion RotationToDirection(Vector3 direction)
    {
        return RotationToDirection(direction, Vector3.zero);
    }

    protected Quaternion RotationToDirection(Vector3 direction, Vector3 offset)
    {
        Quaternion toRot = Quaternion.LookRotation(Vector3.forward, direction);
        toRot.eulerAngles += offset;
        return Quaternion.RotateTowards(transform.rotation, toRot, Time.deltaTime * 1000f);
    }

    protected Quaternion RotationToPoint(Vector3 point)
    {
        return RotationToPoint(point, 0f);
    }

    protected Quaternion RotationToPoint(Vector3 point, float offset)
    {
        float rotZ = AngleToPoint(point, offset);
        return Quaternion.Euler(0, 0, rotZ);
    } */
}
