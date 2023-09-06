using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _timeToDestroy = 5f;

    private float _speed;
    private float _damage;
    private Rigidbody2D _rb;
    private Tank _owner;
    private int _teamID;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(float damage, float speed, float size, Tank owner)
    {
        _damage = damage;
        _speed = speed;
        transform.localScale = new(size, size, size);
        _owner = owner;
        _teamID = owner.TeamID;
        
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
            DestoryMe();
            return;
        }

        if (!collision.TryGetComponent<Tank>(out var tank)) return;
        if (_teamID == tank.TeamID) return;

        if (tank.TakeDamage(_damage, _owner))
            _owner.AddXP(tank.RewardXP);
        
        DestoryMe();
    }

    public void DestoryMe()
    {
        WorldManager.Instance.BulletsPool.AddToPool(gameObject);
        _rb.velocity = Vector2.zero;
    }

    private IEnumerator DestroyIE()
    {
        yield return new WaitForSeconds(_timeToDestroy);
        DestoryMe();
    }
}
