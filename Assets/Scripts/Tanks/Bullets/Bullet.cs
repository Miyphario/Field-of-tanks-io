using System;
using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour, IPoolable
{
    private float _timeToDestroy;
    private float _speed;
    private float _damage;
    private Rigidbody2D _rb;
    private Tank _owner;
    private int _teamID;

    private bool _isAlive = true;

    public event Action OnAddedToPool;

    public bool IsAlive => _isAlive;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(float damage, float speed, float size, Tank owner)
    {
        _isAlive = true;
        _damage = damage;
        _speed = speed;
        transform.localScale = new(size, size, size);
        _owner = owner;
        _teamID = owner.TeamID;

        Vector2 maxDist = WorldManager.Instance.GetMaxDistanceToCamera(size);
        Vector2 curSpd = transform.up * speed;
        curSpd -= owner.Controller.Velocity;
        curSpd = curSpd.Abs();
        Vector2 times = new(maxDist.x / curSpd.x, maxDist.y / curSpd.y);
        _timeToDestroy = Mathf.Min(times.x, times.y);

        StartCoroutine(DestroyIE());
    }

    private void FixedUpdate()
    {
        _rb.velocity = transform.up * _speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Destructible>(out var dest))
        {
            dest.Push(transform.position);
            dest.TakeDamage(_damage, _owner);
            AddToPool();
            return;
        }

        if (!collision.TryGetComponent<Tank>(out var tank)) return;
        if (_teamID == tank.TeamID) return;

        if (tank.TakeDamage(_damage, _owner))
            _owner.AddXP(tank.RewardXP);
        
        AddToPool();
    }

    private IEnumerator DestroyIE()
    {
        yield return new WaitForSeconds(_timeToDestroy);
        AddToPool();
    }

    public void AddToPool()
    {
        WorldManager.Instance.BulletsPool.AddToPool(this);
        _rb.velocity = Vector2.zero;
        _isAlive = false;
        OnAddedToPool?.Invoke();
    }

    public void Kill()
    {
        Destroy(gameObject);
    }
}
