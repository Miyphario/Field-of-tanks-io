using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tank : MonoBehaviour
{
    public virtual float MaxHealth { get; protected set; } = Constants.DEFAULT_MAX_HEALTH;
    public virtual float Health { get; protected set; }
    public event Action<Tank> OnTakeDamage;

    public virtual float Speed { get; protected set; } = Constants.DEFAULT_MOVE_SPEED;
    public virtual float BulletSpeed { get; protected set; } = Constants.DEFAULT_BULLET_SPEED;
    public virtual float Damage { get; protected set; } = Constants.DEFAULT_DAMAGE;
    public virtual float FireRate { get; protected set; } = Constants.DEFAULT_FIRE_RATE;

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

    protected virtual void Awake()
    {
        Controller = GetComponent<TankController>();
        Health = MaxHealth;
        _gun.Initialize(this);
        InitBar();
    }

    protected virtual void Start()
    {
        /* _healthbar = HUDManager.Instance.CreateHealthbar();
        bool enableBar = this switch
        {
            Player => true,
            _ => false
        };
        _healthbar.Initialize(gameObject, 0f, MaxHealth, enableBar); */
        InitBar();
    }

    // Change this code please
    private void InitBar()
    {
        if (_healthbar == null && HUDManager.Instance != null)
        {
            _healthbar = HUDManager.Instance.CreateHealthbar();
            bool enableBar = this switch
            {
                Player => true,
                _ => false
            };
            _healthbar.Initialize(gameObject, 0f, MaxHealth, enableBar);
        }
    }

    public void Initialize(int teamID)
    {
        _teamID = teamID;
        Health = MaxHealth;
    }

    public bool TakeDamage(float damage, Tank attacker)
    {
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
            _healthbar.Disable();
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

    protected virtual void DestroyMe()
    {
        StopAllCoroutines();
        _healthbar.Dispose();
        Destroy(gameObject);
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

        Damage = Constants.DEFAULT_DAMAGE;
        Speed = Constants.DEFAULT_MOVE_SPEED;
        FireRate = Constants.DEFAULT_FIRE_RATE;
        BulletSpeed = Constants.DEFAULT_BULLET_SPEED;
        MaxHealth = Constants.DEFAULT_MAX_HEALTH;
        MaxXP = Constants.DEFAULT_MAX_XP;
        XP = 0;
        _teamID = 0;
        _upgradeCount = 0;
        Level = 0;
        Tier = 0;

        _healthbar.SetMaxValue(MaxHealth);
        _healthbar.SetValue(MaxHealth, true);
    }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        Health = MaxHealth;
    }
#endif
}
