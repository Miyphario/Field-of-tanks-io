using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Destructible : MonoBehaviour
{
    [SerializeField] private float _maxHealth;
    private float _health;
    public float Health => _health;

    [Header("Reward")]
    [SerializeField]
    private int _minRewardXp;
    [SerializeField] private int _maxRewardXp;
    private int _rewardXp;

    [Header("Health Restore")]
    [SerializeField]
    private float _minHealthRestore;
    [SerializeField] private float _maxHealthRestore;
    private float _healthRestore;

    private Rigidbody2D _rb;
    private BarUI _healthbar;

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;

    public event Action<Destructible> OnDestroyed;

    [Header("Visual")]
    [SerializeField] private GameObject _destroyParticles;

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
        _healthbar.Initialize(gameObject, 0f, _maxHealth, NearFromCamera());

        StartCoroutine(CheckPositionIE());
    }

    public bool TakeDamage(float damage, Tank attacker)
    {
        if (_health <= 0) return false;
        
        if (_health - damage > 0)
        {
            _health -= damage;
            _healthbar.SetValue(_health);
            if (NearFromCamera())
                PlayHitSound();
        }
        else
        {
            _health = 0;
            OnDestroyed?.Invoke(this);
            if (attacker != null)
            {
                attacker.AddXP(_rewardXp);
                attacker.TakeDamage(-_healthRestore);
            }
            DestroyMe();
            return true;
        }

        return false;
    }

    public bool TakeDamage(float damage)
    {
        return TakeDamage(damage, null);
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
                Push(finalPos, 600f);

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

    private void PlayHitSound()
    {
        _audioSource.SetRandomPitchAndVolume(0.9f, 1.1f, 0.8f, 0.9f);
        _audioSource.PlayOneShot(SoundManager.Instance.HitObj);
    }

    private void PlayDestroySound()
    {
        _audioSource.SetRandomPitchAndVolume(0.9f, 1.1f, 0.9f, 1f);
        _audioSource.clip = SoundManager.Instance.DestroyObj;
        _audioSource.Play();
    }

    public void DestroyMe()
    {
        _healthbar.Dispose();
        GetComponent<Collider2D>().enabled = false;
        gameObject.DisableAllExcept(_audioSource);
        if (NearFromCamera())
        {
            PlayDestroySound();
            Instantiate(_destroyParticles, transform.position, transform.rotation);
        }
        
        StartCoroutine(DestroyMeIE());
    }

    private bool NearFromCamera()
    {
        return !WorldManager.Instance.IsFarFromCamera(transform.position, transform.localScale.x * 5f);
    }

    private IEnumerator DestroyMeIE()
    {
        yield return new WaitForSeconds(_audioSource.GetClipRemainingTime());
        Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        _health = _maxHealth;
    }
#endif
}
