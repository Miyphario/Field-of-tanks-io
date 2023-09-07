using UnityEngine;

public class TankController : MonoBehaviour
{
    public Tank Tank { get; protected set; }
    public Vector2 Velocity => _rb.velocity;
    public bool DisabledInput { get; protected set; }

    protected Rigidbody2D _rb;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        Tank = GetComponent<Tank>();
    }

    protected virtual void Start() { }
}
