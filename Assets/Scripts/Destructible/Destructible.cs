using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Destructible : MonoBehaviour
{
    [SerializeField] private float _maxHealth;
    private float _health;
    public float Health => _health;

    [SerializeField, Header("Reward")]
    private int _minRewardXp;
    [SerializeField] private int _maxRewardXp;
    private int _rewardXp;
    [SerializeField, Header("Health Restore")]
    private float _minHealthRestore;
    [SerializeField] private float _maxHealthRestore;
    private float _healthRestore;

    private Rigidbody2D _rb;
    private BarUI _healthbar;

    public event Action OnDestroyed;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _health = _maxHealth;
        _rewardXp = Random.Range(_minRewardXp, _maxRewardXp + 1);
        _healthRestore = Random.Range(_minHealthRestore, _maxHealthRestore);
    }

    private void Start()
    {
        _healthbar = HUDManager.Instance.CreateHealthbar(true);
        _healthbar.Initialize(gameObject, 0f, _maxHealth);

        StartCoroutine(CheckPositionIE());
    }

    public bool TakeDamage(float damage, Tank attacker)
    {
        if (_health - damage > 0)
        {
            _health -= damage;
            _healthbar.SetValue(_health);
        }
        else
        {
            _health = 0;
            _healthbar.Dispose();
            OnDestroyed?.Invoke();
            if (attacker != null)
            {
                attacker.AddXP(_rewardXp);
                attacker.TakeDamage(-_healthRestore);
            }
            Destroy(gameObject);
            return true;
        }

        return false;
    }

    public void Push(Vector3 position, float force)
    {
        _rb.AddExplosionForce(force, position, 5f);
    }

    public void Push(Vector3 position)
    {
        Push(position, 100f);
    }

    private IEnumerator CheckPositionIE()
    {
        float maxX = WorldManager.Instance.WorldSize.x / 2f - transform.localScale.x;
        float maxY = WorldManager.Instance.WorldSize.y / 2f - transform.localScale.x;

        while (true)
        {
            bool push = false;
            Vector2 finalPos = _rb.position;

            if (_rb.position.x < -maxX)
            {
                finalPos.x = _rb.position.x - transform.localScale.x;
                push = true;
            }
            else if (_rb.position.x > maxX)
            {
                finalPos.x = _rb.position.x + transform.localScale.x;
                push = true;
            }

            if (_rb.position.y < -maxY)
            {
                finalPos.y = _rb.position.y - transform.localScale.x;
                push = true;
            }
            else if (_rb.position.y > maxY)
            {
                finalPos.y = _rb.position.y + transform.localScale.x;
                push = true;
            }

            if (push)
                Push(finalPos, 300f);

            if (WorldManager.Instance.IsFarFromCamera(transform.position, transform.localScale.x))
            {
                _healthbar.Disable();
            }
            else
            {
                _healthbar.Enable();
            }

            yield return new WaitForSeconds(Constants.HEALTHBAR_RENDER_TIME);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        _health = _maxHealth;
    }
#endif
}
