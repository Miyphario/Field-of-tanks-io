using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tank : MonoBehaviour
{
    public virtual float MaxHealth { get; protected set; } = Constants.DEFAULT_MAX_HEALTH;
    public virtual float Health { get; protected set; } = Constants.DEFAULT_MAX_HEALTH;
    public event Action<Tank> OnTakeDamage;

    public virtual float Speed { get; protected set; } = Constants.DEFAULT_MOVE_SPEED;
    public virtual float BulletSpeed { get; protected set; } = Constants.DEFAULT_BULLET_SPEED;
    public virtual float Damage { get; protected set; } = Constants.DEFAULT_DAMAGE;
    public virtual float TouchDamage { get; protected set; } = Constants.DEFAULT_TOUCH_DAMAGE;
    public virtual float FireRate { get; protected set; } = Constants.DEFAULT_FIRE_RATE;
    private bool _canTouchDamage = true;
    private readonly float _touchDamageReloadSpeed = 0.6f;
    private readonly float _touchDamageAngle = 40f;

    public TankController Controller { get; protected set; }

    [SerializeField] protected Gun _gun;
    public Gun Gun => _gun;

    private int _teamID;
    public int TeamID => _teamID;
    public event Action OnDestroyed;

    public virtual int XP { get; protected set; }
    public virtual int MaxXP { get; protected set; } = Constants.DEFAULT_MAX_XP;
    public virtual int Level { get; protected set; }
    public virtual int Tier { get; protected set; }
    public event Action<int> OnTierUpdated;
    public int RewardXP => MaxXP / 2;
    private int _upgradeCount;
    public int UpgradeCount => _upgradeCount;
    public event Action<int> OnLevelUp;
    public event Action<int, int> OnXpUpdated;
    public event Action OnUpgrade;

    protected BarUI _healthbar;

    private bool _isInvisible;
    public bool IsInvisible => _isInvisible;
    public event Action<bool> OnInvisible;

    [SerializeField] private AudioSource _audioSource;

    protected virtual void Awake()
    {
        Controller = GetComponent<TankController>();
        _gun.Initialize(this);

        _healthbar = HUDManager.Instance.CreateHealthbar();
        bool enableBar = !WorldManager.Instance.IsFarFromCamera(transform.position, transform.localScale.x);
        _healthbar.Initialize(gameObject, 0f, MaxHealth, enableBar);
    }

    protected virtual void Start() { }

    public void Initialize(int teamID)
    {
        _teamID = teamID;
        Health = MaxHealth;
        _canTouchDamage = true;
    }

    public bool TakeDamage(float damage, Tank attacker)
    {
        if (Health <= 0f) return false;

        if (Health - damage > 0f)
        {
            Health = Mathf.Clamp(Health - damage, 0f, MaxHealth);
            _healthbar.SetValue(Health);
            if (attacker != null && gameObject.activeSelf)
                OnTakeDamage?.Invoke(attacker);
        }
        else
        {
            Health = 0f;
            if (Controller != null)
                OnDestroyed?.Invoke();
            if (attacker != null)
                attacker.TakeDamage(-(MaxHealth / 3.5f));
            DestroyMe();
            return true;
        }

        return false;
    }

    public bool TakeDamage(float damage)
    {
        return TakeDamage(damage, null);
    }

    private void PlayDestroySound()
    {
        _audioSource.SetRandomPitchAndVolume(0.9f, 1.1f, 0.9f, 1f);
        _audioSource.clip = SoundManager.Instance.DestroyTank;
        _audioSource.Play();
    }

    private void DestroyMe()
    {
        _healthbar.Disable();
        StopAllCoroutines();
        GetComponent<Collider2D>().enabled = false;
        Helper.DisableAllExcept(transform, _audioSource);
        if (NearFromCamera())
        {
            PlayDestroySound();
            PrefabManager.Instance.CreateParticles(ParticlesType.TankExplode, transform.position, Quaternion.identity);
        }

        StartCoroutine(DestroyMeIE());
    }

    private bool NearFromCamera()
    {
        return !WorldManager.Instance.IsFarFromCamera(transform.position, transform.localScale.x * 5f);
    }

    protected virtual void DestroySelf()
    {
        Destroy(gameObject);
    }

    private IEnumerator DestroyMeIE()
    {
        yield return new WaitForSeconds(_audioSource.GetClipRemainingTime());
        DestroySelf();
        GetComponent<Collider2D>().enabled = true;
    }

    public void AddXP(int xp)
    {
        if (xp <= 0) return;

        int curXp = XP + xp - MaxXP;
        if (curXp >= 0)
        {
            XP = 0;
            AddLevel();

            if (curXp > 0)
                AddXP(curXp);
        }
        else
        {
            XP += xp;
        }

        OnXpUpdated?.Invoke(XP, MaxXP);
    }

    public void AddLevel(int level)
    {
        if (level == 0) return;

        Level += level;
        _upgradeCount += level;
        MaxXP += level * Constants.MAX_XP_MULTIPLIER;
        int newTier = PrefabManager.Instance.GetLastTier(Level);
        if (Tier != newTier)
        {
            Tier = newTier;
            OnTierUpdated?.Invoke(Tier);
        }
        OnLevelUp?.Invoke(Level);
        TakeDamage(-MaxHealth);
    }

    public void AddLevel()
    {
        AddLevel(1);
    }

    public bool CanUpgrade(UpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case UpgradeType.Damage:
                if (Damage < Constants.MAX_DAMAGE) return true;
                break;

            case UpgradeType.FireRate:
                if (FireRate > Constants.MIN_FIRE_RATE) return true;
                break;

            case UpgradeType.BulletSpeed:
                if (BulletSpeed < Constants.MAX_SPEED) return true;
                break;

            case UpgradeType.Speed:
                if (Speed < Constants.MAX_SPEED) return true;
                break;

            case UpgradeType.MaxHealth:
                if (MaxHealth < Constants.MAX_HEALTH) return true;
                break;

            case UpgradeType.TouchDamage:
                if (TouchDamage < Constants.MAX_DAMAGE) return true;
                break;
        }

        return false;
    }

    public void Upgrade(UpgradeType upgradeType)
    {
        if (_upgradeCount <= 0) return;

        switch (upgradeType)
        {
            case UpgradeType.Damage:
                Damage = Mathf.Clamp(Damage + Constants.DAMAGE_PER_LEVEL, 0, Constants.MAX_DAMAGE);
                break;

            case UpgradeType.FireRate:
                FireRate = Mathf.Clamp(FireRate - Constants.FIRE_RATE_PER_LEVEL, Constants.MIN_FIRE_RATE, FireRate);
                break;

            case UpgradeType.BulletSpeed:
                BulletSpeed = Mathf.Clamp(BulletSpeed + Constants.SPEED_PER_LEVEL, 0, Constants.MAX_SPEED);
                break;

            case UpgradeType.MaxHealth:
                float healthPerc = Health / MaxHealth;
                MaxHealth = Mathf.Clamp(MaxHealth + Constants.MAX_HEALTH_PER_LEVEL, 0, Constants.MAX_HEALTH);
                Health = healthPerc * MaxHealth;
                _healthbar.SetMaxValue(MaxHealth);
                _healthbar.SetValue(Health);
                break;

            case UpgradeType.Speed:
                Speed = Mathf.Clamp(Speed + Constants.SPEED_PER_LEVEL, 0, Constants.MAX_SPEED);
                break;

            case UpgradeType.TouchDamage:
                TouchDamage = Mathf.Clamp(TouchDamage + Constants.TOUCH_DAMAGE_PER_LEVEL, 0, Constants.MAX_DAMAGE);
                break;
        }

        _upgradeCount--;
        OnUpgrade?.Invoke();
    }

    public void Upgrade(Gun newGun)
    {
        if (_upgradeCount <= 0) return;

        bool isShooting = _gun.IsShooting;
        UpdateWeapon(newGun);
        _upgradeCount--;
        OnUpgrade?.Invoke();

        if (isShooting)
            _gun.ShootStart();
    }

    private void UpdateWeapon(Gun gun)
    {
        _gun = _gun.Change(gun);
        _gun.Initialize(this);
    }

    public void SelectNewGun(int gunIndex, int tier)
    {
        GameObject[] guns = PrefabManager.Instance.GetGunsByTier(tier);
        if (guns.Length <= 0) return;

        Upgrade(guns[gunIndex].GetComponent<Gun>());
    }

    public void SelectNewRandomGun()
    {
        SelectNewRandomGun(Tier);
    }

    public void SelectNewRandomGun(int tier)
    {
        GameObject[] guns = PrefabManager.Instance.GetGunsByTier(tier);
        if (guns.Length <= 0) return;

        int index = Random.Range(0, guns.Length);
        SelectNewGun(index, tier);
    }

    protected void ResetToDefault()
    {
        UpdateWeapon(PrefabManager.Instance.DefaultGun);

        MaxXP = Constants.DEFAULT_MAX_XP;
        Damage = Constants.DEFAULT_DAMAGE;
        Speed = Constants.DEFAULT_MOVE_SPEED;
        FireRate = Constants.DEFAULT_FIRE_RATE;
        MaxHealth = Constants.DEFAULT_MAX_HEALTH;
        BulletSpeed = Constants.DEFAULT_BULLET_SPEED;
        TouchDamage = Constants.DEFAULT_TOUCH_DAMAGE;
        _upgradeCount = 0;
        _teamID = 0;
        Level = 0;
        Tier = 0;
        XP = 0;

        _canTouchDamage = true;

        _healthbar.SetMaxValue(MaxHealth);
        _healthbar.SetValue(MaxHealth, true);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!_canTouchDamage || Controller.DisabledInput) return;

        bool canDamage(Vector3 position)
        {
            Vector3 targetDir = position - transform.position;
            float angle = Vector2.Angle(targetDir, transform.up);
            return angle <= _touchDamageAngle;
        }

        if (col.collider.TryGetComponent<Tank>(out var tank))
        {
            if (canDamage(tank.transform.position))
                tank.TakeDamage(TouchDamage);
            else
                return;
        }
        else if (col.collider.TryGetComponent<Destructible>(out var dest))
        {
            if (canDamage(dest.transform.position))
                dest.TakeDamage(TouchDamage, this);
            else
                return;
        }
        else
            return;

        _canTouchDamage = false;
        StartCoroutine(ReloadTouchDamageIE());
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Bushes"))
        {
            _isInvisible = true;
            OnInvisible?.Invoke(_isInvisible);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Bushes"))
        {
            _isInvisible = false;
            OnInvisible?.Invoke(_isInvisible);
        }
    }

    private IEnumerator ReloadTouchDamageIE()
    {
        yield return new WaitForSeconds(_touchDamageReloadSpeed);
        _canTouchDamage = true;
    }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        Health = MaxHealth;
    }
#endif
}
